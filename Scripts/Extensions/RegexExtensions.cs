using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static bool TryMatch(this Regex @this, string input, out Match match)
		{
			match = @this.Match(input);
			return match.Success;
		}
	}
}
