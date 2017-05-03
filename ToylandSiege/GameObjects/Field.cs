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
        public bool IsSpawner = false;

        public Spawner Spawner { get; set; }

        public Unit unit;
        public Enemy enemy;

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

        public override void Update(GameTime gameTime)
        {
            //TODO: Remove line below
            CreateTransformationMatrix();
            foreach (var child in Childs.Values)
            {
                child.Update(gameTime);
            }
        }

        public bool HasUnit()
        {
            return unit != null;
        }
        public bool HasEnemy()
        {
            return enemy != null;
        }

        public bool SetUnit(Unit unit)
        {
            if (!HasUnit() && !HasEnemy())
            {
                this.unit = unit;
                Logger.Log.Debug("Setting unit (" + unit + ") to field  + " + Name);
                return true;
            }

            Logger.Log.Debug("Can't place unit (" + unit + ") to field  + " + Name + " beacause field already contains  unit or enemy");
            return false;
        }

        public bool SetEnemy(Enemy enemy)
        {
            if (!HasEnemy() && !HasUnit())
            {
                this.enemy = enemy;
                Logger.Log.Debug("Setting enemy (" + enemy + ") to field  + " + Name);
                return true;
            }

            Logger.Log.Debug("Can't place enemy (" + enemy + ") to field  + " + Name + " beacause field already contains unit or enemy");
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
                        if (ToylandSiege.GetInstance().configurationManager.LigthningEnabled)
                            effect.EnableDefaultLighting();
                        if (StartingTile && !HasUnit())
                            effect.AmbientLightColor = new Vector3(0, 0.9f, 0.1f);
                        else if (HasUnit() && unit.TargetFields.Contains(this))
                            effect.AmbientLightColor = new Vector3(0.9f, 0.3f, 0.3f);
                        else if (IsSpawner)
                            effect.AmbientLightColor = new Vector3(1f, 0.0f, 0.0f);
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
