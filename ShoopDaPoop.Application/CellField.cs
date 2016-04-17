using System;
using System.Collections.Generic;
using System.Linq;
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

		public void PreRender(Point position)
		{
			Container.Position = position;
			ForEachCell((point, cell) => cell.PreRender(new Point(point.X * cell.Width, point.Y * cell.Height)));
		}

		public Container Container { get; private set; }

		public int Width { get; private set; }

		public int Height { get; private set; }

		private void Shift(IntPoint from, IntPoint to)
		{
			if (from.X != to.X && from.Y != to.Y)
				throw new InvalidOperationException("Not a staright line");
			var direction = new IntPoint(from.X - to.X, from.Y - to.Y);
			if (direction.X == 0)
				direction.Y = direction.Y/Math.Abs(direction.Y);
			else
				direction.X = direction.X / Math.Abs(direction.X);
			var currentPoint = to;
			while (currentPoint.X != from.X || currentPoint.Y != from.Y)
			{
				var nextPoint = currentPoint.Add(direction);
				var moveToCell = this[currentPoint];
				var moveFromCell = this[nextPoint];
				currentPoint = nextPoint;
				if (moveToCell.TargetedBy != null) continue;
				var item = moveFromCell.TargetedBy;
				if (item != null)
					item.AssignTarget(moveToCell);
			}
		}

		private Dictionary<Side, List<BodySide>> bodySides = new Dictionary<Side, List<BodySide>>
		{
			{Side.Bottom, new List<BodySide>
			{
				new BodySide {Belly = new IntPoint(4, 5), Border = new IntPoint(4, 9), InnerSide = Side.Left},
				new BodySide {Belly = new IntPoint(5, 5), Border = new IntPoint(5, 9), InnerSide = Side.Right}
			} },
			{Side.Left, new List<BodySide>
			{
				new BodySide {Belly = new IntPoint(4, 4), Border = new IntPoint(0, 4), InnerSide = Side.Top},
				new BodySide {Belly = new IntPoint(4, 5), Border = new IntPoint(0, 5), InnerSide = Side.Bottom}
			} },
			{Side.Top, new List<BodySide>
			{
				new BodySide {Belly = new IntPoint(4, 4), Border = new IntPoint(4, 0), InnerSide = Side.Left},
				new BodySide {Belly = new IntPoint(5, 4), Border = new IntPoint(5, 0), InnerSide = Side.Right}
			} },
			{Side.Right, new List<BodySide>
			{
				new BodySide {Belly = new IntPoint(5, 4), Border = new IntPoint(9, 4), InnerSide = Side.Top},
				new BodySide {Belly = new IntPoint(5, 5), Border = new IntPoint(9, 5), InnerSide = Side.Bottom}
			} }
		};

		public void Pull(Side side)
		{
			var sides = bodySides[side];
			foreach (var bodySide in sides)
			{
				Shift(bodySide.Belly, bodySide.Border);
			}
			var pushedSides = bodySides
				.SelectMany(s => s.Value)
				.Where(s => s.InnerSide == side);
			foreach (var pushedSide in pushedSides)
			{
				Shift(pushedSide.Border, pushedSide.Belly);
			}
		}

		public void Push(Side side)
		{
			var pushedSides = bodySides
				.SelectMany(s => s.Value)
				.Where(s => s.InnerSide == side);
			foreach (var pushedSide in pushedSides)
			{
				Shift(pushedSide.Belly, pushedSide.Border);
			}
			var sides = bodySides[side];
			foreach (var bodySide in sides)
			{
				Shift(bodySide.Border, bodySide.Belly);
			}
		}

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

		public bool IsInBounds(IntPoint position)
		{
			return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
		}

		private class BodySide
		{
			public IntPoint Belly;
			public IntPoint Border;
			public Side InnerSide;
		}
	}
}