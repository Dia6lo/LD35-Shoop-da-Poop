using System;
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
		private static int level = 0;
		private static TextScreen textScreen;

		[Ready]
		public static void Main()
		{
			renderer = Pixi.AutoDetectRenderer(600, 600, new RendererOptions
			{
				BackgroundColor = 0x1099bb,
				//RoundPixels = true
			});
			Document.Body.AppendChild(renderer.View);
			var audio = new AudioElement("assets/untitled.mp3");
			Document.Body.AppendChild(audio);
			audio.Loop = true;
			audio.Play();

			stage = new Container();
			stage.AddChild(Sprite.FromImage("assets/Bathroom.png"));
			SetupBoard();
			textScreen = new TextScreen();
			textScreen.FadeOut();
			stage.AddChild(textScreen.Container);
			Animate();
		}

		private static void SetupBoard()
		{
			board = new Board(10, 10)
			{
				OnMatch = OnBoardMatch,
				OnComplete = OnBoardComplete
			};
			var temperatureDictionary = new Dictionary<float, IntPoint[]>
			{
				{
					0.2f, new[]
					{
						new IntPoint(0, 4),
						new IntPoint(0, 5),
						new IntPoint(4, 0),
						new IntPoint(5, 0),
						new IntPoint(4, 9),
						new IntPoint(5, 9),
						new IntPoint(9, 4),
						new IntPoint(9, 5)
					}
				},
				{
					0.4f, new[]
					{
						new IntPoint(1, 4),
						new IntPoint(1, 5),
						new IntPoint(4, 1),
						new IntPoint(5, 1),
						new IntPoint(4, 8),
						new IntPoint(5, 8),
						new IntPoint(8, 4),
						new IntPoint(8, 5)
					}
				},
				{
					0.6f, new[]
					{
						new IntPoint(2, 4),
						new IntPoint(2, 5),
						new IntPoint(4, 2),
						new IntPoint(5, 2),
						new IntPoint(4, 7),
						new IntPoint(5, 7),
						new IntPoint(7, 4),
						new IntPoint(7, 5)
					}
				},
				{
					0.8f, new[]
					{
						new IntPoint(3, 4),
						new IntPoint(3, 5),
						new IntPoint(4, 3),
						new IntPoint(5, 3),
						new IntPoint(4, 6),
						new IntPoint(5, 6),
						new IntPoint(6, 4),
						new IntPoint(6, 5)
					}
				}
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
			board.LoadTutorial();
			board.Interactive = false;
			//LoadLevel(3);
			stage.AddChild(board.Container);
		}

		private static void OnBoardComplete()
		{
			level++;
			LoadLevel(level);
		}

		private static void OnBoardMatch(Sprite arg)
		{
			var position = arg.ToGlobal(stage.Position).Subtract(
				new Point(arg.Width / 2, arg.Height / 2));
			var poop = new Poop(position)
			{
				Target = new Point(position.X, 400)
			};
			poops.Add(poop);
			stage.AddChild(poop.Sprite);
			poop.OnExit = () =>
			{
				poops.Remove(poop);
				stage.RemoveChild(poop.Sprite);
			};
		}

		private static void LoadLevel(int level)
		{
			var items = Enumerable.Range(0, 8 + level*4)
				.Select(GetItem)
				.ToList();
			board.FillWithItems(items);
		}

		private static List<Poop> poops = new List<Poop>();

		private static void Animate()
		{
			Window.RequestAnimationFrame(Animate);
			textScreen.Update();
			if (textScreen.CurrentState == TextScreen.State.Visible) return;
			board.Update();
			board.PreRender(new Point(200, 400), new Point(600, 400));
			if (board.UpdatesSinceCreation > 3)
				renderer.Render(stage);
			foreach (var poop in poops)
			{
				poop.Update();
			}
		}

		private static Item GetItem(int index)
		{
			if (index < 4) return new Square();
			if (index < 8) return new Diamond();
			if (index < 12) return new Circle();
			return new Snake();
		}
	}

	public class TextScreen
	{
		private Graphics graphics;

		public TextScreen(bool visible = true)
		{
			Container = new Container();
			CurrentState = visible ? State.Visible : State.Invisible;
			graphics = new Graphics
			{
				Alpha = visible ? 1f : 0f
			};
			Container.AddChild(graphics);
		}

		public Container Container { get; private set; }

		public State CurrentState { get; set; }

		public void FadeIn()
		{
			CurrentState = State.FadeIn;
		}

		public void FadeOut()
		{
			CurrentState = State.FadeOut;
		}

		public void Update()
		{
			switch (CurrentState)
			{
				case State.FadeIn:
					graphics.Alpha += 0.01f;
					if (graphics.Alpha >= 1f)
					{
						CurrentState = State.Visible;
						graphics.Alpha = 1f;
					}
					break;
				case State.FadeOut:
					graphics.Alpha -= 0.01f;
					if (graphics.Alpha <= 0f)
					{
						CurrentState = State.Visible;
						graphics.Alpha = 0f;
					}
					break;
			}
			graphics.Clear()
				.LineStyle(0, 0x000000, graphics.Alpha)
				.BeginFill(0x000000, graphics.Alpha)
				.DrawRect(0, 0, 600, 600)
				.EndFill();

		}

		public enum State
		{
			FadeIn,
			FadeOut,
			Visible,
			Invisible
		}
	}
}