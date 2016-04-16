using System.Collections.Generic;
using Bridge.Pixi;
using Bridge.Pixi.Interaction;

namespace ShoopDaPoop.Application
{
	public class Board
	{
		private Texture itemTexture = Texture.FromImage("assets/poop.png");
		private int updatesSinceCreation;

		public Board(int width, int height)
		{
			Items = new List<Item>();
			Container = new Container();
			CellField = new CellField(width, height);
			Container.AddChild(CellField.Container);
			Container["interactive"] = true;
			Container.OnMouseDown(OnClick);
			//AddItem(2);
		}

		public CellField CellField { get; private set; }

		private void OnClick(InteractionEvent e)
		{
			Expand();
		}

		public void AddItem(int x)
		{
			var position = new IntPoint(x, -1);
			var cell = CellField[new IntPoint(x, 0)];
			var item = new Item(itemTexture)
			{
				Position = position,
				State = ItemState.Spawned,
				Sprite = {Position = new Point(cell.WorldPosition.X, cell.WorldPosition.Y - cell.Sprite.Height)},
			};
			Items.Add(item);
			Container.AddChild(item.Sprite);
		}

		public void Update()
		{
			updatesSinceCreation++;
			if (SpawningCell.TargetedBy == null && updatesSinceCreation > 3)
			{
				AddItem(SpawningX);
			}
			foreach (var item in Items)
			{
				item.Update(this);
			}
		}

		private void Expand()
		{
			CellField.Expand();
		}

		private int SpawningX
		{
			get { return Width/2; }
		}

		private Cell SpawningCell
		{
			get { return CellField[new IntPoint(SpawningX, 0)]; }
		}

		public void PreRender()
		{
			CellField.PreRender(new Point());
			foreach (var item in Items)
			{
				item.PreRender();
			}
		}

		public Container Container { get; private set; }

		public List<Item> Items { get; set; }

		public int Width
		{
			get { return CellField.Width; }
		}

		public int Height
		{
			get { return CellField.Height; }
		}

		public bool IsInBounds(IntPoint position)
		{
			return CellField.IsInBounds(position);
		}
	}
}
