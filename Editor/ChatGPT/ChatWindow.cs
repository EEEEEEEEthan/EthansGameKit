using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor.ChatGPT
{
	sealed class ChatWindow : ChatGPTWindow
	{
		[MenuItem("Window/ChatGPT/Chat")]
		public static void ShowWindow()
		{
			GetWindow<ChatWindow>("Chat");
		}
		protected override string DefaultInput => "你好";
		protected override void OnResponse(string response)
		{
			Messages.Add(
				new()
				{
					role = "system",
					content = response.Trim(),
				}
			);
		}
		protected override void OnSend()
		{
		}
		void OnGUI()
		{
			if (GUILayout.Button("Clear"))
				Messages.Clear();
			DrawChat();
		}
	}
}
