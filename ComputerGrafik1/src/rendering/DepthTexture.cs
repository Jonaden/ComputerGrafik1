using OpenTK.Graphics.OpenGL4;

namespace ComputerGrafik1
{
	public class DepthTexture : Texture
	{
		public DepthTexture(int depthMapFBO)
		{
			_handle = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, _handle);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, 1024, 1024, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
			float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f};
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

			// attach depth texture as FBO's deph buffer
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFBO);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _handle, 0);
			GL.DrawBuffer(DrawBufferMode.None);
			GL.ReadBuffer(ReadBufferMode.None);
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}

		public override void Use(TextureUnit unit = 0)
		{
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D, _handle);
		}
	}
}
