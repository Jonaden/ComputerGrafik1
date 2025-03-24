using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;


namespace ComputerGrafik1
{
    public class GameObject
    {
        public Transform transform;
        public Renderer renderer;

        List<Behaviour> behaviours = new List<Behaviour>();

        protected GameWindow gameWindow;
        public T GetComponent<T>() where T : Behaviour
        {
            foreach (Behaviour component in behaviours)
            {
                T componentAsT = component as T;
                if (componentAsT != null) return componentAsT;
            }
            return null;
        }
        public void AddComponent<T>() where T : Behaviour
        {
            behaviours.Add(Activator.CreateInstance(typeof(T), this, gameWindow) as T);
        }

        public GameObject(Renderer renderer, GameWindow gameWindow)
        {
            this.renderer = renderer;
            this.gameWindow = gameWindow;
            transform = new Transform();
        }

        public void Update(FrameEventArgs args)
        {
            foreach (Behaviour behaviour in behaviours)
            {
                behaviour.Update(args);
            }
        }

        public void Draw(Matrix4 vp)
        {
            renderer.Draw(transform.CalculateModel() * vp);
        }
    }
}
