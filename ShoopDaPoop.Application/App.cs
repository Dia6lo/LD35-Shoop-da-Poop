using Bridge;
using Bridge.Html5;
using Bridge.Pixi;
using Bridge.Pixi.Interaction;

namespace ShoopDaPoop.Application
{
	public class App
	{
		private static Sprite sprite;
		private static IRenderer renderer;

		[Ready]
		public static void Main()
		{
			renderer = Pixi.AutoDetectRenderer(800, 600, new RendererOptions {BackgroundColor = 0x1099bb});
			Document.Body.AppendChild(renderer.View);
			var stage = new Container();
			sprite = Sprite.FromImage("assets/bunny.png");
			sprite.Position.Set(230, 264);
			sprite["interactive"] = true;
			sprite.OnMouseDown(OnDown);
			sprite.OnTouchStart(OnDown);
			stage.AddChild(sprite);
			Animate();
		}

		private static void Animate()
		{
			Window.RequestAnimationFrame(Animate);
			renderer.Render(sprite);
		}

		private static void OnDown(InteractionEvent arg)
		{
			sprite.Scale.X += 0.3f;
			sprite.Scale.Y += 0.3f;
		}
	}

}