using Bridge.Html5;
using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class App
	{
		private static IRenderer renderer;
		private static Board board;

		[Ready]
		public static void Main()
		{
			renderer = Pixi.AutoDetectRenderer(800, 600, new RendererOptions {BackgroundColor = 0x1099bb});
			Document.Body.AppendChild(renderer.View);
			var stage = new Container();
			board = new Board(5, 5);
			board.Container.Position.Set(100, 100);
			stage.AddChild(board.Container);
			Animate();
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