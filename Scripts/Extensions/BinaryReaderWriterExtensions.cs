using System;
using System.IO;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static IDisposable BeginBlock(this BinaryWriter writer) => new BlockWriter(writer);

		public static IDisposable BeginBlock(this BinaryReader reader) => new BlockReader(reader);

		public static IDisposable BeginBlock(this BinaryWriter writer, string key)
		{
			writer.Write(key);
			return new BlockWriter(writer);
		}

		public static IDisposable BeginBlock(this BinaryReader reader, string key)
		{
			var actualKey = reader.ReadString();
			if (actualKey != key)
				throw new($"Expected block key '{key}' but got '{actualKey}'");
			return new BlockReader(reader);
		}

		readonly struct BlockWriter : IDisposable
		{
			readonly BinaryWriter writer;
			readonly int startPosition;

			public BlockWriter(BinaryWriter writer)
			{
				this.writer = writer;
				startPosition = (int)writer.BaseStream.Position;
				writer.Write(0);
			}

			public void Dispose()
			{
				var endPosition = (int)writer.BaseStream.Position;
				var length = endPosition - startPosition - sizeof(int);
				writer.Seek(startPosition, SeekOrigin.Begin);
				writer.Write(length);
				writer.Seek(endPosition, SeekOrigin.Begin);
			}
		}

		readonly struct BlockReader : IDisposable
		{
			readonly BinaryReader reader;
			readonly int endPosition;

			public BlockReader(BinaryReader reader)
			{
				this.reader = reader;
				endPosition = reader.ReadInt32() + (int)reader.BaseStream.Position;
			}

			public void Dispose()
			{
				reader.BaseStream.Position = endPosition;
			}
		}
	}
}