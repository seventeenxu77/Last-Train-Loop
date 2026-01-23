using UnityEngine;
using CW.Common;
using UnityEngine.Events;

namespace PaintCore
{
	/// <summary>This component performs an action every time you undo/redo/etc.</summary>
	[HelpURL(CwCommon.HelpUrlPrefix + "CwUndoAction")]
	[AddComponentMenu(CwCommon.ComponentHitMenuPrefix + "Undo Action")]
	public class CwUndoAction : MonoBehaviour
	{
		/// <summary>Listen for <b>CwStateManager.UndoAll</b> calls?</summary>
		public bool PreUndoAll { set { preUndoAll = value; } get { return preUndoAll; } } [SerializeField] private bool preUndoAll = true;

		/// <summary>Listen for <b>CwStateManager.RedoAll</b> calls?</summary>
		public bool PreRedoAll { set { preRedoAll = value; } get { return preRedoAll; } } [SerializeField] private bool preRedoAll = true;

		/// <summary>The event that will be invoked.</summary>
		public UnityEvent Action { get { if (action == null) action = new UnityEvent(); return action; } } [SerializeField] public UnityEvent action;

		protected virtual void OnEnable()
		{
			CwStateManager.OnPreUndoAll += HandlePreUndoAll;
			CwStateManager.OnPreRedoAll += HandlePreRedoAll;
		}

		protected virtual void OnDisable()
		{
			CwStateManager.OnPreUndoAll -= HandlePreUndoAll;
			CwStateManager.OnPreRedoAll -= HandlePreRedoAll;
		}

		private void HandlePreUndoAll()
		{
			if (preUndoAll == true && action != null)
			{
				action.Invoke();
			}
		}

		private void HandlePreRedoAll()
		{
			if (preRedoAll == true && action != null)
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
	using TARGET = CwUndoAction;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwUndoAction_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("preUndoAll");
			Draw("preRedoAll");
			Draw("action");
		}
	}
}
#endif