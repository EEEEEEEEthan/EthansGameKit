using System;
using System.Collections.Generic;
using EthansGameKit.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace EthansGameKit.Components
{
	public class FlowLayoutGroup : LayoutGroup
	{
		[SerializeField, DisplayAs("间距")] Vector2 spacing = new(1, 2);
		[SerializeField, DisplayAs("以行间距为固定行高")] bool fixedLineHeight;
		public Vector2 Spacing
		{
			get => spacing;
			set
			{
				if (spacing == value) return;
				spacing = value;
				SetDirty();
			}
		}
		public bool FixedLineHeight
		{
			get => fixedLineHeight;
			set
			{
				if (fixedLineHeight == value) return;
				fixedLineHeight = value;
				SetDirty();
			}
		}
		void OnDrawGizmosSelected()
		{
			var rect = rectTransform.rect;
			rect.x += padding.left;
			rect.y += padding.bottom;
			rect.width -= padding.left + padding.right;
			rect.height -= padding.top + padding.bottom;
			// 画出边框
			Gizmos.color = Color.green;
			Gizmos.DrawLine(rectTransform.TransformPoint(new Vector2(rect.xMin, rect.yMin)), rectTransform.TransformPoint(new Vector2(rect.xMax, rect.yMin)));
			Gizmos.DrawLine(rectTransform.TransformPoint(new Vector2(rect.xMax, rect.yMin)), rectTransform.TransformPoint(new Vector2(rect.xMax, rect.yMax)));
			Gizmos.DrawLine(rectTransform.TransformPoint(new Vector2(rect.xMax, rect.yMax)), rectTransform.TransformPoint(new Vector2(rect.xMin, rect.yMax)));
			Gizmos.DrawLine(rectTransform.TransformPoint(new Vector2(rect.xMin, rect.yMax)), rectTransform.TransformPoint(new Vector2(rect.xMin, rect.yMin)));
		}
		public override void CalculateLayoutInputHorizontal()
		{
			CalculateLayoutInput();
		}
		public override void CalculateLayoutInputVertical()
		{
			CalculateLayoutInput();
		}
		public override void SetLayoutHorizontal()
		{
		}
		public override void SetLayoutVertical()
		{
		}
		void CalculateLayoutInput()
		{
			List<(List<RectTransform> lineElements, Vector2 lineSize)> getLines(out Vector2 fullSize)
			{
				var width = rectTransform.rect.width - padding.right - padding.left;
				var lineElements = new List<RectTransform>();
				var lineSize = new Vector2();
				var result = new List<(List<RectTransform> lineElements, Vector2 lineSize)>();
				fullSize = default;
				foreach (var child in rectChildren)
				{
					var sizeDelta = child.sizeDelta;
					if (lineSize.x > 0 && lineSize.x + sizeDelta.x > width)
					{
						if (fixedLineHeight) lineSize.y = spacing.y;
						result.Add((lineElements, lineSize));
						fullSize.x = Mathf.Max(fullSize.x, lineSize.x);
						fullSize.y += lineSize.y + spacing.y;
						lineElements = new();
						lineSize = new();
					}
					lineElements.Add(child);
					lineSize.x += sizeDelta.x + (lineSize.x <= 0 ? 0 : spacing.x);
					lineSize.y = Mathf.Max(lineSize.y, sizeDelta.y);
				}
				result.Add((lineElements, lineSize));
				fullSize.x = Mathf.Max(fullSize.x, lineSize.x);
				fullSize.y += lineSize.y;
				return result;
			}
			base.CalculateLayoutInputHorizontal();
			var list = getLines(out var fullSize);
			var myRect = rectTransform.rect;
			switch (childAlignment)
			{
				case TextAnchor.UpperLeft:
				{
					var y = (float)padding.top;
					foreach (var (lineElements, lineSize) in list)
					{
						var x = (float)padding.left;
						foreach (var recTransform in lineElements)
						{
							SetChildAlongAxis(recTransform, 0, x);
							SetChildAlongAxis(recTransform, 1, y);
							x += recTransform.sizeDelta.x + spacing.x;
						}
						y += lineSize.y + spacing.y;
					}
					break;
				}
				case TextAnchor.UpperCenter:
				{
					var y = (float)padding.top;
					var fullWidth = myRect.width - padding.right - padding.left;
					foreach (var (lineElements, lineSize) in list)
					{
						var xOffset = (fullWidth - lineSize.x) / 2;
						var x = padding.left + xOffset;
						foreach (var recTransform in lineElements)
						{
							SetChildAlongAxis(recTransform, 0, x);
							SetChildAlongAxis(recTransform, 1, y);
							x += recTransform.sizeDelta.x + spacing.x;
						}
						y += lineSize.y + spacing.y;
					}
					break;
				}
				case TextAnchor.UpperRight:
				{
					var y = (float)padding.top;
					var fullWidth = myRect.width - padding.right - padding.left;
					foreach (var (lineElements, lineSize) in list)
					{
						var xOffset = fullWidth - lineSize.x;
						var x = padding.left + xOffset;
						foreach (var recTransform in lineElements)
						{
							SetChildAlongAxis(recTransform, 0, x);
							SetChildAlongAxis(recTransform, 1, y);
							x += recTransform.sizeDelta.x + spacing.x;
						}
						y += lineSize.y + spacing.y;
					}
					break;
				}
				case TextAnchor.MiddleLeft:
				{
					var totalHeight = fullSize.y - padding.top + padding.bottom;
					var startY = (myRect.height - totalHeight) / 2;
					var y = startY;
					foreach (var (lineElements, lineSize) in list)
					{
						var x = (float)padding.left;
						foreach (var recTransform in lineElements)
						{
							SetChildAlongAxis(recTransform, 0, x);
							SetChildAlongAxis(recTransform, 1, y);
							x += recTransform.sizeDelta.x + spacing.x;
						}
						y += lineSize.y + spacing.y;
					}
					break;
				}
				case TextAnchor.MiddleCenter:
				{
					var totalHeight = fullSize.y - padding.top + padding.bottom;
					var startY = (myRect.height - totalHeight) / 2;
					var y = startY;
					var fullWidth = myRect.width - padding.right - padding.left;
					foreach (var (lineElements, lineSize) in list)
					{
						var xOffset = (fullWidth - lineSize.x) / 2;
						var x = padding.left + xOffset;
						foreach (var recTransform in lineElements)
						{
							SetChildAlongAxis(recTransform, 0, x);
							SetChildAlongAxis(recTransform, 1, y);
							x += recTransform.sizeDelta.x + spacing.x;
						}
						y += lineSize.y + spacing.y;
					}
					break;
				}
				case TextAnchor.MiddleRight:
				{
					var totalHeight = fullSize.y - padding.top + padding.bottom;
					var startY = (myRect.height - totalHeight) / 2;
					var y = startY;
					var fullWidth = myRect.width - padding.right - padding.left;
					foreach (var (lineElements, lineSize) in list)
					{
						var xOffset = fullWidth - lineSize.x;
						var x = padding.left + xOffset;
						foreach (var recTransform in lineElements)
						{
							SetChildAlongAxis(recTransform, 0, x);
							SetChildAlongAxis(recTransform, 1, y);
							x += recTransform.sizeDelta.x + spacing.x;
						}
						y += lineSize.y + spacing.y;
					}
					break;
				}
				case TextAnchor.LowerLeft:
				{
					var offset = fullSize.y + padding.bottom;
					var startY = myRect.height - offset;
					var y = startY;
					foreach (var (lineElements, lineSize) in list)
					{
						var x = (float)padding.left;
						foreach (var recTransform in lineElements)
						{
							SetChildAlongAxis(recTransform, 0, x);
							SetChildAlongAxis(recTransform, 1, y);
							x += recTransform.sizeDelta.x + spacing.x;
						}
						y += lineSize.y + spacing.y;
					}
					break;
				}
				case TextAnchor.LowerCenter:
				{
					var offset = fullSize.y + padding.bottom;
					var startY = myRect.height - offset;
					var y = startY;
					var fullWidth = myRect.width - padding.right - padding.left;
					foreach (var (lineElements, lineSize) in list)
					{
						var xOffset = (fullWidth - lineSize.x) / 2;
						var x = padding.left + xOffset;
						foreach (var recTransform in lineElements)
						{
							SetChildAlongAxis(recTransform, 0, x);
							SetChildAlongAxis(recTransform, 1, y);
							x += recTransform.sizeDelta.x + spacing.x;
						}
						y += lineSize.y + spacing.y;
					}
					break;
				}
				case TextAnchor.LowerRight:
				{
					var offset = fullSize.y + padding.bottom;
					var startY = myRect.height - offset;
					var y = startY;
					var fullWidth = myRect.width - padding.right - padding.left;
					foreach (var (lineElements, lineSize) in list)
					{
						var xOffset = fullWidth - lineSize.x;
						var x = padding.left + xOffset;
						foreach (var recTransform in lineElements)
						{
							SetChildAlongAxis(recTransform, 0, x);
							SetChildAlongAxis(recTransform, 1, y);
							x += recTransform.sizeDelta.x + spacing.x;
						}
						y += lineSize.y + spacing.y;
					}
					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
