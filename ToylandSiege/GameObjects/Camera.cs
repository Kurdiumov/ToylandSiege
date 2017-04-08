using System;
using log4net;
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class Camera : GameObject
    {
        //FirstPersonCamera
        public Vector3 CamTarget;
        public Matrix ProjectionMatrix;
        public Matrix ViewMatrix;
        public Matrix WorldMatrix;

        private static Camera _currentCamera;

        public Camera(string Name)
        {
            this.Name = Name;
            CreateFirstPersonCamera();
            _currentCamera = this;
            Initialize();
        }

        protected override void Initialize()
        {
            this.Type = "FirstPersonCamera";

        }

        protected void CreateFirstPersonCamera()
        {
            //Setup FirstPersonCamera
            CamTarget = new Vector3(0f, 0f, 0f);
            Position = new Vector3(0f, 0f, 5);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f), ToylandSiege.Graphics.
                               GraphicsDevice.Viewport.AspectRatio,
                1f, 1000f);
            ViewMatrix = Matrix.CreateLookAt(Position, CamTarget,
                         new Vector3(0f, 1f, 0f));// Y up
            WorldMatrix = Matrix.CreateWorld(CamTarget, Vector3.
                          Forward, Vector3.Up);
        }

        public override void Update()
        {
        }

        public override void Draw()
        {

        }

        public static Camera GetCurrentCamera()
        {
            if (_currentCamera == null)
            {
                Logger.Log.Error("_currenCamera was not initialized");
                throw new TypeInitializationException("_currenCamera was not initialized", null);
            }
            return

            _currentCamera;
        }
    }
}
