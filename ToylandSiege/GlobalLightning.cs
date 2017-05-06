using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege
{
    public static class GlobalLightning
    {
        private static Vector3 _diffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
        private static Vector3 _direction = new Vector3(0.8f, -0.2f, -1);
        private static Vector3 _specularColor = new Vector3(0.7f, 0.7f, 0.7f);


        public static void DrawGlobalLightning(Effect effect)
        {
            if (!ToylandSiege.GetInstance().configurationManager.LigthningEnabled)
                return;
            if (effect is BasicEffect)
            {
                (effect as BasicEffect).DirectionalLight0.DiffuseColor = _diffuseColor;
                (effect as BasicEffect).DirectionalLight0.Direction = _direction;
                (effect as BasicEffect).DirectionalLight0.SpecularColor = _specularColor;

            }
            else if (effect is SkinnedEffect)
            {
                (effect as SkinnedEffect).DirectionalLight0.DiffuseColor = _diffuseColor;
                (effect as SkinnedEffect).DirectionalLight0.Direction = _direction;
                (effect as SkinnedEffect).DirectionalLight0.SpecularColor = _specularColor;
            }
            else
            {
                Logger.Log.Debug("Unsupported effect: " + effect);
            }
        }
    }
}
