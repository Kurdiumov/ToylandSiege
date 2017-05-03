using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace ToylandSiege.GameObjects
{
    public class Unit : UnitBase
    {
        public float Speed { get; set; }

        public List<Field> TargetFields { get; set; }

        protected override void Initialize()
        {
                TargetFields = new List<Field>();
        }

        public override void Update(GameTime gameTime)
        {
            CreateTransformationMatrix();
            AnimationPlayer.Update(gameTime.ElapsedGameTime, true, TransformationMatrix);

            if (!WaveController.RoundRunning)
                return;

            if (Field != null && TargetFields.Count > 0)
                MoveToTarget();
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

            RotateToTarget(direction);
            Position += direction * Speed * elapsed;
            if (distance < 0.1f)

                if (TargetFields.Count > 0)
                {
                    Field.unit = null;
                    Field = null;
                    if (TargetFields.First().Position == targetPosition)
                    {
                        Field = TargetFields.First();
                        Field.unit = this;
                        TargetFields.RemoveAt(0);
                    }
                }

            //TODO: Use pathfinder
        }

        public void RotateToTarget(Vector3 direction)
        {
            //TODO: Implement this Method
        }
    }
}
