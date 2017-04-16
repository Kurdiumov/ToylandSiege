using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class Unit : UnitBase
    {
        public Field Field { get; set; }

        public List<Field> TargetFields = new List<Field>();

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        public override void Update()
        {
            CreateTransformationMatrix();

            // if (!this.IsStatic)
            {
                //TODO: Remove line belowe. Used to check drawing and update methods
                this.Rotation += new Vector3(0.01f, 0, 0);
                if (Field != null && TargetFields.Count > 0)
                {
                    MoveToTarget();
                }

            }
        }

        public bool IsOnField()
        {
            return Field != null;
        }

        public bool PlaceToField(Field field)
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

        public bool PlaceToField(int fieldIndex)
        {
            if (Level.GetCurrentLevel().RootGameObject.Childs["Board"] is Board)
            {
                Board board = Level.GetCurrentLevel().RootGameObject.Childs["Board"] as Board;
                return PlaceToField(board.GetByIndex(fieldIndex));
            }
            Logger.Log.Error("Board is not typeof board!");
            return false;
        }

        public void MoveToTarget()
        {
            float elapsed = 0.01f;
            Vector3 targetPosition = TargetFields.First().Position;

            float distance = Vector3.Distance(Position, targetPosition);
            Vector3 direction = Vector3.Normalize(targetPosition - Position);

            Position += direction * 5 * elapsed;
            if (distance < 0.1f)
                if (TargetFields.Count > 0)
                {
                    if (TargetFields.First().Position == targetPosition)
                        TargetFields.RemoveAt(0);
                }

            //TODO:
            //1. Raycast down for changing current field
            //2. Use pathfinder

        }
    }
}
