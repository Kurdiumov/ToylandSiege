using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege
{
    class Menubutton
    {
        public Texture2D texture;
        public Vector2 coords;
        public Rectangle rectangle;

        bool isClicked = false;
        bool isHovered = false;

        public Menubutton(Texture2D textur, int X, int Y, int width, int heihgt)
        {
            this.texture = textur;
            this.coords.X = X;
            this.coords.Y = Y;
            rectangle  = new Rectangle(X, Y, width, heihgt);

        }

        public void Update(GameWindow window)
        {


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, coords, Color.White);
        }
    }
}
