using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ToylandSiege.GameObjects;


namespace ToylandSiege
{
    public class Level
    {
        public string Name;
        public GameObject RootGameObject = null;

        private static Level _currenLevel;
        public WaveController WaveController = new WaveController();

        public Level(string Name)
        {
            this.Name = Name;
            _currenLevel = this;

            var WaveBuilder = new WavesBuilder();
            WaveBuilder.Build(WaveController);
        }

        public static Level GetCurrentLevel()
        {
            if(_currenLevel == null)
                throw new TypeInitializationException("_currentLevel was not initialized", null);
            return _currenLevel;
        }

        public void Update(GameTime gameTime)
        {
            Camera.GetCurrentCamera().Update(gameTime);
            WaveController.Update(gameTime);
            foreach (var child  in RootGameObject.Childs.Values)
                child.Update(gameTime);
        }

        public void Draw()
        {
            Camera.GetCurrentCamera().Draw();

            foreach (var child in RootGameObject.Childs.Values)
                child.Draw();
        }
    }
}
