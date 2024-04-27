using EthansGameKit.Attributes;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor.Scripts
{
	[CustomPropertyDrawer(typeof(DisplayAsAttribute))]
	internal sealed class DisplayAsAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attribute = (DisplayAsAttribute)this.attribute;
			if (attribute.ShowCodeName) label.text = $"{attribute.Name}({property.name})";
			EditorGUI.PropertyField(position, property, label, true);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
			base.GetPropertyHeight(property, label); // + EditorGUIUtility.singleLineHeight;
	}
}