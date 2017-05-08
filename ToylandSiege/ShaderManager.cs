using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege
{
    public class ShaderManager
    {
        public static Dictionary<string, Effect > LoadedShaders = new Dictionary<string, Effect>();

        public void Initialize()
        {
            LoadedShaders.Add("ShadowShader", ToylandSiege.GetInstance().Content.Load<Effect>("Shaders/ShadowShader"));
            LoadedShaders.Add("LightShader", ToylandSiege.GetInstance().Content.Load<Effect>("Shaders/LightShader"));
        }

        public static Effect Get(string shaderName)
        {
            return LoadedShaders[shaderName];
        }
        
    }
}
