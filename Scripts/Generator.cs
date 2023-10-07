using System;
using System.Runtime.InteropServices;
using EthansGameKit.VText;
using EthansGameKit.VText.VText;
using UnityEditor;
using UnityEngine;

namespace EthansGameKit
{
	public class Text3DGenerator : IDisposable
	{
		[DllImport("VText")]
		static extern IntPtr OpenFont([In] IntPtr fontFilename);
		[DllImport("VText")]
		static extern void CloseFont([In] IntPtr fontHandle);
		[DllImport("VText")]
		static extern bool GetFontBounds([Out] IntPtr b, [In] IntPtr fontHandle);
		IntPtr fontHandle;
		bool disposed;
		public Text3DGenerator(string fontFilePath)
		{
			var filePathPtr = Marshal.StringToHGlobalAnsi(fontFilePath);
			fontHandle = OpenFont(Marshal.StringToHGlobalAnsi(fontFilePath));
			Marshal.FreeHGlobal(filePathPtr);
		}
		public void Dispose()
		{
			if (disposed) return;
			disposed = true;
			CloseFont(fontHandle);
			fontHandle = default;
		}
		public void Generate(char c, float depth = 0.1f, bool backFace = true)
		{
			var info = new VGlyphInfo(fontHandle, c);
			FontBounds bounds = default;
			var ip = Marshal.AllocHGlobal(Marshal.SizeOf(bounds));
			if (GetFontBounds(ip, fontHandle))
			{
				bounds = (FontBounds)Marshal.PtrToStructure(ip, typeof(FontBounds));
			}
			else
			{
				Debug.LogWarning("Get bounds failed");
			}
			Marshal.FreeHGlobal(ip);
			var mesh = info.GetMesh(default, new(bounds.maxX - bounds.minX, bounds.maxY - bounds.minY), new() { Depth = depth, Backface = backFace });
			AssetDatabase.CreateAsset(mesh, $"outputPath/{c}.asset");
		}
	}
}
