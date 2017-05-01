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
        private static ToylandSiege _ts;

       
        public GameStateManager gameStateManager;
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

            DebugUtilities.ShowAllGameObjects(CurrentLevel.RootGameObject);
            IsMouseVisible = configurationManager.MouseVisible;
 
            gameStateManager = new GameStateManager();
            configurationManager.InitGameStates();

        }

        protected override void LoadContent()
        {

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            gameStateManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            gameStateManager.Draw(gameTime);
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
