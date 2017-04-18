using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SkinnedModel;

namespace ToylandSiege.GameObjects
{
    public abstract class UnitBase: GameObject
    {
        public float Health;
        public string UnitType;

        public AnimationPlayer AnimationPlayer;

        public UnitBase()
        {
            Initialize();
        }
        protected abstract override void Initialize();

        public abstract override void Update(GameTime gameTime);

    }
}
