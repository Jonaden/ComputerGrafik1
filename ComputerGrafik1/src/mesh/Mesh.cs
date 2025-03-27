using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ComputerGrafik1
{
    public class Mesh
    {
        protected virtual float[] Vertices { get; set; }
        public virtual uint[] Indices { get; protected set; }

        public int vertexArrayObject;
        int elementBufferObject;
        int vertexBufferObject;

        float[] vertices =
        {
            //Position          Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };
        uint[] indices =
        {
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };
        public Mesh()
        {
            //GenerateBuffers();
        }

        public Mesh(float[] vertices, uint[] indices)
        {
            this.Vertices = vertices;
            this.Indices = indices;
            GenerateBuffers();
        }
        protected virtual void GenerateBuffers()
        {
            vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, (int)vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float),
                Vertices, BufferUsageHint.StaticDraw);
            vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, (int)elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
        }

        public virtual void Draw()
        {
            GL.PointSize(10f);
            GL.DrawArrays(PrimitiveType.Points, 0, Vertices.Length / 5);
            Console.WriteLine(Vertices[0]);
            Console.WriteLine(Vertices[1]);
            Console.WriteLine(Vertices[2]);
            Console.WriteLine(Vertices[3]);
            Console.WriteLine(Vertices[4]);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}
