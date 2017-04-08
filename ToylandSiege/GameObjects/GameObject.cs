using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public abstract class GameObject
    {
        public string Name = "GameObject";
        public string Type;
        public Vector3 Position;
        public Matrix Rotation;

        protected abstract void Initialize();
        public abstract void Update();
        public abstract void Draw();
    }
}
