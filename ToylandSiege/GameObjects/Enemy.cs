using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class Enemy:UnitBase
    {
        private Field _field;
        public Field Field
        {
            get
            {
                return _field;
            }
            set
            {
                _field = value;
                if (value == null)
                    Logger.Log.Debug("Field set to NULL for enemy " + this);
                else
                    Logger.Log.Debug("New field (" + _field + ") set for enemy " + this);
            }
        }
        protected override void Initialize()
        {
            //throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            CreateTransformationMatrix();
            AnimationPlayer.Update(gameTime.ElapsedGameTime, true, TransformationMatrix);

            if (!WaveController.RoundRunning)
                return;
        }

        public bool PlaceToField(Field field)
        {
            if (!field.HasUnit())
            {
                field.SetEnemy(this);
                Field = field;
                Position = Field.Position;
                return true;
            }
            return false;
        }
    }
}
