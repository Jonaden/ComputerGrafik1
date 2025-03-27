using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
public abstract class Texture
{
    protected int _handle;

    public abstract void Use(TextureUnit unit = 0);
}
