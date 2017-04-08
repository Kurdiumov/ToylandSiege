using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege.GameObjects
{
    public abstract class GameObject
    {
        public string Name = "GameObject";
        public string Type;
        public Vector3 Position;
        public Matrix Rotation;
        public Model Model;
        public bool IsEnabled = true;
        public GameObject Parent = null;
        public List<GameObject> Childs = new List<GameObject>();

        protected abstract void Initialize();
        public abstract void Update();
        public abstract void Draw();

        public void AddChild(GameObject obj)
        {
            Childs.Add(obj);
            obj.Parent = this;
            Logger.Log.Debug("GameObject " + obj.Name + " added to " + Name + " GameObject type: " + Type);
        }

        public void RemoveChild(GameObject obj)
        {
            if (Childs.Remove(obj))
                Logger.Log.Debug("GameObject " + obj.Name + " removed from " + Name + " GameObject type: " + Type);
            else
                Logger.Log.Debug("Cant remove GameObject " + obj.Name + " from " + Name + " GameObject type: " + Type);

        }

        public bool RemoveChild(string name)
        {
            bool removed = false;
            for (int i = 0; i < Childs.Count; i++)
                if (Childs[i].Name == name)
                {
                    Childs.RemoveAt(i);
                    Logger.Log.Debug("GameObject " + Childs[i].Name + " removed from scene");
                    i--;

                    removed = true;
                }
            return removed;
        }

        public GameObject FindGameObjectInLevelByName(string name)
        {
            //TODO: Implement reccursive search
            foreach (var item in Childs)
                if (item.Name == name)
                    return item;
            return null;
        }

        public List<GameObject> FindGameObjectsInLevelByName(string name)
        {
            //TODO: Implement reccursive search
            List<GameObject> FoundedItems = new List<GameObject>();

            foreach (var item in Childs)
                if (item.Name == name)
                    FoundedItems.Add(item);


            if (FoundedItems.Count != 0)
                return FoundedItems;
            return null;
        }

        public override string ToString()
        {
            if(Parent == null)
                return "Name: " + this.Name + ", Type:" + this.Type;
            return "Name: " + this.Name + ", Type: " + this.Type + ", Parent: " + Parent.Name;
        }
    }
}
