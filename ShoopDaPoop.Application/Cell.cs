using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Cell
	{
		public Cell(Texture texture = null)
		{
			Sprite = new Sprite(texture)
			{
				Anchor = new Point(0.5f, 0.5f)
			};
		}

		public float Temperature { get; set; }

		public IntPoint Position { get; set; }

		public Point WorldPosition
		{
			get { return Sprite.Position; }
			set { Sprite.Position = value; }
		}

		public Sprite Sprite { get; private set; }

		public float Width
		{
			get { return Sprite.Texture.Width; }
		}

		public float Height
		{
			get { return Sprite.Texture.Height; }
		}

		public Item TargetedBy { get; set; }

		public void PreRender(Point position)
		{
			Sprite.Position.Set(position);
			Sprite.Alpha = 0;
			/*if (Sprite.Alpha == 0)
			{
				Sprite.Alpha = 0.1f;
			}*/
		}
	}
}