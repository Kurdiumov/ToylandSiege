using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ToylandSiege.GameState;

namespace ToylandSiege.GameObjects
{
    public class Field : GameObject
    {
        public readonly int Index;
        public bool StartingTile = false;
        public bool FinishingTile = false;
        public bool IsSpawner = false;
        public bool IsWater = false;
        public int FieldSpeed = 3;

        public Spawner Spawner { get; set; }
        public Vector3 ColorVector3 = new Vector3(0.5f, 0.8f, 0.3f);
        public Vector3 AmbientVector3 = new Vector3(0.3f, 0.3f, 0.3f);
        public float Alpha = 0.5f;

        public Unit unit;
        public Enemy enemy;
        public bool IsPartOfWay = false;
        private Effect effect;


        public Field(string name, int index, Vector3 position, Vector3 scale)
        {
            Name = name;
            Index = index;
            Position = position;
            Scale = scale;
            Type = "Field";
            effect = new BasicEffect(ToylandSiege.GetInstance().GraphicsDevice);
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
                IsPartOfWay = true;
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
            return ((Board)Parent.Parent).GetNearestFields(this);
        }

        public override void Draw()
        {
            if (!IsEnabled)
                return;

            UpdateColors();
            if (Model != null)
            {
                foreach (ModelMesh mesh in Model.Meshes)
                {
                    if (IsWater)
                    {
                        _DrawWater(mesh);
                    }
                    else
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                        {
                            part.Effect = effect;
                           
                            if (ToylandSiege.GetInstance().configurationManager.LigthningEnabled)
                                (effect as BasicEffect).EnableDefaultLighting();
                            (effect as BasicEffect).AmbientLightColor = AmbientVector3;
                            (effect as BasicEffect).DiffuseColor = ColorVector3;

                            (effect as BasicEffect).View = Camera.GetCurrentCamera().ViewMatrix;

                            (effect as BasicEffect).World = TransformationMatrix;
                            (effect as BasicEffect).Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                            (effect as BasicEffect).Alpha = Alpha;
                            GlobalLightning.DrawGlobalLightning(effect);
                        }
                    }
                    mesh.Draw();
                }
            }
        }

        private void _DrawWater(ModelMesh mesh)
        {
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                part.Effect = effect;
                effect.Parameters["World"].SetValue( TransformationMatrix);
                effect.Parameters["View"].SetValue(Camera.GetCurrentCamera().ViewMatrix);
                effect.Parameters["Projection"].SetValue(Camera.GetCurrentCamera().ProjectionMatrix);
                effect.Parameters["SkyboxTexture"].SetValue(ToylandSiege.GetInstance().Content.Load<Texture2D>("SkyTexture"));
                effect.Parameters["CameraPosition"].SetValue(Camera.GetCurrentCamera().Position);
                effect.Parameters["WorldInverseTranspose"].SetValue(
                                        Matrix.Transpose(TransformationMatrix  * mesh.ParentBone.Transform));
            }
        }


        public bool CanPlaceUnit()
        {
            if (!IsEnabled)
                return false;
            if (IsSpawner)
                return false;
            if (HasUnit() || HasEnemy())
                return false;
            //if (IsPartOfWay)
            //return false;
            return true;
        }

        public void UpdateColors()
        {
            SetDefaultColor();
            if (IsPartOfWay)
            {
                AmbientVector3 = new Vector3(0.0f, 0.1f, 0.0f);
            }
            else if (Strategic.FirstUnitPlacing)
            {
                if (StartingTile)
                {
                    if (!HasUnit())
                    {
                        ColorVector3 = new Vector3(0.4f, 1f, 0.1f);
                        AmbientVector3 = new Vector3(1, 1, 1);
                    }
                }
            }
            else if (Strategic.UnitSelected && FinishingTile)
            {
                ColorVector3 = new Vector3(0.1f, 0.4f, 0.2f);
                AmbientVector3 = new Vector3(1, 1, 1);
            }
        }

        public void SetDefaultColor()
        {
            if (IsSpawner)
            {
                ColorVector3 = new Vector3(0.9f, 0.1f, 0.1f);
                AmbientVector3 = new Vector3(1, 1, 1);
            }
            else if (FieldSpeed == 3)
            {
                ColorVector3 = new Vector3(0.5f, 0.8f, 0.3f);
                AmbientVector3 = new Vector3(0.3f, 0.3f, 0.3f);
            }
            else
            {
                AmbientVector3 = new Vector3(0.3f, 0.3f, 0.3f);
            }
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }

        public void SetWater()
        {
            this.IsWater = true;
            this.effect = ShaderManager.Get("ReflectionShader");
        }
    }
}
