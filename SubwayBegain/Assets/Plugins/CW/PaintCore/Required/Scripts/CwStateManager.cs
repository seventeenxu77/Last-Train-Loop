namespace PaintCore
{
	/// <summary>This component allows you to manage undo/redo states on all CwPaintableTextures in your scene.</summary>
	public static class CwStateManager
	{
		private static bool allStatesStored;

		private static bool potentiallyStoreStates;

		public static event System.Action OnPreUndoAll;

		public static event System.Action OnPreRedoAll;

		public static bool CanUndo
		{
			get
			{
				foreach (var paintableTexture in CwPaintableTexture.Instances)
				{
					if (paintableTexture.CanUndo == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		public static bool CanRedo
		{
			get
			{
				foreach (var paintableTexture in CwPaintableTexture.Instances)
				{
					if (paintableTexture.CanRedo == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		public static bool PotentiallyStoreStates
		{
			set
			{
				potentiallyStoreStates = value;
			}

			get
			{
				return potentiallyStoreStates;
			}
		}

		public static bool AllStatesStored
		{
			set
			{
				allStatesStored = value;
			}

			get
			{
				return allStatesStored;
			}
		}

		/// <summary>This method will call <b>StoreState</b> on all active and enabled CwPaintableTextures.
		/// NOTE: This should be called before you perform manual changes to paintable textures.</summary>
		public static void StoreAllStates()
		{
			allStatesStored        = true;
			potentiallyStoreStates = false;

			foreach (var paintableTexture in CwPaintableTexture.Instances)
			{
				paintableTexture.StoreState();
			}
		}

		/// <summary>This method should be called if you're about to send paint hits that might apply paint to objects. If so, <b>StoreState</b> will be called on all active and enabled CwPaintableTextures</summary>
		public static void PotentiallyStoreAllStates()
		{
			potentiallyStoreStates = true;
		}

		/// <summary>This method will call <b>ClearStates</b> on all active and enabled CwPaintableTextures.</summary>
		public static void ClearAllStates()
		{
			foreach (var paintableTexture in CwPaintableTexture.Instances)
			{
				paintableTexture.ClearStates();
			}
		}

		/// <summary>This method will call <b>Undo</b> on all active and enabled CwPaintableTextures.</summary>
		public static void UndoAll()
		{
			if (OnPreUndoAll != null) OnPreUndoAll.Invoke();

			foreach (var paintableTexture in CwPaintableTexture.Instances)
			{
				paintableTexture.Undo();
			}
		}

		/// <summary>This method will call <b>Redo</b> on all active and enabled CwPaintableTextures.</summary>
		public static void RedoAll()
		{
			if (OnPreRedoAll != null) OnPreRedoAll.Invoke();

			foreach (var paintableTexture in CwPaintableTexture.Instances)
			{
				paintableTexture.Redo();
			}
		}
	}
}