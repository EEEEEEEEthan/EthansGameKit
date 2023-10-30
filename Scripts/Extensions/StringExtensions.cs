using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool IsMatch(this string @this, string pattern, out Match match)
		{
			match = Regex.Match(@this, pattern);
			return match.Success;
		}
		public static bool IsMatch(this string @this, string pattern)
		{
			var match = Regex.Match(@this, pattern);
			return match.Success;
		}
		public static bool IsNullOrEmpty(this string @this)
		{
			return string.IsNullOrEmpty(@this);
		}
	}
}
