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
        private Field _field;

        public Field Field {
            get
            {
                return _field;
            }
            set
            {
                _field = value;
                if(value == null)
                    Logger.Log.Debug("Field set to NULL for unit " + this);
                else
                    Logger.Log.Debug("New field ("+ _field +") set for unit " + this);
            }
        }

        public List<Field> TargetFields { get; set; }

        protected override void Initialize()
        {
            TargetFields = new List<Field>();
        }

        public override void Update()
        {
            //TODO: Remove line belowe. Used to check drawing and update methods
            Rotation += new Vector3(0.01f, 0, 0);
            if (Field != null && TargetFields.Count > 0)
                MoveToTarget();

            CreateTransformationMatrix();
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

        public void AddField(Field field)
        {
            if (field.StartingTile && Field == null)
            {
                PlaceToField(field); // Set starting field
            }
            else if (Field != null)
            {
                if (TargetFields.Count > 0)
                {
                    if (TargetFields.Last().GetNearestFields().Contains(field))
                        addTargetField(field);
                }
                else if (Field.GetNearestFields().Contains(field))
                    addTargetField(field);
            }
        }

        private void addTargetField(Field field)
        {
            TargetFields.Add(field);
            Logger.Log.Debug("New target field (" + field + ") added to unit " + this);
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
                    Field.unit = null;
                    Field = null;
                    if (TargetFields.First().Position == targetPosition)
                    {
                        Field = TargetFields.First();
                        TargetFields.RemoveAt(0);
                    }
                }

            //TODO:
            //1. Raycast down for changing current field
            //2. Use pathfinder

        }
    }
}
