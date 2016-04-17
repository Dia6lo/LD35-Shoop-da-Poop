using System;
using System.Collections.Generic;
using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Poop
	{
		private float MoveSpeed = 1f;
		private PoopState state = PoopState.Spawning;
		public Point Target { get; set; }
		public Action OnExit { get; set; }
		public Sprite Sprite { get; set; }
		private int updatesSinceLastAnimation = RequiredUpdatesToAnimate;
		private const int RequiredUpdatesToAnimate = 5;
		private int currentAnimationFrame = 0;
		private Text caption;
		private Random random = new Random();
		private List<Texture> animationTextures = new List<Texture>
		{
			Texture.FromImage("assets/Poop_0.png"),
			Texture.FromImage("assets/Poop_1.png"),
			Texture.FromImage("assets/Poop_2.png"),
			Texture.FromImage("assets/Poop_3.png"),
		};
		private List<string> captions = new List<string>
		{
			"YAY!",
			"FREEDOM!",
			"OH BOY!",
			"I'LL BE BACK!",
			"FOR THE KING!",
			"( ͡° ͜ʖ ͡°)"
		};

		public Poop(Point position)
		{
			Sprite = Sprite.FromImage("assets/Poop_0.png");
			Sprite.Position = position;
			Sprite.Scale = new Point(0f, 0f);
			Sprite.Anchor = new Point(0.5f, 0.5f);
		}

		public void Update()
		{
			if (caption == null)
			{
				var randomValue = random.Next(500);
				if (randomValue == 499)
				{
					var text = captions[random.Next(captions.Count)];
					caption = new Text(text, new TextStyle
					{
						Stroke = random.Next(int.MaxValue)
					});
					caption.Position = new Point(
						-0.2f * (Sprite.Width + caption.Width),
						-0.5f * (Sprite.Height + caption.Height)
						);
					Sprite.AddChild(caption);
				}
			}
			switch (state)
			{
				case PoopState.Spawning:
					if (Sprite.Scale.X < 1f)
					{
						Sprite.Scale = Sprite.Scale.Add(new Point(0.1f, 0.1f));
					}
					else
					{
						state = PoopState.Falling;
					}
					break;
				case PoopState.Falling:
					var differenceToTarget = Target.Subtract(Sprite.Position);
					if (Math.Abs(differenceToTarget.X) < MoveSpeed && Math.Abs(differenceToTarget.Y) < MoveSpeed)
					{
						Sprite.Position = Target;
						state = PoopState.Running;
					}
					else
					{
						differenceToTarget.Normalize();
						Sprite.Position = Sprite.Position.Add(differenceToTarget.Multiply(MoveSpeed));
					}
					break;
				case PoopState.Running:
					Sprite.Position = Sprite.Position.Add(new Point(-1, 0).Multiply(MoveSpeed));
					if (updatesSinceLastAnimation++ >= RequiredUpdatesToAnimate)
					{
						updatesSinceLastAnimation = 0;
						currentAnimationFrame++;
						if (currentAnimationFrame == animationTextures.Count)
							currentAnimationFrame = 0;
						Sprite.Texture = animationTextures[currentAnimationFrame];
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
		}
	}
}