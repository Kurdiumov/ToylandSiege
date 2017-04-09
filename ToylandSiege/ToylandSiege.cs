using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class ToylandSiege : Game
    {
        public static GraphicsDeviceManager Graphics;
        public static Level CurrentLevel;
        public bool FpsEnabled = true;

        private static ToylandSiege _ts;
        private readonly FPSCounter _frameCounter = new FPSCounter();
        private SpriteFont _spriteFont;
        private SpriteBatch _spriteBatch;
        private GameState _gameState;
        private InputHelper _inputHelper;

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

            //TODO: Read properties from configuration file 
            _gameState  = new GameState(State.GodMode);
            _inputHelper = new InputHelper();


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
            _inputHelper.Update(_gameState);

            if (_gameState.GetCurrentGameState() == State.Paused)
                return;
            
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

        //Used in scene parser and in inputHelper
        public static ToylandSiege GetToylandSiege()
        {
            if (_ts == null)
                throw new NullReferenceException("Toyland siege is not created");
            return _ts;
        }
    }
}
