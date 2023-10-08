using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	static class Extensions
	{
		public static string GetPath(this Object @this)
		{
			return AssetDatabase.GetAssetPath(@this);
		}
		public static FieldInfo GetField(this SerializedProperty @this)
		{
			var serializedObject = @this.serializedObject;
			var path = @this.propertyPath;
			return serializedObject.targetObject.GetType().GetField(path, (BindingFlags)0xffff);
		}
		public static object GetObject(this SerializedProperty @this)
		{
			var serializedObject = @this.serializedObject;
			var path = @this.propertyPath;
			var field = serializedObject.targetObject.GetType().GetField(path, (BindingFlags)0xffff);
			return field.GetValue(serializedObject.targetObject);
		}
	}
}
