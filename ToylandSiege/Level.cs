using System;
using System.Collections.Generic;
using ToylandSiege.GameObjects;


namespace ToylandSiege
{
    public class Level
    {
        public string Name;
        public Camera FirstPersonCamera;
        public GameObject RootGameObject = new RootGameObject();

        private static Level _currenLevel;

        public Level(string Name)
        {
            this.Name = Name;
            FirstPersonCamera = new Camera("FirstPersonCamera");
            _currenLevel = this;
        }

        public static Level GetCurrentLevel()
        {
            if(_currenLevel == null)
                throw new TypeInitializationException("_currentLevel was not initialized", null);
            return _currenLevel;
        }

        public void Update()
        {
            FirstPersonCamera.Update();

            foreach (var child  in RootGameObject.Childs)
                child.Update();
        }

        public void Draw()
        {
            FirstPersonCamera.Draw();

            foreach (var child in RootGameObject.Childs)
                child.Draw();
        }
    }
}
