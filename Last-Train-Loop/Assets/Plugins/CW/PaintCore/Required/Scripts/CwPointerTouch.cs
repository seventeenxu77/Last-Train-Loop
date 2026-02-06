using CW.Common;
using UnityEngine;

namespace PaintCore
{
	/// <summary>This component sends pointer information to any <b>CwHitScreen</b> component, allowing you to paint with a touchscreen.</summary>
	[RequireComponent(typeof(CwHitPointers))]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwPointerTouch")]
	[AddComponentMenu(CwCommon.ComponentHitMenuPrefix + "Pointer Touch")]
	public class CwPointerTouch : CwPointer
	{
		/// <summary>If you want the paint to appear above the finger, then you can set this number to something positive.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>If you want to limit how many fingers can paint at the same time, set it here.
		/// -1 = Any number of fingers.</summary>
		public int MaxFingers { set { maxFingers = value; } get { return maxFingers; } } [SerializeField] private int maxFingers = -1;

		protected virtual void Update()
		{
			CwInputManager.Finger finger;

			for (var i = 0; i < CwInput.GetTouchCount(); i++)
			{
				int     index;
				Vector2 position;
				float   pressure;
				bool    set;

				CwInput.GetTouch(i, out index, out position, out pressure, out set);

				position.y += offset * CwInputManager.ScaleFactor;

				var down = GetFinger(index, position, pressure, set, out finger);

				// New painting finger?
				if (down == true && finger.StartedOverGui == false)
				{
					// If there are too many, ignore this
					if (maxFingers >= 0 && GetFingerCount(true) > maxFingers)
					{
						TryNullFinger(index);

						continue;
					}
				}

				cachedHitPointers.HandleFingerUpdate(finger, down, set == false);

				if (set == false)
				{
					TryNullFinger(index);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintCore
{
	using UnityEditor;
	using TARGET = CwPointerTouch;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwPointerTouch_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("offset", "If you want the paint to appear above the finger, then you can set this number to something positive.");
			Draw("maxFingers", "If you want to limit how many fingers can paint at the same time, set it here.\n\n-1 = Any number of fingers.");
		}
	}
}
#endif