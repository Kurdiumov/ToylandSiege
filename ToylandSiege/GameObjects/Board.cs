
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege.GameObjects
{
    public class Board : GameObject
    {
        public List<List<Field>> board;
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
            board = new List<List<Field>>();
            for (int i = 0; i < _numberOfColumns; i++)
            {
                if (i % 2 == 0)
                    board.Add(new List<Field>(_numberOfRows));
                else
                    board.Add(new List<Field>(_numberOfRows - 1));
                
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
                    };
                    board[column].Add(field);
                    index++;
                    ++FieldCount;
                }
            }
        }

        private Vector3 CalculatePosition(int column, int row, int index)
        {
            float x = StartingPosition.X;
            float y = StartingPosition.Y;
            float z = StartingPosition.Z - (column) * (Step-0.5f);

            if(column%2 == 0)
                x = x + (row * Step);
           else
                x = (x + Step/2) + (row * Step);

            return new Vector3(x, y ,z);
        }

        public override void Update()
        {
            //Should only do it in Strategic or FirstPersonModes
            foreach (var row in board)
                foreach (var item in row)
                    item.Update();
        }

        public override void Draw()
        {
            //Should only do it in Strategic or FirstPersonModes
            foreach (var row in board)
                foreach (var item in row)
                    item.Draw();
        }

        public Field GetByIndex(int index)
        {
            if (IsInRange(index))
            {
                Logger.Log.Error("Index out of Range");
                return null;
            }

            int column = 0;
            for (; index >= board[column].Count; index -= board[column].Count, column++);
            return board[column][index];
        }

        public bool IsInRange(int index)
        {
            return (index < 0 || index >= FieldCount);
        }
    }
}
