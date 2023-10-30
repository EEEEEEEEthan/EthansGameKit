using UnityEditor;
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
		public static T GetOrAddComponent<T>(this GameObject @this) where T : Component
		{
			var component = @this.GetComponent<T>();
			if (!component) component = @this.AddComponent<T>();
			return component;
		}
		public static T Editor_GetOrAddComponentWithUndo<T>(this GameObject @this) where T : Component
		{
			if (!Application.isEditor)
			{
				Debug.LogError($"use {nameof(GetOrAddComponent)} in non-editor mode");
				return @this.GetOrAddComponent<T>();
			}
			var component = @this.GetComponent<T>();
			if (!component)
			{
#if UNITY_EDITOR
				Undo.RecordObject(@this, $"Add Component {typeof(T).Name}");
				@this.AddComponent<T>();
				EditorUtility.SetDirty(@this);
#endif
			}
			return component;
		}
	}
}
