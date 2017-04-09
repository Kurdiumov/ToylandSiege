using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class Enemy:UnitBase
    {
        protected override void Initialize()
        {
            //throw new NotImplementedException();
        }

        public override void Update()
        {
            CreateTransformationMatrix();

            //TODO: Remove line belowe. Used to check drawing and update methods
            this.Position -= new Vector3(0.01f, 0f, 0f);
        }
    }
}
