using System;
using System.Linq;
using Bridge.Pixi;
using Bridge.Pixi.Interaction;

namespace ShoopDaPoop.Application
{
	public abstract class Item
	{
		public Action OnDeath;

		public Item(Texture texture = null)
		{
			Sprite = new Sprite(texture)
			{
				Anchor = new Point(0.5f, 0.5f)
			};
			Sprite["interactive"] = true;
			Sprite.OnClick(OnClick);
			MoveSpeed = 4;
		}

		private void OnClick(InteractionEvent arg)
		{
			if (State != ItemState.Idle) return;
			var selectedItem = Board.Items.FirstOrDefault(i => i.Selected);
			if (selectedItem == null)
			{
				Selected = true;
			}
			else
			{
				var isNearHorizontally = Math.Abs(selectedItem.Position.X - Position.X) == 1 &&
				                         selectedItem.Position.Y == Position.Y;
				var isNearVertically = Math.Abs(selectedItem.Position.Y - Position.Y) == 1 &&
										 selectedItem.Position.X == Position.X;
				if (isNearVertically ^ isNearHorizontally)
				{
					Swap(selectedItem);
					SwappedWith = selectedItem;
					selectedItem.SwappedWith = this;
				}
				else
				{
					selectedItem.Selected = false;
					selectedItem.Sprite.Rotation = 0;
					Selected = true;
				}
			}
		}

		public void Swap(Item item)
		{
			var currentTarget = Target;
			Target = item.Target;
			Target.TargetedBy = this;
			State = ItemState.Moving;
			Selected = false;
			Sprite.Rotation = 0;
			item.Target = currentTarget;
			item.Target.TargetedBy = item;
			item.State = ItemState.Moving;
			item.Selected = false;
			item.Sprite.Rotation = 0;
		}

		public Item SwappedWith { get; set; }

		public bool Selected { get; set; }

		public Board Board { get; set; }

		public IntPoint Position { get; set; }

		public ItemState State { get; set; }

		public Sprite Sprite { get; private set; }

		public abstract ItemType Type { get; }

		public void Update()
		{
			var summary = new BoardSummary(this, Board);
			switch (State)
			{
				case ItemState.Spawned:
					HandleSpawnedState(summary);
					break;
				case ItemState.Moving:
					break;
				case ItemState.Idle:
					HandleIdleState(summary);
					break;
				case ItemState.Dying:
					HandleDying();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void HandleDying()
		{
			if (Sprite.Scale.X < 0.1f)
			{
				Sprite.Destroy();
				State = ItemState.Died;
				OnDeath();
				return;
			}
			Sprite.Scale = Sprite.Scale.Subtract(new Point(0.1f, 0.1f));
		}

		private void HandleIdleState(BoardSummary summary)
		{
			if (Selected)
			{
				Sprite.Rotation += 0.1f;
			}
			if (summary.PrefferedTarget != null)
			{
				AssignTarget(summary.PrefferedTarget);
			}
		}

		private void HandleSpawnedState(BoardSummary summary)
		{
			if (summary.PrefferedTarget != null)
			{
				AssignTarget(summary.PrefferedTarget);
			}
		}

		public void AssignTarget(Cell target)
		{
			if (Target != null)
			{
				Target.TargetedBy = null;
			}
			Target = target;
			Target.TargetedBy = this;
			State = ItemState.Moving;
			Selected = false;
			Sprite.Rotation = 0;
		}

		public Cell Target { get; set; }

		public void PreRender()
		{
			if (Target != null)
			{
				var differenceToTarget = Target.Sprite.Position.Subtract(Sprite.Position);
				if (Math.Abs(differenceToTarget.X) < MoveSpeed && Math.Abs(differenceToTarget.Y) < MoveSpeed)
				{
					State = ItemState.Idle;
					Position = Target.Position;
					Sprite.Position = Target.Sprite.Position;
					return;
				}
				State = ItemState.Moving;
				differenceToTarget.Normalize();
				Sprite.Position = Sprite.Position.Add(differenceToTarget.Multiply(MoveSpeed));
			}
		}

		public float MoveSpeed { get; set; }

		public void Die()
		{
			State = ItemState.Dying;
			Target.TargetedBy = null;
			Target = null;
		}
	}
}