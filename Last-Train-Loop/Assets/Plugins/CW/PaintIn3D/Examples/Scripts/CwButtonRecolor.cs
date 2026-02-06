using UnityEngine;
using UnityEngine.EventSystems;
using CW.Common;
using System.Collections.Generic;

namespace PaintCore
{
	/// <summary>This component allows you to perform the Undo All action. This can be done by attaching it to a clickable object, or manually from the RedoAll method.</summary>
	[HelpURL(CwCommon.HelpUrlPrefix + "CwButtonRecolor")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Button Recolor")]
	public class CwButtonRecolor : MonoBehaviour, IPointerClickHandler
	{
		public enum ApplyType
		{
			CloneMaterial,
			ApplyToExisting,
			PropertyBlock
		}

		/// <summary>The renderer whose color will be changed.</summary>
		public Renderer TargetRenderer { set { targetRenderer = value; } get { return targetRenderer; } } [SerializeField] private Renderer targetRenderer;

		/// <summary>The material index in the target renderer.</summary>
		public int TargetMaterial { set { targetMaterial = value; } get { return targetMaterial; } } [SerializeField] private int targetMaterial;

		/// <summary>The material property that will be changed.</summary>
		public string TargetProperty { set { targetProperty = value; } get { return targetProperty; } } [SerializeField] private string targetProperty = "_Color";

		/// <summary>How should the new color be applied to the target renderer?</summary>
		public ApplyType Apply { set { apply = value; } get { return apply; } } [SerializeField] private ApplyType apply = ApplyType.PropertyBlock;

		public void OnPointerClick(PointerEventData eventData)
		{
			Recolor();
		}

		private static MaterialPropertyBlock properties;

		private static List<Material> tempMaterials = new List<Material>();

		/// <summary>If you want to manually trigger Recolor, then call this function.</summary>
		[ContextMenu("Recolor")]
		public void Recolor()
		{
			var newColor = Random.ColorHSV();

			if (apply == ApplyType.CloneMaterial)
			{
				if (targetRenderer != null)
				{
					targetRenderer.GetSharedMaterials(tempMaterials);

					if (tempMaterials.Count >= targetMaterial && tempMaterials[targetMaterial] != null)
					{
						tempMaterials[targetMaterial] = Instantiate(tempMaterials[targetMaterial]);

						targetRenderer.sharedMaterials = tempMaterials.ToArray();

						apply = ApplyType.ApplyToExisting;
					}
				}
			}

			if (apply == ApplyType.ApplyToExisting)
			{
				if (targetRenderer != null)
				{
					targetRenderer.GetSharedMaterials(tempMaterials);

					if (tempMaterials.Count >= targetMaterial && tempMaterials[targetMaterial] != null)
					{
						tempMaterials[targetMaterial].SetColor(targetProperty, newColor);
					}
				}
			}

			if (apply == ApplyType.PropertyBlock)
			{
				if (targetRenderer != null)
				{
					if (properties == null)
					{
						properties = new MaterialPropertyBlock();
					}

					targetRenderer.GetPropertyBlock(properties, targetMaterial);

					properties.SetColor(targetProperty, newColor);

					targetRenderer.SetPropertyBlock(properties, targetMaterial);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintCore
{
	using UnityEditor;
	using TARGET = CwButtonRecolor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwButtonRecolor_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("targetRenderer", "The renderer whose color will be changed.");
			Draw("targetMaterial", "The material index in the target renderer.");
			Draw("targetProperty", "The material property that will be changed.");
			Draw("apply", "How should the new color be applied to the target renderer?");
		}
	}
}
#endif