using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGrafik1
{
    public class QuadBeheaviour2 : Behaviour
    {
        float movementSpeed = 1;
        public QuadBeheaviour2(GameObject gameObject, Game window) : base(gameObject, window) { }
        float elapsedTime;

        public override void Update(FrameEventArgs args)
        {
            KeyboardState input = window.KeyboardState;
            elapsedTime += (float)args.Time;
            if (input.IsKeyDown(Keys.Left))
            {
                gameObject.transform.Position.X -= movementSpeed * (float)args.Time;

            }
            if (input.IsKeyDown(Keys.Right))
            {
                gameObject.transform.Position.X += movementSpeed * (float)args.Time;
            }
            gameObject.renderer.material.SetUniform("theta", elapsedTime);
        }
    }
}
