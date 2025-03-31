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

		float _time;

        private PointLight[] _pointLights = new PointLight[4];

		DirLight _dirLight = new DirLight
        {
            Direction = new Vector3(-0.2f, -1.0f, -0.3f),
            Ambient = new Vector3(0.1f),
            Diffuse = new Vector3(0.2f, 0.2f, 0.2f),
            Specular = new Vector3(1.0f)
        };

		SpotLight _spotLight = new SpotLight
		{
			Position = new Vector3(-3.0f, 10.0f, 0.0f),
			Direction = new Vector3(0.5f, -1.0f, 0.1f)
		};


		Shader _simpleDepthShader_dir;
		Shader _simpleDepthShader_point;
		const int SHADOW_WIDTH = 1024, SHADOW_HEIGHT = 1024;

        int depthMapFBO_dir;
		DepthTexture _depthMap;

		int _depthMapFBO_point;
		DepthCubeMapTexture _depthCubemap;

		float _nearPlane = 0.1f;
		float _farPlane = 50.0f;

		List<GameObject> _gameObjects = new List<GameObject>();
        Camera _camera;

		Transform _pointLightTransform;

        protected override void OnLoad()
        {
            base.OnLoad();

			_simpleDepthShader_dir = new Shader("Shaders/DirShadowDepth.vert", "Shaders/DirShadowDepth.frag");
			_simpleDepthShader_point = new Shader("Shaders/pointShadowDepth.vert", "Shaders/pointShadowDepth.frag", "Shaders/pointShadowDepth.geom");

			depthMapFBO_dir = GL.GenFramebuffer();
			_depthMap = new DepthTexture(depthMapFBO_dir, SHADOW_WIDTH, SHADOW_HEIGHT);

            _depthMapFBO_point = GL.GenFramebuffer();
			_depthCubemap = new DepthCubeMapTexture(_depthMapFBO_point, SHADOW_WIDTH, SHADOW_HEIGHT);


			ImageTexture texture_diffuse = new ImageTexture("Textures/container2.png");
			ImageTexture texture_specular = new ImageTexture("Textures/container2_specular.png");
			ImageTexture texture_normal = new ImageTexture("Textures/brickwall_normal.jpg");
            Dictionary<string, object> uniforms = new Dictionary<string, object>();

            uniforms.Add("material.diffuse", texture_diffuse);
            uniforms.Add("material.specular", texture_specular);
            uniforms.Add("material.normal", texture_normal);
			uniforms.Add("material.shininess", 32.0f);
            uniforms.Add("shadowMap", _depthMap);
            uniforms.Add("depthMap", _depthCubemap);
			uniforms.Add("far_plane", _farPlane);
            Material mat_Lit = new Material("Shaders/shaderLit.vert", "Shaders/shaderLit.frag", uniforms);
            Material mat_Unlit = new Material("Shaders/shaderLit.vert", "Shaders/shaderBasic.frag", []);


            Model gunModel = new Model("Models/gun2.fbx");
		    Renderer gunRend = new Renderer(mat_Lit, gunModel);
            GameObject gunGO = new GameObject(gunRend, this);
			gunGO.Transform.Position = new Vector3(0.0f);
            _gameObjects.Add(gunGO);


		    Renderer boxRend = new Renderer(mat_Lit, new CubeMesh());
            for (int i = -10; i < 10; i++)
            {
				for (int j = -10; j < 10; j++)
				{
					GameObject cube = new GameObject(boxRend, this);
					cube.Transform.Position = new Vector3(i, -2, j);
					_gameObjects.Add(cube);
				}

            }

			GameObject cam = new GameObject(null, this);
			cam.AddComponent<Camera>(60.0f, (float)Size.X, (float)Size.Y, 0.3f, 1000.0f);
			_camera = cam.GetComponent<Camera>();
			cam.AddComponent<CameraMovementBehaviour>();
			_gameObjects.Add(cam);

			Vector3[] pointLightPositions = {
			new Vector3( 0.7f,  1.2f,   2.0f),
			new Vector3( 2.3f, -3.3f,  -4.0f),
			new Vector3(-4.0f,  2.0f, -12.0f),
			new Vector3( 0.0f,  0.0f,  -3.0f)};

			Renderer rendLight = new Renderer(mat_Unlit, new CubeMesh(), false);
			for (int i = 0; i < 1; i++)
			{
				_pointLights[i] = new PointLight(pointLightPositions[i]);

			    GameObject light = new GameObject(rendLight, this);
				light.Transform.Position = pointLightPositions[i];
				light.Transform.Scale = new Vector3(0.25f);
				_gameObjects.Add(light);
				_pointLightTransform = light.Transform;

			}

            CursorState = CursorState.Grabbed;
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);
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

            GL.ClearColor(clearColor);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.CullFace(TriangleFace.Front);


			// Directional/SpotLight Shadows ------------------------------------------------------------------------------------------------------------------
			// 0. Create depth texture transformation and matrices
			//Matrix4 lightProjection = Matrix4.CreateOrthographicOffCenter(-10.0f, 10.0f, -10, 10, _nearPlane, _farPlane);											  // for directional light shadows
			Matrix4 lightProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), SHADOW_WIDTH / SHADOW_HEIGHT, _nearPlane, _farPlane);  // for Spotlight shadows
			Matrix4 lightView = Matrix4.LookAt(new Vector3(_spotLight.Position), new Vector3(_spotLight.Position + _spotLight.Direction), new Vector3(0.0f, 1.0f, 0.0f));
			Matrix4 lightSpaceMatrix = lightView * lightProjection;

			// 1. render from light's POV
			_depthMap.Use(TextureUnit.Texture0);
			_simpleDepthShader_dir.Use();
            _simpleDepthShader_dir.SetMatrix("lightSpaceMatrix", lightSpaceMatrix);
			GL.Viewport(0, 0, SHADOW_WIDTH, SHADOW_HEIGHT);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFBO_dir);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			_gameObjects.ForEach(x => x.RenderDepth(_simpleDepthShader_dir));
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			
			// Point Shadows ---------------------------------------------------------------------------------------------------------------------------------
			// 0. create depth cubemap transformation matrices
			Matrix4 shadowProj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), SHADOW_WIDTH / SHADOW_HEIGHT, _nearPlane, _farPlane);
			List<Matrix4> shadowTransforms = new List<Matrix4>();
            Vector3 lightPos = _pointLightTransform.Position;                        
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(1.0f, 0.0f, 0.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 1.0f, 0.0f),  new Vector3(0.0f,  0.0f,  1.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f,  0.0f, -1.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, 1.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
			shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);

            // 1. Render scene to depth cubemap
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, _depthMapFBO_point);
			GL.Clear(ClearBufferMask.DepthBufferBit);
			_simpleDepthShader_point.Use();
			for (int i = 0; i < 6; i++)
				_simpleDepthShader_point.SetMatrix($"shadowMatrices[{i}]", shadowTransforms[i]);
			_simpleDepthShader_point.SetFloat("far_plane", _farPlane);
			_simpleDepthShader_point.SetVector3("lightPos", lightPos);
			_gameObjects.ForEach(x => x.RenderDepth(_simpleDepthShader_point));
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			// reset viewport
			GL.CullFace(TriangleFace.Back);
			GL.Viewport(0, 0, Size.X, Size.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			// render scene as normal
			_gameObjects.ForEach(x => x.Draw(_camera.GetViewProjection(), _dirLight, _spotLight, _pointLights, lightSpaceMatrix, _camera.Transform.Position));


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
            _gameObjects.ForEach(x => x.Update(args));

			//Vector3 offset = camera.Transform.GetForwardVector() * 0.5f + new Vector3(0.0f, -0.1f, 0.0f);
			//_spotLight.Position = camera.Transform.Position + offset;
			//_spotLight.Direction = camera.Transform.GetForwardVector();

			_pointLightTransform.Position = new Vector3(MathF.Sin(_time * 0.4f) * 5f, MathF.Sin(_time * 0.2f) + 2, MathF.Cos(_time * 0.4f) * 2f);
			_pointLights[0].Position = _pointLightTransform.Position;

			KeyboardState input = KeyboardState;
			MouseState mouse = MouseState;

			if (input.IsKeyPressed(Keys.Escape) && CursorState == CursorState.Grabbed)
				CursorState = CursorState.Normal;

			Vector2i mousePos = new Vector2i((int)mouse.X, (int)mouse.Y);
			if (mouse.IsButtonPressed(MouseButton.Left) && ClientRectangle.ContainsExclusive(mousePos) && CursorState == CursorState.Normal)
			{
				CursorState = CursorState.Grabbed;
			}
			
		}

	}
}
