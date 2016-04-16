using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Pixi;
using Bridge.Pixi.Interaction;

namespace ShoopDaPoop.Application
{
	public class Board
	{
		public int UpdatesSinceCreation;
		private List<Func<Item>> itemFactories = new List<Func<Item>>
		{
			() => new Square(),
			() => new Circle(),
			() => new Diamond(),
			() => new Snake()
		};

		public Board(int width, int height)
		{
			Items = new List<Item>();
			Container = new Container()
			{
				Position = new Point(32, 32)
			};
			CellField = new CellField(width, height);
			Container.AddChild(CellField.Container);
			SpawningColumns = new List<int>();
		}

		public CellField CellField { get; private set; }

		public void FillWithItems(List<Item> items)
		{
			var firstType = items.First().Type;
			while (items.Take(4).All(item => item.Type == firstType))
			{
				items = Bridge.Linq.Enumerable.Shuffle(items).ToList();
				firstType = items.First().Type;
			}
			var cells = new List<Cell>();
			CellField.ForEachCell((point, cell) => cells.Add(cell));
			var cellGroups = cells
				.GroupBy(cell => cell.Temperature)
				.OrderByDescending(group => group.Key);
			var currentIndex = 0;
			foreach (var cellGroup in cellGroups)
			{
				foreach (var cell in cellGroup.Shuffle())
				{
					var item = items[currentIndex++];
					item.Position = cell.Position;
					item.State = ItemState.Idle;
					item.Sprite.Position = cell.Sprite.Position;
					item.Board = this;
					item.Target = cell;
					cell.TargetedBy = item;
					Items.Add(item);
					Container.AddChild(item.Sprite);
					item.OnDeath = () =>
					{
						Items.Remove(item);
						Container.RemoveChild(item.Sprite);
					};
					if (currentIndex == items.Count) return;
				}
			}
		}

		public void AddItem()
		{
			var x = GetSpawningColumn();
			var position = new IntPoint(x, -1);
			var cell = CellField[new IntPoint(x, 0)];
			var randomIndex = new Random().Next(itemFactories.Count);
			var item = itemFactories[randomIndex]();
			item.Position = position;
			item.State = ItemState.Spawned;
			item.Sprite.Position = new Point(cell.WorldPosition.X, cell.WorldPosition.Y - cell.Sprite.Height);
			item.Board = this;
			Items.Add(item);
			Container.AddChild(item.Sprite);
			item.OnDeath = () =>
			{
				Items.Remove(item);
				Container.RemoveChild(item.Sprite);
			};
		}

		private int GetSpawningColumn()
		{
			if (SpawningColumns == null || !SpawningColumns.Any())
			{
				return Width/2;
			}
			var allowedColumns = SpawningColumns
				.Where(column => Items.All(item => item.State != ItemState.Spawned || item.Position.X != column))
				.ToList();
			var randomIndex = new Random().Next(SpawningColumns.Count);
			return SpawningColumns[randomIndex];
		}

		private bool IsSpawningAllowed
		{
			get { return Items.Count < 20 && Items.Count(item => item.State == ItemState.Spawned) <= 2; }
		}

		public void Update()
		{
			UpdatesSinceCreation++;
			/*if (IsSpawningAllowed && updatesSinceCreation > 3)
			{
				AddItem();
			}*/
			HandleMatches();
			foreach (var item in Items)
			{
				item.Update();
			}
		}

		private void HandleMatches()
		{
			var belly = new[]
			{
				new IntPoint(4, 4),
				new IntPoint(5, 4),
				new IntPoint(4, 5),
				new IntPoint(5, 5)
			};
			var items = belly
				.Select(pos => CellField[pos].TargetedBy)
				.ToList();
			if(items.Any(item => item == null || item.State != ItemState.Idle)) return;
			var firstType = items.First().Type;
			if (items.Any(item => item.Type != firstType)) return;
			foreach (var item in items)
			{
				item.Die();
			}
			/*var matches = FindMatches();
			foreach (var item in matches)
			{
				item.Die();
			}*/
		}

		public List<Item> GetColumnMatches(int columnIndex)
		{
			var column = GetColumn(columnIndex);
			return GetLineMatches(column);
		}

		public List<Item> GetColumn(int columnIndex)
		{
			if (columnIndex < 0 || columnIndex >= Width)
				return null;
			return Enumerable.Range(0, Height)
				.Select(y => CellField[columnIndex, y])
				.Select(cell => cell.TargetedBy)
				.ToList();
		}

		public List<Item> GetLineMatches(List<Item> line)
		{
			var result = new List<Item>();
			for (var i = 1; i < line.Count - 1; i++)
			{
				var toCompare = line.GetRange(i - 1, 3);
				if (toCompare.Any(item => item == null || item.State != ItemState.Idle)) continue;
				var current = i;
				if (toCompare.Any(item => item.Type != line[current].Type)) continue;
				result.AddRange(toCompare);
			}
			return result.MyDistinct().ToList();
		}

		public List<Item> GetRowMatches(int rowIndex)
		{
			var row = GetRow(rowIndex);
			return GetLineMatches(row);
		}

		public List<Item> GetRow(int rowIndex)
		{
			if (rowIndex < 0 || rowIndex >= Height)
				return null;
			return Enumerable.Range(0, Width)
				.Select(x => CellField[x, rowIndex])
				.Select(cell => cell.TargetedBy)
				.ToList();
		}

		public List<Item> FindMatches()
		{
			var columnMatches = Enumerable.Range(0, Width)
				.SelectMany(GetColumnMatches)
				.ToList();
			var rowMatches = Enumerable.Range(0, Height)
				.SelectMany(GetRowMatches)
				.ToList();
			return columnMatches.Concat(rowMatches).MyDistinct().ToList();
		}

		public void Expand()
		{
			CellField.Expand();
		}

		public List<int> SpawningColumns { get; set; }

		private int SpawningX
		{
			get { return Width/2; }
		}

		private Cell SpawningCell
		{
			get { return CellField[new IntPoint(SpawningX, 0)]; }
		}

		public void PreRender()
		{
			CellField.PreRender(new Point());
			foreach (var item in Items)
			{
				item.PreRender();
			}
		}

		public Container Container { get; private set; }

		public List<Item> Items { get; set; }

		public int Width
		{
			get { return CellField.Width; }
		}

		public int Height
		{
			get { return CellField.Height; }
		}

		public bool IsInBounds(IntPoint position)
		{
			return CellField.IsInBounds(position);
		}
	}
}
