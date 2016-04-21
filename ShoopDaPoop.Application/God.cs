using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class God : Item
	{
		public God(): base(Texture.FromImage("assets/God.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.God; }
		}
	}

	public class Snickers : Item
	{
		public Snickers() : base(Texture.FromImage("assets/Snickers.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.Snickers; }
		}
	}

	public class Fish : Item
	{
		public Fish() : base(Texture.FromImage("assets/Fish.png"))
		{

		}

		public override ItemType Type
		{
			get { return ItemType.Fish; }
		}
	}
}