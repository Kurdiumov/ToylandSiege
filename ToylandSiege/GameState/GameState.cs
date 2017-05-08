using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;

namespace ToylandSiege.GameState
{
    // public enum State { GodMode, FirstPerson, Strategic, Paused, Menu }
    public abstract class GameState
    {
        private SpriteFont _spriteFont;
        private SpriteBatch _spriteBatch;
        private FPSCounter _frameCounter;
        public bool FpsEnabled;

        protected static ConfigurationManager _configurationManager;
        public static GameStateManager _GameStateManager;

        protected static KeyboardState _previousKeyboardState = Keyboard.GetState();
        protected static MouseState _previousMouseState = Mouse.GetState();

        public GameState()
        {
            Init();
        }

        public void Init()
        {
            FpsEnabled = ToylandSiege.GetInstance().configurationManager.FPSEnabled;
            _spriteBatch = new SpriteBatch(ToylandSiege.GetInstance().GraphicsDevice);
            _spriteFont = ToylandSiege.GetInstance().Content.Load<SpriteFont>("Fonts/FPS");
            _configurationManager = ToylandSiege.GetInstance().configurationManager;
            Mouse.SetPosition(ToylandSiege.GetInstance().Window.ClientBounds.Width / 2, ToylandSiege.GetInstance().Window.ClientBounds.Height / 2);
            _previousMouseState = Mouse.GetState();
            _frameCounter = new FPSCounter();
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(GameTime gameTime)
        {
            Level.GetCurrentLevel().Draw();


            if (FpsEnabled)
            {
                _frameCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

                _spriteBatch.Begin();
                _spriteBatch.DrawString(_spriteFont, fps, new Vector2(10, 10), Color.Black);
                _spriteBatch.End();
            }

            if (ToylandSiege.GetInstance().configurationManager.DebugDraw)
                DebugUtilities.DrawColliderWireframes();

            DrawUI();
        }

        public abstract void ProcessInput();



        protected bool IsSimpleKeyPress(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        protected bool IsSimpleClick()
        {
            return Mouse.GetState().LeftButton != _previousMouseState.LeftButton ||
                   Mouse.GetState().RightButton != _previousMouseState.RightButton ||
                   Mouse.GetState().MiddleButton != _previousMouseState.MiddleButton ||
                   Mouse.GetState().XButton1 != _previousMouseState.XButton1 ||
                   Mouse.GetState().XButton2 != _previousMouseState.XButton2 ||
                   Mouse.GetState().ScrollWheelValue != _previousMouseState.ScrollWheelValue;
        }

        protected GameObject PickObject(List<Type> types = null)
        {
            var pickRay = CalculateRay(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                Camera.GetCurrentCamera().ViewMatrix, Camera.GetCurrentCamera().ProjectionMatrix,
                ToylandSiege.GetInstance().GraphicsDevice.Viewport);

            GameObject closestObject = null;
            float? closestObjectDistance = null;

            foreach (var child in Level.GetCurrentLevel().RootGameObject.GetAllChilds(Level.GetCurrentLevel().RootGameObject))
            {
                if (child.Model == null || child.IsEnabled == false || (types !=null && types.All(type => child.GetType() != type)))
                    continue;

                foreach (var mesh in child.Model.Meshes)
                {
                    BoundingSphere sphere = mesh.BoundingSphere;
                    sphere = sphere.Transform(child.TransformationMatrix);

                    var currentDistance = pickRay.Intersects(sphere);
                    if (currentDistance != null)
                        if (closestObjectDistance == null || currentDistance < closestObjectDistance)
                        {
                            closestObject = child;
                            closestObjectDistance = currentDistance;
                        }
                }
            }
            return closestObject;
        }

        protected Ray CalculateRay(Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 0.0f),
                    projection,
                    view,
                    Matrix.Identity);

            Vector3 farPoint = viewport.Unproject(new Vector3(mouseLocation.X,
                    mouseLocation.Y, 1.0f),
                    projection,
                    view,
                    Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        public virtual void DrawUI()
        {

        }

        public virtual  void GameStateChanged()
        {
            
        }

        public virtual void LevelChanged(Level level)
        {

        }
    }
}
