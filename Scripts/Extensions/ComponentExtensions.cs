using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool TryGetComponentInParent<T>(this Component @this, out T component)
		{
			component = @this.GetComponentInParent<T>();
			return component != null;
		}
		public static T GetOrAddComponent<T>(this Component @this, out bool isNew) where T : Component
		{
			if (@this.TryGetComponent<T>(out var component))
			{
				isNew = false;
				return component;
			}
			isNew = true;
			return @this.gameObject.AddComponent<T>();
		}
		public static T GetOrAddComponent<T>(this Component @this) where T : Component
		{
			return @this.GetOrAddComponent<T>(out _);
		}
		public static T GetOrAddComponent<T>(this Component @this, string path, out bool isNew) where T : Component
		{
			var transform = @this.transform.FindOrAdd(path, out var isNewGameObject);
			var component = transform.GetOrAddComponent<T>(out var isNewComponent);
			isNew = isNewGameObject || isNewComponent;
			return component;
		}
		public static T GetOrAddComponent<T>(this Component @this, string path) where T : Component
		{
			return @this.GetOrAddComponent<T>(path, out _);
		}
	}
}
