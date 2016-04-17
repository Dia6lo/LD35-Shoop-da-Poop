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
		private BodyPoints bodyPoints;
		private const float ShapeShiftSpeed = 1f;
		public Action LeftArmAction;
		public Action RightArmAction;
		public Action LeftLegAction;
		public Action RightLegAction;
		public Action HeadAction;

		public Body()
		{
			graphics = new Graphics();
			head = new Sprite(Texture.FromImage("assets/Dood.png"))
			{
				Anchor = new Point(0.5f, 0.6f)
			};
			head["interactive"] = true;
			head.OnClick(OnHeadClick);
			leftLeg = new Sprite(leftLimbTexture)
			{
				Anchor = new Point(0.5f, 0)
			};
			leftLeg["interactive"] = true;
			leftLeg.OnClick(OnLeftLegClick);
			rightLeg = new Sprite(rightLimbTexture)
			{
				Anchor = new Point(0.5f, 0)
			};
			rightLeg["interactive"] = true;
			rightLeg.OnClick(OnRightLegClick);
			leftArm = new Sprite(leftLimbTexture)
			{
				Rotation = Pixi.DegToRad * 45f,
				Anchor = new Point(0.5f, 0)
			};
			leftArm["interactive"] = true;
			leftArm.OnClick(OnLeftArmClick);
			rightArm = new Sprite(rightLimbTexture)
			{
				Rotation = Pixi.DegToRad * -45f,
				Anchor = new Point(0.5f, 0)
			};
			rightArm["interactive"] = true;
			rightArm.OnClick(OnRightArmClick);
			Container = new Container();
			Container.AddChild(leftLeg);
			Container.AddChild(rightLeg);
			Container.AddChild(leftArm);
			Container.AddChild(rightArm);
			Container.AddChild(head);
			Container.AddChild(graphics);
		}

		private void OnHeadClick(InteractionEvent arg)
		{
			HeadAction();
		}

		private void OnRightArmClick(InteractionEvent arg)
		{
			RightArmAction();
		}

		private void OnLeftArmClick(InteractionEvent arg)
		{
			LeftArmAction();
		}

		private void OnRightLegClick(InteractionEvent arg)
		{
			RightLegAction();
		}

		private void OnLeftLegClick(InteractionEvent arg)
		{
			LeftLegAction();
		}

		public Container Container { get; set; }

		public void Render(BodyPoints newBodyPoints)
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
					new Tuple<Point, Point>(bodyPoints.MaxX, newBodyPoints.MaxX),
					new Tuple<Point, Point>(bodyPoints.MaxY, newBodyPoints.MaxY),
					new Tuple<Point, Point>(bodyPoints.MinX, newBodyPoints.MinX),
					new Tuple<Point, Point>(bodyPoints.MinY, newBodyPoints.MinY)
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

			graphics.Clear();
			graphics.LineStyle(10, 0xFF7000, 0.8f);
			graphics.MoveTo(bodyPoints.MinX.X, bodyPoints.MinX.Y);
			graphics.BeginFill(0xFF9F7B, 1);
			graphics.QuadraticCurveTo(bodyPoints.MinX.X, bodyPoints.MinY.Y, bodyPoints.MinY.X, bodyPoints.MinY.Y);
			graphics.QuadraticCurveTo(bodyPoints.MaxX.X, bodyPoints.MinY.Y, bodyPoints.MaxX.X, bodyPoints.MaxX.Y);
			graphics.QuadraticCurveTo(bodyPoints.MaxX.X, bodyPoints.MaxY.Y, bodyPoints.MaxY.X, bodyPoints.MaxY.Y);
			graphics.QuadraticCurveTo(bodyPoints.MinX.X, bodyPoints.MaxY.Y, bodyPoints.MinX.X, bodyPoints.MinX.Y);
			graphics.EndFill();
			var positionDifference = bodyPoints.MaxY.Subtract(bodyPoints.MinX);
			leftLeg.Position = bodyPoints.MaxY.Subtract(positionDifference.Multiply(0.5f));
			positionDifference = bodyPoints.MaxX.Subtract(bodyPoints.MaxY);
			rightLeg.Position = bodyPoints.MaxY.Add(positionDifference.Multiply(0.5f));
			leftArm.Position = bodyPoints.MinX;
			rightArm.Position = bodyPoints.MaxX;
			head.Position = bodyPoints.MinY;
			LeftFoot = leftLeg.Position.Add(new Point(-leftLeg.Width*0.3f, leftLeg.Height));
			RightFoot = rightLeg.Position.Add(new Point(rightLeg.Width * 0.3f, rightLeg.Height));
		}

		public Point LeftFoot { get; private set; }

		public Point RightFoot { get; private set; }
	}
}