using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace ChatGPTUtility.Editor
{
	public class CommandWindow : EditorWindow
	{
		const string commandMenuItem = "Command/Execute";
		const string TempFilePath = "Assets/IgnoredAssets/ChatGPTUtility/Editor/AICommandTemp.cs";
		static bool FileExists => File.Exists(TempFilePath);
		static string Template =>
			"using UnityEditor;\n" +
			"// 补充usings\n" +
			"static class ChatGPT_DO_TASK\n" +
			"{\n" +
			$"    [MenuItem(\"{commandMenuItem}\")]\n" +
			"    static void Execute()\n" +
			"    {\n" +
			"        // 填写代码\n" +
			"    }\n" +
			"}";
		static Message SystemMessage =>
			new()
			{
				role = "system",
				content = "我需要你帮我写一些Unity Editor功能在下面代码的Execute()方法里.对于我之后的每一条发言,你需要针对我描述的功能补充我的脚本代码.\n" +
						"脚本代码模板如下:\n" +
						"```\n" +
						Template +
						"```\n" +
						"不要修改Execute方法的标签\n" +
						"补全你需要用到的任何usings\n" +
						"要求你回复的格式如下:\n" +
						" - 回复包含模板在内的完整代码\n" +
						" - 只回复我代码,不需要任何解释说明\n" +
						" - 不包含注释\n" +
						" - 不包含任何空行\n",
			};
		[MenuItem("Window/ChatGPT/Command")]
		public static void ShowWindow()
		{
			GetWindow<CommandWindow>("Command");
		}
		static void DrawSeparator()
		{
			EditorGUILayout.Space(2);
			EditorGUI.DrawRect(GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight * 0.1f), Color.gray);
			EditorGUILayout.Space(2);
		}
		static void DeleteFile()
		{
			if (!FileExists) return;
			AssetDatabase.DeleteAsset(TempFilePath);
		}
		static bool ExecuteFile()
		{
			if (!FileExists) return false;
			EditorApplication.ExecuteMenuItem(commandMenuItem);
			return true;
		}
		static void CreateScriptAsset(string code)
		{
			const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
			var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", flags);
			Assert.IsNotNull(method);
			method.Invoke(null, new object[] { TempFilePath, code });
		}
		[SerializeField] bool debug;
		[SerializeField] GUIStyle messageStyle = new();
		[SerializeField] List<Message> messages = new();
		[SerializeField] string inputField = "帮我创建10个方块";
		[SerializeField] Vector2 scrollPosition;
		void Awake()
		{
			messages = new()
			{
				SystemMessage,
			};
			if (FileExists)
				messages.Add(new()
				{
					role = "assistant",
					content = File.ReadAllText(TempFilePath),
				});
		}
		void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("clear", GUILayout.Width(80), GUILayout.Height(20)))
			{
				messages.Clear();
				GUI.FocusControl(null);
			}
			if (!FileExists) GUI.enabled = false;
			if (GUILayout.Button("delete", GUILayout.Width(80), GUILayout.Height(20)))
			{
				messages.Clear();
				DeleteFile();
				GUI.FocusControl(null);
			}
			if (GUILayout.Button("execute", GUILayout.Width(80), GUILayout.Height(20)))
			{
				ExecuteFile();
				GUI.FocusControl(null);
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && Event.current.control)
			{
				Send(inputField.Trim());
				inputField = "";
				GUI.FocusControl(null);
				Event.current.Use();
			}
			EditorGUILayout.BeginVertical();
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			DrawMessages();
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginHorizontal();
			DrawInputArea();
			if (GUILayout.Button("send", GUILayout.Width(50), GUILayout.Height(20)))
			{
				Send(inputField.Trim());
				inputField = "";
				GUI.FocusControl(null);
			}
			EditorGUILayout.EndHorizontal();
		}
		void DrawMessages()
		{
			messageStyle = new(GUI.skin.label)
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
			var textAreaStyle = new GUIStyle(GUI.skin.textArea)
			{
				wordWrap = true,
			};
			EditorGUILayout.BeginVertical(GUILayout.Height(50));
			inputField = EditorGUILayout.TextArea(inputField, textAreaStyle, GUILayout.ExpandHeight(true));
			EditorGUILayout.EndVertical();
		}
		void Send(string content)
		{
			messages.Clear();
			messages.Add(SystemMessage);
			messages.Add(new()
			{
				role = "user",
				content = content,
			});
			//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
			//API.SendRequest("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", messages.ToArray(), callback);
			API.SendRequest("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", messages.ToArray(), callback);

			void callback(string response)
			{
				if (response.StartsWith("```"))
					response = response.Substring(3, response.Length - 6);
				messages.Add(new()
				{
					role = "assistant",
					content = response.Trim(),
				});
				Repaint();
				CreateScriptAsset(response);
			}
		}
	}
}
