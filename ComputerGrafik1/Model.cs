using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using Assimp;
using AssimpMesh = Assimp.Mesh;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;

namespace ComputerGrafik1
{


    public static class Extensions
    {
        public static Vector3 ConvertAssimpVector3(this Vector3D AssimpVector)
        {
            // Reinterpret the assimp vector into an OpenTK vector.
            return Unsafe.As<Vector3D, Vector3>(ref AssimpVector);
        }

        public static Matrix4 ConvertAssimpMatrix4(this Matrix4x4 AssimpMatrix)
        {
            // Take the column-major assimp matrix and convert it to a row-major OpenTK matrix.
            return Matrix4.Transpose(Unsafe.As<Matrix4x4, Matrix4>(ref AssimpMatrix));
        }
    }

    public class Model
    {

        public List<ModelMesh> meshes;

        public Model(string path)
        {
            AssimpContext importer = new AssimpContext();

            LogStream logstream = new LogStream((String msg, String userData) =>
            {
                Debug.WriteLine(msg);
            });
            logstream.Attach();


            Scene scene = importer.ImportFile(path, PostProcessSteps.Triangulate);




            if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
            {
                Console.WriteLine("Unable to load model from: "+ path);
                return;
            }

            meshes = new List<ModelMesh>();

            float scale = 1/2000f;
            Matrix4 scalingMatrix = Matrix4.CreateScale(scale);


            ProcessNode(scene.RootNode, scene, scalingMatrix);

            importer.Dispose();
        }


        public virtual void Draw()
        {
            foreach (ModelMesh mesh in meshes)
            {
                mesh.Draw();
            }
        }

        private void ProcessNode(Node node, Scene scene, Matrix4 parentTransform)
        {
            Matrix4 transform = node.Transform.ConvertAssimpMatrix4() * parentTransform;

            for (int i = 0; i < node.MeshCount; i++)
            {
                AssimpMesh mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, transform));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene, transform);
            }
        }

        private ModelMesh ProcessMesh(AssimpMesh mesh, Matrix4 transform)
        {
            List<float> vertices = new();
            List<int> indices = new();

            Matrix4 inverseTransform = Matrix4.Invert(transform);

            // Walk through each of the mesh's vertices
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                // Positions
                Vector3 position = mesh.Vertices[i].ConvertAssimpVector3();
                Vector3 transformedPosition = Vector3.TransformPosition(position, transform);
                vertices.Add(transformedPosition.X);
                vertices.Add(transformedPosition.Y);
                vertices.Add(transformedPosition.Z);

                Vector3 normal = mesh.Normals[i].ConvertAssimpVector3();
                Vector3 transformedNormal = Vector3.TransformNormalInverse(normal, inverseTransform);
                vertices.Add(transformedNormal.X);
                vertices.Add(transformedNormal.Y);
                vertices.Add(transformedNormal.Z);
                
                Vector2 vec;
                vec.X = mesh.TextureCoordinateChannels[0][i].X;
                vec.Y = mesh.TextureCoordinateChannels[0][i].Y;
                vertices.Add(vec.X);
                vertices.Add(vec.Y);
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add(face.Indices[j]);
                }
            }

            List<uint> uints = indices.Select(i => (uint)i).ToList();

            return new ModelMesh(vertices.ToArray(), uints.ToArray());
        }

    }

}
