using EthansGameKit.Internal;

namespace EthansGameKit
{
	public static class Singletons
	{
		public static T Get<T>() where T : UnityEngine.Object
		{
			return SingletonReferencer.Get<T>();
		}
		public static void Set<T>(T value) where T : UnityEngine.Object
		{
			SingletonReferencer.Set(value);
		}
	}
}
