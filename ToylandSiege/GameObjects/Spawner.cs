using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class Spawner: GameObject
    {
        public readonly Field Field;
        public bool IsEnabled = true;
        public readonly string EnemyType;
        public static int EnemyCount = 0;

        public Spawner(Field field, string EnemyType)
        {
            Field = field;
            this.EnemyType = EnemyType;
            Initialize();
        }

        public Spawner(int fieldIndex, Board board, string EnemyType)
        {
            Field = board.GetByIndex(fieldIndex);
            this.EnemyType = EnemyType;
            Initialize();
        }

        protected override void Initialize()
        {
            this.Name = "SpawnerOn" + Field.Index + "Field";
            Field.Spawner = this;
            Field.IsSpawner = true;
            _addToGameObjects();
        }

        private void _addToGameObjects()
        {
            if (!Level.GetCurrentLevel().RootGameObject.Childs.ContainsKey("Spawners"))
            {
                Level.GetCurrentLevel().RootGameObject.Childs.Add("Spawners", new Group("Spawners"));
            }
            var Spawners = Level.GetCurrentLevel().RootGameObject.Childs["Spawners"];
            Spawners.AddChild(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsEnabled)
                return;
            if (!Field.HasEnemy())
            {
                Enemy enemy = EnemyBuilder.BuildEnemy("Enemy" + EnemyCount, EnemyType);
                Field.SetEnemy(enemy);
                enemy.PlaceToField(Field);
                enemy.IsEnabled = true;

                Level.GetCurrentLevel().RootGameObject.Childs["Enemies"].AddChild(enemy);
            }
        }

        public bool HasUnit()
        {
            return this.Field.HasUnit();
        }
    }
}
