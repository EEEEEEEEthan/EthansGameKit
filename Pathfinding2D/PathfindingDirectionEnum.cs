using UnityEngine.Assertions;

namespace EthansGameKit.Pathfinding2D
{
	public enum PathfindingDirectionEnum
	{
		N = 0,
		E = 1,
		S = 2,
		W = 3,
		NE = 4,
		SE = 5,
		SW = 6,
		NW = 7,
	}

	static class PathfindingDirectionEnumExtensions
	{
		internal static PathfindingDirectionEnum Opposite(this PathfindingDirectionEnum direction)
		{
			switch (direction)
			{
				case PathfindingDirectionEnum.N:
					return PathfindingDirectionEnum.S;
				case PathfindingDirectionEnum.E:
					return PathfindingDirectionEnum.W;
				case PathfindingDirectionEnum.S:
					return PathfindingDirectionEnum.N;
				case PathfindingDirectionEnum.W:
					return PathfindingDirectionEnum.E;
				case PathfindingDirectionEnum.NE:
					return PathfindingDirectionEnum.SW;
				case PathfindingDirectionEnum.SE:
					return PathfindingDirectionEnum.NW;
				case PathfindingDirectionEnum.SW:
					return PathfindingDirectionEnum.NE;
				case PathfindingDirectionEnum.NW:
					return PathfindingDirectionEnum.SE;
				default:
					Assert.IsTrue(false, $"unexpected direction {direction}");
					return PathfindingDirectionEnum.N;
			}
		}
		internal static PathfindingDirectionEnum Previous(this PathfindingDirectionEnum direction)
		{
			switch (direction)
			{
				case PathfindingDirectionEnum.N:
					return PathfindingDirectionEnum.NW;
				case PathfindingDirectionEnum.E:
					return PathfindingDirectionEnum.NE;
				case PathfindingDirectionEnum.S:
					return PathfindingDirectionEnum.SE;
				case PathfindingDirectionEnum.W:
					return PathfindingDirectionEnum.SW;
				case PathfindingDirectionEnum.NE:
					return PathfindingDirectionEnum.N;
				case PathfindingDirectionEnum.SE:
					return PathfindingDirectionEnum.E;
				case PathfindingDirectionEnum.SW:
					return PathfindingDirectionEnum.S;
				case PathfindingDirectionEnum.NW:
					return PathfindingDirectionEnum.W;
				default:
					Assert.IsTrue(false, $"unexpected direction {direction}");
					return PathfindingDirectionEnum.N;
			}
		}
		internal static PathfindingDirectionEnum Next(this PathfindingDirectionEnum direction)
		{
			switch (direction)
			{
				case PathfindingDirectionEnum.N:
					return PathfindingDirectionEnum.NE;
				case PathfindingDirectionEnum.E:
					return PathfindingDirectionEnum.SE;
				case PathfindingDirectionEnum.S:
					return PathfindingDirectionEnum.SW;
				case PathfindingDirectionEnum.W:
					return PathfindingDirectionEnum.NW;
				case PathfindingDirectionEnum.NE:
					return PathfindingDirectionEnum.E;
				case PathfindingDirectionEnum.SE:
					return PathfindingDirectionEnum.S;
				case PathfindingDirectionEnum.SW:
					return PathfindingDirectionEnum.W;
				case PathfindingDirectionEnum.NW:
					return PathfindingDirectionEnum.N;
				default:
					Assert.IsTrue(false, $"unexpected direction {direction}");
					return PathfindingDirectionEnum.N;
			}
		}
	}
}
