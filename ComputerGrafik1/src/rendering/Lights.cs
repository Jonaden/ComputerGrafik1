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
			Diffuse = new Vector3(0.5f, 0.5f, 1.0f);
			Specular = new Vector3(1.0f);
			Constant = 0.8f;
			Linear = 0.00009f;
			Quadratic = 0.00032f;
		}
	};

	public struct SpotLight
	{
		public SpotLight()
		{
			CutOff = MathF.Cos(MathHelper.DegreesToRadians(30.0f));
			OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(35.5f));

			Ambient = new Vector3(0.2f);
			Diffuse = new Vector3(1.0f);
			Specular = new Vector3(1.0f);

			Constant = 1.0f;
			Linear = 0.0045f;
			Quadratic = 0.00075f;
		}

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
