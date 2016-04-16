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

		[Ready]
		public static void Main()
		{
			renderer = Pixi.AutoDetectRenderer(Window.InnerWidth, Window.InnerHeight, new RendererOptions
			{
				BackgroundColor = 0x1099bb,
				RoundPixels = true
			});
			Document.Body.AppendChild(renderer.View);
			var stage = new Container();
			board = new Board(6,6);
			var centerIndexes = new[] {2, 3};
			var bodyColumns = centerIndexes
				.SelectMany(x => Enumerable.Range(0, 6).Select(y => new IntPoint(x, y)));
			var bodyRows = centerIndexes
				.SelectMany(y => Enumerable.Range(0, 6).Select(x => new IntPoint(x, y)));
			board.CellField.SetTemperature(0.5f, bodyRows.Concat(bodyColumns).ToArray());
			var belly = new[]
			{
				new IntPoint(2, 2),
				new IntPoint(3, 2),
				new IntPoint(2, 3),
				new IntPoint(3, 3)
			};
			board.CellField.SetTemperature(1, belly);
			stage.AddChild(board.Container);
			var expandButton = new Sprite(Texture.FromImage("assets/Poop.png"))
			{
				Position = new Point(10, 600)
			};
			expandButton["interactive"] = true;
			expandButton.OnClick(OnClick);
			stage.AddChild(expandButton);
			Animate();
		}

		private static void OnClick(InteractionEvent arg)
		{
			board.Expand();
		}

		private static void Animate()
		{
			Window.RequestAnimationFrame(Animate);
			board.Update();
			board.PreRender();
			renderer.Render(board.Container);
		}
	}

}