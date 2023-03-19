using System.IO;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
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
	}
}
