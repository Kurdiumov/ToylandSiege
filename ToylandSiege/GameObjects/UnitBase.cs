using Microsoft.Xna.Framework;


namespace ToylandSiege.GameObjects
{
    public abstract class UnitBase: GameObject
    {
        public float Health { get; set; }
        public float Damage { get; set; }
        public uint ShotDistance { get; set; }
        public float TimeBetweeenShoots { get; set; }

        public string UnitType;
        public UnitBase()
        {
            Initialize();
        }
        protected abstract override void Initialize();

        public abstract override void Update(GameTime gameTime);

    }
}
