﻿using System.Collections.Generic;
using System.Linq;
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
            Rotation = new Vector3(0,0,0);
            Scale = new Vector3(1,1,1);
            CreateTransformationMatrix();
            Childs.Add("Units", new Group("Units"));
        }

        public override void Update(GameTime gameTime)
        {
            //Leave this empty
        }
    }
}
