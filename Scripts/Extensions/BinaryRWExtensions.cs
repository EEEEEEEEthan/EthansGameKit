using System;
using System.Collections.Generic;
using System.IO;
using EthansGameKit.CachePools;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	readonly struct ReaderBlockScope : IDisposable
	{
		readonly Stream stream;
		readonly int endPosition;
		public ReaderBlockScope(BinaryReader reader)
		{
			stream = reader.BaseStream;
			var length = reader.ReadInt32();
			var position = reader.BaseStream.Position;
			endPosition = (int)position + length;
		}
		void IDisposable.Dispose()
		{
			if (stream.Position != endPosition)
			{
				throw new("ReaderBlockScope: stream.Position != endPosition");
			}
			stream.Position = endPosition;
		}
	}

	readonly struct WriterBlockScope : IDisposable
	{
		readonly BinaryWriter writer;
		readonly int beginPosition;
		public WriterBlockScope(BinaryWriter writer)
		{
			this.writer = writer;
			beginPosition = (int)writer.BaseStream.Position;
			writer.Write(0);
		}
		void IDisposable.Dispose()
		{
			var endPosition = (int)writer.BaseStream.Position;
			var length = endPosition - beginPosition - sizeof(int);
			writer.BaseStream.Position = beginPosition;
			writer.Write(length);
			writer.BaseStream.Position = endPosition;
		}
	}

	public static partial class Extensions
	{
		public static void Write(this BinaryWriter @this, Vector3 vector)
		{
			@this.Write(vector.x);
			@this.Write(vector.y);
			@this.Write(vector.z);
		}
		public static Vector3 ReadVector3(this BinaryReader @this)
		{
			return new(@this.ReadSingle(), @this.ReadSingle(), @this.ReadSingle());
		}
		public static void Write(this BinaryWriter @this, Vector2Int vector)
		{
			@this.Write(vector.x);
			@this.Write(vector.y);
		}
		public static Vector2Int ReadVector2Int(this BinaryReader @this)
		{
			return new(@this.ReadInt32(), @this.ReadInt32());
		}
		public static void Write(this BinaryWriter @this, Quaternion quaternion)
		{
			@this.Write(quaternion.x);
			@this.Write(quaternion.y);
			@this.Write(quaternion.z);
			@this.Write(quaternion.w);
		}
		public static Quaternion ReadQuaternion(this BinaryReader @this)
		{
			return new(@this.ReadSingle(), @this.ReadSingle(), @this.ReadSingle(), @this.ReadSingle());
		}
		public static void Write(this BinaryWriter @this, Vector3Int vector)
		{
			@this.Write(vector.x);
			@this.Write(vector.y);
			@this.Write(vector.z);
		}
		public static Vector3Int ReadVector3Int(this BinaryReader @this)
		{
			return new(@this.ReadInt32(), @this.ReadInt32(), @this.ReadInt32());
		}
		public static void Write(this BinaryWriter @this, IReadOnlyCollection<Vector2Int> enumerable)
		{
			@this.Write(enumerable.Count);
			foreach (var element in enumerable)
				@this.Write(element);
		}
		public static Queue<Vector2Int> ReadVector2IntQueue(this BinaryReader @this)
		{
			var queue = QueuePool<Vector2Int>.Generate();
			@this.ReadVector2IntQueue(queue);
			return queue;
		}
		public static Queue<Vector2Int> ReadVector2IntQueue(this BinaryReader @this, Queue<Vector2Int> queue)
		{
			for (var i = @this.ReadInt32(); i-- > 0;)
				queue.Enqueue(@this.ReadVector2Int());
			return queue;
		}
		public static IDisposable BeginBlock(this BinaryWriter @this)
		{
			return new WriterBlockScope(@this);
		}
		public static IDisposable BeginBlock(this BinaryReader @this)
		{
			return new ReaderBlockScope(@this);
		}
	}
}
