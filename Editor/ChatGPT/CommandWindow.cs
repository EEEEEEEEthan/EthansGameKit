using System.IO;
using System.Reflection;
using EthansGameKit.ChatGPT;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor.ChatGPT
{
	sealed class CommandWindow : ChatGPTWindow
	{
		const string commandMenuItem = "Command/Execute";
		const string TempFilePath = "Assets/ChatGPTUtility/AICommandTemp.cs";
		static bool FileExists => File.Exists(TempFilePath);
		static string Template =>
			"using UnityEditor;\n" +
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
						"可以补全usings\n" +
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
		static void DeleteFile()
		{
			if (!FileExists) return;
			AssetDatabase.DeleteAsset(TempFilePath);
		}
		static void ExecuteFile()
		{
			if (!FileExists) return;
			EditorApplication.ExecuteMenuItem(commandMenuItem);
		}
		protected override string DefaultInput => "帮我把我选中的GameObject命名为`GameObject{i}`, {i}是我选的序号";
		new void Awake()
		{
			Messages.Add(SystemMessage);
			if (FileExists)
			{
				Messages.Add(
					new()
					{
						role = "assistant",
						content = File.ReadAllText(TempFilePath),
					}
				);
			}
		}
		void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			if (!FileExists) GUI.enabled = false;
			if (GUILayout.Button("delete", GUILayout.Width(80), GUILayout.Height(20)))
			{
				Messages.Clear();
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
			DrawChat();
		}
		protected override void OnResponse(string response)
		{
			const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
			var method = typeof(ProjectWindowUtil).GetMethod("CreateScriptAssetWithContent", flags);
			//Assert.IsNotNull(method);
			method.Invoke(null, new object[] { TempFilePath, response });
		}
		protected override void OnSend()
		{
			Messages.Clear();
		}
	}
}
