using System;

namespace Utilities
{
	struct Timer : IComparable<Timer>, IEquatable<Timer>
	{
		public Action callback;
		public bool crossScene;
		public double time;
		public uint id;
		public int CompareTo(Timer other)
		{
			var result = time.CompareTo(other.time);
			if (result == 0)
				return id.CompareTo(other.id);
			return result;
		}
		public bool Equals(Timer other)
		{
			return id == other.id;
		}
	}
}
