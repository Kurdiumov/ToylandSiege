using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege
{
    public class ShaderManager
    {
        public static Dictionary<string, Effect> LoadedShaders = new Dictionary<string, Effect>();

        public void Initialize()
        {
            LoadedShaders.Add("ReflectionShader", ToylandSiege.GetInstance().Content.Load<Effect>("Shaders/ReflectionShader"));
        }

        public static Effect Get(string shaderName)
        {
            return LoadedShaders[shaderName];
        }

    }
}