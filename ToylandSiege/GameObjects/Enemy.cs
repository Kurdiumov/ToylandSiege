using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Framework.Content.Pipeline.Builder;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege.GameObjects
{
    public class Enemy:UnitBase
    {
        Effect effect;
        protected readonly HashSet<Unit> UnitsInRange = new HashSet<Unit>();

        public Enemy()
        {
            effect = ToylandSiege.GetInstance().Content.Load<Effect>("Shaders/Plastic");
        }

        protected override void Initialize()
        {

        }

        public override void Update(GameTime gameTime)
        {
            CreateTransformationMatrix();
            AnimationPlayer.Update(gameTime.ElapsedGameTime, true, TransformationMatrix);

            if (!WaveController.RoundRunning)
                return;

            UpdateUnitsInRange();
            
            if (CanShoot(gameTime) && UnitsInRange.Count > 0)
            {
                Shoot(gameTime, UnitsInRange.First());
            }
        }

        public bool PlaceToField(Field field)
        {
            if (!field.HasUnit())
            {
                field.SetEnemy(this);
                Field = field;
                Position = Field.Position;

                UpdateFieldsInRange();
                UpdateUnitsInRange();

                return true;
            }
            return false;
        }

        public void UpdateUnitsInRange()
        {
            UnitsInRange.Clear();

            foreach (Field field in FieldsInRange)
            {
                if (field.HasUnit())
                {
                    UnitsInRange.Add(field.unit);
                }
            }
        }
        /*
        public override void Draw()
        {
            DrawHealthBar();
            ToylandSiege.GetInstance().GraphicsDevice.BlendState = BlendState.Opaque;
            ToylandSiege.GetInstance().GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ToylandSiege.GetInstance().GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(TransformationMatrix * mesh.ParentBone.Transform);
                    effect.Parameters["View"].SetValue(Camera.GetCurrentCamera().ViewMatrix);
                    effect.Parameters["Projection"].SetValue(Camera.GetCurrentCamera().ProjectionMatrix);
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * TransformationMatrix * mesh.ParentBone.Transform));
                    effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
            }
        }*/
    }
}
