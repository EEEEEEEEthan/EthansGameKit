using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EthansGameKit
{
	/// <summary>
	///     对资源的弱引用
	/// </summary>
	[Serializable]
	public struct WeakResourceReference
	{
		/// <summary>
		///     资源路径.在Inspector上显示为对象引用
		/// </summary>
		[SerializeField]
		public string path;
#if UNITY_EDITOR
		public Object GetPrefabForEditor()
		{
			return UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
		}
#endif
	}
}
