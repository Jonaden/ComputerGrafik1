using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGrafik1
{
    public class ModelLoader: Mesh
    {
        Vector3[] vertices;
        Vector3[] colors;
        Vector2[] textureCoords;


        List<Tuple<float, float, float>> faces = new List<Tuple<float, float, float>>();

        public override float[] Vertices { get { return (int)vertices.Length; } }
        public override uint[] Indices { get { return faces.Count * 3; } }

        public int ColorDataCount { get { return (int)colors.Length; } }

        public Vector3[] GetVerts()
        {
            return vertices;
        }

        public int[] GetIndices (int offset = 0)
        {
            List<float> temp = new List<float>();

            foreach(var face in faces)
            {
                temp.Add(face.Item1 + offset);

            }
            return null;
        }
            
           
    }
}
