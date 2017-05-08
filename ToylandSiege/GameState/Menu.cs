using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ToylandSiege.GameState
{
    public class Menu : GameState
    {
        private SpriteBatch _spriteBatch;

        private int btnWidth = 500, btnHeight = 120;
        private Menubutton btn1;
        private Menubutton btn2;
        private Menubutton btn3;

        public Menu()
        {
            _spriteBatch = new SpriteBatch(ToylandSiege.GetInstance().GraphicsDevice);

            var btn1Texture = new Texture2D(ToylandSiege.GetInstance().GraphicsDevice, btnWidth, btnHeight);
            var btn2Texture = new Texture2D(ToylandSiege.GetInstance().GraphicsDevice, btnWidth, btnHeight);
            var btn3Texture = new Texture2D(ToylandSiege.GetInstance().GraphicsDevice, btnWidth, btnHeight);

            btn1Texture.SetData(CreateTexture(Color.Aqua));
            btn2Texture.SetData(CreateTexture(Color.White));
            btn3Texture.SetData(CreateTexture(Color.Red));

            btn1 = new Menubutton(btn1Texture, (ToylandSiege.GetInstance().GraphicsDevice.DisplayMode.Width / 2) - (btnWidth / 2), 150, btnWidth, btnHeight, "Start Tutorial");
            btn2 = new Menubutton(btn2Texture, (ToylandSiege.GetInstance().GraphicsDevice.DisplayMode.Width / 2) - (btnWidth / 2), 300, btnWidth, btnHeight, "Start Level 1");
            btn3 = new Menubutton(btn3Texture, (ToylandSiege.GetInstance().GraphicsDevice.DisplayMode.Width / 2) - (btnWidth / 2), 450, btnWidth, btnHeight, "Exit");
        }

        public Color[] CreateTexture(Color color)
        {
            var data = new Color[btnWidth * btnHeight];
            for (int i = 0; i < data.Length; ++i)
                data[i] = color;
            return data;
        }

        public override void Update(GameTime gameTime)
        {
            ProcessInput();
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();
        }

        public override void ProcessInput()
        {
            if (!ToylandSiege.GetInstance().IsActive)
                return;

            if (IsSimpleKeyPress(Keys.Escape))
                ToylandSiege.GetInstance().Exit();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && IsSimpleClick())
            {
                Logger.Log.Debug("Clicked in Menu view!");

                if (btn1.rectangle.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)))
                {
                    ToylandSiege.CurrentLevel = new Level("TutorialLevel");
                    SceneParser parser = new SceneParser();
                    ToylandSiege.CurrentLevel.RootGameObject = parser.Parse("TutorialLevel");
                    ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["Strategic"]);
                    ToylandSiege.GetInstance().gameStateManager.LevelChanged(Level.GetCurrentLevel());
                }
                else if (btn2.rectangle.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)))
                {
                    ToylandSiege.CurrentLevel = new Level("Level1");
                    SceneParser parser = new SceneParser();
                    ToylandSiege.CurrentLevel.RootGameObject = parser.Parse("Level1");
                    ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["Strategic"]);
                    ToylandSiege.GetInstance().gameStateManager.LevelChanged(Level.GetCurrentLevel());
                }
                else if (btn3.rectangle.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)))
                {
                    ToylandSiege.GetInstance().Exit();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            btn1.Draw(_spriteBatch);
            btn2.Draw(_spriteBatch);
            btn3.Draw(_spriteBatch);
            _spriteBatch.End();

        }
    }
}
