using System.Collections.Generic;
using EthansGameKit.ChatGPT;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor.ChatGPT
{
	internal abstract class ChatGPTWindow : EditorWindow
	{
		static void DrawSeparator()
		{
			EditorGUILayout.Space(2);
			EditorGUI.DrawRect(GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight * 0.1f), Color.gray);
			EditorGUILayout.Space(2);
		}
		[SerializeField] GUIStyle messageStyle = new();
		[SerializeField] List<Message> messages = new();
		[SerializeField] string inputField;
		[SerializeField] Vector2 historyScrollPosition;
		[SerializeField] Vector2 inputAreaScrollPosition;
		protected abstract string DefaultInput { get; }
		protected List<Message> Messages => messages;
		protected void Awake()
		{
			inputField = DefaultInput;
		}
		protected void DrawChat()
		{
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && Event.current.control)
			{
				Send(inputField.Trim());
				inputField = "";
				GUI.FocusControl(null);
				Event.current.Use();
			}
			EditorGUILayout.BeginVertical();
			historyScrollPosition = EditorGUILayout.BeginScrollView(historyScrollPosition);
			DrawMessages();
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			DrawInputArea();
		}
		void DrawMessages()
		{
			messageStyle = new(GUI.skin.textArea)
			{
				wordWrap = true,
				margin = new(0, 0, 5, 5),
			};
			for (var i = 0; i < messages.Count; i++)
			{
				var message = messages[i];
				EditorGUILayout.TextArea(message.role, EditorStyles.boldLabel);
				messageStyle.fixedWidth = position.width;
				EditorGUILayout.TextArea(message.content, messageStyle);
				if (i != messages.Count - 1) DrawSeparator();
			}
		}
		void DrawInputArea()
		{
			EditorGUILayout.BeginHorizontal();
			var textAreaStyle = new GUIStyle(GUI.skin.textArea)
			{
				wordWrap = true,
			};
			inputAreaScrollPosition = EditorGUILayout.BeginScrollView(inputAreaScrollPosition); // 最大高度为200
			EditorGUILayout.BeginVertical(GUILayout.Height(50));
			inputField = EditorGUILayout.TextArea(inputField, textAreaStyle, GUILayout.ExpandHeight(true));
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
			if (GUILayout.Button("send", GUILayout.Width(50), GUILayout.Height(20)))
			{
				Send(inputField.Trim());
				inputField = "";
				GUI.FocusControl(null);
			}
			EditorGUILayout.EndHorizontal();
		}
		void Send(string content)
		{
			messages.Add(
				new()
				{
					role = "user",
					content = content,
				}
			);
			historyScrollPosition = new(historyScrollPosition.x, float.MaxValue);
			OnSend();
			Repaint();
			API.SendRequest("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", messages.ToArray(), callback);

			void callback(string response)
			{
				historyScrollPosition = new(historyScrollPosition.x, float.MaxValue);
				OnResponse(response);
				Repaint();
			}
		}
		protected abstract void OnSend();
		protected abstract void OnResponse(string response);
	}
}