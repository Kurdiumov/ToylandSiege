using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;
using System.Collections.Generic;
using System.Linq;

namespace ToylandSiege
{
    public class ToylandSiege : Game
    {
        public static GraphicsDeviceManager Graphics;
        public static Level CurrentLevel;
        public bool FpsEnabled = false;

        private static ToylandSiege _ts;
        private readonly FPSCounter _frameCounter = new FPSCounter();
        private SpriteFont _spriteFont;
        private SpriteFont _spritePausedFont;
        private SpriteBatch _spriteBatch;
        private GameState _gameState;
        private InputHelper _inputHelper;
        public ConfigurationManager configurationManager;

        public ToylandSiege()
        {
            _ts = this;
            Graphics = new GraphicsDeviceManager(this);


            configurationManager = new ConfigurationManager();
            if (configurationManager.IsFullScreen)
                Graphics.IsFullScreen = true;

            Graphics.PreferredBackBufferHeight = configurationManager.HeightResolution;
            Graphics.PreferredBackBufferWidth = configurationManager.WidthResolution;

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
            FpsEnabled = configurationManager.FPSEnabled;

            IsMouseVisible = configurationManager.MouseVisible;
            _gameState = new GameState(configurationManager.GameState);
            _inputHelper = new InputHelper(configurationManager);

        }

        protected override void LoadContent()
        {
            _spriteFont = Content.Load<SpriteFont>("FPS");
            _spritePausedFont = Content.Load<SpriteFont>("PausedSpriteFont");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            _inputHelper.Update(_gameState);

            if (_gameState.GetCurrentGameState() == State.Paused)
                return;

            CollisionHelper.CalculateCollisions(); //TODO: Ensure if is in good order?
            CurrentLevel.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            CurrentLevel.Draw();
            base.Draw(gameTime);

            if (FpsEnabled)
            {
                _frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

                _spriteBatch.Begin();
                _spriteBatch.DrawString(_spriteFont, fps, new Vector2(10, 10), Color.Black);
                _spriteBatch.End();

            }
            if (_gameState.GetCurrentGameState() == State.Paused)
            {
                _spriteBatch.Begin();
                string TextToDraw = "Game Paused";
                _spriteBatch.DrawString(_spritePausedFont, TextToDraw, 
                    new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2, 
                    GraphicsDevice.Viewport.Bounds.Height / 2) - _spritePausedFont.MeasureString(TextToDraw) / 2, 
                    Color.Red);
                _spriteBatch.End();
            }
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
