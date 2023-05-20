using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace EthansGameKit.Editor
{
	[InitializeOnLoad]
	static class SceneEditorLauncher
	{
		static SceneEditorLauncher()
		{
			SceneEditor.Initialize();
		}
	}

	public abstract class SceneEditor
	{
		static readonly List<SceneEditor> instances = new();
		static readonly List<GameObject> buffer = new();
		protected static Scene ActiveScene => SceneManager.GetActiveScene();
		protected static string EditorScriptName
		{
			get
			{
				var stackTrace = new StackTrace(true);
				var fileName = stackTrace.GetFrame(1).GetFileName();
				var basePath = Environment.CurrentDirectory;
				return Path.GetRelativePath(basePath, fileName);
			}
		}
		protected static void SetDirty(Object obj, bool ping)
		{
			if (ping)
				EditorGUIUtility.PingObject(obj);
			EditorUtility.SetDirty(obj);
		}
		internal static void Initialize()
		{
			EditorSceneManager.sceneOpened += OnSceneLoaded;
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					if (type.IsSubclassOf(typeof(SceneEditor)) && !type.IsAbstract)
						instances.Add((SceneEditor)Activator.CreateInstance(type));
				}
				for (var i = instances.Count; i-- > 0;)
				{
					var instance = instances[i];
					instance.Active = instance.IsTargetScene;
				}
			}
		}
		static void OnSceneLoaded(Scene scene, OpenSceneMode _)
		{
			for (var i = instances.Count; i-- > 0;)
			{
				var instance = instances[i];
				instance.Active = instance.IsTargetScene;
			}
		}
		bool active;
		/// <summary>
		///     GUI面板宽度
		/// </summary>
		protected virtual float Width => 150;
		protected abstract bool AlwaysUpdate { get; }
		/// <summary>
		///     是否是目标场景。高频调用,注意计算量
		/// </summary>
		protected abstract bool IsTargetScene { get; }
		bool Active
		{
			set
			{
				if (active == value) return;
				if (active) Exit();
				else Enter();
				active = value;
				if (active) Enter();
				else Exit();
			}
		}
		protected abstract void OnGUI(SceneView sceneView);
		protected abstract void OnEnter();
		protected abstract void OnExit();
		void Enter()
		{
			SceneView.duringSceneGui += OnSceneGUI;
			try
			{
				OnEnter();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		void Exit()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
			try
			{
				OnExit();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		void OnSceneGUI(SceneView sceneView)
		{
			if (!IsTargetScene) return;
			Handles.BeginGUI();
			GUILayout.BeginArea(new(0, 0, Width, 100000));
			if (GUILayout.Button("跳转至当前场景文件"))
			{
				EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(ActiveScene.path));
			}
			try
			{
				OnGUI(sceneView);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			GUILayout.EndArea();
			Handles.EndGUI();
			GUI.changed = GUI.changed || AlwaysUpdate;
		}
	}
}
