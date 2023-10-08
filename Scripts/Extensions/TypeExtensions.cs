using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static FieldInfo GetField(this Type @this, string name, BindingFlags bindingFlags, bool reverse)
		{
			if (reverse)
			{
				var type = @this;
				while (type != null)
				{
					var field = type.GetField(name, bindingFlags);
					if (field != null) return field;
					type = type.BaseType;
				}
			}
			else
			{
				return @this.GetField(name, bindingFlags);
			}
			return null;
		}
		public static PropertyInfo GetProperty(this Type @this, string name, BindingFlags bindingFlags, bool reverse)
		{
			if (reverse)
			{
				var type = @this;
				while (type != null)
				{
					var property = type.GetProperty(name, bindingFlags);
					if (property != null) return property;
					type = type.BaseType;
				}
			}
			else
			{
				return @this.GetProperty(name, bindingFlags);
			}
			return null;
		}
	}
}
