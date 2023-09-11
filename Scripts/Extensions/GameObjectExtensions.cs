using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool TryGetComponentInParent<T>(this GameObject @this, bool includeInactive, out T component)
		{
			component = @this.GetComponentInParent<T>(includeInactive);
			return component != null;
		}
		public static bool TryGetComponentInChildren<T>(this GameObject @this, bool includeInactive, out T component)
		{
			component = @this.GetComponentInChildren<T>(includeInactive);
			return component != null;
		}
		public static bool TryGetComponentInParent<T>(this GameObject @this, out T component)
		{
			component = @this.GetComponentInParent<T>(false);
			return component != null;
		}
		public static bool TryGetComponentInChildren<T>(this GameObject @this, out T component)
		{
			component = @this.GetComponentInChildren<T>(false);
			return component != null;
		}
		public static T GetOrAddComponent<T>(this GameObject @this, bool withUndo = false) where T : Component
		{
			var component = @this.GetComponent<T>();
			if (!component)
			{
				#if UNITY_EDITOR
				if (withUndo)
				{
					UnityEditor.Undo.RecordObject(@this, $"Add Component {typeof(T).Name}");
					component = @this.AddComponent<T>();
					UnityEditor.EditorUtility.SetDirty(@this);
				}
				#endif
				component = @this.AddComponent<T>();
			}
			return component;
		}
	}
}
