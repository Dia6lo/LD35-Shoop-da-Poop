using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class CocaCola : Item
	{
		public CocaCola(): base(Texture.FromImage("assets/CocaCola.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.CocaCola; }
		}
	}
}