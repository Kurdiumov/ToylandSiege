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
        public List<Field> FieldsInWay { get; set; }
        protected readonly HashSet<Enemy> EnemiesInRange = new HashSet<Enemy>();
        bool Walking = false;

        protected override void Initialize()
        {
            TargetFields = new List<Field>();
            FieldsInWay = new List<Field>();
        }

        public override void Update(GameTime gameTime)
        {
            CreateTransformationMatrix();
            AnimationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            UpdateFieldsInRange();
            UpdatEnemiesInRange();
            if (!WaveController.RoundRunning)
                return;

            if (Field != null && TargetFields.Count > 0)
                MoveToTarget();

            if (CanShoot(gameTime) && EnemiesInRange.Count > 0)
            {
                Shoot(gameTime, EnemiesInRange.First());
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
                UpdateFieldsInRange();
                UpdatEnemiesInRange();
                return true;
            }

            return false;
        }

        public bool PlaceToField(int fieldIndex)
        {
            if (Level.GetCurrentLevel().RootGameObject.Childs["Board"] is Board)
            {
                Board board = Level.GetCurrentLevel().RootGameObject.Childs["Board"] as Board;
                var field = board.GetByIndex(fieldIndex);
                field.IsPartOfWay = true;
                return PlaceToField(field);
            }
            Logger.Log.Error("Board is not typeof board!");
            return false;
        }

        public void AddField(Field field)
        {
            FieldsInWay.Add(field);
            if (field.StartingTile && Field == null && !field.HasUnit())
            {
                PlaceToField(field); // Set starting field            
            }
            else if (Field != null)
            {
                if (!field.StartingTile)
                {
                    addTargetField(field);
                }
            }
        }

        private void addTargetField(Field field)
        {
            TargetFields.Add(field);
            field.IsPartOfWay = true;
            Logger.Log.Debug("New target field (" + field + ") added to unit " + this);
        }

        public void MoveToTarget()
        {
            if (TargetFields.FirstOrDefault() != null && TargetFields.FirstOrDefault().HasUnit())
               return;

            if(!Walking)
            {
                this.AnimationPlayer.StartClip(this.Clips["walking"]);
                Walking = true;
            }

            float elapsed = 0.01f;
            Vector3 targetPosition = TargetFields.First().Position;

            float distance = Vector3.Distance(Position, targetPosition);
            Vector3 direction = Vector3.Normalize(targetPosition - Position);

            RotateToTarget(direction);
            Position += direction * (Speed * Field.FieldSpeed / 2) * elapsed;
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
                        if (TargetFields.First().FinishingTile)
                        {
                            Walking = false;
                            this.AnimationPlayer.StartClip(this.Clips["standing"]);
                            if (TargetFields.First().isNeutralObject)
                            {
                                Level.GetCurrentLevel().RootGameObject.Childs["TerrainObjectsGroup"].RemoveChild(Level.GetCurrentLevel().RootGameObject.Childs["TerrainObjectsGroup"].Childs["FallenTree"]);
                                ((Board)Field.Parent.Parent).GetByIndex(55).IsEnabled = true;
                                ((Board)Field.Parent.Parent).GetByIndex(56).IsEnabled = true;
                            }
                        }
                        
                    }
                }
        }

        public void RotateToTarget(Vector3 direction)
        {
            //TODO: Implement this Method
        }

        public void UpdatEnemiesInRange()
        {
            EnemiesInRange.Clear();

            foreach (Field field in FieldsInRange)
            {
                if (field.HasEnemy())
                {
                    EnemiesInRange.Add(field.enemy);
                }
            }
        }
    }
}
