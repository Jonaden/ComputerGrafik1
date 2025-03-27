using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ComputerGrafik1
{
    public class Renderer
    {
        public Material material;
        Mesh mesh;
        public Renderer(Material material, Mesh mesh)
        {
            this.material = material;
            this.mesh = mesh;
        }

        public void Draw(in Matrix4 model, in Matrix4 viewProjection, in DirLight dirLight, in PointLight[] pointLights, in Matrix4 lightSpaceMatrix, Vector3 viewPos)
        {
            material.UseShader();
            material.SetUniform("model", model);
            material.SetUniform("viewProjection", viewProjection);
            material.SetUniform("lightSpaceMatrix", lightSpaceMatrix);

            // Directional light
            material.SetUniform("dirLight.direction", dirLight.Direction);
            material.SetUniform("dirLight.ambient", dirLight.Ambient);
            material.SetUniform("dirLight.diffuse", dirLight.Diffuse);
            material.SetUniform("dirLight.specular", dirLight.Specular);

            // Point lights
            for (int i = 0; i < pointLights.Length; i++)
            {
                material.SetUniform($"pointLights[{i}].position", pointLights[i].Position);
                material.SetUniform($"pointLights[{i}].ambient", pointLights[i].Ambient);
                material.SetUniform($"pointLights[{i}].diffuse", pointLights[i].Diffuse);
                material.SetUniform($"pointLights[{i}].specular", pointLights[i].Specular);
                material.SetUniform($"pointLights[{i}].constant", pointLights[i].Constant);
                material.SetUniform($"pointLights[{i}].linear", pointLights[i].Linear);
                material.SetUniform($"pointLights[{i}].quadratic", pointLights[i].Quadratic);
            }

            material.SetUniform("viewPos", viewPos);
            mesh.Draw();
        }

        public void RenderDepth(Shader shader, in Matrix4 model)
        {
            shader.SetMatrix("model", model);
			mesh.Draw();
		}
    }
}