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
		[MenuItem("Tools/" + PackageDefines.packageName + "/自检 &F5")]
		static void ShowWindow()
		{
			var window = GetWindow<SelfCheckWindow>("自检");
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
			if (GUILayout.Button("自检") || !checkedOnce)
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
				if (GUILayout.Button("带我看看", GUILayout.Width(70)))
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
				ShowNotification(new("自检完成并伴有异常"), 1);
			else if (problems.Count == 0)
				ShowNotification(new("自检完成，未发现问题"), 1);
			else
				ShowNotification(new("自检完成，发现问题"), 1);
		}
	}
}
