using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege.GameObjects
{
    public class Field : GameObject
    {
        public readonly int Index;
        public bool StartingTile = false;
        public bool FinishingTile = false;
        public Unit unit;

        public Field(string name, int index, Vector3 position, Vector3 scale)
        {
            Name = name;
            Index = index;
            Position = position;
            Scale = scale;
            Type = "Field";
            Initialize();
        }

        protected override void Initialize()
        {
            IsStatic = false;
            IsCollidable = true;
            CreateTransformationMatrix();
        }

        public override void Update()
        {
            //TODO: Remove line below
            CreateTransformationMatrix();
        }

        public bool HasUnit()
        {
            return unit != null;
        }

        public bool SetUnit(Unit unit)
        {
            if (!HasUnit())
            {
                this.unit = unit;
                Logger.Log.Debug("Setting unit (" + unit + ") to field  + " + Name);
                return true;
            }

            Logger.Log.Debug("Can't place unit (" + unit + ") to field  + " + Name + " beacause field already contains unit (" + this.unit + ")");
            return false;
        }
    }
}
