using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class Unit:UnitBase
    {
        protected override void Initialize()
        {
           // throw new NotImplementedException();
        }

        public override void Update()
        {
            CreateTransformationMatrix();

            //TODO: Remove line belowe. Used to check drawing and update methods
            this.Rotation += new Vector3(0.01f, 0, 0);
        }
    }
}
