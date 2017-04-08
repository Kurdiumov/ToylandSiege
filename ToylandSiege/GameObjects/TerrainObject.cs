using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege.GameObjects
{
    public class TerrainObject: GameObject
    {

        public TerrainObject(string Name, Model model)
        {
            this.Name = Name;
            Model = model;
            Initialize();
        }

        protected override void Initialize()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.AmbientLightColor = new Vector3(1f, 0, 0);
                    effect.View = Camera.GetCurrentCamera().ViewMatrix;
                    effect.World = Camera.GetCurrentCamera().WorldMatrix;
                    effect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
