using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Cell
	{
		public Cell(Texture texture = null)
		{
			Sprite = new Sprite(texture);
		}

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
		}
	}
}