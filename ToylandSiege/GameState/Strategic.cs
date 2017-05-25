using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege.GameState
{
    public class Strategic : GameState
    {
        private Texture2D soldierTexture2D;
        private Texture2D scoutTexture2D;
        private Texture2D tankTexture2D;

        private SpriteBatch _spriteBatch;
        private Wave _currentWave;

        private readonly int _soldierTextureSize = 100;
        private int _soldierTextureOffset = 0;
        private readonly int _soldierTexturePadding = 10;

        private Unit SelectedUnit;
        private Field SelectedField;
        private List<Rectangle> sodlierstTextures = new List<Rectangle>();


        public static bool FirstUnitPlacing = false;
        public static bool UnitSelected = false;

        enum StrategicState
        {
            Default,
            SelectedUnit,
            PlacedUnit
        }

        private int btnWidth = 250, btnHeight = 60;
        private Menubutton StartWaveBtn;
        public Strategic()
        {
            soldierTexture2D = ToylandSiege.GetInstance().Content.Load<Texture2D>("soldierTexture");
            scoutTexture2D = ToylandSiege.GetInstance().Content.Load<Texture2D>("scoutTexture");
            tankTexture2D = ToylandSiege.GetInstance().Content.Load<Texture2D>("TankTexture");
            _spriteBatch = new SpriteBatch(ToylandSiege.GetInstance().GraphicsDevice);

            //Create start Wave button
            var startWaveTexture = new Texture2D(ToylandSiege.GetInstance().GraphicsDevice, btnWidth, btnHeight);
            var data = new Color[btnWidth * btnHeight];
            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Green;
            startWaveTexture.SetData(data);
            StartWaveBtn = new Menubutton(startWaveTexture, (ToylandSiege.GetInstance().GraphicsDevice.DisplayMode.Width / 2) - (btnWidth / 2), ToylandSiege.GetInstance().GraphicsDevice.DisplayMode.Height - 70, btnWidth, btnHeight, "Start Wave   ");
        }

        public override void Update(GameTime gameTime)
        {
            Level.GetCurrentLevel().Update(gameTime);
            ProcessInput();
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();
        }


        public override void ProcessInput()
        {
            if (!ToylandSiege.GetInstance().IsActive)
                return;
            if (IsSimpleKeyPress(Keys.P))
            {
                _GameStateManager.SetNewGameState(_GameStateManager.AvailableGameStates["Paused"]);
            }

            if (!WaveController.RoundRunning && IsSimpleKeyPress(Keys.Space))
            {
                if (_CanStartWave())
                    Level.GetCurrentLevel().WaveController.StartRound();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
             ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["Menu"]);


            if (Mouse.GetState().LeftButton == ButtonState.Pressed && IsSimpleClick())
            {
                if (_CanStartWave() && StartWaveBtn.rectangle.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)))
                {
                    Level.GetCurrentLevel().WaveController.StartRound();
                }
                else
                {
                    Logger.Log.Debug("Clicked in Startegic view!");
                    _PlacingUnit();
                }
            }

            //Secret combination to switch  to god mode
            if (IsSimpleKeyPress(Keys.G) && Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            {
                ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["GodMode"]);
            }
        }

        public override void DrawUI()
        {
            _spriteBatch.Begin();

            if (sodlierstTextures.Count != _currentWave.AvailableUnits.Count)
                throw new ArgumentException("Something went wrong. Count of rectangles should be the same as Units");
            for (int i = 0; i < sodlierstTextures.Count; i++)
            {
                switch (_currentWave.AvailableUnits[i].UnitType.ToLower())
                {
                    case "soldier":
                        _spriteBatch.Draw(soldierTexture2D, sodlierstTextures[i], Color.White);
                        break;
                    case "tank":
                        _spriteBatch.Draw(tankTexture2D, sodlierstTextures[i], Color.White);
                        break;
                    case "scout":
                        _spriteBatch.Draw(scoutTexture2D, sodlierstTextures[i], Color.White);
                        break;
                    default:
                        throw new ArgumentException("Unsupported unit type " + (_currentWave.AvailableUnits[i].UnitType));
                }
            }
            if (_CanStartWave() && !UnitSelected)
            {
                StartWaveBtn.Draw(_spriteBatch);
            }
            _spriteBatch.End();
        }

        public override void GameStateChanged()
        {
            _currentWave = Level.GetCurrentLevel().WaveController.CurrentWave;
            _updateUI();
        }

        private void _updateUI()
        {
            _soldierTextureOffset = (ToylandSiege.GetInstance().GraphicsDevice.DisplayMode.Width - (_currentWave.AvailableUnits.Count * _soldierTextureSize)) / 2;

            sodlierstTextures.Clear();

            for (int offset = 0, i = 0; i < _currentWave.AvailableUnits.Count; i++, offset += _soldierTexturePadding)
            {
                sodlierstTextures.Add(new Rectangle(_soldierTextureOffset + (_soldierTextureSize * i) + offset, 10, _soldierTextureSize, _soldierTextureSize));
            }
        }

        private void _placeUnitOnField(Unit SelectedUnit, Field field)
        {
            Logger.Log.Debug("Placing unit to field");
            SelectedUnit.IsEnabled = true;
            SelectedUnit.FieldsInWay.Add(field);
            field.SetUnit(SelectedUnit);
            this.SelectedUnit.Field = field;
            _currentWave.AvailableUnits.Remove(SelectedUnit);
            _currentWave.UnitsInWave.Add(SelectedUnit);

            //Add unit to level
            SelectedUnit.Position = field.Position;
            Level.GetCurrentLevel().RootGameObject.Childs["Units"].AddChild(SelectedUnit);

            _updateUI();
        }

        private void _replaceUnit(Unit SelectedUnit, Field field)
        {
            Logger.Log.Debug("Replaced unit");
            SelectedUnit.FieldsInWay.Clear();
            SelectedUnit.TargetFields.Clear();
            SelectedUnit.FieldsInWay.Add(field);
            field.SetUnit(SelectedUnit);
            this.SelectedUnit.Field = field;

            SelectedUnit.Position = field.Position;
        }

        private void _unselectPath(Unit SelectedUnit)
        {
            foreach(Field f in SelectedUnit.FieldsInWay)
            {
                f.IsPartOfWay = false;
            }
        }

        private void _removeUnitFromField(Unit unit)
        {
            foreach (Field f in unit.FieldsInWay)
                f.IsPartOfWay = false;

            if (unit.Field != null)
            {
                unit.Field.IsPartOfWay = false;
                unit.Field.unit = null;
            }

            unit.FieldsInWay.Clear();
            unit.TargetFields.Clear();

            _currentWave.AvailableUnits.Add(SelectedUnit);
            _currentWave.UnitsInWave.Remove(SelectedUnit);
            unit.IsEnabled = false;
            _updateUI();
        }

        private void _PlacingUnit()
        {
            FirstUnitPlacing = false;
            UnitSelected = true;

            // Selecting unit on list
            if (sodlierstTextures.Any(rectangle => rectangle.Contains(Mouse.GetState().Position)))
            {
                if(SelectedUnit != null)
                {
                    if(SelectedUnit.Field != null)
                    {
                        if(SelectedUnit.TargetFields.Count > 0)
                        {
                            if (!SelectedUnit.TargetFields.Last().FinishingTile)
                                _removeUnitFromField(SelectedUnit);
                        }
                        else _removeUnitFromField(SelectedUnit);
                    }
                }
                else
                {
                    FirstUnitPlacing = true;
                    int UnitIndex = 0;
                    for (int i = 0; i < sodlierstTextures.Count; i++)
                    {
                        if (sodlierstTextures[i].Contains(Mouse.GetState().Position))
                        {
                            UnitIndex = i;
                            break;
                        }
                    }
                    SelectedUnit = _currentWave.AvailableUnits[UnitIndex];
                    if (SelectedUnit != null)
                    {
                        Logger.Log.Debug("Unit selected:  " + SelectedUnit);
                    }
                }
            }
            //  Selecting field when unit not null
            else if (SelectedUnit != null) 
            {
                SelectedField = (Field)PickObject(new List<Type>() { typeof(Field) });
                if (SelectedField == null)
                {
                    if(SelectedUnit.Field != null && !SelectedUnit.FieldsInWay.Last().FinishingTile)
                    {
                        _removeUnitFromField(SelectedUnit);

                    }

                    _unselectPath(SelectedUnit);
                    return;
                }

                // Placing unit on starting tile
                else if (SelectedUnit.Field == null)
                {
                    if (SelectedField.StartingTile)
                    {
                        if (SelectedUnit != null && SelectedField != null)
                        {
                            _placeUnitOnField(SelectedUnit, SelectedField);
                        }
                    }
                    else
                    {
                        _unselectPath(SelectedUnit);
                        return;
                    }
                }
                else if (SelectedUnit.Field.StartingTile && SelectedField.StartingTile)
                {
                    _unselectPath(SelectedUnit);
                    _replaceUnit(SelectedUnit, SelectedField);
                }

                //  Finding path for selected unit
                else if (SelectedUnit.Field.Index != SelectedField.Index && !SelectedUnit.TargetFields.Contains(SelectedField))
                {
                    //Logger.Log.Debug("Pathfinding run");
                    if (!SelectedField.CanPlaceUnit())
                        return;
                    List<int> path;
                    if (SelectedUnit.TargetFields.Count > 0)
                        path = Pathfinder.FindPath(SelectedUnit.TargetFields.Last(), SelectedField);
                    else
                        path = Pathfinder.FindPath(SelectedUnit.Field, SelectedField);

                    if(path != null)
                    {
                        for (int i = 0; i < path.Count; i++)
                        {
                            SelectedUnit.AddField(((Board)SelectedUnit.Field.Parent.Parent).GetByIndex(path.ElementAt(i)));
                            Logger.Log.Debug(path.ElementAt(i));
                        }
                    }
                    else
                    {
                        Logger.Log.Debug("Path not found");
                    }
                    Logger.Log.Debug("Pathfinding done");
                    if (SelectedUnit.FieldsInWay.Last().FinishingTile)
                    {
                        SelectedUnit = null;
                        SelectedField = null;
                    }
                }
            }
            else if (SelectedUnit == null)
            {
                SelectedField = (Field)PickObject(new List<Type>() { typeof(Field) });
                if (SelectedField == null)
                {
                    FirstUnitPlacing = false;
                    UnitSelected = false;
                    SelectedUnit = null;
                    SelectedField = null;
                    return;
                }
                else if (SelectedField.StartingTile && SelectedField.HasUnit())
                {
                    var unit = SelectedField.unit;
                    unit.Field.IsPartOfWay = false;
                    unit.Field = null;
                    foreach (var field in unit.FieldsInWay)
                        field.IsPartOfWay = false;

                    unit.FieldsInWay.Clear();
                    foreach (var field in unit.TargetFields)
                        field.IsPartOfWay = false;

                    unit.TargetFields.Clear();
                    SelectedField.unit = null;
                    unit.Position = Vector3.Zero;
                    _currentWave.UnitsInWave.Remove(unit);
                    _currentWave.AvailableUnits.Add(unit);
                    Level.GetCurrentLevel().RootGameObject.Childs["Units"].Childs.Remove(unit.Name);

                   var  offset = 0;
                    for (int i = 0; i < sodlierstTextures.Count; i++)
                        offset += _soldierTexturePadding;
                    sodlierstTextures.Add(new Rectangle(_soldierTextureOffset + (_soldierTextureSize * sodlierstTextures.Count) + offset, 10, _soldierTextureSize, _soldierTextureSize));
                }
                else
                {
                    FirstUnitPlacing = false;
                    UnitSelected = false;
                    SelectedUnit = null;
                    SelectedField = null;
                }
            }
            else
            {
                FirstUnitPlacing = false;
                UnitSelected = false;
                SelectedUnit = null;
                SelectedField = null;
            }
        }

        private bool _CanStartWave()
        {
            if (_currentWave.UnitsInWave.Count == 0)
                return false;
            return
                _currentWave.UnitsInWave.All(
                    unit => (unit.TargetFields.Count > 0 && unit.TargetFields.Last().FinishingTile));
        }
    }
}
