using OpenTK.Mathematics;

namespace ComputerGrafik1
{
    public class Transform
    {
        public Vector3 Position;

        public Vector3 Rotation;

        private Matrix4 _transformMatrix;

        public Matrix4 TransformMatrix { get { return _transformMatrix; } }

        public Vector3 Scale = new Vector3(1, 1, 1);

        public void CalculateModel()
        {
            Matrix4 t = Matrix4.CreateTranslation(Position);
            Matrix4 rX = Matrix4.CreateRotationX(Rotation.X);
            Matrix4 rY = Matrix4.CreateRotationY(Rotation.Y);
            Matrix4 rZ = Matrix4.CreateRotationZ(Rotation.Z);
            Matrix4 s = Matrix4.CreateScale(Scale);

			_transformMatrix = s * rX * rY * rZ * t;
        }

		public Vector3 GetForwardVector()
		{
            Matrix4 transform = TransformMatrix;
			Vector3 forward = Vector3.Normalize(new Vector3(transform.M31, transform.M32, transform.M33));

			return forward;
		}
		public Vector3 GetRightVector()
		{
			Matrix4 transform = TransformMatrix;
			Vector3 forward = Vector3.Normalize(new Vector3(transform.M11, transform.M12, transform.M13));

			return forward;
		}
	}
}