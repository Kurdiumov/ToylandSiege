using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class Unit:UnitBase
    {
        public Field Field { get; set; }

        protected override void Initialize()
        {
           // throw new NotImplementedException();
        }

        public override void Update()
        {
            CreateTransformationMatrix();

            //TODO: Remove line belowe. Used to check drawing and update methods
            if (!this.IsStatic)
            {
                this.Rotation += new Vector3(0.01f, 0, 0);
               // MoveToField(5);
            }
        }

        public bool IsOnField()
        {
            return Field != null;
        }

        public bool MoveToField(Field field)
        {
            if (!field.HasUnit())
            {
                field.SetUnit(this);
                Field = field;
                Position = Field.Position;
                return true;
            }
            return false;
        }

        public bool MoveToField(int fieldIndex)
        {
            if (Level.GetCurrentLevel().RootGameObject.Childs["Board"] is Board)
            {
                Board board = Level.GetCurrentLevel().RootGameObject.Childs["Board"] as Board;
                return MoveToField(board.GetByIndex(fieldIndex));
            }
            Logger.Log.Error("Board is not typeof board!");
            return false;
        }
    }
}
