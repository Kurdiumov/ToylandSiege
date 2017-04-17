using System.Collections.Generic;
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

        public List<Field> GetNearestFields()
        {
            return ((Board) Parent.Parent).GetNearestFields(this);
        }

        public override void Draw()
        {
            if (!IsEnabled)
                return;

            if (Model != null)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        if (ToylandSiege.GetToylandSiege().configurationManager.LigthningEnabled)
                            effect.EnableDefaultLighting();
                        if (StartingTile && !HasUnit())
                            effect.AmbientLightColor = new Vector3(0, 0.9f, 0.1f);
                        else if (HasUnit() && unit.TargetFields.Contains(this))
                            effect.AmbientLightColor = new Vector3(0.9f, 0.3f, 0.3f);
                        else
                            effect.AmbientLightColor = new Vector3(0, 0.3f, 0.3f);
                        effect.View = Camera.GetCurrentCamera().ViewMatrix;

                        effect.World = TransformationMatrix;
                        effect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
