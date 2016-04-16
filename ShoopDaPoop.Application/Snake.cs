using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Snake : Item
	{
		public Snake(): base(Texture.FromImage("assets/Snake.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.Snake; }
		}
	}
}