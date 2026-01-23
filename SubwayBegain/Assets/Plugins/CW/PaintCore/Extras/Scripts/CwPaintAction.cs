using UnityEngine;
using CW.Common;
using UnityEngine.Events;

namespace PaintCore
{
	/// <summary>This component performs an action every time a paint hit is received. Hit points will automatically be sent by any <b>CwHit___</b> component on this GameObject, or its ancestors.</summary>
	[HelpURL(CwCommon.HelpUrlPrefix + "CwPaintAction")]
	[AddComponentMenu(CwCommon.ComponentHitMenuPrefix + "Paint Action")]
	public class CwPaintAction : MonoBehaviour, IHitPoint, IHitLine, IHitTriangle, IHitQuad
	{
		/// <summary>The event that will be invoked.</summary>
		public UnityEvent Action { get { if (action == null) action = new UnityEvent(); return action; } } [SerializeField] public UnityEvent action;

		public void HandleHitPoint(bool preview, int priority, float pressure, int seed, Vector3 position, Quaternion rotation)
		{
			if (action != null)
			{
				action.Invoke();
			}
		}

		public void HandleHitLine(bool preview, int priority, float pressure, int seed, Vector3 position, Vector3 endPosition, Quaternion rotation, bool clip)
		{
			if (action != null)
			{
				action.Invoke();
			}
		}

		public void HandleHitTriangle(bool preview, int priority, float pressure, int seed, Vector3 positionA, Vector3 positionB, Vector3 positionC, Quaternion rotation)
		{
			if (action != null)
			{
				action.Invoke();
			}
		}

		public void HandleHitQuad(bool preview, int priority, float pressure, int seed, Vector3 position, Vector3 endPosition, Vector3 position2, Vector3 endPosition2, Quaternion rotation, bool clip)
		{
			if (action != null)
			{
				action.Invoke();
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintCore
{
	using UnityEditor;
	using TARGET = CwPaintAction;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwPaintAction_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("action");
		}
	}
}
#endif