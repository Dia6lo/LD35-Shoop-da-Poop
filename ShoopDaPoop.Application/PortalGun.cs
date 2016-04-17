using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class PortalGun : Item
	{
		public PortalGun(): base(Texture.FromImage("assets/PortalGun.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.Snake; }
		}
	}
}