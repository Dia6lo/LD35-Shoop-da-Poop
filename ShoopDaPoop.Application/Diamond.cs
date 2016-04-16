using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Diamond : Item
	{
		public Diamond(): base(Texture.FromImage("assets/Diamond.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.Diamond; }
		}
	}
}