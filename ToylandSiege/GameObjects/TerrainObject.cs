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

        public override void Update(GameTime gameTime)
        {
            if (!IsStatic)
                CreateTransformationMatrix();
        }
    }
}
