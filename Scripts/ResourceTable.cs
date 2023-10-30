using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit
{
	static class ResourceTable
	{
		#region autogen
		public static class Resources
		{
			public static class EthansGameKit
			{
				public static class Materials
				{
					public static ITimedCache<Material> Outline_Material { get; } = new ResourceCache<Material>("EthansGameKit/Materials/Outline");
					public static IReadOnlyDictionary<string, ITimedCache> AllAssets { get; } = new Dictionary<string, ITimedCache>
					{
						["Outline_Material"] = Outline_Material,
					};
				}
			}
		}
		#endregion autogen
	}
}
