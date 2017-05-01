using System;
using System.Web.UI.WebControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ToylandSiege.GameState
{
    public class Paused: GameState
    {
        private SpriteFont _spritePausedFont;
        private SpriteBatch _spriteBatch;
        
        
        public Paused()
        {
            _spriteBatch = new SpriteBatch(ToylandSiege.GetToylandSiege().GraphicsDevice);
            _spritePausedFont = ToylandSiege.GetToylandSiege().Content.Load<SpriteFont>("PausedSpriteFont");
            
        }

        public override void Update(GameTime gameTime)
        {
            ProcessInput();
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();
            return;
        }

        public override void Draw(GameTime gameTime)
        {
            this._spriteBatch.Begin();
            string TextToDraw = "Game Paused";
            this._spriteBatch.DrawString(_spritePausedFont, TextToDraw,
                new Vector2(ToylandSiege.GetToylandSiege().GraphicsDevice.Viewport.Bounds.Width / 2,
                ToylandSiege.GetToylandSiege().GraphicsDevice.Viewport.Bounds.Height / 2) - _spritePausedFont.MeasureString(TextToDraw) / 2,
                Color.Red);
            this._spriteBatch.End();
        }

        public override void ProcessInput()
        {
            if (IsSimpleKeyPress(Keys.P))
            {
                _GameStateManager.SetNewGameState(_GameStateManager.GetPreviousGameState());
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
             ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                ToylandSiege.GetToylandSiege().Exit();
        }
    }
}
