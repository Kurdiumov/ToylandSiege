using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ToylandSiege.GameObjects
{
    class Cannonball : GameObject
    {
        private static int _CannonballIndex = 0; // quick hack to keep names unique, Cannonball#id

        private static float _Speed = 2.0f;
        public static float gravity = 1.0f;
        public static float CannonDamage = 10.0f;

        public Vector3 DirForce = new Vector3(0, 0, 0);

        public Cannonball()
        {
            this.Name = "Cannonball#" + _CannonballIndex;
            this.Type = "Cannonball";
            this.Collider.CreateSingleBoundingSphereForModel();
            _CannonballIndex++;
            this.Model = ToylandSiege.GetInstance().Content.Load<Model>("PrimitiveShapes/Sphere");
            this.IsStatic = false;
            this.IsEnabled = true;
            this.IsCollidable = true;
            Initialize();
        }

        private Vector3 CalculateDirection(Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
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
            return direction;
        }

        protected override void Initialize()
        {
            Vector3 direction = CalculateDirection(new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                Camera.GetCurrentCamera().ViewMatrix, Camera.GetCurrentCamera().ProjectionMatrix,
                ToylandSiege.GetInstance().GraphicsDevice.Viewport);
            DirForce = direction * _Speed;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsStatic)
            {
                DirForce += Vector3.Down * gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position += DirForce;
                CreateTransformationMatrix();
            }
        }

        public override void HandleCollisionWith(GameObject obj2)
        {
            if (obj2.Type == "Enemy")
            {
                Enemy enemy = (Enemy) obj2;
                enemy.GetDamage(CannonDamage);
                this.Destroy();
            }
            else
            {
                this.Destroy();
            }
        }
    }
}
