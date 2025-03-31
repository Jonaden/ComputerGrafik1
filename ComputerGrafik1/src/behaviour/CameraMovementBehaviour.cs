using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ComputerGrafik1
{
	public class CameraMovementBehaviour : Behaviour
	{
		public CameraMovementBehaviour(GameObject gameObject, Game window) : base(gameObject, window)
		{
		}
		float baseMovementSpeed = 5.0f;
		float fastMovementSpeed = 10.0f;
		float movementSpeed;
		private bool FirstMove;
		private Vector2 lastPos;
		private float Sensitivity = 0.05f;

		public override void Update(FrameEventArgs args)
		{

			KeyboardState input = window.KeyboardState;
			MouseState mouse = window.MouseState;
			if (input.IsKeyDown(Keys.LeftShift))
			{
				movementSpeed = fastMovementSpeed;
			}
			else
			{
				movementSpeed = baseMovementSpeed;
			}

			if (input.IsKeyDown(Keys.W))
			{
				gameObject.Transform.Position += gameObject.Transform.GetForwardVector() * movementSpeed * (float)args.Time;

			}
			if (input.IsKeyDown(Keys.S))
			{
				gameObject.Transform.Position -= gameObject.Transform.GetForwardVector() * movementSpeed * (float)args.Time;
			}

			if (input.IsKeyDown(Keys.A))
			{
				gameObject.Transform.Position -= gameObject.Transform.GetRightVector() * movementSpeed * (float)args.Time;

			}
			if (input.IsKeyDown(Keys.D))
			{
				gameObject.Transform.Position += gameObject.Transform.GetRightVector() * movementSpeed * (float)args.Time;
			}


			if (FirstMove)
			{
				lastPos = new Vector2(mouse.X, mouse.Y);
				FirstMove = false;
			}
			else
			{
				float deltaX = mouse.X - lastPos.X;
				float deltaY = mouse.Y - lastPos.Y;
				lastPos = new Vector2(mouse.X, mouse.Y);

			if (window.CursorState == CursorState.Normal)
				return;

				gameObject.Transform.Rotation.Y -= deltaX * Sensitivity * (float)args.Time;
				gameObject.Transform.Rotation.X -= deltaY * Sensitivity * (float)args.Time;
				
				if (gameObject.Transform.Rotation.X > 1.57)
				{
					gameObject.Transform.Rotation.X = 1.57f;
				}
				else if (gameObject.Transform.Rotation.X < -1.57f)
				{
					gameObject.Transform.Rotation.X = -1.57f;
				}
			}
		}
	}
}
