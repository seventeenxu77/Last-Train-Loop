using CW.Common;
using UnityEngine;

namespace PaintCore
{
	/// <summary>This this is the base class for hit screen components that receive data from <b>CwPointer___</b> components.</summary>
	public abstract class CwHitPointers : MonoBehaviour
	{
		/// <summary>Fingers that began touching the screen on top of these UI layers will be ignored.</summary>
		public LayerMask GuiLayers { set { guiLayers = value; } get { return guiLayers; } } [SerializeField] private LayerMask guiLayers = 1 << 5;

		public virtual void BreakFinger(CwInputManager.Finger finger)
		{
		}

		public virtual void HandleFingerUpdate(CwInputManager.Finger finger, bool down, bool up)
		{
			if (up == true)
			{
				HandleFingerUp(finger);
			}

			CwPaintableManager.MarkActivelyPainting();
		}

		protected virtual void HandleFingerUp(CwInputManager.Finger finger)
		{
		}
	}
}