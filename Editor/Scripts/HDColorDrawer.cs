using EthansGameKit.Attributes;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(HDColor))]
	public class HDColorDrawer : UnityEditor.PropertyDrawer
	{
		public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == UnityEditor.SerializedPropertyType.Color)
			{
				UnityEditor.EditorGUI.BeginChangeCheck();
				var oldColor = property.colorValue;
				var newColor = UnityEditor.EditorGUI.ColorField(position, label, oldColor, true, true, true);
				if (UnityEditor.EditorGUI.EndChangeCheck())
				{
					property.colorValue = newColor;
				}
			}
			else
			{
				UnityEditor.EditorGUI.LabelField(position, label.text, $"Use {nameof(HDColor)} with Color field.");
			}
		}
	}
}
