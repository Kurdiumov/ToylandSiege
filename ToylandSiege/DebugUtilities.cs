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

        public static Model GetSphereModel(float radius, Matrix transformMatrix)
        {
            Model model = PrimitiveSphere;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.AmbientLightColor = new Vector3(0, 0.3f, 0.3f);
                    effect.View = Camera.GetCurrentCamera().ViewMatrix;
                    effect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                    effect.World = Matrix.CreateScale(radius) * transformMatrix;
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

            foreach (var child in Level.GetCurrentLevel().RootGameObject.Childs.Values)
                if (child.IsEnabled && child.IsCollidable)
                {
                    if (child.BType == GameObject.BoundingType.Sphere)
                    {
                        Model colliderModel = GetSphereModel(child.BSphere.Radius, Matrix.CreateTranslation(child.BSphere.Center));

                        foreach (ModelMesh mesh in colliderModel.Meshes)
                        {
                            mesh.Draw();
                        }

                    } else if (child.BType == GameObject.BoundingType.Box)
                    {
                        
                    }
                }

            ToylandSiege.GetToylandSiege().GraphicsDevice.RasterizerState = rasterizerStateOriginal;
        }
    }
}
