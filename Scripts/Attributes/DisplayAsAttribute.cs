using System;
using UnityEngine;

namespace EthansGameKit.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DisplayAsAttribute : PropertyAttribute
	{
		public DisplayAsAttribute(string name, bool showCodeName = true)
		{
			Name = name;
			ShowCodeName = showCodeName;
		}

		public string Name { get; }
		public bool ShowCodeName { get; }
	}
}