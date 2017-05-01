using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ToylandSiege.GameState
{
    public class Menu : GameState
    {
        public override void Update(GameTime gameTime)
        {
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
             ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                ToylandSiege.GetInstance().Exit();
        }
    }
}
