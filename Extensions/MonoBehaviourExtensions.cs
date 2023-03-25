using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void EditorSetField(this MonoBehaviour @this, string field, Object value)
		{
#if UNITY_EDITOR
			var serializedObject = new UnityEditor.SerializedObject(@this);
			serializedObject.FindProperty(field).objectReferenceValue = value;
			serializedObject.ApplyModifiedProperties();
#endif
		}
	}
}
