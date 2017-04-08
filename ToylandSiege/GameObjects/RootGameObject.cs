
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class RootGameObject: GameObject
    {
        public RootGameObject()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            Name = "RootGameObject";
            Type = "RootGameObject";
            Position = new Vector3(0, 0, 0);
        }

        public override void Update()
        {
            //Leave this empty
        }

        

        public override void Draw()
        {
            //Leave this empty
        }
    }
}
