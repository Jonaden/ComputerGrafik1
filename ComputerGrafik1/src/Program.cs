using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace ComputerGrafik1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = new GameWindowSettings()
            {              
                UpdateFrequency = 60.0
            };

            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1600, 900),
                Title = "The Dank 3D rendering program",
                //AspectRatio = (16, 9)
            };
            Game game = new Game(gameWindowSettings, nativeWindowSettings);
            game.Run();

        }
    }
}
