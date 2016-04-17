using System;
using System.Collections.Generic;
using Bridge.Pixi;
using Bridge.Pixi.Interaction;

namespace ShoopDaPoop.Application
{
	public class Body
	{
		private Graphics graphics;
		private Texture rightLimbTexture = Texture.FromImage("assets/RightLimb.png");
		private Texture leftLimbTexture = Texture.FromImage("assets/LeftLimb.png");
		private Sprite leftLeg;
		private Sprite rightLeg;
		private Sprite leftArm;
		private Sprite rightArm;
		private Sprite head;
		private Sprite pinus;
		private BodyPoints bodyPoints;
		private const float ShapeShiftSpeed = 1f;
		private DragData dragData;
		public Dictionary<Limb, DragActions> DragActions = new Dictionary<Limb, DragActions>();
		private Texture normalFace = Texture.FromImage("assets/Dood.png");
		private Texture yeeeFace = Texture.FromImage("assets/Yeee.png");
		private Texture painFace = Texture.FromImage("assets/Pain.png");

		public Body()
		{
			var pullOffset = 50f;
			var pushOffset = 20f;
			graphics = new Graphics();
			head = new Sprite(normalFace)
			{
				Anchor = new Point(0.5f, 0.6f)
			};
			head["interactive"] = true;
			ApplyDragBehavior(head, new DragDataParams
			{
				Limb = Limb.Head,
				DragResult = (from, to) =>
				{
					if (from.Y - to.Y >= pullOffset) return DragStatus.Pull;
					if (to.Y - from.Y >= pushOffset) return DragStatus.Push;
					return DragStatus.None;
				},
				RotationMultiplier = (from, to) => to.X < from.X ? -1 : 1
			});

			leftLeg = new Sprite(leftLimbTexture)
			{
				Anchor = new Point(0.5f, 0)
			};

			rightLeg = new Sprite(rightLimbTexture)
			{
				Anchor = new Point(0.5f, 0)
			};

			leftArm = new Sprite(leftLimbTexture)
			{
				Rotation = Pixi.DegToRad*45f,
				Anchor = new Point(0.5f, 0.1f)
			};
			leftArm["interactive"] = true;
			ApplyDragBehavior(leftArm, new DragDataParams
			{
				Limb = Limb.LeftArm,
				DragResult = (from, to) =>
				{
					if (from.X - to.X >= pullOffset) return DragStatus.Pull;
					if (to.X - from.X >= pushOffset) return DragStatus.Push;
					return DragStatus.None;
				},
				RotationMultiplier = (from, to) => from.Y < to.Y ? -1 : 1
			});

			rightArm = new Sprite(rightLimbTexture)
			{
				Rotation = Pixi.DegToRad*-45f,
				Anchor = new Point(0.5f, 0.1f)
			};
			rightArm["interactive"] = true;
			ApplyDragBehavior(rightArm, new DragDataParams
			{
				Limb = Limb.RightArm,
				DragResult = (from, to) =>
				{
					if (to.X - from.X >= pullOffset) return DragStatus.Pull;
					if (from.X - to.X >= pushOffset) return DragStatus.Push;
					return DragStatus.None;
				},
				RotationMultiplier = (from, to) => to.Y < from.Y ? -1 : 1
			});


			pinus = new Sprite(Texture.FromImage("assets/Pinus.png"))
			{
				Anchor = new Point(0.5f, 0.1f)
			};
			pinus["interactive"] = true;
			ApplyDragBehavior(pinus, new DragDataParams
			{
				Limb = Limb.Pinus,
				DragResult = (from, to) =>
				{
					if (to.Y - from.Y >= pullOffset) return DragStatus.Pull;
					if (from.Y - to.Y >= pushOffset) return DragStatus.Push;
					return DragStatus.None;
				},
				RotationMultiplier = (from, to) => from.X < to.X ? -1 : 1
			});

			Container = new Container();
			Container.AddChild(leftLeg);
			Container.AddChild(rightLeg);
			Container.AddChild(head);
			Container.AddChild(leftArm);
			Container.AddChild(rightArm);
			Container.AddChild(graphics);
			Container.AddChild(pinus);
		}

		private void ApplyDragBehavior(Sprite sprite, DragDataParams @params)
		{
			sprite.OnMouseDown(e => OnDragStart(sprite, e, @params))
				.OnTouchStart(e => OnDragStart(sprite, e, @params))
				.OnMouseUp(e => OnDragEnd(sprite, e))
				.OnMouseUpOutside(e => OnDragEnd(sprite, e))
				.OnTouchEnd(e => OnDragEnd(sprite, e))
				.OnTouchEndOutside(e => OnDragEnd(sprite, e))
				.OnTouchMove(e => OnDragMove(sprite, e))
				.On("mousemove", new Action<InteractionEvent>(e => OnDragMove(sprite, e)));
		}

		private void OnDragMove(Sprite target, InteractionEvent arg)
		{
			if (dragData == null || dragData.Target != target) return;
			dragData.Current = arg.Data.GetLocalPosition(Container);
		}

		private void OnDragEnd(Sprite target, InteractionEvent arg)
		{
			if (dragData == null || dragData.Target != target) return;
			dragData.Target.Rotation = 0;
			dragData = null;
			head.Texture = normalFace;
		}

		private void OnDragStart(Sprite target, InteractionEvent arg, DragDataParams @params)
		{
			if (dragData != null) return;
			dragData = new DragData(@params)
			{
				Target = target,
				Start = arg.Data.GetLocalPosition(Container),
				Current = arg.Data.GetLocalPosition(Container),
				StartTargetPosition = target.Position.Clone()
			};
			switch (dragData.Params.Limb)
			{
				case Limb.LeftArm:
				case Limb.RightArm:
				case Limb.Head:
					head.Texture = painFace;
					break;
				case Limb.Pinus:
					head.Texture = yeeeFace;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public Container Container { get; set; }

		public void Render(BodyPoints newBodyPoints, BodyPoints bellyPoints)
		{
			if (bodyPoints == null)
			{
				if (newBodyPoints == null) return;
				bodyPoints = newBodyPoints;
			}
			else
			{
				var pointPairs = new List<Tuple<Point, Point>>
				{
					new Tuple<Point, Point>(bodyPoints.MaxX, newBodyPoints.MaxX), new Tuple<Point, Point>(bodyPoints.MaxY, newBodyPoints.MaxY), new Tuple<Point, Point>(bodyPoints.MinX, newBodyPoints.MinX), new Tuple<Point, Point>(bodyPoints.MinY, newBodyPoints.MinY)
				};
				foreach (var tuple in pointPairs)
				{
					var from = tuple.Item1.Clone();
					var to = tuple.Item2.Clone();
					Point result;
					var differenceToTarget = to.Subtract(from);
					if (Math.Abs(differenceToTarget.X) < ShapeShiftSpeed && Math.Abs(differenceToTarget.Y) < ShapeShiftSpeed)
					{
						result = to;
					}
					else
					{
						differenceToTarget.Normalize();
						result = from.Add(differenceToTarget.Multiply(ShapeShiftSpeed));
					}
					tuple.Item1.X = result.X;
					tuple.Item1.Y = result.Y;
				}
			}

			graphics.Clear().LineStyle(10, 0xFF7000, 0.8f).MoveTo(bodyPoints.MinX.X, bodyPoints.MinX.Y).BeginFill(0xFF9F7B, 1).QuadraticCurveTo(bodyPoints.MinX.X, bodyPoints.MinY.Y, bodyPoints.MinY.X, bodyPoints.MinY.Y).QuadraticCurveTo(bodyPoints.MaxX.X, bodyPoints.MinY.Y, bodyPoints.MaxX.X, bodyPoints.MaxX.Y).QuadraticCurveTo(bodyPoints.MaxX.X, bodyPoints.MaxY.Y, bodyPoints.MaxY.X, bodyPoints.MaxY.Y).QuadraticCurveTo(bodyPoints.MinX.X, bodyPoints.MaxY.Y, bodyPoints.MinX.X, bodyPoints.MinX.Y).EndFill().LineStyle(5, 0xFF3333).BeginFill(0xFF5555).DrawRoundedRect(bellyPoints.MinX.X, bellyPoints.MinY.Y, bellyPoints.MaxX.X - bellyPoints.MinX.X, bellyPoints.MaxY.Y - bellyPoints.MinY.Y, 3).EndFill();
			var positionDifference = bodyPoints.MaxY.Subtract(bodyPoints.MinX);
			leftLeg.Position = bodyPoints.MaxY.Subtract(positionDifference.Multiply(0.5f));
			positionDifference = bodyPoints.MaxX.Subtract(bodyPoints.MaxY);
			rightLeg.Position = bodyPoints.MaxY.Add(positionDifference.Multiply(0.5f));
			leftArm.Position = bodyPoints.MinX;
			rightArm.Position = bodyPoints.MaxX;
			head.Position = bodyPoints.MinY;
			pinus.Position = bodyPoints.MaxY.Subtract(new Point(0, 10));
			if (dragData != null)
			{
				dragData.ApplyDragging();
				dragData.ApplyRotation();
				var handler = DragActions[dragData.Params.Limb];
				switch (dragData.GetDragStatus())
				{
					case DragStatus.Pull:
						handler.Pull();
						break;
					case DragStatus.Push:
						handler.Push();
						break;
				}
			}
			LeftFoot = leftLeg.Position.Add(new Point(-leftLeg.Width*0.3f, leftLeg.Height));
			RightFoot = rightLeg.Position.Add(new Point(rightLeg.Width*0.3f, rightLeg.Height));
		}

		public Point LeftFoot { get; private set; }

		public Point RightFoot { get; private set; }

		private class DragDataParams
		{
			public Func<Point, Point, DragStatus> DragResult { get; set; }
			public Func<Point, Point, float> RotationMultiplier { get; set; }
			public Limb Limb { get; set; }
		}

		private class DragData
		{
			public DragData(DragDataParams @params)
			{
				Params = @params;
			}

			public DragDataParams Params { get; set; }
			public Sprite Target { get; set; }
			public Point Start { get; set; }
			public Point StartTargetPosition { get; set; }
			public Point Current { get; set; }

			public void ApplyDragging()
			{
				var difference = Current.Subtract(Target.Position);
				Target.Position = Target.Position.Add(difference.Multiply(0.2f));
			}

			public void ApplyRotation()
			{
				var currentVector = Start.Subtract(StartTargetPosition);
				var targetVector = Current.Subtract(StartTargetPosition);
				var currentLength = currentVector.Length();
				var targetLength = targetVector.Length();
				var dotProduct = currentVector.X*targetVector.X + currentVector.Y*targetVector.Y;
				var cos = dotProduct/(currentLength*targetLength);
				var angle = (float) Math.Acos(cos);
				if (Params.RotationMultiplier != null)
					angle *= Params.RotationMultiplier(currentVector, targetVector);
				Target.Rotation = angle;
				Bridge.Html5.Console.WriteLine(targetVector.X + " " + targetVector.Y + " " + currentVector.X + " " + currentVector.Y + " " + cos + " " + angle*Pixi.RadToDeg);
			}

			public DragStatus GetDragStatus()
			{
				return Params.DragResult(Start, Current);
			}
		}

		private enum DragStatus
		{
			None,
			Pull,
			Push
		}
	}

	public enum FaceStatus
	{
		Normal,
		Yeee,
		Pain
	}

	public class DragActions
	{
		public Action Pull { get; set; }
		public Action Push { get; set; }
	}

	public enum Limb
	{
		LeftArm,
		RightArm,
		Head,
		Pinus
	}
}