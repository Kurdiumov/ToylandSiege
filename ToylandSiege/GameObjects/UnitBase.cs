using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public abstract override void Update();

    }
}
