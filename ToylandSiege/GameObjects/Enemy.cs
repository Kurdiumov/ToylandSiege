﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}