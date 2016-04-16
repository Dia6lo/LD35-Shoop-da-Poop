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
			if (item.Position.Y < board.Height)
			{
				var pointUnderItem = new IntPoint(item.Position.X, item.Position.Y + 1);
				if (board.IsInBounds(pointUnderItem) && board.CellField[pointUnderItem].TargetedBy == null)
				{
					var cell = board.CellField[pointUnderItem];
					if (cell.TargetedBy == null)
					{
						PrefferedTarget = cell;
					}
				}
				else
				{
					var possibleTargetPositions = new[]
					{
						new IntPoint(item.Position.X - 1, item.Position.Y + 1),
						new IntPoint(item.Position.X + 1, item.Position.Y + 1),
					}
						.Where(board.IsInBounds)
						.Where(pos => board.CellField[pos].TargetedBy == null).ToList();
					if (possibleTargetPositions.Count > 1)
					{
						var index = new Random().Next(possibleTargetPositions.Count);
						PrefferedTarget = board.CellField[possibleTargetPositions[index]];
					}
					else if (possibleTargetPositions.Any())
					{
						PrefferedTarget = board.CellField[possibleTargetPositions.First()];
					}
				}
			}
		}

		public Cell PrefferedTarget { get; private set; }

		public List<Item> Items { get; private set; }
	}
}