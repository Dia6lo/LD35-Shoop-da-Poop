using System;
using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class CellField
	{
		private Cell[,] cells;
		private Texture cellTexture;

		public CellField(int width, int height)
		{
			Width = width;
			Height = height;
			Container = new Container();
			cellTexture = Texture.FromImage("assets/tile.png");
			cells = new Cell[width, height];
			ForEachPosition(point => this[point] = new Cell(cellTexture) {Position = point});
			ForEachCell((point, cell) =>
			{
				cell.WorldPosition = new Point(point.X * cell.Width, point.Y * cell.Height);
				Container.AddChild(cell.Sprite);
			});
		}

		public Cell this[IntPoint point]
		{
			get { return cells[point.X, point.Y]; }
			set { cells[point.X, point.Y] = value; }
		}

		public void Update()
		{
		}

		public void PreRender(Point position)
		{
			Container.Position = position;
			ForEachCell((point, cell) => cell.PreRender(new Point(point.X * cell.Width, point.Y * cell.Height)));
		}

		public Container Container { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		public void ForEachCell(Action<IntPoint, Cell> action)
		{
			ForEachPosition(point => action(point, cells[point.X, point.Y]));
		}

		public void ForEachPosition(Action<IntPoint> action)
		{
			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					action(new IntPoint(x, y));
				}
			}
		}

		public void Expand()
		{
			var newCells = new Cell[Width+1, Height];
			ForEachPosition(point => newCells[point.X, point.Y] = cells[point.X, point.Y]);
			cells = newCells;
			Width ++;
			ForEachPosition(point =>
			{
				if (this[point] != null) return;
				var cell = new Cell(cellTexture) { Position = point };
				this[point] = cell;
				cell.WorldPosition = new Point(point.X * cell.Width, point.Y * cell.Height);
				Container.AddChild(cell.Sprite);
			});
		}

		public bool IsInBounds(IntPoint position)
		{
			return position.X >= 0 && position.X < Width
				   && position.Y >= 0 && position.Y < Height;
		}
	}
}