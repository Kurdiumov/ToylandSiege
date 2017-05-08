using System;
using System.CodeDom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ToylandSiege.GameObjects;
using System.Collections.Generic;
using System.Linq;

namespace ToylandSiege
{
    public class ToylandSiege : Game
    {
        public static GraphicsDeviceManager Graphics;
        public static Level CurrentLevel;
        private static ToylandSiege _ts;

        public GameStateManager gameStateManager;
        public ConfigurationManager configurationManager;
        public RenderTarget2D shadowMapRenderTarget;
        public Effect _ShadowMapGenerate;

        public SpriteBatch _spriteBatch;
        public ToylandSiege()
        {
            _ts = this;
            Graphics = new GraphicsDeviceManager(this);


            configurationManager = new ConfigurationManager();
            if (configurationManager.IsFullScreen)
                Graphics.IsFullScreen = true;

            Graphics.PreferredBackBufferHeight = configurationManager.HeightResolution;
            Graphics.PreferredBackBufferWidth = configurationManager.WidthResolution;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Logger.Log.Debug("Initializing");
            base.Initialize();

            SoundManager SoundManager = new SoundManager();
            SoundManager.Initialize();

            ShaderManager shaderManager = new ShaderManager();
            shaderManager.Initialize();

            CurrentLevel = new Level("Level1");

            SceneParser parser = new SceneParser();
            CurrentLevel.RootGameObject = parser.Parse("Level1");

            DebugUtilities.ShowAllGameObjects(CurrentLevel.RootGameObject);
            IsMouseVisible = configurationManager.MouseVisible;

            gameStateManager = new GameStateManager();
            configurationManager.InitGameStates();

            shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            _ShadowMapGenerate = ShaderManager.Get("ShadowShader");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void LoadContent()
        {

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            gameStateManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            gameStateManager.Draw(gameTime);
            base.Draw(gameTime);
            DrawShadows();
            
            //Uncomment to see shadow map
            /*
            _spriteBatch.Begin(0, BlendState.Opaque, SamplerState.AnisotropicClamp);
            _spriteBatch.Draw(shadowMapRenderTarget,  new Rectangle(0, 0, 1366, 768), Color.White);
            _spriteBatch.End();
            */
        }

        //Used in scene parser and in inputHelper
        public static ToylandSiege GetInstance()
        {
            if (_ts == null)
                throw new NullReferenceException("Toyland siege is not created");
            return _ts;
        }


        private void DrawShadows()
        {
            //Uncomment to see shadow map
            //GraphicsDevice.SetRenderTarget(shadowMapRenderTarget);

            foreach (GameObject gameObject in CurrentLevel.RootGameObject.GetAllChilds(CurrentLevel.RootGameObject))
            {
                if (gameObject.Model != null)
                {
                    DrawShadowMap(gameObject.Model, gameObject.TransformationMatrix);

                }
            }
            GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawShadowMap(Model model, Matrix world)
        {
            for (int index = 0; index < model.Meshes.Count; index++)
            {
                ModelMesh mesh = model.Meshes[index];
                for (int i = 0; i < mesh.MeshParts.Count; i++)
                {
                    ModelMeshPart meshpart = mesh.MeshParts[i];
                    _ShadowMapGenerate.Parameters["LightViewProj"].SetValue(world * GlobalLightning.LightViewProjection);

                    _ShadowMapGenerate.CurrentTechnique.Passes[0].Apply();

                    GraphicsDevice.SetVertexBuffer(meshpart.VertexBuffer);
                    GraphicsDevice.Indices = (meshpart.IndexBuffer);
                    int primitiveCount = meshpart.PrimitiveCount;
                    int vertexOffset = meshpart.VertexOffset;
                    int vCount = meshpart.NumVertices;
                    int startIndex = meshpart.StartIndex;

                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vertexOffset, startIndex,
                        primitiveCount);
                }
            }
        }
    }
}
