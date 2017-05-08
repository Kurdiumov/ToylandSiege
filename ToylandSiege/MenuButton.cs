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

        private string text;
        private SpriteFont font;
        private int heigth;
        private int width;
        public Menubutton(Texture2D textur, int X, int Y, int width, int heihgt, string text)
        {
            this.texture = textur;
            this.coords.X = X;
            this.coords.Y = Y;
            this.text = text;
            this.heigth = heihgt;
            this.width = width;
            rectangle  = new Rectangle(X, Y, width, heihgt);
            font = ToylandSiege.GetInstance().Content.Load<SpriteFont>("Fonts/MenuFont");

        }

        public void Update(GameWindow window)
        {


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, coords, Color.White);
            spriteBatch.DrawString(font, text, new Vector2(coords.X+(width / 2) - text.Length*7, coords.Y+ (heigth/2)-25), Color.Black);

        }
    }
}
