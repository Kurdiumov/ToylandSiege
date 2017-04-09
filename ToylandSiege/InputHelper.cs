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
        private MouseState PrevMouseState;
        private readonly ConfigurationManager _configurationManager;

        public InputHelper(ConfigurationManager configurationManager)
        {
            Mouse.SetPosition(ToylandSiege.GetToylandSiege().Window.ClientBounds.Width / 2, ToylandSiege.GetToylandSiege().Window.ClientBounds.Height / 2);
            PrevMouseState = Mouse.GetState();
            _configurationManager = configurationManager;
        }

        public void Update(GameState gameState)
        {
            switch (gameState.GetCurrentGameState())
            {
                case State.GodMode:
                    if(_configurationManager.GodModeEnabled)
                        UpdateGodMod(gameState);
                    else
                        UpdateFirstPerson(gameState);
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
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Camera.GetCurrentCamera().Position += Vector3.Cross(Camera.GetCurrentCamera().Up, Camera.GetCurrentCamera().Direction) * Camera.GetCurrentCamera().Speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Camera.GetCurrentCamera().Position -= Vector3.Cross(Camera.GetCurrentCamera().Up, Camera.GetCurrentCamera().Direction) * Camera.GetCurrentCamera().Speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Camera.GetCurrentCamera().Position += Camera.GetCurrentCamera().Direction *
                                                      Camera.GetCurrentCamera().Speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Camera.GetCurrentCamera().Position -= Camera.GetCurrentCamera().Direction *
                                      Camera.GetCurrentCamera().Speed;
            }

            if (Mouse.GetState() != PrevMouseState)
            {
                Camera.GetCurrentCamera().Direction = Vector3.Transform(
                    Camera.GetCurrentCamera().Direction,
                    Matrix.CreateFromAxisAngle(Camera.GetCurrentCamera().Up,
                        (-MathHelper.PiOver4 / 150) * (Mouse.GetState().X - PrevMouseState.X)));


                Camera.GetCurrentCamera().Direction = Vector3.Transform(
                    Camera.GetCurrentCamera().Direction,
                    Matrix.CreateFromAxisAngle(
                        Vector3.Cross(Camera.GetCurrentCamera().Up, Camera.GetCurrentCamera().Direction),
                        (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - PrevMouseState.Y)));

                // Reset PrevMouseState
                PrevMouseState = Mouse.GetState();
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
