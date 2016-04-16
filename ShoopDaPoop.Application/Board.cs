using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Pixi;
using Bridge.Pixi.Interaction;

namespace ShoopDaPoop.Application
{
	public class Board
	{
		private int updatesSinceCreation;
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
			Container = new Container();
			CellField = new CellField(width, height);
			Container.AddChild(CellField.Container);
			Container["interactive"] = true;
			Container.OnMouseDown(OnClick);
		}

		public CellField CellField { get; private set; }

		private void OnClick(InteractionEvent e)
		{
			Expand();
		}

		public void AddItem(int x)
		{
			var position = new IntPoint(x, -1);
			var cell = CellField[new IntPoint(x, 0)];
			var randomIndex = new Random().Next(itemFactories.Count);
			var item = itemFactories[randomIndex]();
			item.Position = position;
			item.State = ItemState.Spawned;
			item.Sprite.Position = new Point(cell.WorldPosition.X, cell.WorldPosition.Y - cell.Sprite.Height);
			Items.Add(item);
			Container.AddChild(item.Sprite);
			item.OnDeath = () =>
			{
				Items.Remove(item);
				Container.RemoveChild(item.Sprite);
			};
		}

		public void Update()
		{
			updatesSinceCreation++;
			if (SpawningCell.TargetedBy == null && updatesSinceCreation > 3)
			{
				AddItem(SpawningX);
			}
			HandleMatches();
			foreach (var item in Items)
			{
				item.Update(this);
			}
		}

		private void HandleMatches()
		{
			var matches = FindMatches();
			foreach (var item in matches)
			{
				item.Die();
			}
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
				.Where(item => item != null && item.State == ItemState.Idle)
				.ToList();
		}

		public List<Item> GetLineMatches(List<Item> line)
		{
			var result = new List<Item>();
			for (var i = 1; i < line.Count - 1; i++)
			{
				var toCompare = line.GetRange(i - 1, 3);
				if (toCompare.Any(item => item == null)) continue;
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
				.Where(item => item != null && item.State == ItemState.Idle)
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

		private void Expand()
		{
			CellField.Expand();
		}

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
