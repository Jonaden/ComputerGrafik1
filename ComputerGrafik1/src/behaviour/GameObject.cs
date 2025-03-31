using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;


namespace ComputerGrafik1
{
    public class GameObject
    {
        public Transform Transform;
        public Renderer Renderer;

        List<Behaviour> behaviours = new List<Behaviour>();

        protected GameWindow gameWindow;
        public T GetComponent<T>() where T : Behaviour
        {
            foreach (Behaviour component in behaviours)
            {
                T componentAsT = component as T;
                if (componentAsT != null) return componentAsT;
            }
            return null;
        }
		public void AddComponent<T>(params object?[]? args) where T : Behaviour
		{
			if (args == null)
			{
				behaviours.Add(Activator.CreateInstance(typeof(T), this, gameWindow) as T);
			}
			else
			{
				int initialParameters = 2;
				int totalParams = args.Length + initialParameters;
				object?[]? objects = new object[totalParams];
				objects[0] = this;
				objects[1] = gameWindow;
				for (int i = initialParameters; i < totalParams; i++)
				{
					objects[i] = args[i - 2];
				}

				behaviours.Add(Activator.CreateInstance(typeof(T), objects) as T);
			}
		}

		public GameObject(Renderer renderer, GameWindow gameWindow)
        {
            this.Renderer = renderer;
            this.gameWindow = gameWindow;
            Transform = new Transform();
			Transform.CalculateModel();
		}

        public void Update(FrameEventArgs args)
        {
            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.Update(args);
            }
            Transform.CalculateModel();
        }

        public void Draw(in Matrix4 viewProjection, in DirLight dirLight, in SpotLight spotLight, in PointLight[] pointLights, in Matrix4 lightSpaceMatrix, Vector3 viewPos)
        {
            if (Renderer != null)
                Renderer.Draw(Transform.TransformMatrix, viewProjection, dirLight, spotLight, pointLights, lightSpaceMatrix, viewPos);
        }

        public void RenderDepth(Shader shader)
        {
			if (Renderer != null)
				Renderer.RenderDepth(shader, Transform.TransformMatrix);
        }
    }
}
