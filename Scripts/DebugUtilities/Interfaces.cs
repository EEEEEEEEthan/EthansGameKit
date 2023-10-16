using UnityEngine;

namespace EthansGameKit.DebugUtilities
{
	/// <summary>
	///     继承这个接口的MonoBehaviour可以在鼠标指向时显示一个调试信息.sceneView/gameView都可以
	/// </summary>
	public interface IDebugMessageProvider
	{
		public static bool Enabled
		{
			get => DebugMessageDrawer.Enabled;
			set => DebugMessageDrawer.Enabled = value;
		}
		/// <summary>
		///     调试信息
		/// </summary>
		/// <param name="hit">鼠标指向的物体</param>
		/// <param name="box">box信息</param>
		/// <param name="debugMessage">文字信息</param>
		void GetDebugMessage(RaycastHit hit, out Matrix4x4 box, out string debugMessage);
	}

	/// <summary>
	///     继承这个接口的MonoBehaviour可以在ctrl+右键时显示一个调试GUI
	/// </summary>
	public interface IDebugGUIProvider
	{
		public static bool Enabled
		{
			get => DebugGUIDrawer.Enabled;
			set => DebugGUIDrawer.Enabled = value;
		}
		/// <summary>
		///     GUI
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="title"></param>
		void OnDebugGUI(out float width, out float height, out string title);
		void ShowDebugGUI()
		{
			DebugGUIDrawer.Show(this);
		}
		void HideDebugGUI()
		{
			DebugGUIDrawer.Hide(this);
		}
	}
}
