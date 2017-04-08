using System;
using log4net;
using Microsoft.Xna.Framework;

namespace ToylandSiege.GameObjects
{
    public class Camera : GameObject
    {
        public Vector3 CamTarget;
        public Matrix ProjectionMatrix;
        public Matrix ViewMatrix;
        public Matrix WorldMatrix;

        private static Camera _currentCamera;

        public Camera(string Name)
        {
            this.Name = Name;
            Initialize();
        }

        public static void SetCurrentCamera(Camera cam)
        {
            _currentCamera = cam;
        }

        protected override void Initialize()
        {
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
