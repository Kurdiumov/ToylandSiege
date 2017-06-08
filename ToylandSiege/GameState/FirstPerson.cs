using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege.GameState
{
    public class FirstPerson : GameState
    {
        public double ReloadTime = 1.0;

        private bool aim;
        private double _ReloadLeft = 0.0;

        private SpriteFont _SpawnersFont;
        private SpriteFont _TimerSpriteFont;
        private SpriteBatch _TimerSpriteBatch;
        private SpriteBatch _AimSpriteBatch;
        private Texture2D _AimTexture;
        private Board _CurrentBorad;

        public FirstPerson()
        {
            _TimerSpriteBatch = new SpriteBatch(ToylandSiege.GetInstance().GraphicsDevice);
            _TimerSpriteFont = ToylandSiege.GetInstance().Content.Load<SpriteFont>("Fonts/TimerFont");
            _SpawnersFont = ToylandSiege.GetInstance().Content.Load<SpriteFont>("Fonts/SpawnersFont");

            _AimTexture = ToylandSiege.GetInstance().Content.Load<Texture2D>("Aim");
            _AimSpriteBatch = new SpriteBatch(ToylandSiege.GetInstance().GraphicsDevice);
        }

        public override void LevelChanged(Level level)
        {
            _CurrentBorad = level.RootGameObject.Childs["Board"] as Board;
        }

        public override void Update(GameTime gameTime)
        {
            Camera.GetCurrentCamera().RotateCamera(_previousMouseState);
            Level.GetCurrentLevel().Update(gameTime);
            if (_ReloadLeft < 0)
                _ReloadLeft = 0;
            else if (_ReloadLeft > 0)
                _ReloadLeft -= gameTime.ElapsedGameTime.TotalSeconds;
            CollisionHelper.CalculateCollisions(); //TODO: Ensure if is in good order?
            ProcessInput();
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();
        }

        public void SpawnCannonball()
        {
            Cannonball ball = new Cannonball();
            ball.Position = Camera.GetCurrentCamera().Position;

            //Level.GetCurrentLevel().RootGameObject.Childs["Cannonballs"].AddChild(ball);
            Level.GetCurrentLevel().RootGameObject.AddChild(ball);
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
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && aim && _ReloadLeft < 0.1f)
            {
                Logger.Log.Debug("Shot");
                /*
                var PickedObject = PickObject();
                if (PickedObject != null)
                {
                    Logger.Log.Debug("Shot picked: " + PickedObject.ToString());
                }
                */
                SpawnCannonball();

                _ReloadLeft = ReloadTime;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
             ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["Menu"]);



            //Secret combination to finish wave immediately
            if (IsSimpleKeyPress(Keys.F) && Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            {
                Level.GetCurrentLevel().WaveController.FinishRound();
            }
        }


        public override void DrawUI()
        {
            _TimerSpriteBatch.Begin();
            _TimerSpriteBatch.DrawString(_TimerSpriteFont, Math.Round(Level.GetCurrentLevel().WaveController.CurrentWave.TimeLeft).ToString(), new Vector2((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2) - 12, 10), Color.Black);
            _TimerSpriteBatch.DrawString(_TimerSpriteFont, "Wave " + WaveController.RoundNumber + "/" + WaveController.WawesCount, new Vector2((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) - 180, 10), Color.DarkRed);
            if (_CurrentBorad != null)
                _TimerSpriteBatch.DrawString(_SpawnersFont, "Spawners: " + _CurrentBorad.GetEnabledEnemiesSpawnersCount() + "/" + _CurrentBorad.GetAllEnemiesSpawnersCount(), new Vector2(10, ToylandSiege.GetInstance().GraphicsDevice.DisplayMode.Height - 30), Color.DarkRed);
            _TimerSpriteBatch.End();
            if (aim)
            {
                _AimSpriteBatch.Begin();
                _AimSpriteBatch.Draw(_AimTexture, new Vector2((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2), (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2)), Color.White);
                _AimSpriteBatch.End();
            }
        }
    }
}
