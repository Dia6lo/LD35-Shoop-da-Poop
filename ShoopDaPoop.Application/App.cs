using System.Collections.Generic;
using System.Linq;
using Bridge.Html5;
using Bridge.Pixi;
using Bridge.Pixi.Interaction;

namespace ShoopDaPoop.Application
{
	public class App
	{
		private static IRenderer renderer;
		private static Board board;
		private static Container stage;

		[Ready]
		public static void Main()
		{
			renderer = Pixi.AutoDetectRenderer(Window.InnerWidth, Window.InnerHeight, new RendererOptions
			{
				BackgroundColor = 0x1099bb,
				RoundPixels = true
			});
			Document.Body.AppendChild(renderer.View);
			stage = new Container();
			board = new Board(10, 10);
			board.Container.Position = new Point(200, 200);
			var temperatureDictionary = new Dictionary<float, IntPoint[]>
			{
				{0.2f, new []
				{
					new IntPoint(0, 4),
					new IntPoint(0, 5),
					new IntPoint(4, 0),
					new IntPoint(5, 0),
					new IntPoint(4, 9),
					new IntPoint(5, 9),
					new IntPoint(9, 4),
					new IntPoint(9, 5)
				} },
				{0.4f, new []
				{
					new IntPoint(1, 4),
					new IntPoint(1, 5),
					new IntPoint(4, 1),
					new IntPoint(5, 1),
					new IntPoint(4, 8),
					new IntPoint(5, 8),
					new IntPoint(8, 4),
					new IntPoint(8, 5)
				} },
				{0.6f, new []
				{
					new IntPoint(2, 4),
					new IntPoint(2, 5),
					new IntPoint(4, 2),
					new IntPoint(5, 2),
					new IntPoint(4, 7),
					new IntPoint(5, 7),
					new IntPoint(7, 4),
					new IntPoint(7, 5)
				} },
				{0.8f, new []
				{
					new IntPoint(3, 4),
					new IntPoint(3, 5),
					new IntPoint(4, 3),
					new IntPoint(5, 3),
					new IntPoint(4, 6),
					new IntPoint(5, 6),
					new IntPoint(6, 4),
					new IntPoint(6, 5)
				} }
			};
			foreach (var kvp in temperatureDictionary)
			{
				board.CellField.SetTemperature(kvp.Key, kvp.Value);
			}
			var belly = new[]
			{
				new IntPoint(4, 4),
				new IntPoint(5, 4),
				new IntPoint(4, 5),
				new IntPoint(5, 5)
			};
			board.CellField.SetTemperature(1, belly);
			var test = new Graphics();
			test.LineStyle(2, 0xFFFFFF, 0.8f);
			test.MoveTo(0, 400);
			test.LineTo(600, 400);
			stage.AddChild(test);
			stage.AddChild(board.Container);
			Animate();
		}

		private static bool spawnedItems;

		private static void Animate()
		{
			Window.RequestAnimationFrame(Animate);
			board.Update();
			if (board.UpdatesSinceCreation > 1 && !spawnedItems)
			{
				var items = Enumerable.Range(0, 8)
					.Select(GetItem)
					.ToList();
				board.FillWithItems(items);
				spawnedItems = true;
			}
			board.PreRender(new Point(200, 400), new Point(600, 400));
			renderer.Render(stage);
		}

		private static Item GetItem(int index)
		{
			if (index < 4) return new Square();
			if (index < 8) return new Diamond();
			if (index < 12) return new Circle();
			return new Snake();
		}
	}

}