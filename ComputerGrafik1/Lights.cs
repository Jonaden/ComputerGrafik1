using OpenTK.Mathematics;

namespace ComputerGrafik1
{
	public struct DirLight
	{
		public Vector3 Direction { get; set; }

		public Vector3 Ambient { get; set; }
		public Vector3 Diffuse { get; set; }
		public Vector3 Specular { get; set; }
	};

	public struct PointLight
	{
		public Vector3 Position { get; set; }

		public float Constant { get; set; }
		public float Linear { get; set; }
		public float Quadratic { get; set; }

		public Vector3 Ambient { get; set; }
		public Vector3 Diffuse { get; set; }
		public Vector3 Specular { get; set; }

		public PointLight(Vector3 position)
		{
			Position = position;
			Ambient = new Vector3(0.05f);
			Diffuse = new Vector3(0.8f);
			Specular = new Vector3(1.0f);
			Constant = 1.0f;
			Linear = 0.09f;
			Quadratic = 0.032f;
		}
	};

	public struct SpotLight
	{
		public Vector3 Position { get; set; }
		public Vector3 Direction { get; set; }
		public float CutOff { get; set; }
		public float OuterCutOff { get; set; }

		public Vector3 Ambient { get; set; }
		public Vector3 Diffuse { get; set; }
		public Vector3 Specular { get; set; }

		public float Constant { get; set; }
		public float Linear { get; set; }
		public float Quadratic { get; set; }
	};

}
