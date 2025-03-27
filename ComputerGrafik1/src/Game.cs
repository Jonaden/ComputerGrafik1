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

		float _time;

        private PointLight[] _pointLights = new PointLight[4];

		DirLight _dirLight = new DirLight
        {
            Direction = new Vector3(-0.2f, -1.0f, -0.3f),
            Ambient = new Vector3(0.1f),
            Diffuse = new Vector3(0.2f, 0.0f, 0.2f),
            Specular = new Vector3(1.0f)
        };

        Shader debugDepthQuad;


		Shader simpleDepthShader_dir;
		Shader simpleDepthShader_point;
		const int SHADOW_WIDTH = 1024, SHADOW_HEIGHT = 1024;

        int depthMapFBO_dir;
		DepthTexture depthMap;

		int depthMapFBO_point;
		DepthCubeMapTexture depthCubemap;

		float near_plane = 0.1f;
		float far_plane = 50.0f;

		List<GameObject> gameObjects = new List<GameObject>();
        Camera camera;
        protected override void OnLoad()
        {
            base.OnLoad();
            debugDepthQuad = new Shader("Shaders/shaderDebug.vert", "Shaders/shaderDebug.frag");
			debugDepthQuad.Use();
			debugDepthQuad.SetInt("depthMap", 0);

			simpleDepthShader_dir = new Shader("Shaders/ShaderSDM.vert", "Shaders/ShaderSDM.frag");
			simpleDepthShader_point = new Shader("Shaders/pointShadowDepth.vert", "Shaders/pointShadowDepth.frag", "Shaders/pointShadowDepth.geom");

			depthMapFBO_dir = GL.GenFramebuffer();
			depthMap = new DepthTexture(depthMapFBO_dir);

            depthMapFBO_point = GL.GenFramebuffer();
			depthCubemap = new DepthCubeMapTexture(depthMapFBO_point);


			ImageTexture texture0 = new ImageTexture("Textures/container2.png");
			ImageTexture texture1 = new ImageTexture("Textures/container2_specular.png");
			ImageTexture texture3 = new ImageTexture("Textures/container2_specular.png");
            Dictionary<string, object> uniforms = new Dictionary<string, object>();

            uniforms.Add("material.diffuse", texture0);
            uniforms.Add("shadowMap", depthMap);
            uniforms.Add("depthMap", depthCubemap);
            uniforms.Add("material.specular", texture1);
			uniforms.Add("material.shininess", 32.0f);
			uniforms.Add("far_plane", far_plane);
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

            Model gun = new Model("Models/gun2.fbx");

		    Renderer rend = new Renderer(mat, gun);
            for (int i = 0; i < cubePositions.Length; i++)
            {
                GameObject cube = new GameObject(rend, this);
                cube.transform.Position = cubePositions[i];
                gameObjects.Add(cube);


            }

		    Renderer boxRend = new Renderer(mat, new CubeMesh());
            GameObject cube1 = new GameObject(boxRend, this);
            cube1.transform.Scale = new Vector3(30f, 1.0f, 30f);
            cube1.transform.Position = new Vector3(0.0f, -5.0f, -4.0f);
			gameObjects.Add(cube1);

			GameObject cam = new GameObject(null, this);
			cam.AddComponent<Camera>(60.0f, (float)Size.X, (float)Size.Y, 0.3f, 1000.0f);
			camera = cam.GetComponent<Camera>();
			cam.AddComponent<CameraMovementBehaviour>();
			gameObjects.Add(cam);

			Vector3[] pointLightPositions = {
			new Vector3( 0.7f,  1.2f,   2.0f),
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
			//GL.Enable(EnableCap.CullFace);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

			_time += (float)args.Time;

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			Vector3 camPos = new Vector3(0.0f, 0.0f, 3.0f);


			// Dir Shadows -----------------------------------------------------------------------------------------------------------------------
			// 0. Create depth texture transformation and matrices
			Matrix4 lightProjection = Matrix4.CreateOrthographicOffCenter(-20.0f, 20.0f, -20, 20, near_plane, far_plane);
			//Matrix4 lightProjection = Matrix4.CreatePerspectiveOffCenter(-2f, 2f, -2f, 2f, 1f, 50f);
			Matrix4 lightView = Matrix4.LookAt(new Vector3(0.8f, 4.0f, 1.2f), new Vector3(0.0f), new Vector3(0.0f, 1.0f, 0.0f));
			Matrix4 lightSpaceMatrix = lightView * lightProjection;

            // 1. render from light's POV
            depthMap.Use(TextureUnit.Texture0);
			simpleDepthShader_dir.Use();
            simpleDepthShader_dir.SetMatrix("lightSpaceMatrix", lightSpaceMatrix);
			GL.Viewport(0, 0, SHADOW_WIDTH, SHADOW_HEIGHT);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFBO_dir);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			gameObjects.ForEach(x => x.RenderDepth(simpleDepthShader_dir));
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			GL.Viewport(0, 0, SHADOW_WIDTH, SHADOW_HEIGHT);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// Point Shadows ---------------------------------------------------------------------------------------------------------------------
			// 0. create depth cubemap transformation matrices
			Matrix4 shadowProj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), SHADOW_WIDTH / SHADOW_HEIGHT, near_plane, far_plane);
			//Matrix4 shadowProj = Matrix4.CreatePerspectiveOffCenter(-20f, 20f, -20f, 20f, near_plane, far_plane);
			_pointLights[0].Position = new Vector3(_pointLights[0].Position.X, _pointLights[0].Position.Y, MathF.Sin(_time));
			List<Matrix4> shadowTransforms = new List<Matrix4>();
            Vector3 lightPos = _pointLights[0].Position;                        
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(1.0f, 0.0f, 0.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 1.0f, 0.0f),  new Vector3(0.0f,  0.0f,  1.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f,  0.0f, -1.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, 1.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);

            // 1. Render scene to depth cubemap
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFBO_point);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			simpleDepthShader_point.Use();
			for (int i = 0; i < 6; i++)
				simpleDepthShader_point.SetMatrix($"shadowMatrices[{i}]", shadowTransforms[i]);
			simpleDepthShader_point.SetFloat("far_plane", far_plane);
			simpleDepthShader_point.SetVector3("lightPos", lightPos);
			gameObjects.ForEach(x => x.RenderDepth(simpleDepthShader_point));
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			// reset viewport
			GL.Viewport(0, 0, 800, 600);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			gameObjects.ForEach(x => x.Draw(camera.GetViewProjection(), _dirLight, _pointLights, lightSpaceMatrix, camera.GetPosition()));

            // debug for dir shadows
            debugDepthQuad.Use();
            debugDepthQuad.SetFloat("near_plane", near_plane);
            debugDepthQuad.SetFloat("far_plane", far_plane);
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
