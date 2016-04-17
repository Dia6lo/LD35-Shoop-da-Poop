using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Pizza : Item
	{
		public Pizza(): base(Texture.FromImage("assets/Pizza.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.Diamond; }
		}
	}
}