using System;
using System.Collections.Generic;
using EthansGameKit.Assurance;
using UnityEditor;
using UnityEngine;
using MessageType = EthansGameKit.Assurance.MessageType;

namespace EthansGameKit.Editor
{
	class SelfCheckWindow : EditorWindow
	{
		// F5快捷键
		[MenuItem("Tools/" + PackageDefines.packageName + "/Self Check &F5")]
		static void ShowWindow()
		{
			var window = GetWindow<SelfCheckWindow>("Self Check");
			window.checkedOnce = false;
		}
		[SerializeField] List<Problem> problems = new();
		bool checkedOnce;
		void OnEnable()
		{
			checkedOnce = false;
		}
		void OnGUI()
		{
			if (GUILayout.Button("Self Check") || !checkedOnce)
			{
				SelfCheck();
			}
			foreach (var problem in problems)
			{
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				EditorGUILayout.BeginHorizontal();
				{
					var rect = GUILayoutUtility.GetRect(16, 16, 16, 16, GUILayout.ExpandWidth(false));
					rect.width = 16;
					rect.height = 16;
					rect.y += 3;
					rect.x += 3;
					switch (problem.type)
					{
						case MessageType.Error:
							GUI.DrawTexture(rect, EditorGUIUtility.IconContent("console.erroricon.sml").image);
							break;
						case MessageType.Warning:
							GUI.DrawTexture(rect, EditorGUIUtility.IconContent("console.warnicon.sml").image);
							break;
						case MessageType.Suggestion:
							GUI.DrawTexture(rect, EditorGUIUtility.IconContent("console.infoicon.sml").image);
							break;
						case MessageType.Hint:
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				EditorGUILayout.LabelField(problem.title);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.LabelField(problem.details);
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Show me", GUILayout.Width(70)))
					EditorGUIUtility.PingObject(problem.context);
				EditorGUILayout.EndHorizontal();
			}
		}
		void SelfCheck()
		{
			checkedOnce = true;
			problems.Clear();
			List<ISelfCheckable> buffer = new();
			var exceptionOccured = false;
			foreach (var gameObject in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				gameObject.GetComponents(buffer);
				foreach (var selfCheckable in buffer)
				{
					try
					{
						foreach (var problem in selfCheckable.SelfCheck())
							problems.Add(problem);
					}
					catch (Exception e)
					{
						exceptionOccured = true;
						Debug.LogException(e);
					}
				}
				buffer.Clear();
			}
			// 提示完成
			if (exceptionOccured)
				ShowNotification(new("Completed with exceptions."), 1);
			else if (problems.Count == 0)
				ShowNotification(new("Completed."), 1);
			else
				ShowNotification(new("Completed with problems."), 1);
		}
	}
}
