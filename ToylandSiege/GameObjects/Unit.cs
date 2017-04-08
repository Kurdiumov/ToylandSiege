using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //throw new NotImplementedException();
            CreateTransformationMatrix();
        }
    }
}
