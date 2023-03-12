using System.IO;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool Write(this BinaryWriter @this, Vector3 vector)
		{
			@this.Write(vector.x);
			@this.Write(vector.y);
			@this.Write(vector.z);
			return true;
		}
		public static Vector3 ReadVector3(this BinaryReader @this)
		{
			return new(@this.ReadSingle(), @this.ReadSingle(), @this.ReadSingle());
		}
		public static bool Write(this BinaryWriter @this, Quaternion quaternion)
		{
			@this.Write(quaternion.x);
			@this.Write(quaternion.y);
			@this.Write(quaternion.z);
			@this.Write(quaternion.w);
			return true;
		}
		public static Quaternion ReadQuaternion(this BinaryReader @this)
		{
			return new(@this.ReadSingle(), @this.ReadSingle(), @this.ReadSingle(), @this.ReadSingle());
		}
	}
}
