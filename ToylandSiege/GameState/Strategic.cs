using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ToylandSiege.GameState
{
    public class Strategic: GameState
    {
        private Texture2D soldierTexture2D;
        private SpriteBatch _spriteBatch;
        private Wave _currentWave;

        private readonly int _soldierTextureSize = 100;
        private  int _soldierTextureOffset = 0;
        public Strategic()
        {
            soldierTexture2D = ToylandSiege.GetInstance().Content.Load<Texture2D>("soldierTexture");
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
        }

        public override void DrawUI()
        {
            _spriteBatch.Begin();
            for (int i = 0; i < _currentWave.AvailableUnits.Count; i++)
            {
                _spriteBatch.Draw(soldierTexture2D, new Rectangle(_soldierTextureOffset + (_soldierTextureSize * i), 10, _soldierTextureSize, _soldierTextureSize), Color.White);
            }
            _spriteBatch.End();
        }

        public override void GameStateChanged()
        {
            _currentWave = Level.GetCurrentLevel().WaveController.CurrentWave;
            _soldierTextureOffset = (ToylandSiege.GetInstance().GraphicsDevice.DisplayMode.Width - (_currentWave.AvailableUnits.Count * _soldierTextureSize)) / 2;
        }
    }
}
