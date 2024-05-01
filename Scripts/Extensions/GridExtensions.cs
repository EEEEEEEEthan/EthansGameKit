using UnityEngine;

namespace EthansGameKit
{
	public static partial class Extensions
	{
		public static void Gizmos_DrawTile(this Grid grid, Vector2Int tile)
		{
			var a = grid.CellToWorld((Vector3Int)tile);
			var b = grid.CellToWorld((Vector3Int)tile + Vector3Int.right);
			var c = grid.CellToWorld((Vector3Int)tile + Vector3Int.one);
			var d = grid.CellToWorld((Vector3Int)tile + Vector3Int.up);
			Gizmos.DrawLine(a, b);
			Gizmos.DrawLine(b, c);
			Gizmos.DrawLine(c, d);
			Gizmos.DrawLine(d, a);
		}

		public static void Gizmos_DrawRect(this Grid grid, RectInt rect)
		{
			var a = grid.CellToWorld((Vector3Int)rect.min);
			var b = grid.CellToWorld((Vector3Int)rect.min + Vector3Int.right * rect.width);
			var c = grid.CellToWorld((Vector3Int)rect.min + Vector3Int.right * rect.width +
			                         Vector3Int.up * rect.height);
			var d = grid.CellToWorld((Vector3Int)rect.min + Vector3Int.up * rect.height);
			Gizmos.DrawLine(a, b);
			Gizmos.DrawLine(b, c);
			Gizmos.DrawLine(c, d);
			Gizmos.DrawLine(d, a);
		}
	}
}