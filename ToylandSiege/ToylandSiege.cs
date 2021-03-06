﻿using System;
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

        public RenderTarget2D _blurrenderTarget;
        public Effect _blurEffect;

        public SpriteBatch _spriteBatch;
        public Boolean blur = false;
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
            SoundManager SoundManager = new SoundManager();
            SoundManager.Initialize();
        }

        protected override void Initialize()
        {
            Logger.Log.Debug("Initializing");
            base.Initialize();
            IsMouseVisible = configurationManager.MouseVisible;

            _blurrenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight,
                                    false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);

            shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            //shadowMapRenderTarget = 
            _ShadowMapGenerate = Content.Load<Effect>("Shaders/ShadowShader");
            _blurEffect = Content.Load<Effect>("Shaders/Blur");

            ShaderManager shaderManager = new ShaderManager();
            shaderManager.Initialize();


            gameStateManager = new GameStateManager();
            configurationManager.InitGameStates();

            CurrentLevel = new Level("Level1");

            SceneParser parser = new SceneParser();
            CurrentLevel.RootGameObject = parser.Parse("Level1");

            DebugUtilities.ShowAllGameObjects(CurrentLevel.RootGameObject);


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
            try
            {
                gameStateManager.Update(gameTime);
                base.Update(gameTime);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            try
            {

                //GraphicsDevice.SetRenderTarget(_blurrenderTarget);
                if (blur)
                {
                    GraphicsDevice.SetRenderTarget(_blurrenderTarget);
                }


                GraphicsDevice.Clear(Color.CornflowerBlue);

                gameStateManager.Draw(gameTime);


                if (blur)
                {

                    GraphicsDevice.SetRenderTarget(null);
                    //GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    _blurEffect.CurrentTechnique.Passes[0].Apply();
                    _spriteBatch.Draw(_blurrenderTarget, new Vector2(0, 0), Color.White);
                    _spriteBatch.End();
                }

                GraphicsDevice.SetRenderTarget(null);
                //DrawShadows();

                //Uncomment to see shadow map
                /*
                GraphicsDevice.SetRenderTarget(null);
                _spriteBatch.Begin(0, BlendState.Opaque, SamplerState.AnisotropicClamp);

                //_spriteBatch.Draw(_blurrenderTarget, new Vector2(0, 0), Color.White);
                _spriteBatch.Draw(_blurrenderTarget, new Rectangle(0, 0, ToylandSiege.GetInstance().configurationManager.WidthResolution, ToylandSiege.GetInstance().configurationManager.HeightResolution), Color.White);
                _spriteBatch.End();*/




                base.Draw(gameTime);
                
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
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
            GraphicsDevice.SetRenderTarget(shadowMapRenderTarget);

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
