using System;
using UnityEngine;

namespace EthansGameKit.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DisplayAsAttribute : PropertyAttribute
	{
		public string Name { get; }
		public bool ShowCodeName { get; }
		public DisplayAsAttribute(string name, bool showCodeName = true)
		{
			Name = name;
			ShowCodeName = showCodeName;
		}
	}
}
