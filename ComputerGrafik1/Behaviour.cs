using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Threading;

namespace ComputerGrafik1
{
    public abstract class Behaviour
    {
        protected GameObject gameObject;
        protected Game window;
        public Behaviour(GameObject gameObject, Game window)
        {
            this.gameObject = gameObject;
            this.window = window;
        }

        public abstract void Update(FrameEventArgs args);
    }
}