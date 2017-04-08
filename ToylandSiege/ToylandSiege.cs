using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ToylandSiege : Game
    {
        public static GraphicsDeviceManager Graphics;
        public static Level CurrentLevel;


        public ToylandSiege()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Logger.Log.Debug("Initializing");
            base.Initialize();
            CurrentLevel = new Level("Level1");
            CurrentLevel.AddGameObjectToLevel(new TerrainObject("Cube", Content.Load<Model>("MonoCube")));
        }

        protected override void LoadContent()
        {

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                Keys.Escape))
                Exit();

            //TODO: Create class InputHelper
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Camera.GetCurrentCamera().Position.X -= 0.1f;
                Camera.GetCurrentCamera().CamTarget.X -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Camera.GetCurrentCamera().Position.X += 0.1f;
                Camera.GetCurrentCamera().CamTarget.X += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Camera.GetCurrentCamera().Position.Y += 0.1f;
                Camera.GetCurrentCamera().CamTarget.Y += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Camera.GetCurrentCamera().Position.Y -= 0.1f;
                Camera.GetCurrentCamera().CamTarget.Y -= 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                Camera.GetCurrentCamera().Position.Z += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                Camera.GetCurrentCamera().Position.Z -= 0.1f;
            }


            Camera.GetCurrentCamera().ViewMatrix = Matrix.CreateLookAt(CurrentLevel.FirstPersonCamera.Position, CurrentLevel.FirstPersonCamera.CamTarget,
                         Vector3.Up);

            CurrentLevel.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

           CurrentLevel.Draw();
            base.Draw(gameTime);
        }
    }
}
