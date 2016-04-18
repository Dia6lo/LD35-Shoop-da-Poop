using System;
using System.Collections.Generic;
using System.Linq;
using Bridge.Html5;
using Bridge.Pixi;
using Bridge.Pixi.Interaction;
using Text = Bridge.Pixi.Text;
using TextAlign = Bridge.Pixi.TextAlign;

namespace ShoopDaPoop.Application
{
	public class App
	{
		private static IRenderer renderer;
		private static Board board;
		private static Container stage;
		private static int level = 0;
		private static TextScreen textScreen;
		private static Container poopContainer = new Container();
		private static Sprite hint = Sprite.FromImage("assets/Hint.png");
		private static Sprite restartButton = Sprite.FromImage("assets/Dood.png");

		[Ready]
		public static void Main()
		{
			renderer = Pixi.AutoDetectRenderer(600, 600, new RendererOptions
			{
				BackgroundColor = 0x1099bb,
				//RoundPixels = true
			});
			Document.Body.AppendChild(renderer.View);
			var audio = new AudioElement("assets/sandstorm.mp3")
			{
				Loop = true
			};
			Document.Body.AppendChild(audio);
			audio.Play();
			stage = new Container();
			stage.AddChild(Sprite.FromImage("assets/Bathroom.png"));
			SetupBoard();
			stage.AddChild(hint);
			hint.Visible = false;
			hint.Position = new Point(320, 120);
			stage.AddChild(restartButton);
			restartButton["interactive"] = false;
			restartButton.OnClick(OnRestartClick);
			restartButton.Alpha = 0f;
			restartButton.Position = new Point(510, 410);
			textScreen = new TextScreen();
			textScreen.LoadStartingScreen();
			textScreen.OnFadeOut = () =>
			{
				board.LoadTutorial();
				hint.Visible = true;
				restartButton["interactive"] = true;
			};
			stage.AddChild(poopContainer);
			stage.AddChild(textScreen.Container);
			Animate();
		}

		private static void OnRestartClick(InteractionEvent arg)
		{
			board.Restart();
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
			stage.AddChild(board.Container);
		}

		private static void OnBoardComplete()
		{
			level++;
			hint.Visible = false;
			textScreen.FadeIn();
			if (level == 5)
			{
				textScreen.LoadGameOver();
			}
			else
			{
				textScreen.LoadPreLevelScreen(level);
			}
			textScreen.OnFadeOut = () => LoadLevel(level);
			restartButton["interactive"] = false;
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
			poopContainer.AddChild(poop.Sprite);
			poop.OnExit = () =>
			{
				poops.Remove(poop);
				poopContainer.RemoveChild(poop.Sprite);
			};
		}

		private static void LoadLevel(int level)
		{
			var items = Enumerable.Range(0, 8 + level*4)
				.Select(GetItem)
				.ToList();
			board.FillWithItems(items);
			restartButton["interactive"] = true;
		}

		private static List<Poop> poops = new List<Poop>();

		private static void Animate()
		{
			Window.RequestAnimationFrame(Animate);
			textScreen.Update();
			if (textScreen.CurrentState != TextScreen.State.Visible)
			{
				board.Update();
				board.PreRender(new Point(200, 400), new Point(600, 400));
				foreach (var poop in poops)
				{
					poop.Update();
				}
			}
			else
			{
				foreach (var poop in poops)
				{
					poop.OnExit();
				}
			}
			renderer.Render(stage);
		}

		private static Item GetItem(int index)
		{
			if (index < 4) return new CocaCola();
			if (index < 8) return new Pizza();
			if (index < 12) return new Snickers();
			if (index < 16) return new Fish();
			if (index < 20) return new PortalGun();
			return new God();
		}
	}

	public class TextScreen
	{
		private Graphics graphics;
		private bool shiaOnScreen;
		private int updatesSinceVisible;
		private Text shiaHint;
		private Container content = new Container();
		public Action OnFadeOut;

		public TextScreen(bool visible = true)
		{
			Container = new Container
			{
				Alpha = visible ? 1f : 0f
			};
			CurrentState = visible ? State.Visible : State.Invisible;
			graphics = new Graphics();
			Container.AddChild(graphics);
			Container.AddChild(content);
		}

		public void LoadGameOver()
		{
			var firstLine = GetText("Good job, my friend!");
			firstLine.Position = new Point(300, 25);
			var secondLine = GetText("after THAT MUCH effort");
			secondLine.Position = new Point(300, 75);
			var thirdLine = GetText("You've got best body in");
			thirdLine.Position = new Point(300, 125);
			var preFourthLine = GetText("THE WORLD (time stops)");
			preFourthLine.Position = new Point(300, 175);
			var fourthLine = GetText("YOU JUST DID IT!");
			fourthLine.Position = new Point(300, 225);
			content.AddChild(firstLine);
			content.AddChild(secondLine);
			content.AddChild(thirdLine);
			content.AddChild(preFourthLine);
			content.AddChild(fourthLine);
			LoadShia(false, "That's it");
		}

		public void LoadPreLevelScreen(int level)
		{
			var firstLine = GetText("Good job, my friend!");
			firstLine.Position = new Point(300, 25);
			var secondLine = GetText("but " + 1 + " year" + (level == 1 ? "" : "s") + " later");
			secondLine.Position = new Point(300, 75);
			var thirdLine = GetText("You've got even MORE FAT");
			thirdLine.Position = new Point(300, 125);
			var fourthLine = GetText("JUST DO IT! AGAIN! (YES, YOU CAN!)");
			fourthLine.Position = new Point(300, 175);
			content.AddChild(firstLine);
			content.AddChild(secondLine);
			content.AddChild(thirdLine);
			content.AddChild(fourthLine);
			LoadShia();
		}

		public void LoadStartingScreen()
		{
			var firstLine = GetText("You've worked hard last year");
			firstLine.Position = new Point(300, 25);
			var secondLine = GetText("to get FAT (shame on you)");
			secondLine.Position = new Point(300, 75);
			var thirdLine = GetText("It's time to get in SHAPE before summer");
			thirdLine.Position = new Point(300, 125);
			var fourthLine = GetText("JUST DO IT! (YES, YOU CAN!)");
			fourthLine.Position = new Point(300, 175);
			content.AddChild(firstLine);
			content.AddChild(secondLine);
			content.AddChild(thirdLine);
			content.AddChild(fourthLine);
			LoadShia();
		}

		private void LoadShia(bool interactive = true, string hint = "Click me ->")
		{
			var shia = Sprite.FromImage("assets/Shia.png");
			shia.Anchor = new Point(0.5f, 0f);
			shia.Position = new Point(300, 310);
			shia["interactive"] = interactive;
			shia.OnClick(e =>
			{
				shia["interactive"] = false;
				FadeOut();
				shiaOnScreen = false;
				if (OnFadeOut != null)
				{
					OnFadeOut();
				}
			});
			content.AddChild(shia);
			shiaOnScreen = true;
			shiaHint = GetText(hint);
			shiaHint.Position = new Point(150, 450);
			shiaHint.Visible = false;
			content.AddChild(shiaHint);
		}

		private Text GetText(string text)
		{
			return new Text(text, new TextStyle
			{
				Fill = "white",
				Align = TextAlign.Center
			})
			{
				Anchor = new Point(0.5f, 0),
			};
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
					Container.Alpha += 0.01f;
					if (Container.Alpha >= 1f)
					{
						CurrentState = State.Visible;
						updatesSinceVisible = 0;
						Container.Alpha = 1f;
					}
					break;
				case State.FadeOut:
					Container.Alpha -= 0.01f;
					if (Container.Alpha <= 0f)
					{
						CurrentState = State.Invisible;
						Container.Alpha = 0f;
						content.RemoveChildren();
						shiaHint = null;
					}
					break;
				case State.Visible:
					updatesSinceVisible++;
					if (updatesSinceVisible > 180 && shiaOnScreen)
						shiaHint.Visible = true;
					break;
			}
			graphics.Clear()
				.BeginFill(0x333333)
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