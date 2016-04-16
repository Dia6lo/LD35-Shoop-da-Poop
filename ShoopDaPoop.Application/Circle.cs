using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Circle : Item
	{
		public Circle(): base(Texture.FromImage("assets/Circle.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.Circle; }
		}
	}
}