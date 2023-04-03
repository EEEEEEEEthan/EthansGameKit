using System;
using UnityEngine;

namespace EthansGameKit.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class InspectorReadOnlyAttribute : PropertyAttribute
	{
	}
}