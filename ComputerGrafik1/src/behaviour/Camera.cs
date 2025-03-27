using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace ComputerGrafik1
{
	public class Camera : Behaviour
	{
		Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
		Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
		float speed = 1;
		private float FOV;
		private float aspectX;
		private float aspectY;
		private float near;
		private float far;

		public Camera(GameObject gameObject, Game window, float FOV, float aspectX, float aspectY, float near, float far) : base(gameObject, window)
		{
			gameObject.transform.Position = new Vector3(0.0f, 0.0f, 3.0f);
			this.FOV = FOV;
			this.aspectX = aspectX;
			this.aspectY = aspectY;
			this.near = near;
			this.far = far;
		}

		public override void Update(FrameEventArgs args)
		{
		}
		public Matrix4 GetViewProjection()
		{
			//Matrix4 view = Matrix4.LookAt(gameObject.transform.Position, gameObject.transform.Position + front, up);
			Matrix4 view;
			Matrix4.Invert(gameObject.transform.TransformMatrix, out view);
			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), aspectX / aspectY, near, far);
			return view * projection;
		}

		public Vector3 GetPosition()
		{
			return gameObject.transform.Position;
		}
	}
}