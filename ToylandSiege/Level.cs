using System;
using System.Collections.Generic;
using ToylandSiege.GameObjects;


namespace ToylandSiege
{
    public class Level
    {
        public string Name;
        public Camera FirstPersonCamera;

        private readonly List<GameObject> _gameObjectsInLevel;
        private static Level _currenLevel;

        public Level(string Name)
        {
            this.Name = Name;
            FirstPersonCamera = new Camera("FirstPersonCamera");
            _currenLevel = this;
            _gameObjectsInLevel = new List<GameObject>();
        }

        public void AddGameObjectToLevel(GameObject obj)
        {
            Logger.Log.Debug("GameObject " + obj.Name + " added to scene");
            _gameObjectsInLevel.Add(obj);
        }

        public void RemoveGameObjectFromLevel(GameObject obj)
        {
            if (_gameObjectsInLevel.Remove(obj))
                Logger.Log.Debug("GameObject " + obj.Name + " removed from scene");
            else
                Logger.Log.Debug("Cant remove GameObject " + obj.Name + " from scene. Scene dos not contain this object");
        }

        public bool RemoveGameObjectFromLevel(string gameObjectName)
        {
            bool removed = false;
            for(int i = 0 ; i < _gameObjectsInLevel.Count; i++)
                if (_gameObjectsInLevel[i].Name == gameObjectName)
                {
                    Logger.Log.Debug("GameObject " + _gameObjectsInLevel[i].Name + " removed from scene");
                    _gameObjectsInLevel.RemoveAt(i);
                    i--;
                    
                    removed = true;
                }
            return removed;
        }

        public GameObject FindGameObjectInLevelByName(string name)
        {
            foreach (var item in _gameObjectsInLevel)
                if (item.Name == name)
                    return item;
            return null;
        }

        public List<GameObject> FindGameObjectsInLevelByName(string name)
        {
            List<GameObject> FoundedItems = new List<GameObject>();

            foreach (var item in _gameObjectsInLevel)
                if (item.Name == name)
                    FoundedItems.Add(item);


            if(FoundedItems.Count != 0)
                return FoundedItems;
            return null;
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

            foreach (var item in _gameObjectsInLevel)
                item.Update();
        }

        public void Draw()
        {
            FirstPersonCamera.Draw();

            foreach (var item in _gameObjectsInLevel)
                item.Draw();
        }
    }
}
