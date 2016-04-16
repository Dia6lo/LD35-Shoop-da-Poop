using System;
using Bridge.Pixi;

namespace ShoopDaPoop.Application
{
	public class Item
	{
		public Item(Texture texture = null)
		{
			Sprite = new Sprite(texture);
			MoveSpeed = 4;
		}

		public IntPoint Position { get; set; }

		public ItemState State { get; set; }

		public Sprite Sprite { get; private set; }

		public void Update(Board board)
		{
			var summary = new BoardSummary(this, board);
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
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void HandleIdleState(BoardSummary summary)
		{
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

		private void AssignTarget(Cell target)
		{
			if (Target != null)
			{
				Target.TargetedBy = null;
			}
			Target = target;
			Target.TargetedBy = this;
			State = ItemState.Moving;
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
	}
}