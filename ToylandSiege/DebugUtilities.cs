using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class DebugUtilities
    {
        private static readonly Model PrimitiveBox;
        private static readonly Model PrimitiveSphere;

        static DebugUtilities()
        {
            PrimitiveBox = ToylandSiege.GetToylandSiege().Content.Load<Model>("PrimitiveShapes/Cube");
            PrimitiveSphere = ToylandSiege.GetToylandSiege().Content.Load<Model>("PrimitiveShapes/Sphere");
        }

        public static Model GetSphereModel(float radius, Vector3 position)
        {
            Model model = PrimitiveSphere;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.AmbientLightColor = new Vector3(0, 0.3f, 0.3f);
                    effect.View = Camera.GetCurrentCamera().ViewMatrix;
                    effect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                    effect.World = Matrix.CreateScale(radius) * Matrix.CreateTranslation(position);
                }
            }
            return model;
        }

        public static Model GetBoxModel(Vector3 min, Vector3 max)
        {
            Model model = PrimitiveBox;

            Vector3 center = max - min;
            double diagonal = Vector3.Distance(max, min);
            float edge = (float) (diagonal / Math.Sqrt(3));  // from the Pitagoras theorem, scale should be length of the edge (as cube model is 1x1x1)

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.AmbientLightColor = new Vector3(0, 0.3f, 0.3f);
                    effect.View = Camera.GetCurrentCamera().ViewMatrix;
                    effect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                    effect.World = Matrix.CreateScale(edge) * Matrix.CreateTranslation(center);
                }
            }
            return model;
        }

        public static void ShowAllGameObjects(GameObject rootObject, int tabulation = 0)
        {
            string output = "";
            for (int i = 0; i <= tabulation; i++)
                output += "\t";

            System.Diagnostics.Debug.Print(output + rootObject);
            foreach (var child in rootObject.Childs.Values)
            {
                ShowAllGameObjects(child, tabulation+1);
            }
        }

        public static void DrawColliderWireframes()
        {
            RasterizerState rasterizerStateOriginal = ToylandSiege.GetToylandSiege().GraphicsDevice.RasterizerState;

            RasterizerState rasterizerStateWireframe = new RasterizerState();
            rasterizerStateWireframe.FillMode = FillMode.WireFrame;
            rasterizerStateWireframe.CullMode = CullMode.CullCounterClockwiseFace;

            ToylandSiege.GetToylandSiege().GraphicsDevice.RasterizerState = rasterizerStateWireframe;
  
            ToylandSiege.GetToylandSiege().GraphicsDevice.BlendState = BlendState.Opaque;
            ToylandSiege.GetToylandSiege().GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var child in Level.GetCurrentLevel().RootGameObject.GetAllChilds(Level.GetCurrentLevel().RootGameObject))
                if (child.IsEnabled && child.IsCollidable)
                {
                    switch (child.Collider.BType)
                    {
                        case Collider.BoundingType.Sphere:
                        {
                            Model colliderModel = GetSphereModel(child.Collider.SphereCollider.Radius,
                                child.Collider.SphereCollider.Center);
                            foreach (ModelMesh mesh in colliderModel.Meshes)
                            {
                                mesh.Draw();
                            }
                            break;
                        }

                        // TODO: Add Box debug drawing
                        case Collider.BoundingType.Box:
                        {
                            Model colliderModel = GetBoxModel(child.Collider.BoxCollider.Min,
                                child.Collider.BoxCollider.Max);
                            foreach (ModelMesh mesh in colliderModel.Meshes)
                            {
                                mesh.Draw();
                            }
                            break;
                        }

                        // TODO: Add Complex debug drawing
                        case Collider.BoundingType.Complex:
                        {
                            foreach (BoundingSphere sphere in child.Collider.SphereColliders)
                            {
                                Model colliderModel = GetSphereModel(sphere.Radius, sphere.Center);
                                foreach (ModelMesh mesh in colliderModel.Meshes)
                                {
                                    mesh.Draw();
                                }
                            }
                            break;
                        }

                    }
                }

            ToylandSiege.GetToylandSiege().GraphicsDevice.RasterizerState = rasterizerStateOriginal;
        }
    }
}
