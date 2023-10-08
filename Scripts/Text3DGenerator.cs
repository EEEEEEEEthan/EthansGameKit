using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EthansGameKit.VText;
using EthansGameKit.VText.VText;
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
		readonly FontBounds bounds;
		IntPtr fontHandle;
		bool disposed;
		public Text3DGenerator(string fontFilePath)
		{
			var filePathPtr = Marshal.StringToHGlobalAnsi(fontFilePath);
			fontHandle = OpenFont(Marshal.StringToHGlobalAnsi(fontFilePath));
			Marshal.FreeHGlobal(filePathPtr);
			var boundsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(bounds));
			if (GetFontBounds(boundsPtr, fontHandle))
				bounds = (FontBounds)Marshal.PtrToStructure(boundsPtr, typeof(FontBounds));
			else
				Debug.LogWarning("Get bounds failed");
			Marshal.FreeHGlobal(boundsPtr);
		}
		public void Dispose()
		{
			if (disposed) return;
			disposed = true;
			CloseFont(fontHandle);
			fontHandle = default;
		}
		public void BuildMesh(Mesh mesh, char c, float depth = 0.1f, bool backFace = true)
		{
			if (char.IsControl(c)) throw new ArgumentException("Control characters are not supported");
			var info = new VGlyphInfo(fontHandle, c);
			info.GetMesh(mesh, default, new(bounds.maxX - bounds.minX, bounds.maxY - bounds.minY), new() { Depth = depth, Backface = backFace });
			//return mesh;
			/*
			mesh.vertices = newMesh.vertices;
			mesh.subMeshCount = newMesh.subMeshCount;
			for (var i = 0; i < newMesh.subMeshCount; i++)
			{
				mesh.SetTriangles(newMesh.GetTriangles(i), i);
			}
			newMesh.Destroy();
			*/
		}
		~Text3DGenerator()
		{
			Dispose();
		}
	}

	[Serializable]
	public class RuntimeText3DManager
	{
		[SerializeField] string fontFilePath;
		[SerializeField] float depth;
		[SerializeField] bool backFace;
		Dictionary<char, WeakReference<Mesh>> meshes = new();
		Text3DGenerator generator;
		Text3DGenerator Generator => generator ??= new(fontFilePath);
		public RuntimeText3DManager(string fontFilePath, float depth, bool backFace)
		{
			this.depth = depth;
			this.backFace = backFace;
			this.fontFilePath = fontFilePath;
		}
		public Mesh GetMesh(char c)
		{
			Mesh mesh;
			if (meshes.TryGetValue(c, out var weakReference))
			{
				if (weakReference.TryGetTarget(out mesh) && mesh)
				{
					return mesh;
				}
			}
			mesh = new();
			Generator.BuildMesh(mesh, c, depth, backFace);
			meshes[c] = new(mesh);
			return mesh;
		}
		public void BuildMesh(Mesh mesh, string str)
		{
			mesh.Clear();
			var position = Vector3.zero;
			var vertices = new List<Vector3>();
			var triangles = new List<int>[]
			{
				new(),
				new(),
				new(),
			};
			var characterTriangles = new List<int>();
			foreach (var c in str)
			{
				var characterMesh = GetMesh(c);
				var count = vertices.Count;
				var characterVertices = characterMesh.vertices;
				for (var i = 0; i < characterVertices.Length; i++)
				{
					vertices.Add(characterVertices[i] + position);
				}
				for (var subMeshIndex = 0; subMeshIndex < 3; ++subMeshIndex)
				{
					characterMesh.GetTriangles(characterTriangles, subMeshIndex);
					for (var i = 0; i < characterTriangles.Count; i++)
					{
						triangles[subMeshIndex].Add(characterTriangles[i] + count);
					}
					characterTriangles.Clear();
				}
				position.x += characterMesh.bounds.size.x;
			}
			mesh.SetVertices(vertices);
			mesh.subMeshCount = 3;
			mesh.SetTriangles(triangles[0], 0);
			mesh.SetTriangles(triangles[1], 1);
			mesh.SetTriangles(triangles[2], 2);
		}
	}
}
