using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

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
		public static bool IsNullOrEmpty(this string @this) => string.IsNullOrEmpty(@this);
		public static string ReplaceAsCode(this string @this, string startMark, string endMark, string replace, string newLine, int ident = 0)
		{
			var oldCodeLines = @this.Split(newLine);
			var startIndex = -1;
			var endIndex = -1;
			for (var i = 0; i < oldCodeLines.Length; i++)
			{
				if (startIndex < 0)
				{
					var idx = oldCodeLines[i].IndexOf(startMark, StringComparison.Ordinal);
					if (idx > 0)
					{
						startIndex = i + 1;
					}
				}
				if (oldCodeLines[i].Contains(endMark))
				{
					endIndex = i;
					break;
				}
			}
			if (startIndex != -1 && endIndex != -1)
			{
				var newLines = new List<string>();
				var lines = replace.Split(newLine);
				foreach (var t in lines)
				{
					newLines.Add($"{new string('\t', ident)}{t}");
				}
				var newCodeLines = new string[oldCodeLines.Length - (endIndex - startIndex - 1) + newLines.Count - 1];
				Array.Copy(oldCodeLines, 0, newCodeLines, 0, startIndex);
				newLines.CopyTo(newCodeLines, startIndex);
				Array.Copy(oldCodeLines, endIndex, newCodeLines, startIndex + newLines.Count, oldCodeLines.Length - endIndex);
				return string.Join(newLine, newCodeLines);
			}
			Debug.LogError("Start and/or end marks not found.");
			return @this;
		}
	}
}
