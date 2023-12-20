using System.Collections.Generic;
using EthansGameKit.Collections.Wrappers;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static DictToDict<TOldKey, TOldValue, TNewKey, TNewValue> WrapAsDict<TOldKey, TOldValue, TNewKey, TNewValue>(
			this IDictionary<TOldKey, TOldValue> @this,
			IConverter<TOldKey, TNewKey> oldKey2NewKey,
			IConverter<TNewKey, TOldKey> newKey2OldKey,
			IConverter<TOldValue, TNewValue> oldValue2NewValue,
			IConverter<TNewValue, TOldValue> newValue2OldValue
		)
		{
			return new(@this, oldKey2NewKey, newKey2OldKey, oldValue2NewValue, newValue2OldValue);
		}
	}
}
