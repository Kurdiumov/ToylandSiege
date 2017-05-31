using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ToylandSiege.GameObjects
{
    public class Board : GameObject
    {
        /// <summary>
        /// THIS CLASS SHOULD BE REWRITEN IN REE TIME!!!!
        /// WAS TOO DRUNK WHILE WRITTING IT
        /// DONT TRY TO UNDERSTAND IT
        /// </summary>
        public string FieldName;

        public Vector3 StartingPosition;
        public string FieldModel;
        public float Step;

        private readonly int _numberOfRows;
        private readonly int _numberOfColumns;

        public int FieldCount = 0;
        public List<Spawner> Spawners;
        public Board(string name, int rows, int colums)
        {
            Name = name;
            _numberOfRows = rows;
            _numberOfColumns = colums;
            Spawners = new List<Spawner>();
            Initialize();
        }

        protected override void Initialize()
        {
            //Initializing board
            CreateTransformationMatrix();
            for (int i = 0; i < _numberOfColumns; i++)
            {
                AddChild(new Group("Row" + i));
            }
        }

        public void CreateFields()
        {
            int index = 0;
            for (int column = 0; column < _numberOfColumns; column++)
            {
                int rowsInCurrentColumn = _numberOfRows;
                if (column % 2 != 0)
                    rowsInCurrentColumn = _numberOfRows - 1;

                for (int row = 0; row < rowsInCurrentColumn; row++)
                {
                    var field = new Field(FieldName + index, index, CalculatePosition(column, row, index), Scale)
                    {
                        Model = ToylandSiege.GetInstance().Content.Load<Model>(FieldModel),
                    };
                    
                    /*
                    //Setting starting and finishing fields
                    if (column == 0)
                        field.StartingTile = true;
                    else if (column == _numberOfColumns - 1)
                        field.FinishingTile = true;*/


                    Childs["Row" + column].AddChild(field);
                    index++;
                    ++FieldCount;
                }
            }
        }

        public void InitializeLevel1()
        {
            new Spawner(172, this, "Defender");
            new Spawner(176, this, "Sniper");

            //Create river
            for (int i = 144; i < 164; i++)
            {
                GetByIndex(i).FieldSpeed = 1;
                GetByIndex(i).ColorVector3 = new Vector3(0, 0.7f, 0.9f);
            }
            for(int i = 0; i < 20; i++)
            {
                GetByIndex(i).StartingTile = true;
            }
            for ( int i = 267; i < 287; i++)
            {
                GetByIndex(i).FinishingTile = true;
            }
        }

        public void EnableFields(int[] l)
        {
            foreach(int i in l)
            {
                if (GetByIndex(i) != null)
                {
                    GetByIndex(i).IsEnabled = true;
                }
            }
        }

        public void InitializeTutorialLevel()
        {
            for (int i = 0; i < 133; i++)
            {
                GetByIndex(i).IsEnabled = false;
            }
            int[] enabled = new int[] { 5, 6, 12, 13, 14, 15, 16, 20, 21, 22, 23, 24, 25, 26, 27, 29, 30, 36, 37, 45, 46, 53, 54, 55, 62, 63, 64, 71, 72, 80, 81 };
            EnableFields(enabled);
            new Spawner(71, this, "Standart");
            GetByIndex(5).StartingTile = true;
            GetByIndex(6).StartingTile = true;
            
            for(int i = 89; i < 92; i++)
            {
                GetByIndex(i).FinishingTile = true;
                GetByIndex(i).IsEnabled = true;
            }
            /*
            for (int i = 98; i < 102; i++)
            {
                GetByIndex(i).FinishingTile = true;
                GetByIndex(i).IsEnabled = true;
            }*/
                /*for (int i = 105; i < 133; i++)
                    GetByIndex(i).IsEnabled = false;
                for(int i = 0; i <= 78; )
                {
                    GetByIndex(i).IsEnabled = false;
                    i += 10;
                    GetByIndex(i).IsEnabled = false;
                    i += 9;
                }*/
                //List<int> enabled = new List<int>( new int[] { 5, 6 });

            }


        private Vector3 CalculatePosition(int column, int row, int index)
        {
            float x = StartingPosition.X;
            float y = StartingPosition.Y;
            float z = StartingPosition.Z - (column) * (Step - 0.5f);

            if (column % 2 == 0)
                x = x + (row * Step);
            else
                x = (x + Step / 2) + (row * Step);

            return new Vector3(x, y, z);
        }

        public override void Update(GameTime gameTime)
        {
            //Should only do it in Strategic or FirstPersonModes
            foreach (var child in Childs.Values)
            {
                child.Update(gameTime);
            }
        }

        public Field GetByIndex(int index)
        {
            if (!IsInRange(index))
            {
                Logger.Log.Error("Index out of Range");
                return null;
            }

            int field = index;
            int column = 0;
            for (; index >= Childs["Row" + column].Childs.Count; index -= Childs["Row" + column].Childs.Count, column++)
                ;

            return Childs["Row" + column].Childs["Field" + field] as Field;
        }

        public bool IsInRange(int index)
        {
            return (index >= 0 && index < FieldCount);
        }

        public List<Field> GetNearestFields(Field field)
        {
            int index = field.Index;
            int CurrentRow = Childs.Values.ToList().IndexOf(field.Parent);
            List<Field> nearestFields = new List<Field>();

            void AddToNearestField(int ind, int row)
            {
                if (IsInRange(ind))
                {
                    if (row == -1 && CurrentRow - 1 == Childs.Values.ToList().IndexOf(GetByIndex(ind).Parent))
                    {
                        nearestFields.Add(GetByIndex(ind));
                    }
                    else if (row == 0 && field.Parent == GetByIndex(ind).Parent)
                    {
                        nearestFields.Add(GetByIndex(ind));
                    }
                    else if (row == 1 && CurrentRow + 1 == Childs.Values.ToList().IndexOf(GetByIndex(ind).Parent))
                    {
                        nearestFields.Add(GetByIndex(ind));
                    }
                }
            }

            //Left and right nearest values should be at the same row
            AddToNearestField(index - 1, 0);
            AddToNearestField(index + 1, 0);

            AddToNearestField(index + _numberOfRows, 1);
            AddToNearestField(index + _numberOfRows - 1, 1);
            AddToNearestField(index - _numberOfRows, -1);
            AddToNearestField(index - _numberOfRows + 1, -1);

            return nearestFields;
        }

        public List<Field> GetNearestFields(int index)
        {
            return GetNearestFields(GetByIndex(index));
        }

        public int GetEnabledEnemiesSpawnersCount()
        {
            var count = 0;
            foreach (var spawner in Spawners)
            {
                if (spawner.IsEnabled)
                    count++;
            }
            return count;
        }

        public int GetAllEnemiesSpawnersCount()
        {
            return Spawners.Count;
        }
    }
}