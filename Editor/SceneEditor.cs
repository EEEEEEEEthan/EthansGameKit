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
		protected static T GetComponent<T>(string path, bool create) where T : Component
		{
			var gameObject = GetGameObject(path, create);
			if (!gameObject) return null;
			if (!gameObject.TryGetComponent<T>(out var component) && create)
				component = gameObject.AddComponent<T>();
			return component;
		}
		protected static GameObject GetGameObject(string path, bool create)
		{
			var pieces = new Queue<string>(path.Split('/'));
			ActiveScene.GetRootGameObjects(buffer);
			var rootName = pieces.Dequeue();
			var count = buffer.Count;
			for (var i = 0; i < count; ++i)
			{
				if (buffer[i].name == rootName)
					return GetGameObject(buffer[i].transform, pieces, create);
			}
			if (create)
			{
				var gameObject = new GameObject(rootName);
				SetDirty(gameObject, true);
				return GetGameObject(gameObject.transform, pieces, true);
			}
			return null;
		}
		protected static GameObject GetGameObject(Transform parent, string path, bool create)
		{
			var pieces = new Queue<string>(path.Split('/'));
			return GetGameObject(parent, pieces, create);
		}
		protected static void LockLocalPosition(Transform transform, Vector3 localPosition)
		{
			if (transform.localPosition != localPosition)
			{
				transform.localPosition = localPosition;
				SetDirty(transform, false);
			}
		}
		protected static void LockLocalRotation(Transform transform, Quaternion localRotation)
		{
			if (transform.localRotation != localRotation)
			{
				transform.localRotation = localRotation;
				SetDirty(transform, false);
			}
		}
		protected static void LockLocalScale(Transform transform, Vector3 localScale)
		{
			if (transform.localScale != localScale)
			{
				transform.localScale = localScale;
				SetDirty(transform, false);
			}
		}
		protected static void LockLocalRotation(Transform transform, Vector3 localEulerAngles)
		{
			LockLocalRotation(transform, Quaternion.Euler(localEulerAngles));
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
		static GameObject GetGameObject(Transform parent, Queue<string> pieces, bool create)
		{
			while (true)
			{
				if (pieces.Count <= 0) return parent.gameObject;
				var childCount = parent.childCount;
				var name = pieces.Dequeue();
				for (var i = 0; i < childCount; ++i)
				{
					var child = parent.GetChild(i);
					if (child.name == name) return GetGameObject(child, pieces, true);
				}
				if (create)
				{
					var gameObject = new GameObject(name);
					parent = gameObject.transform;
					continue;
				}
				return null;
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
