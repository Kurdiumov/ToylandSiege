using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege
{
    public class TerrainObject: GameObject
    {
        private readonly Model _model;

        public TerrainObject(string Name, Model model)
        {
            this.Name = Name;
            _model = model;
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
            foreach (ModelMesh mesh in _model.Meshes)
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
