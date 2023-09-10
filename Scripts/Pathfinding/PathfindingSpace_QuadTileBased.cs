namespace EthansGameKit.Pathfinding
{
	class PathfindingSpace_QuadTileBased : PathfindingSpace
	{
		class ImpPathfinder : Pathfinder
		{
			readonly PathfindingSpace_QuadTileBased pathfindingSpace;
			readonly int[] fromMap;
			readonly float[] costMap;
			public ImpPathfinder(PathfindingSpace_QuadTileBased pathfindingSpace) : base(pathfindingSpace)
			{
				this.pathfindingSpace = pathfindingSpace;
				fromMap = new int[pathfindingSpace.count];
				costMap = new float[pathfindingSpace.count];
				Reset();
			}
			protected override bool TryGetFromInfo(int nodeId, out int fromNodeId, out float totalCost)
			{
				fromNodeId = fromMap[nodeId];
				totalCost = costMap[nodeId];
				return fromNodeId != -1;
			}
			protected override void SetFromInfo(int fromNodeId, int toNodeId, float totalCost)
			{
				fromMap[toNodeId] = fromNodeId;
				costMap[toNodeId] = totalCost;
			}
			public void Reset()
			{
				fromMap.MemSet(-1);
				costMap.MemSet(float.PositiveInfinity);
			}
		}

		readonly int width;
		readonly int widthPower;
		readonly int count;
		
		public PathfindingSpace_QuadTileBased(int widthPower)
		{
			this.widthPower = widthPower;
			width = 1 << widthPower;
			count = width << widthPower;
		}
		public override Pathfinder GeneratePathfinder()
		{
			return new ImpPathfinder(this);
		}
		public override void SetLink(int fromNodeId, int toNodeId, float cost)
		{
			throw new System.NotImplementedException();
		}
		public override void RemoveLink(int fromNodeId, int toNodeId)
		{
			throw new System.NotImplementedException();
		}
		protected override int GetLinks(int fromNodeId, ref StepInfo[] toAndCost)
		{
			throw new System.NotImplementedException();
		}
	}
}
