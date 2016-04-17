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
				Anchor = new Point(0.5f, 0.5f),
				Scale = new Point()
			};
			MoveSpeed = 1;
		}

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
			if (Sprite.Scale.X < 1f)
			{
				Sprite.Scale.X += 0.1f;
				Sprite.Scale.Y += 0.1f;
			}
			else
			{
				Sprite.Scale = new Point(1, 1);
				State = ItemState.Idle;
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
			if (State == ItemState.Spawned) return;
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