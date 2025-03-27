using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ComputerGrafik1
{
    public class Renderer
    {
        public Material material;
        Mesh mesh;
        Model model;
        public Renderer(Material material, Mesh mesh)
        {
            this.material = material;
            this.mesh = mesh;
        }

        public Renderer(Material material, Model model)
        {
            this.material = material;
            this.model = model;
        }

        public void Draw(Matrix4 mvp)
        {
            material.UseShader();
            material.SetUniform("mvp", mvp);
            if (mesh != null)
            {
                mesh.Draw();
            }
            if(model != null)
            {
                model.Draw();
            }
        }
    }
}