using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace ToylandSiege.GameObjects
{
    public class Enemy:UnitBase
    {
        protected readonly HashSet<Unit> UnitsInRange = new HashSet<Unit>();

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
    }
}
