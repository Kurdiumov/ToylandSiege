using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class InputHelper
    {
        public void Update(GameState gameState)
        {
            switch (gameState.GetCurrentGameState())
            {
                case State.GodMode:
                    UpdateGodMod(gameState);
                    break;
                case State.FirstPerson:
                    UpdateFirstPerson(gameState);
                    break;
                case State.Strategic:
                    UpdateStrategic(gameState);
                    break;
                case State.Paused:
                    UpdatePaused(gameState);
                    break;
                case State.Menu:
                    UpdateMenu(gameState);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                Keys.Escape))
                ToylandSiege.GetToylandSiege().Exit();
        }

        private void UpdateGodMod(GameState gameState)
        {
            //TODO: Create class InputHelper
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Camera.GetCurrentCamera().Position.X -= 0.1f;
                Camera.GetCurrentCamera().CamTarget.X -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Camera.GetCurrentCamera().Position.X += 0.1f;
                Camera.GetCurrentCamera().CamTarget.X += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Camera.GetCurrentCamera().Position.Y += 0.1f;
                Camera.GetCurrentCamera().CamTarget.Y += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Camera.GetCurrentCamera().Position.Y -= 0.1f;
                Camera.GetCurrentCamera().CamTarget.Y -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                Camera.GetCurrentCamera().Position.Z += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                Camera.GetCurrentCamera().Position.Z -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                gameState.SetNewGameState(State.Paused);
            }
        }

        private void UpdateFirstPerson(GameState gameState)
        {
            throw new NotImplementedException();
        }

        private void UpdateStrategic(GameState gameState)
        {
            throw new NotImplementedException();
        }

        private void UpdatePaused(GameState gameState)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                gameState.SetNewGameState(gameState.GetPreviousGameState());
            }
        }

        private void UpdateMenu(GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
}
