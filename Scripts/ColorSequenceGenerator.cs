using System;
using System.Collections.Generic;
using UnityEngine;

namespace EthansGameKit
{
	/// <summary>
	///     颜色序列生成器。生成与已有颜色差异最大的颜色
	/// </summary>
	public class ColorSequenceGenerator
	{
		readonly List<Color> colors = new();
		readonly Func<Color, bool> colorFilter;
		readonly int sampleFreq;
		/// <param name="firstColor">生成器中的第一个颜色</param>
		/// <param name="sampleFreq">采样频率</param>
		public ColorSequenceGenerator(Color firstColor, int sampleFreq)
		{
			this.sampleFreq = sampleFreq.Clamped(0, 5);
			colors.Add(firstColor);
		}
		/// <param name="firstColor">生成器中的第一个颜色</param>
		/// <param name="sampleFreq">采样频率</param>
		/// <param name="colorFilter">颜色筛选器</param>
		public ColorSequenceGenerator(Color firstColor, int sampleFreq, Func<Color, bool> colorFilter)
		{
			this.sampleFreq = sampleFreq.Clamped(0, 5);
			colors.Add(firstColor);
			this.colorFilter = colorFilter;
		}
		public Color GetColor(int index)
		{
			while (colors.Count <= index)
			{
				colors.Add(FindNextColor());
			}
			return colors[index];
		}
		float GetMinDistance(Color input)
		{
			var minDistance = float.MaxValue;
			foreach (var color in colors)
			{
				var distance =
					(color.r - input.r) * (color.r - input.r) +
					(color.g - input.g) * (color.g - input.g) +
					(color.b - input.b) * (color.b - input.b);
				if (distance < minDistance) minDistance = distance;
			}
			return minDistance;
		}
		Color FindNextColor()
		{
			var stepCount = colors.Count << sampleFreq;
			var stepLength = 1f / stepCount;
			var maxDistance = 0f;
			var targetColor = Color.black;
			for (var i = 0; i <= stepCount; ++i)
			{
				for (var j = 0; j <= stepCount; ++j)
				{
					for (var k = 0; k <= stepCount; ++k)
					{
						var color = new Color(i * stepLength, j * stepLength, k * stepLength);
						if (colorFilter != null && !colorFilter(color)) continue;
						var distance = GetMinDistance(color);
						if (distance > maxDistance)
						{
							maxDistance = distance;
							targetColor = color;
						}
					}
				}
			}
			return targetColor;
		}
	}
}
