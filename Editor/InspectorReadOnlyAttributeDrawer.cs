using EthansGameKit.Attributes;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	[CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
	public class InspectorReadOnlyAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginDisabledGroup(true);
			if(property.isArray)
			{
				// 绘制数组或列表的标签
				EditorGUI.LabelField(position, label);

				EditorGUI.indentLevel++;
				EditorGUI.BeginChangeCheck();

				int newSize = EditorGUI.DelayedIntField(
					position: new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),
					label: "Size",
					value: property.arraySize,
					style: EditorStyles.numberField
				);

				if (EditorGUI.EndChangeCheck())
				{
					property.arraySize = newSize;
				}

				EditorGUI.indentLevel--;

				for (int i = 0; i < property.arraySize; i++)
				{
					EditorGUI.PropertyField(
						position: new Rect(position.x, position.y + (i + 2) * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),
						property: property.GetArrayElementAtIndex(i),
						label: new GUIContent($"Element {i}"),
						includeChildren: true
					);
				}
			}
			else
				EditorGUI.PropertyField(position, property, label, true);
			EditorGUI.EndDisabledGroup();
		}
	}
}

