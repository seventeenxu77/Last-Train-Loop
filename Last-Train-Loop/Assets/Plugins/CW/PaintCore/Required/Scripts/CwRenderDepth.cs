using UnityEngine;
using CW.Common;
using System.Collections.Generic;

namespace PaintCore
{
	/// <summary>This component renders scene depth to a RenderTexture. This scene depth can be set in a <b>CwPaint___</b> component's <b>Advanced/DepthMask</b> setting, which allows you to paint on the first surface in the view of the specified camera.</summary>
	[HelpURL(CwCommon.HelpUrlPrefix + "CwRenderDepth")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Render Depth")]
	public class CwRenderDepth : MonoBehaviour
	{
		/// <summary>The camera whose depth information will be read.</summary>
		public Camera SourceCamera { set { sourceCamera = value; } get { return sourceCamera; } } [SerializeField] private Camera sourceCamera;

		/// <summary>The transformation matrix of the camera when the depth texture was generated.</summary>
		public Matrix4x4 SourceMatrix { set { sourceMatrix = value; } get { return sourceMatrix; } } [SerializeField] private Matrix4x4 sourceMatrix;

		/// <summary>If this is 0, the RenderTexture size will match the viewport. If it's above 0, then the RenderTexture size will be set to the viewport size divided by this value.</summary>
		public int ResizeAndDownscale { set { resizeAndDownscale = value; } get { return resizeAndDownscale; } } [SerializeField] private int resizeAndDownscale;

		/// <summary>The rendered depth must be at least this mant units different from the painted surface for the paint to be masked out.</summary>
		public float Bias { set { bias = value; } get { return bias; } } [SerializeField] private float bias = 0.0000001f;

		/// <summary>If you enable this, then the depth mask will be sampled multiple times per pixel, allowing you to paint both foreground and background objects with little to no edge seams.</summary>
		public bool Multitap { set { multitap = value; } get { return multitap; } } [SerializeField] private bool multitap;

		/// <summary>Should the scene depth be rendered in <b>Start</b>?</summary>
		public bool ReadInStart { set { readInStart = value; } get { return readInStart; } } [SerializeField] private bool readInStart = true;

		/// <summary>Should the scene depth be rendered every frame in <b>Update</b>?</summary>
		public bool ReadInUpdate { set { readInUpdate = value; } get { return readInUpdate; } } [SerializeField] private bool readInUpdate;

		private Shader cachedShader;

		[System.NonSerialized]
		private Camera depthCamera;

		[System.NonSerialized]
		private RenderTexture depthTexture;

		private static int _CwDepthMatrix = Shader.PropertyToID("_CwDepthMatrix");

		/// <summary>This stores all active and enabled instances in the open scenes.</summary>
		public static LinkedList<CwRenderDepth> Instances { get { return instances; } } private static LinkedList<CwRenderDepth> instances = new LinkedList<CwRenderDepth>(); private LinkedListNode<CwRenderDepth> instancesNode;

		public RenderTexture DepthTexture
		{
			get
			{
				return depthTexture;
			}
		}

		public int TapCount
		{
			get
			{
				return multitap == true ? 8 : 0;
			}
		}

		public static CwRenderDepth Find()
		{
			return instances.Count > 0 ? instances.First.Value : null;
		}

		/// <summary>This method will update the <b>TargetTexture</b> with what the <b>SourceCamera</b> currently sees.</summary>
		[ContextMenu("Read Now")]
		public void ReadNow()
		{
			if (cachedShader == null)
			{
				cachedShader = Shader.Find("Hidden/PaintCore/CwRenderDepth");
			}

			if (depthCamera == null)
			{
				CreateDepthCamera();
			}

			if (depthTexture == null)
			{
				depthTexture = new RenderTexture(64, 64, 32, RenderTextureFormat.Depth);
			}

			if (sourceCamera != null)
			{
				var newWidth  = sourceCamera.pixelWidth;
				var newHeight = sourceCamera.pixelHeight;

				if (resizeAndDownscale > 0)
				{
					newWidth  /= resizeAndDownscale;
					newHeight /= resizeAndDownscale;
				}

				if (depthTexture.width != newWidth || depthTexture.height != newHeight)
				{
					if (depthTexture.IsCreated() == true)
					{
						depthTexture.Release();
					}

					depthTexture.width  = newWidth;
					depthTexture.height = newHeight;

					depthTexture.Create();
				}

				sourceMatrix = Matrix4x4.Translate(new Vector3(0.5f, 0.5f, 0.5f)) * Matrix4x4.Scale(new Vector3(0.5f, 0.5f, 0.5f)) * sourceCamera.projectionMatrix * sourceCamera.worldToCameraMatrix;

				Shader.SetGlobalMatrix(_CwDepthMatrix, sourceMatrix);
				
				depthCamera.CopyFrom(sourceCamera);

				depthCamera.enabled         = false;
				depthCamera.clearFlags      = CameraClearFlags.SolidColor;
				depthCamera.backgroundColor = Color.black;
				depthCamera.targetTexture   = depthTexture;

				depthCamera.transform.position = sourceCamera.transform.position;
				depthCamera.transform.rotation = sourceCamera.transform.rotation;

				depthCamera.RenderWithShader(cachedShader, "RenderType");
			}
		}

		protected virtual void Start()
		{
			if (readInStart == true)
			{
				ReadNow();
			}
		}

		protected virtual void Update()
		{
			if (readInUpdate == true)
			{
				ReadNow();
			}
		}

		protected virtual void OnEnable()
		{
			instancesNode = instances.AddLast(this);
		}

		protected virtual void OnDisable()
		{
			instances.Remove(instancesNode); instancesNode = null;

			ClearDepthCamera();
		}

		private void CreateDepthCamera()
		{
			ClearDepthCamera();

			var root = new GameObject("DepthCamera");

			root.hideFlags = HideFlags.HideAndDontSave;

			depthCamera = root.AddComponent<Camera>();
			depthCamera.enabled = false;
		}

		private void ClearDepthCamera()
		{
			if (depthCamera != null)
			{
				DestroyImmediate(depthCamera.gameObject);

				depthCamera = null;
			}

			if (depthTexture != null)
			{
				depthTexture.Release();

				DestroyImmediate(depthTexture);

				depthTexture = null;
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			var m = sourceMatrix.inverse;
			Gizmos.color = Color.white;
			Gizmos.DrawLine(m.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.0f)), m.MultiplyPoint(new Vector3(1.0f, 0.0f, 0.0f)));
			Gizmos.DrawLine(m.MultiplyPoint(new Vector3(0.0f, 0.0f, 0.0f)), m.MultiplyPoint(new Vector3(0.0f, 1.0f, 0.0f)));
				
			Gizmos.DrawLine(m.MultiplyPoint(new Vector3(1.0f, 1.0f, 0.0f)), m.MultiplyPoint(new Vector3(0.0f, 1.0f, 0.0f)));
			Gizmos.DrawLine(m.MultiplyPoint(new Vector3(1.0f, 1.0f, 0.0f)), m.MultiplyPoint(new Vector3(1.0f, 0.0f, 0.0f)));

			Gizmos.color = Color.black;
			Gizmos.DrawLine(m.MultiplyPoint(new Vector3(0.0f, 0.0f, 1.0f)), m.MultiplyPoint(new Vector3(1.0f, 0.0f, 1.0f)));
			Gizmos.DrawLine(m.MultiplyPoint(new Vector3(0.0f, 0.0f, 1.0f)), m.MultiplyPoint(new Vector3(0.0f, 1.0f, 1.0f)));
				
			Gizmos.DrawLine(m.MultiplyPoint(new Vector3(1.0f, 1.0f, 1.0f)), m.MultiplyPoint(new Vector3(0.0f, 1.0f, 1.0f)));
			Gizmos.DrawLine(m.MultiplyPoint(new Vector3(1.0f, 1.0f, 1.0f)), m.MultiplyPoint(new Vector3(1.0f, 0.0f, 1.0f)));
		}
#endif
	}
}

#if UNITY_EDITOR
namespace PaintCore
{
	using UnityEditor;
	using TARGET = CwRenderDepth;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwRenderDepth_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.SourceCamera == null));
				Draw("sourceCamera", "The camera whose depth information will be read.");
			EndError();
			Draw("resizeAndDownscale", "If this is 0, the RenderTexture size will match the viewport. If it's above 0, then the RenderTexture size will be set to the viewport size divided by this value.");
			Draw("bias", "The rendered depth must be at least this mant units different from the painted surface for the paint to be masked out.");
			Draw("multitap", "If you enable this, then the depth mask will be sampled multiple times per pixel, allowing you to paint both foreground and background objects with little to no edge seams.");
			Draw("readInStart", "Should the scene depth be rendered in <b>Start</b>?");
			Draw("readInUpdate", "Should the scene depth be rendered every frame in <b>Update</b>?");

			Separator();

			if (Button("Read Now") == true)
			{
				Each(tgts, t => t.ReadNow(), true, true);
			}
		}
	}
}
#endif