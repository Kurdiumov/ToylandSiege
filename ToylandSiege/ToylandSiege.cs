using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ToylandSiege : Game
    {
        public static GraphicsDeviceManager Graphics;
        public static Level CurrentLevel;
        public bool FpsEnabled = true;

        private static ToylandSiege _ts;
        private readonly FPSCounter _frameCounter = new FPSCounter();
        private SpriteFont _spriteFont;
        private SpriteBatch _spriteBatch;

        public ToylandSiege()
        {
            _ts = this;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Logger.Log.Debug("Initializing");
            base.Initialize();
            CurrentLevel = new Level("Level1");

            SceneParser parser = new SceneParser();
            CurrentLevel.RootGameObject = parser.Parse("Level1");
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            DebugUtilities.ShowAllGameObjects(CurrentLevel.RootGameObject);
        }

        protected override void LoadContent()
        {
            _spriteFont = Content.Load<SpriteFont>("FPS");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                Keys.Escape))
                Exit();

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


            Camera.GetCurrentCamera().ViewMatrix = Matrix.CreateLookAt(Camera.GetCurrentCamera().Position, Camera.GetCurrentCamera().CamTarget,
                         Vector3.Up);

            CurrentLevel.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (FpsEnabled)
            {
                _frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

                _spriteBatch.Begin();
                _spriteBatch.DrawString(_spriteFont, fps, new Vector2(10, 10), Color.Black);
                _spriteBatch.End();
            }

            CurrentLevel.Draw();
            base.Draw(gameTime);
        }

        //Used in scene parser
        public static ToylandSiege GetToylandSiege()
        {
            if (_ts == null)
                throw new NullReferenceException("Toyland siege is not created");
            return _ts;
        }
    }
}
