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
    public class QuadBeheaviour : Behaviour
    {
        float movementSpeed = 1;
        public QuadBeheaviour(GameObject gameObject, Game window) : base(gameObject, window)
        {
        }

        public override void Update(FrameEventArgs args)
        {
            KeyboardState input = window.KeyboardState;

            if (input.IsKeyDown(Keys.Up))
            {
                gameObject.transform.Rotation.X += movementSpeed * (float)args.Time;

            }
            if (input.IsKeyDown(Keys.Down))
            {
                gameObject.transform.Rotation.X -= movementSpeed * (float)args.Time;

            }
			if (input.IsKeyDown(Keys.Left))
			{
				gameObject.transform.Rotation.Y += movementSpeed * (float)args.Time;

			}
			if (input.IsKeyDown(Keys.Right))
			{
				gameObject.transform.Rotation.Y -= movementSpeed * (float)args.Time;

			}

		}
    }
}
