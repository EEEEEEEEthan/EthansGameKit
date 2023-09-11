using System.IO;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit.Editor
{
	class ScriptCreator : ScriptableSingleton<ScriptCreator>
	{
		static readonly string[] prefixes =
		{
			"Assets/EthansGameKit/Editor/ScriptTemplates/", "Packages/Ethan's Game Kit/Editor/ScriptTemplates/",
		};
		[MenuItem("Assets/Create/C# Script/Ethan's Game Kit/MonoBehaviour")]
		static void CreateMonoBehaviour()
		{
			const string fileName = "DefaultMonoBehaviour.txt";
			foreach (var prefix in prefixes)
			{
				var path = prefix + fileName;
				if (File.Exists(path))
				{
					ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "NewMonoBehaviour.cs");
					return;
				}
			}
			Debug.LogError("template file missing.");
		}
		[MenuItem("Assets/Create/C# Script/Ethan's Game Kit/EnumLikedStruct")]
		static void CreateEnumLikedStruct()
		{
			const string fileName = "EnumLikedStruct.txt";
			foreach (var prefix in prefixes)
			{
				var path = prefix + fileName;
				if (File.Exists(path))
				{
					ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, "NewEnumLikedStruct.cs");
					return;
				}
			}
		}
	}
}
