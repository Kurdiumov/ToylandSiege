using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ToylandSiege.GameObjects
{
    public class Camera : GameObject
    {  
        public Matrix ProjectionMatrix;
        public Matrix ViewMatrix;

        public Vector3 Direction = Vector3.Forward;
        public Vector3 Up = Vector3.Up;

        public float Angle;
        public float NearDistance;
        public float FarDistance;

        private float _zoomstrength = 2.0f;
        public float Speed = 0.3F;

        private static Camera _currentCamera;
        public static Dictionary<string, Camera> AvailableCameras = new Dictionary<string, Camera>();


        public Camera(string Name)
        {
            this.Name = Name;
            CreateLookAt();
            if(!AvailableCameras.ContainsKey(Name))
                AvailableCameras.Add(Name, this);
            Initialize();
        }

        public static void SetCurrentCamera(Camera cam)
        {
            _currentCamera = cam;
        }

        protected override void Initialize()
        {
            Direction.Normalize();
        }


        public override void Update(GameTime gameTime)
        {
            CreateLookAt();
        }

        private void CreateLookAt()
        {
            ViewMatrix = Matrix.CreateLookAt(Position, Position + Direction, Up);
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

        public static void SwitchToNextCamera()
                {
            if (AvailableCameras.Count <= 1)
                return;

            int index = 0;

            for (; index < AvailableCameras.Count; index++)
                if (AvailableCameras.ElementAt(index).Value == Camera.GetCurrentCamera())
                {
                    index++;
                    break;
                }
            if (AvailableCameras.Count <= index)
                index = 0;
            Camera.SetCurrentCamera(AvailableCameras.ElementAt(index).Value);
        }
        public void Zoom(bool zoom)
        {
            if(zoom)
                ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Angle/_zoomstrength),
                    ToylandSiege.GetInstance().GraphicsDevice.Viewport.AspectRatio, NearDistance, FarDistance);
            else
                ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Angle),
                    ToylandSiege.GetInstance().GraphicsDevice.Viewport.AspectRatio, NearDistance, FarDistance);
        }

        public void RotateCamera(MouseState _previousMouseState)
        {
            if (!ToylandSiege.GetInstance().IsActive)
                return;
            if (Mouse.GetState() != _previousMouseState)
            {
                Camera.GetCurrentCamera().Direction = Vector3.Transform(
                     Camera.GetCurrentCamera().Direction,
                     Matrix.CreateFromAxisAngle(Camera.GetCurrentCamera().Up,
                         (-MathHelper.PiOver4 / 150) * (Mouse.GetState().X - _previousMouseState.X)));


                Camera.GetCurrentCamera().Direction = Vector3.Transform(
                    Camera.GetCurrentCamera().Direction,
                    Matrix.CreateFromAxisAngle(
                        Vector3.Cross(Camera.GetCurrentCamera().Up, Camera.GetCurrentCamera().Direction),
                        (MathHelper.PiOver4 / 100) * (Mouse.GetState().Y - _previousMouseState.Y)));


                // Reset PrevMouseState
                Mouse.SetPosition(ToylandSiege.GetInstance().Window.ClientBounds.Width / 2, ToylandSiege.GetInstance().Window.ClientBounds.Height / 2);
            }
        }

    }
}
