using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using CW.Common;
using System.Collections.Generic;

namespace PaintCore
{
	/// <summary>This class allows you to read the contents of a <b>RenderTexture</b> immediately or async.</summary>
	[System.Serializable]
	public class CwReader
	{
		enum RequestType
		{
			None,
			Async,
			Syncronous
		}

		[SerializeField]
		private AsyncGPUReadbackRequest readback;

		[SerializeField]
		private int currentLine;

		[SerializeField]
		private NativeArray<Color32> currentColors;

		[SerializeField]
		private bool dirty;

		[SerializeField]
		private RequestType requested;

		[SerializeField]
		private RenderTexture buffer;

		[SerializeField]
		private Vector2Int originalSize;

		[SerializeField]
		private Vector2Int downsampledSize;

		[SerializeField]
		private int downsampleSteps;

		[SerializeField]
		private int downsampleBoost;

		[SerializeField]
		private Texture2D tempTexture;

		[SerializeField]
		private int readCount;

		public event System.Action<NativeArray<Color32>> OnComplete;

		public static LinkedList<CwReader> Instances = new LinkedList<CwReader>(); private LinkedListNode<CwReader> node;

		public static List<CwReader> PendingReaders = new List<CwReader>();

		public bool Dirty
		{
			get
			{
				return dirty;
			}
		}

		public bool Requested
		{
			get
			{
				return requested != RequestType.None;
			}
		}

		public Vector2Int OriginalSize
		{
			get
			{
				return originalSize;
			}
		}

		public int DownsampleSteps
		{
			get
			{
				return downsampleSteps;
			}
		}

		public Vector2Int DownsampledSize
		{
			get
			{
				return downsampledSize;
			}
		}

		public int DownsampleBoost
		{
			get
			{
				return downsampleBoost;
			}
		}

		public int ReadCount
		{
			get
			{
				return readCount;
			}
		}

		public CwReader()
		{
			node = Instances.AddLast(this);
		}

		public void MarkAsDirty()
		{
			dirty = true;
		}

		public void UpdateRequest(ref int pixelBudget)
		{
			if (requested == RequestType.Async)
			{
				if (readback.hasError == true)
				{
					requested   = RequestType.Syncronous;
					currentLine = 0;
				}
				else if (readback.done == true)
				{
					FinishRequest();

					OnComplete(readback.GetData<Color32>());
				}
			}

			if (requested == RequestType.Syncronous && pixelBudget > 0)
			{
				if (currentColors.IsCreated == false)
				{
					currentColors = new NativeArray<Color32>(buffer.width * buffer.height, Allocator.Persistent);
				}
				
				var remainingLines = buffer.height - currentLine;
				var maximumLines  = pixelBudget / buffer.width;

				if (maximumLines == 0)
				{
					maximumLines = 1;
				}

				ReadLines(Mathf.Min(maximumLines, remainingLines));

				if (currentLine >= buffer.height)
				{
					FinishRequest();

					OnComplete(currentColors);

					currentColors.Dispose();
				}
			}
		}

		private void FinishRequest()
		{
			requested = RequestType.None;

			buffer = CwCommon.ReleaseRenderTexture(buffer);

			readCount += 1;

			PendingReaders.Remove(this);
		}

		private void ReadLines(int count)
		{
			if (tempTexture == null)
			{
				tempTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			}

			tempTexture.Reinitialize(buffer.width, count);

			CwHelper.BeginActive(buffer);

			tempTexture.ReadPixels(new Rect(0, currentLine, buffer.width, count), 0, 0);

			CwHelper.EndActive();

			tempTexture.Apply();

			var subColors = tempTexture.GetRawTextureData<Color32>();
			var subOffset = buffer.width * currentLine;

			for (var i = 0; i < subColors.Length; i++)
			{
				currentColors[subOffset + i] = subColors[i];
			}

			currentLine += count;
		}

		public static bool NeedsUpdating<T>(CwReader reader, NativeArray<T> array, RenderTexture texture, int downsampleSteps)
			where T : struct
		{
			if (array.IsCreated == false || reader.dirty == true || reader.DownsampledSize.x * reader.DownsampledSize.y != array.Length)
			{
				return true;
			}

			var originalSize    = Vector2Int.zero;
			var downsampledSize = Vector2Int.zero;

			originalSize.x = downsampledSize.x = texture.width;
			originalSize.y = downsampledSize.y = texture.height;

			for (var i = 0; i < downsampleSteps; i++)
			{
				if (downsampledSize.x > 2)
				{
					downsampledSize.x /= 2;
				}

				if (downsampledSize.y > 2)
				{
					downsampledSize.y /= 2;
				}
			}

			return reader.OriginalSize != originalSize || reader.DownsampledSize != downsampledSize;
		}

		public void Request(RenderTexture texture, int downsample, bool async)
		{
			if (texture == null)
			{
				Debug.LogError("Texture null."); return;
			}

			if (requested != RequestType.None)
			{
				Debug.LogError("Already requested."); return;
			}

			if (buffer != null)
			{
				Debug.LogError("Buffer exists."); return;
			}

			originalSize.x = downsampledSize.x = texture.width;
			originalSize.y = downsampledSize.y = texture.height;

			for (var i = 0; i < downsample; i++)
			{
				if (downsampledSize.x > 2)
				{
					downsampledSize.x /= 2;
				}

				if (downsampledSize.y > 2)
				{
					downsampledSize.y /= 2;
				}
			}

			downsampleSteps = downsample;
			downsampleBoost = (originalSize.x / downsampledSize.x) * (originalSize.y / downsampledSize.y);

			var desc = texture.descriptor;

			desc.useMipMap = false;
			desc.width     = downsampledSize.x;
			desc.height    = downsampledSize.y;

			buffer = CwCommon.GetRenderTexture(desc);

			CwCommandReplace.Blit(buffer, texture, Color.white);

			if (async == true && SystemInfo.supportsAsyncGPUReadback == true)
			{
				requested = RequestType.Async;
				readback  = AsyncGPUReadback.Request(buffer, 0, TextureFormat.RGBA32);
			}
			else
			{
				requested   = RequestType.Syncronous;
				currentLine = 0;
			}

			dirty = false;
		}

		public void Release()
		{
			buffer = CwCommon.ReleaseRenderTexture(buffer);

			tempTexture = CwHelper.Destroy(tempTexture);

			Instances.Remove(node); node = null;
		}
	}
}