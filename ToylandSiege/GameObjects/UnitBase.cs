using Microsoft.Xna.Framework;


namespace ToylandSiege.GameObjects
{
    public abstract class UnitBase: GameObject
    {
        public float Health;
        public string UnitType;

        public UnitBase()
        {
            Initialize();
        }
        protected abstract override void Initialize();

        public abstract override void Update(GameTime gameTime);

    }
}
