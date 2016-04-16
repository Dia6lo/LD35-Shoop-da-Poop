using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Square : Item
	{
		public Square(): base(Texture.FromImage("assets/Square.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.Square; }
		}
	}
}