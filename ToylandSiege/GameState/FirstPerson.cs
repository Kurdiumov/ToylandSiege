using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege.GameState
{
    public class FirstPerson: GameState
    {
        private bool aim;

        private SpriteFont _TimerSpriteFont;
        private SpriteBatch _TimerSpriteBatch;

        public FirstPerson()
        {
            _TimerSpriteBatch = new SpriteBatch(ToylandSiege.GetInstance().GraphicsDevice);
            _TimerSpriteFont = ToylandSiege.GetInstance().Content.Load<SpriteFont>("TimerFont");
        }

        public override void Update(GameTime gameTime)
        {
            Camera.GetCurrentCamera().RotateCamera(_previousMouseState);
            CollisionHelper.CalculateCollisions(); //TODO: Ensure if is in good order?
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
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (!aim)
                {
                    Camera.GetCurrentCamera().Zoom(true);
                }
                aim = true;
            }
            else if (aim)
            {
                Camera.GetCurrentCamera().Zoom(false);
                aim = false;
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && IsSimpleClick() && aim)
            {
                Logger.Log.Debug("Shot");
                var PickedObject = PickObject();
                if (PickedObject != null)
                {
                    Logger.Log.Debug("Shot picked: " + PickedObject.ToString());
                }
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
             ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                ToylandSiege.GetInstance().Exit();
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _TimerSpriteBatch.Begin();
            _TimerSpriteBatch.DrawString(_TimerSpriteFont, Math.Round(ToylandSiege.waveController.CurrentWave.TimeLeft).ToString(), new Vector2((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/2)-12, 10), Color.Black);
            _TimerSpriteBatch.End();
        }
    }
}
