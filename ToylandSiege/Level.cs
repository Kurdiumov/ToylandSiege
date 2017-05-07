using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ToylandSiege.GameObjects;


namespace ToylandSiege
{
    public class Level
    {
        public string Name;
        public GameObject RootGameObject = null;

        private static Level _currenLevel;
        public WaveController WaveController = new WaveController();
        private Sky _sky;

        public Level(string Name)
        {
            this.Name = Name;
            _currenLevel = this;

            var WaveBuilder = new WavesBuilder();
            WaveBuilder.Build(WaveController);
            _sky = new Sky();
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
            _sky.Draw();

            ToylandSiege.GetInstance().GraphicsDevice.BlendState = BlendState.Opaque;
            ToylandSiege.GetInstance().GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ToylandSiege.GetInstance().GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            Camera.GetCurrentCamera().Draw();

            foreach (var child in RootGameObject.Childs.Values)
                child.Draw();
        }
    }
}
