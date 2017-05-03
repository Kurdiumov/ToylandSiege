using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege.GameState
{
    public class Strategic: GameState
    {
        private Texture2D soldierTexture2D;
        private Texture2D scoutTexture2D;
        private Texture2D tankTexture2D;

        private SpriteBatch _spriteBatch;
        private Wave _currentWave;

        private readonly int _soldierTextureSize = 100;
        private  int _soldierTextureOffset = 0;
        private readonly int _soldierTexturePadding = 10;

        private Unit SelectedUnit;
        private Field SelectedField;
        private List<Rectangle> sodlierstTextures = new List<Rectangle>();  

        public Strategic()
        {
            soldierTexture2D = ToylandSiege.GetInstance().Content.Load<Texture2D>("soldierTexture");
            scoutTexture2D = ToylandSiege.GetInstance().Content.Load<Texture2D>("scoutTexture");
            tankTexture2D = ToylandSiege.GetInstance().Content.Load<Texture2D>("TankTexture");
            _spriteBatch = new SpriteBatch(ToylandSiege.GetInstance().GraphicsDevice);
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
                Level.GetCurrentLevel().WaveController.StartRound();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
             ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                ToylandSiege.GetInstance().Exit();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && IsSimpleClick() )
            {
                Logger.Log.Debug("Clicked in Startegic view!");
                if (sodlierstTextures.Any(rectangle => rectangle.Contains(Mouse.GetState().Position)))
                {
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
                else if (SelectedUnit != null) //Selecting field when unit not null
                {
                     SelectedField = (Field)PickObject(new List<Type>(){typeof(Field)});
                    if (SelectedField.HasUnit() || !SelectedField.StartingTile)
                        SelectedField = null;
                    if (SelectedUnit != null && SelectedField != null)
                    {
                        _placeUnitOnField(SelectedUnit, SelectedField);
                        SelectedUnit = null;
                        SelectedField = null;
                    }
                }
                else
                {
                    SelectedUnit = null;
                    SelectedField = null;
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
            field.SetUnit(SelectedUnit);
            this.SelectedUnit.Field = field;
            _currentWave.AvailableUnits.Remove(SelectedUnit);
            
            //Addunit to level
            SelectedUnit.Position = field.Position;
            Level.GetCurrentLevel().RootGameObject.AddChild(SelectedUnit);
            _updateUI();
        }
    }
}
