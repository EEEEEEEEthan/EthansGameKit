using EthansGameKit.Attributes;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CustomPropertyDrawer(typeof(HDColor))]
	public class HDColorDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.Color)
			{
				EditorGUI.BeginChangeCheck();
				var oldColor = property.colorValue;
				var newColor = EditorGUI.ColorField(position, label, oldColor, true, true, true);
				if (EditorGUI.EndChangeCheck())
				{
					property.colorValue = newColor;
				}
			}
			else
			{
				EditorGUI.LabelField(position, label.text, $"Use {nameof(HDColor)} with Color field.");
			}
		}
	}
}
