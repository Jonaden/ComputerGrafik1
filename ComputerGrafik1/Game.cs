using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace ComputerGrafik1
{

	public class Game : GameWindow
    {
        NativeWindowSettings settings;
        Color4 clearColor = new Color4(0.2f, 0.3f, 0.3f, 1.0f);
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            GL.ClearColor(clearColor);
            settings = nativeWindowSettings;
        }

        Stopwatch watch = new Stopwatch();

        private PointLight[] _pointLights = new PointLight[4];

		DirLight _dirLight = new DirLight
        {
            Direction = new Vector3(-0.2f, -1.0f, -0.3f),
            Ambient = new Vector3(0.3f),
            Diffuse = new Vector3(0.8f),
            Specular = new Vector3(1.0f)
        };

        Shader debugDepthQuad;


		Shader simpleDepthShader;
		const int SHADOW_WIDTH = 1024, SHADOW_HEIGHT = 1024;
		DepthTexture depthMap;
        int depthMapFBO;

		List<GameObject> gameObjects = new List<GameObject>();
        Camera camera;
        protected override void OnLoad()
        {
            base.OnLoad();
            debugDepthQuad = new Shader("Shaders/shaderDebug.vert", "Shaders/shaderDebug.frag");
			debugDepthQuad.Use();
			debugDepthQuad.SetInt("depthMap", 0);

			simpleDepthShader = new Shader("Shaders/ShaderSDM.vert", "Shaders/ShaderSDM.frag");

			depthMapFBO = GL.GenFramebuffer();
			depthMap = new DepthTexture(depthMapFBO);


			ImageTexture texture0 = new ImageTexture("Textures/container2.png");
			ImageTexture texture1 = new ImageTexture("Textures/container2_specular.png");
            Dictionary<string, object> uniforms = new Dictionary<string, object>();
            uniforms.Add("material.diffuse", texture0);
            uniforms.Add("shadowMap", depthMap);
            uniforms.Add("material.specular", texture1);
            uniforms.Add("material.shininess", 32.0f);
            Material mat = new Material("Shaders/shaderLit.vert", "Shaders/shaderLit.frag", uniforms);
            Material mat1 = new Material("Shaders/shaderLit.vert", "Shaders/shaderBasic.frag", []);


			Vector3[] cubePositions =
		    {
			    new Vector3(0.0f, 0.0f, 0.0f),
			    new Vector3(2.0f, 5.0f, -15.0f),
			    new Vector3(-1.5f, -2.2f, -2.5f),
			    new Vector3(-3.8f, -2.0f, -12.3f),
			    new Vector3(2.4f, -0.4f, -3.5f),
			    new Vector3(-1.7f, 3.0f, -7.5f),
			    new Vector3(1.3f, -2.0f, -2.5f),
			    new Vector3(1.5f, 2.0f, -2.5f),
			    new Vector3(1.5f, 0.2f, -1.5f),
			    new Vector3(-1.3f, 1.0f, -1.5f)
		    };



		    Renderer rend = new Renderer(mat, new CylinderMesh());
            for (int i = 0; i < cubePositions.Length; i++)
            {
                GameObject cube = new GameObject(rend, this);
                cube.transform.Position = cubePositions[i];
                gameObjects.Add(cube);

                
            }

			GameObject cube1 = new GameObject(rend, this);
            cube1.transform.Scale = new Vector3(30f, 1.0f, 30f);
            cube1.transform.Position = new Vector3(0.0f, -5.0f, -4.0f);
			gameObjects.Add(cube1);

			GameObject cam = new GameObject(null, this);
			cam.AddComponent<Camera>(60.0f, (float)Size.X, (float)Size.Y, 0.3f, 1000.0f);
			camera = cam.GetComponent<Camera>();
			cam.AddComponent<CameraMovementBehaviour>();
			gameObjects.Add(cam);

			Vector3[] pointLightPositions = {
			new Vector3( 0.7f,  0.2f,   2.0f),
			new Vector3( 2.3f, -3.3f,  -4.0f),
			new Vector3(-4.0f,  2.0f, -12.0f),
			new Vector3( 0.0f,  0.0f,  -3.0f)};

			Renderer rend1 = new Renderer(mat1, new CubeMesh());
			for (int i = 0; i < _pointLights.Length; i++)
			{
				_pointLights[i] = new PointLight(pointLightPositions[i]);

			    GameObject light = new GameObject(rend1, this);
				light.transform.Position = pointLightPositions[i];
				light.transform.Scale = new Vector3(0.25f);
				//gameObjects.Add(light);

			}

            watch.Start();
            CursorState = CursorState.Grabbed;
			GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			Vector3 camPos = new Vector3(0.0f, 0.0f, 3.0f);

			Matrix4 lightProjection = Matrix4.CreateOrthographicOffCenter(-20.0f, 20.0f, -20, 20, 1.0f, 70.5f);
			//atrix4 lightProjection = Matrix4.CreatePerspectiveOffCenter(-4f, 4f, -4f, 4f, 1f, 50f);
			Matrix4 lightView = Matrix4.LookAt(new Vector3(0.8f, 4.0f, 1.2f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f));
			Matrix4 lightSpaceMatrix = lightView * lightProjection;
            // render from light's POV
            depthMap.Use(TextureUnit.Texture0);

			simpleDepthShader.Use();
            simpleDepthShader.SetMatrix("lightSpaceMatrix", lightSpaceMatrix);
			GL.Viewport(0, 0, SHADOW_WIDTH, SHADOW_HEIGHT);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFBO);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			gameObjects.ForEach(x => x.RenderDepth(simpleDepthShader));
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // reset viewport
            GL.Viewport(0, 0, 800, 600);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			gameObjects.ForEach(x => x.Draw(camera.GetViewProjection(), _dirLight, _pointLights, lightSpaceMatrix, camera.GetPosition()));

            // debug
            debugDepthQuad.Use();
            debugDepthQuad.SetFloat("near_plane", 1.0f);
            debugDepthQuad.SetFloat("far_plane", 200f);
            depthMap.Use(TextureUnit.Texture0);

            QuadMesh quadMesh = new QuadMesh();
            //quadMesh.Draw();

            SwapBuffers();

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            gameObjects.ForEach(x => x.Update(args));
        }

	}
}
