using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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

        public override void Update(GameTime gameTime)
        {
            if (!IsStatic)
                CreateTransformationMatrix();

            //foreach (var child in Childs.Values)
            //    child.Update(gameTime);
            
            for(int i = 0 ; i < Childs.Values.Count; i++)
                Childs.Values.ElementAt(i).Update(gameTime);
        }
    }
}
