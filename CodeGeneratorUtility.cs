﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit
{
	public static class CodeGeneratorUtility
	{
		public static string Replace(string code, string startMark, string endMark, string replace, int ident)
		{
			var oldCodeLines = code.Split(Environment.NewLine);
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
				var lines = replace.Split("\n");
				foreach (var t in lines)
				{
					var trimed = t.TrimEnd();
					if (trimed.IsNullOrEmpty()) continue;
					newLines.Add($"{new string('\t', ident)}{trimed}");
				}
				var newCodeLines = new string[oldCodeLines.Length - (endIndex - startIndex - 1) + newLines.Count];
				Array.Copy(oldCodeLines, 0, newCodeLines, 0, startIndex);
				newLines.CopyTo(newCodeLines, startIndex);
				Array.Copy(oldCodeLines, endIndex, newCodeLines, startIndex + newLines.Count, oldCodeLines.Length - endIndex);
				return string.Join(Environment.NewLine, newCodeLines);
			}
			Debug.LogError("Start and/or end marks not found.");
			return code;
		}
	}
}
