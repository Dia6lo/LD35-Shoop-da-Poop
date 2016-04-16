using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoopDaPoop.Application
{
	public class BoardSummary
	{
		public BoardSummary(Item item, Board board)
		{
			Items = board.Items.Where(i => i != item).ToList();
			var neighborCells = GetLegalNeighborCells(item, board);
			var cellsByPriority = neighborCells
				.Where(cell => cell.TargetedBy == null && cell.Temperature > 0)
				.OrderByDescending(cell => cell.Temperature)
				.ThenByDescending(cell => cell.Position.Y);
			if (cellsByPriority.Any())
			{
				if (item.Target == null)
				{
					PrefferedTarget = cellsByPriority.First();
					return;
				}
				var firstByTemperature = cellsByPriority
					.FirstOrDefault(cell => cell.Temperature > item.Target.Temperature);
				if (firstByTemperature != null)
				{
					PrefferedTarget = firstByTemperature;
				}
				/*var firstByGravity = cellsByPriority
					.FirstOrDefault(cell => cell.Position.Y > item.Target.Position.Y &&
					                        cell.Temperature >= item.Target.Temperature);
				if (firstByGravity != null)
				{
					PrefferedTarget = firstByGravity;
				}*/
			}
		}

		private List<IntPoint> GetLegalNeighborPositions(Item item, Board board)
		{
			var offsets = new[] { 0, -1, 1 };
			return offsets
				.SelectMany(x => offsets.Select(y => new IntPoint(x, y)))
				.Where(pos => pos.X != 0 || pos.Y != 0)
				.Select(pos => item.Position.Add(pos))
				.Where(board.IsInBounds)
				.ToList();
		}

		private List<IntPoint> GetLegalNeighborPositions(Cell cell, Board board)
		{
			var offsets = new[] { 0, -1, 1 };
			return offsets
				.SelectMany(x => offsets.Select(y => new IntPoint(x, y)))
				.Where(pos => pos.X != 0 || pos.Y != 0)
				.Select(pos => cell.Position.Add(pos))
				.Where(board.IsInBounds)
				.ToList();
		}

		private List<Cell> GetLegalNeighborCells(Cell cell, Board board)
		{
			return GetLegalNeighborPositions(cell, board)
				.Select(pos => board.CellField[pos])
				.ToList();
		}

		private List<Cell> GetLegalNeighborCells(Item item, Board board)
		{
			return GetLegalNeighborPositions(item, board)
				.Select(pos => board.CellField[pos])
				.ToList();
		}

		public Cell PrefferedTarget { get; private set; }

		public List<Item> Items { get; private set; }
	}
}