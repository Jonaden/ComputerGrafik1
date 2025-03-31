using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ComputerGrafik1
{
    public class Renderer
    {
        public bool DepthTest = true;
        public Material Material;
        Mesh _mesh;
        Model _model;
        public Renderer(Material material, Mesh mesh, bool depthTest = true)
        {
            Material = material;
            _mesh = mesh;
			DepthTest = depthTest;

		}
        public Renderer(Material material, Model model, bool depthTest = true)
        {
            Material = material;
            _model = model;
			DepthTest = depthTest;
		}


        public void Draw(in Matrix4 model, in Matrix4 viewProjection, in DirLight dirLight, in SpotLight spotLight, in PointLight[] pointLights, in Matrix4 lightSpaceMatrix, Vector3 viewPos)
        {
            Material.UseShader();
            Material.SetUniform("model", model);
            Material.SetUniform("viewProjection", viewProjection);
            Material.SetUniform("lightSpaceMatrix", lightSpaceMatrix);
            Material.SetUniform("viewPos", viewPos);

            // Directional light
            Material.SetUniform("dirLight.direction", dirLight.Direction);
            Material.SetUniform("dirLight.ambient", dirLight.Ambient);
            Material.SetUniform("dirLight.diffuse", dirLight.Diffuse);
            Material.SetUniform("dirLight.specular", dirLight.Specular);

			// SpotLight
			Material.SetUniform("spotLight.position", spotLight.Position);
			Material.SetUniform("spotLight.direction", spotLight.Direction);
			Material.SetUniform("spotLight.cutOff", spotLight.CutOff);
			Material.SetUniform("spotLight.outerCutOff", spotLight.OuterCutOff);
			Material.SetUniform("spotLight.ambient", spotLight.Ambient);
			Material.SetUniform("spotLight.diffuse", spotLight.Diffuse);
			Material.SetUniform("spotLight.specular", spotLight.Specular);
			Material.SetUniform("spotLight.constant", spotLight.Constant);
			Material.SetUniform("spotLight.linear", spotLight.Linear);
			Material.SetUniform("spotLight.quadratic", spotLight.Quadratic);


			// Point lights
			for (int i = 0; i < pointLights.Length; i++)
            {
                Material.SetUniform($"pointLights[{i}].position", pointLights[i].Position);
                Material.SetUniform($"pointLights[{i}].ambient", pointLights[i].Ambient);
                Material.SetUniform($"pointLights[{i}].diffuse", pointLights[i].Diffuse);
                Material.SetUniform($"pointLights[{i}].specular", pointLights[i].Specular);
                Material.SetUniform($"pointLights[{i}].constant", pointLights[i].Constant);
                Material.SetUniform($"pointLights[{i}].linear", pointLights[i].Linear);
                Material.SetUniform($"pointLights[{i}].quadratic", pointLights[i].Quadratic);
            }


            if (_mesh != null)
            {
                _mesh.Draw();
            }
            if(_model != null)
            {
                _model.Draw();
            }
        }



        public void RenderDepth(Shader shader, in Matrix4 model)
        {
            shader.SetMatrix("model", model);
            GL.DepthMask(DepthTest);
			if (_mesh != null)
			{
				_mesh.Draw();
			}
			if (_model != null)
			{
				_model.Draw();
			}

            GL.DepthMask(true);

		}
    }
}