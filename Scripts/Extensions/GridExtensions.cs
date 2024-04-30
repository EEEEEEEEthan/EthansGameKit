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
	}
}