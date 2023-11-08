using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EthansGameKit.Collections;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EthansGameKit.Editor
{
	static class ChildGameObjectReferencer
	{
		class Window : EditorWindow
		{
			[Serializable]
			class Object2Boolean : SerializableDictionary<Object, bool>
			{
			}

			[Serializable]
			class Object2String : SerializableDictionary<Object, string>
			{
			}

			static readonly List<MonoBehaviour> buffer = new();
			[MenuItem("GameObject/" + PackageDefines.packageName + "/Create References for MonoBehaviour", false, 0)]
			static void ShowWindow()
			{
				var window = GetWindow<Window>();
				GetScript(Selection.activeGameObject, out var monoBehaviour);
				window.target = monoBehaviour;
			}
			[MenuItem("GameObject/" + PackageDefines.packageName + "/Create References for MonoBehaviour", true)]
			static bool ValidShowWindow()
			{
				if (!Selection.activeGameObject) return false;
				if (Selection.objects.Length > 1) return false;
				return GetScript(Selection.activeGameObject, out _);
			}
			static MonoScript GetScript(GameObject gameObject, out MonoBehaviour behaviour)
			{
				gameObject.GetComponents(buffer);
				MonoScript script = null;
				behaviour = null;
				foreach (var b in buffer)
				{
					script = MonoScript.FromMonoBehaviour(b);
					if (!script) continue;
					var behaviourScriptPath = AssetDatabase.GetAssetPath(script);
					if (behaviourScriptPath.StartsWith("Assets/"))
						behaviour = b;
				}
				buffer.Clear();
				return script;
			}
			readonly List<Component> components = new();
			[SerializeField] MonoBehaviour target;
			[SerializeField] Object2Boolean selection = new();
			[SerializeField] Object2String rename = new();
			[SerializeField] Object2Boolean folded = new();
			[SerializeField] string region = "autogen";
			[SerializeField] bool awaitReload;
			Vector2 pos;
			async void OnGUI()
			{
				var newTarget = (MonoBehaviour)EditorGUILayout.ObjectField("Target", target, typeof(MonoBehaviour), true);
				if (target != newTarget) selection.Clear();
				target = newTarget;
				if (!target) return;
				region = EditorGUILayout.TextField("Region", region);
				// scroll
				pos = EditorGUILayout.BeginScrollView(pos);
				{
					foreach (var transform in target.transform.IterChildren(true))
					{
						const int singleIndentPixels = 11;
						var indentPixels = transform.GetDepth(target.transform) * singleIndentPixels;
						// 分割线
						GUILayout.Space(3);
						bool expanded;
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Space(indentPixels);
							expanded = folded.GetValueOrDefault(transform.gameObject, true);
							folded[transform.gameObject] = EditorGUILayout.Foldout(expanded, transform.name);
						}
						EditorGUILayout.EndHorizontal();
						if (!expanded) continue;
						drawToggle(transform.gameObject);
						transform.GetComponents(components);
						foreach (var component in components)
							drawToggle(component);
						components.Clear();

						void drawToggle(Object obj)
						{
							EditorGUILayout.BeginHorizontal();
							GUILayout.Space(indentPixels + singleIndentPixels);
							selection[obj] = EditorGUILayout.ToggleLeft(obj.GetType().Name, selection.GetValueOrDefault(obj));
							rename[obj] = EditorGUILayout.TextField(rename.GetValueOrDefault(obj, $"{transform.name}{obj.GetType().Name}"));
							EditorGUILayout.EndHorizontal();
						}
					}
				}
				EditorGUILayout.EndScrollView();
				if (GUILayout.Button("Apply"))
				{
					var script = MonoScript.FromMonoBehaviour(target);
					var scriptPath = AssetDatabase.GetAssetPath(script);
					var scriptText = script.text;
					var builder = new StringBuilder();
					var names = new HashSet<string>();
					foreach (var (obj, selected) in selection)
					{
						if (!selected) continue;
						var name = rename.GetValueOrDefault(obj, obj.name);
						// 提示名字重复
						if (!names.Add(name))
						{
							ShowNotification(new($"Name \"{name}\" is duplicated."), 1);
							return;
						}
						builder.AppendLine($"[SerializeField, EthansGameKit.Attributes.DisplayAs(\"{obj.name}\")] {obj.GetType().FullName} {name};");
					}
					scriptText = scriptText.ReplaceAsCode($"#region {region}", $"#endregion {region}", builder.ToString(), "\r\n", 2);
					File.WriteAllText(scriptPath, scriptText);
					awaitReload = true;
					await Timers.Await(0);
					AssetDatabase.Refresh();
				}
				if (awaitReload)
				{
					var serializeObject = new SerializedObject(target);
					foreach (var (obj, selected) in selection)
					{
						if (!selected) continue;
						var gameObject = obj as GameObject ?? ((Component)obj).gameObject;
						var name = rename.GetValueOrDefault(gameObject, obj.name);
						var property = serializeObject.FindProperty(name);
						if (property is null) return;
						property.objectReferenceValue = obj;
					}
					serializeObject.ApplyModifiedProperties();
					awaitReload = false;
					ShowNotification(new("Completed."), 1);
				}
			}
		}
	}
}
