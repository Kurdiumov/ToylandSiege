using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege.GameState
{
    public class GodMode : GameState
    {
        public override void Update(GameTime gameTime)
        {
            if (!_configurationManager.GodModeEnabled)
                return;

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
            if (IsSimpleKeyPress(Keys.Tab))
            {
                Camera.SwitchToNextCamera();
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && IsSimpleClick() && _configurationManager.PickingEnabled)
            {
                var PickedObject = PickObject(new List<Type>(){ typeof(Field)}); // Use this for pciking only fields
                //var PickedObject = PickObject(); //USe this for picking all abjects
                if (PickedObject != null)
                {
                    Logger.Log.Debug("Object picked: " + PickedObject.ToString());
                }
                else
                {
                    Logger.Log.Debug("Object picked: NULL");
                    System.Diagnostics.Debug.Print("Object picked: NULL");

                }
            }
            if (IsSimpleKeyPress(Keys.P))
            {
                _GameStateManager.SetNewGameState(_GameStateManager.AvailableGameStates["Paused"]);
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
             ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["Menu"]);
        }
    }
}
