using System;
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class Spawner : GameObject
    {
        public readonly Field Field;
        public bool IsEnabled = true;
        public readonly string EnemyType;
        public static int EnemyCount = 0;

        public double TimeBetweenSpawn = 4;
        public TimeSpan CountDownTimeStarteDateTime;
        public bool TimerStarted;


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

            //Spawns all enemies at start
            SpawnEnemy();
        }

        private void _addToGameObjects()
        {
            if (!Level.GetCurrentLevel().RootGameObject.Childs.ContainsKey("Spawners"))
            {
                Level.GetCurrentLevel().RootGameObject.Childs.Add("Spawners", new Group("Spawners"));
            }
            var Spawners = Level.GetCurrentLevel().RootGameObject.Childs["Spawners"];
            Spawners.AddChild(this);

            if (!Level.GetCurrentLevel().RootGameObject.Childs.ContainsKey("Enemies"))
            {
                Level.GetCurrentLevel().RootGameObject.Childs.Add("Enemies", new Group("Enemies"));
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsEnabled)
                return;
            if (ShouldSpawnEnemy(gameTime))
            {
                SpawnEnemy();
            }
            /*
            //To remove Enemy from field!
            var enemy = Field.enemy;
            Field.enemy.Field = null;
            Field.enemy = null;
            Level.GetCurrentLevel().RootGameObject.Childs["Enemies"].RemoveChild(enemy);
            */
        }

        public void SpawnEnemy()
        {
            Enemy enemy = EnemyBuilder.BuildEnemy("Enemy" + EnemyCount, EnemyType);
            Field.SetEnemy(enemy);
            enemy.PlaceToField(Field);
            enemy.IsEnabled = true;

            Level.GetCurrentLevel().RootGameObject.Childs["Enemies"].AddChild(enemy);
            TimerStarted = false;
        }

        public bool ShouldSpawnEnemy(GameTime gameTime)
        {
            if (Field.HasEnemy())
                return false;

            if (TimerStarted == false)
            {
                TimerStarted = true;
                CountDownTimeStarteDateTime = gameTime.TotalGameTime;
            }
            else
            {
                if (TimeBetweenSpawn - (gameTime.TotalGameTime - CountDownTimeStarteDateTime).TotalSeconds <= 0)
                    return true;
            }
            return false;
        }

        public bool HasUnit()
        {
            return this.Field.HasUnit();
        }
    }
}
