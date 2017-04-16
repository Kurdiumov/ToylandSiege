using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public Board(string name, int rows, int colums)
        {
            Name = name;
            _numberOfRows = rows;
            _numberOfColumns = colums;
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
                        Model = ToylandSiege.GetToylandSiege().Content.Load<Model>(FieldModel),
                    }; ;
                    Childs["Row" + column].AddChild(field);
                    index++;
                    ++FieldCount;
                }
            }
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

        public override void Update()
        {
            //Should only do it in Strategic or FirstPersonModes
            foreach (var child in Childs.Values)
            {
                child.Update();
            }
        }

        public Field GetByIndex(int index)
        {
            if (IsInRange(index))
            {
                Logger.Log.Error("Index out of Range");
                return null;
            }

            int field = index;
            int column = 0;
            for (; index >= Childs["Row" + column].Childs.Count; index -= Childs["Row" + column].Childs.Count, column++);
            return Childs["Row" + column].Childs["Field" + field] as Field;
        }

        public bool IsInRange(int index)
        {
            return (index < 0 || index >= FieldCount);
        }
    }
}
