using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege.GameObjects
{
    public class Field : GameObject
    {
        public readonly int Index;


        public Field(string name, int index, Vector3 position, Vector3 scale)
        {
            Name = name;
            Index = index;
            Position = position;
            Scale = scale;
            Initialize();
        }

        protected override void Initialize()
        {
            IsStatic = true;
            CreateTransformationMatrix();
        }

        public override void Update()
        {


        }
    }
}
