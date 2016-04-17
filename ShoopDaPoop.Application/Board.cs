using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Board
	{
		public int UpdatesSinceCreation;
		private Body body;

		public Board(int width, int height)
		{
			Items = new List<Item>();
			Container = new Container();
			CellField = new CellField(width, height);
			body = new Body
			{
				LeftArmAction = () => Push(Side.Left),
				RightArmAction = () => Push(Side.Right),
				LeftLegAction = () => Push(Side.Down),
				RightLegAction = () => Push(Side.Down),
				HeadAction = () => Push(Side.Top)
			};
			Container.AddChild(CellField.Container);
			Container.AddChild(body.Container);
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

		public void Update()
		{
			UpdatesSinceCreation++;
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
		}

		public void Push(Side side)
		{
			CellField.Push(side);
		}

		public void PreRender(Point leftFootPosition, Point maxRightPosition)
		{
			if (Items.Any() && UpdatesSinceCreation > 3)
			{
				DrawBody();
				var leftFootWorld = Container.Position.Add(body.LeftFoot);
				var rightFootWorld = Container.Position.Add(body.RightFoot);
				var currentVector = rightFootWorld.Subtract(leftFootWorld);
				var targetVector = maxRightPosition.Subtract(leftFootPosition);
				var currentLength = currentVector.Length();
				var targetLength = targetVector.Length();
				var dotProduct = currentVector.X*targetVector.X + currentVector.Y*targetVector.Y;
				var cos = dotProduct/(currentLength*targetLength);
				var angle = (float)Math.Acos(cos);
				if (currentVector.Y > 0)
					angle *= -1;
				Container.Rotation = angle;
				var sin = (float)Math.Sin(angle);
				var actualLeftFootLocal = new Point
				{
					X = cos*body.LeftFoot.X - sin*body.LeftFoot.Y,
					Y = cos*body.LeftFoot.Y + sin*body.LeftFoot.X
				};
				Container.Position = Container.Position.Add(leftFootPosition.Subtract(Container.Position.Add(actualLeftFootLocal)));
			}
			CellField.PreRender(new Point());
			foreach (var item in Items)
			{
				item.PreRender();
			}
		}

		private void DrawBody()
		{
			var positions = Items.Select(item => new { item.Sprite.Position, Bounds = item.Sprite.GetBounds()});
			var orderedX = positions.OrderBy(pos => pos.Position.X);
			var orderedY = positions.OrderBy(pos => pos.Position.Y);
			var firstX = orderedX.First();
			var firstY = orderedY.First();
			var lastX = orderedX.Last();
			var lastY = orderedY.Last();
			var allMinX = positions.Where(pos => pos.Position.X == firstX.Position.X);
			var allMinY = positions.Where(pos => pos.Position.Y == firstY.Position.Y);
			var allMaxX = positions.Where(pos => pos.Position.X == lastX.Position.X);
			var allMaxY = positions.Where(pos => pos.Position.Y == lastY.Position.Y);
			var minX = new Point(firstX.Position.X - firstX.Bounds.Width, allMinX.Average(pos => pos.Position.Y));
			var minY = new Point(allMinY.Average(pos => pos.Position.X), firstY.Position.Y - firstY.Bounds.Height);
			var maxX = new Point(lastX.Position.X + lastX.Bounds.Width, allMaxX.Average(pos => pos.Position.Y));
			var maxY = new Point(allMaxY.Average(pos => pos.Position.X), lastY.Position.Y + lastY.Bounds.Height);
			body.Render(new BodyPoints {MinX = minX, MinY = minY, MaxX = maxX, MaxY = maxY});
		}

		public Container Container { get; private set; }

		public List<Item> Items { get; set; }

		public bool IsInBounds(IntPoint position)
		{
			return CellField.IsInBounds(position);
		}
	}
}
