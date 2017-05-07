using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class Sky:GameObject
    {
        public Model Model;
        public Texture2D Texture;

        public Sky()
        {
            Model = ToylandSiege.GetInstance().Content.Load<Model>("Sphere");
            Texture = ToylandSiege.GetInstance().Content.Load<Texture2D>("SkyTexture");
            Initialize();
        }

        public override void Draw()
        {
            GraphicsDevice GraphicsDevice = Texture.GraphicsDevice;

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = Camera.GetCurrentCamera().ViewMatrix;
                    effect.World = TransformationMatrix;
                    effect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;
                    effect.Texture = Texture;
                    effect.TextureEnabled = true;
                }

                mesh.Draw();
            }
        }
        
        protected override void Initialize()
        {
            Rotation = new Vector3(-1.0f,-1.0f,-1.0f);
            CreateTransformationMatrix();
        }

        public override void Update(GameTime gameTime)
        {
            //Leave empty
        }
    }
}
