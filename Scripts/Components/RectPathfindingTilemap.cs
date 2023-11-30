using System;
using System.Collections.Generic;
using System.Linq;
using EthansGameKit.AStar;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace EthansGameKit.Components
{
	[ExecuteAlways, RequireComponent(typeof(Tilemap))]
	public class RectPathfindingTilemap : MonoBehaviour
	{
		[Serializable]
		public struct StepCost
		{
#if UNITY_EDITOR
			[UnityEditor.CustomPropertyDrawer(typeof(StepCost))]
			class Drawer : UnityEditor.PropertyDrawer
			{
				public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
				{
					//base.OnGUI(position, property, label);
					// 仅显示一行
					//position.height = UnityEditor.EditorGUIUtility.singleLineHeight;
					UnityEditor.EditorGUILayout.BeginHorizontal();
					{
						var rect = new Rect(position.x, position.y, position.width * 0.5f, position.height);
						UnityEditor.EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(tile)), GUIContent.none);
						rect = new(position.x + position.width * 0.5f, position.y, position.width * 0.5f, position.height);
						UnityEditor.EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(cost)), GUIContent.none);
					}
					UnityEditor.EditorGUILayout.EndHorizontal();
				}
				public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label) => UnityEditor.EditorGUIUtility.singleLineHeight;
			}
#endif
			[SerializeField] public TileBase tile;
			[SerializeField] public float cost;
		}
#if UNITY_EDITOR
		[UnityEditor.CustomEditor(typeof(RectPathfindingTilemap))]
		class Editor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				var tilemap = (RectPathfindingTilemap)target;
				if (GUILayout.Button("Bake"))
				{
					tilemap.Bake();
				}
			}
		}
#endif
		Tilemap _tilemap;
		[SerializeField] bool allowDiagonal;
		[SerializeField] StepCost[] tile2cost = { };
		/// <summary>
		///     bake后生效
		/// </summary>
		public IReadOnlyList<StepCost> Tile2Cost
		{
			get => tile2cost;
			set => tile2cost = value.ToArray();
		}
		public RectPathfindingSpace Space { get; private set; }
		public Tilemap Tilemap
		{
			get
			{
				if (!_tilemap) _tilemap = GetComponent<Tilemap>();
				if (_tilemap.cellSwizzle != GridLayout.CellSwizzle.XYZ)
					throw new NotImplementedException();
				return _tilemap;
			}
		}
		void OnEnable()
		{
			_tilemap = GetComponent<Tilemap>();
			Bake();
		}
		void OnDrawGizmos()
		{
			if (!enabled) return;
			if (Space is null) return;
			var grid = Tilemap.layoutGrid;
			if (!grid) return;
			{
				Gizmos.color = new(1, 1, 0, 0.3f);
				var xMin = Space.rawRect.xMin;
				var yMin = Space.rawRect.yMin;
				var xMax = Space.rawRect.xMax;
				var yMax = Space.rawRect.yMax;
				Gizmos.DrawLine(grid.CellToLocalInterpolated(new(xMin, yMin)), grid.CellToLocalInterpolated(new(xMin, yMax)));
				Gizmos.DrawLine(grid.CellToLocalInterpolated(new(xMin, yMax)), grid.CellToLocalInterpolated(new(xMax, yMax)));
				Gizmos.DrawLine(grid.CellToLocalInterpolated(new(xMax, yMax)), grid.CellToLocalInterpolated(new(xMax, yMin)));
				Gizmos.DrawLine(grid.CellToLocalInterpolated(new(xMax, yMin)), grid.CellToLocalInterpolated(new(xMin, yMin)));
			}
			{
				Gizmos.color = new(1, 0, 0, 0.5f);
				var xMin = Space.fullRect.xMin;
				var yMin = Space.fullRect.yMin;
				var xMax = Space.fullRect.xMax;
				var yMax = Space.fullRect.yMax;
				Gizmos.DrawLine(grid.CellToLocalInterpolated(new(xMin, yMin)), grid.CellToLocalInterpolated(new(xMin, yMax)));
				Gizmos.DrawLine(grid.CellToLocalInterpolated(new(xMin, yMax)), grid.CellToLocalInterpolated(new(xMax, yMax)));
				Gizmos.DrawLine(grid.CellToLocalInterpolated(new(xMax, yMax)), grid.CellToLocalInterpolated(new(xMax, yMin)));
				Gizmos.DrawLine(grid.CellToLocalInterpolated(new(xMax, yMin)), grid.CellToLocalInterpolated(new(xMin, yMin)));
			}
			{
				Gizmos.color = new(0, 1, 1, 0.05f);
				var halfCell = new Vector2(0.5f, 0.5f);
				foreach (var (from, to, _) in Space.GetLinks())
				{
					var fromPos = grid.CellToLocalInterpolated(from + halfCell);
					var toPos = grid.CellToLocalInterpolated(to + halfCell);
					var shortten = (toPos - fromPos).normalized * 0.1f;
					GizmosEx.DrawArrow(fromPos + shortten, toPos - shortten, 0.1f);
				}
			}
		}
		public void Bake()
		{
			float getCost(Vector2Int position)
			{
				var tile = Tilemap.GetTile((Vector3Int)position);
				foreach (var t2c in tile2cost)
					if (tile == t2c.tile)
						return t2c.cost;
				return -1;
			}

			var tilemap = Tilemap;
			var bounds = tilemap.cellBounds;
			var rect = new RectInt(bounds.xMin, bounds.yMin, bounds.size.x, bounds.size.y);
			Space = new(rect, allowDiagonal);
			Vector2Int[] neighbors =
			{
				Vector2Int.up,
				Vector2Int.right,
				Vector2Int.down,
				Vector2Int.left,
				Vector2Int.up + Vector2Int.right,
				Vector2Int.down + Vector2Int.right,
				Vector2Int.down + Vector2Int.left,
				Vector2Int.up + Vector2Int.left,
			};
			foreach (var fromPosition in Space.rawRect.allPositionsWithin)
			{
				var cost1 = getCost(fromPosition);
				if (cost1 <= 0) continue;
				for (var dir = 0; dir < 4; ++dir)
				{
					var toPosition = fromPosition + neighbors[dir];
					var cost2 = getCost(toPosition);
					if (cost2 <= 0) continue;
					Space.SetLink(fromPosition, (RectPathfindingSpace.DirectionEnum)dir, (cost1 + cost2) * 0.5f);
				}
			}
			if (allowDiagonal)
			{
				foreach (var fromPosition in Space.rawRect.allPositionsWithin)
				{
					var costa = getCost(fromPosition);
					if (costa <= 0) continue;
					//const float rate = 0.70710678118654752440084436210485f;
					const float rate = 0.70710678118654752440084436210485f * 0.5f;
					{
						var cost1 = Space.GetCost(fromPosition, RectPathfindingSpace.DirectionEnum.Up, RectPathfindingSpace.DirectionEnum.Right);
						var cost2 = Space.GetCost(fromPosition, RectPathfindingSpace.DirectionEnum.Right, RectPathfindingSpace.DirectionEnum.Up);
						if (cost1 > 0 && cost2 > 0)
							Space.SetLink(fromPosition, RectPathfindingSpace.DirectionEnum.UpRight, (cost1 + cost2) * rate);
					}
					{
						var cost1 = Space.GetCost(fromPosition, RectPathfindingSpace.DirectionEnum.Down, RectPathfindingSpace.DirectionEnum.Right);
						var cost2 = Space.GetCost(fromPosition, RectPathfindingSpace.DirectionEnum.Right, RectPathfindingSpace.DirectionEnum.Down);
						if (cost1 > 0 && cost2 > 0)
							Space.SetLink(fromPosition, RectPathfindingSpace.DirectionEnum.DownRight, (cost1 + cost2) * rate);
					}
					{
						var cost1 = Space.GetCost(fromPosition, RectPathfindingSpace.DirectionEnum.Down, RectPathfindingSpace.DirectionEnum.Left);
						var cost2 = Space.GetCost(fromPosition, RectPathfindingSpace.DirectionEnum.Left, RectPathfindingSpace.DirectionEnum.Down);
						if (cost1 > 0 && cost2 > 0)
							Space.SetLink(fromPosition, RectPathfindingSpace.DirectionEnum.DownLeft, (cost1 + cost2) * rate);
					}
					{
						var cost1 = Space.GetCost(fromPosition, RectPathfindingSpace.DirectionEnum.Up, RectPathfindingSpace.DirectionEnum.Left);
						var cost2 = Space.GetCost(fromPosition, RectPathfindingSpace.DirectionEnum.Left, RectPathfindingSpace.DirectionEnum.Up);
						if (cost1 > 0 && cost2 > 0)
							Space.SetLink(fromPosition, RectPathfindingSpace.DirectionEnum.UpLeft, (cost1 + cost2) * rate);
					}
				}
			}
		}
	}
}
