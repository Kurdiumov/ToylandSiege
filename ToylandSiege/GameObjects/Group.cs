using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToylandSiege.GameObjects
{
    public class Group: GameObject
    {
        public Group(string Name)
        {
            this.Name = Name;
            Initialize();
        }

        protected override void Initialize()
        {

        }

        public override void Update()
        {
            if (!IsStatic)
                CreateTransformationMatrix();

            foreach (var child in Childs.Values)
            {
                child.Update();
            }
        }
    }
}
