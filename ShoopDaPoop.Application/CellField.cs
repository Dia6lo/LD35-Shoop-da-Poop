using System;
using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class CellField
	{
		private Texture cellTexture;

		public CellField(int width, int height)
		{
			Width = width;
			Height = height;
			Container = new Container();
			cellTexture = Texture.FromImage("assets/Tile.png");
			Cells = new Cell[width, height];
			ForEachPosition(point => this[point] = new Cell(cellTexture) {Position = point});
			ForEachCell((point, cell) =>
			{
				cell.WorldPosition = new Point(point.X * cell.Width, point.Y * cell.Height);
				Container.AddChild(cell.Sprite);
			});
		}

		public Cell[,] Cells { get; private set; }

		public void SetTemperature(float temperature, params IntPoint[] coordinates)
		{
			foreach (var coordinate in coordinates)
			{
				this[coordinate].Temperature = temperature;
			}
		}

		public Cell this[IntPoint point]
		{
			get { return Cells[point.X, point.Y]; }
			set { Cells[point.X, point.Y] = value; }
		}

		public Cell this[int x, int y]
		{
			get { return Cells[x, y]; }
			set { Cells[x, y] = value; }
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
			ForEachPosition(point => action(point, Cells[point.X, point.Y]));
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
			ForEachPosition(point => newCells[point.X, point.Y] = Cells[point.X, point.Y]);
			Cells = newCells;
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