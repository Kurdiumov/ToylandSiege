using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ToylandSiege
{
    public static class GlobalLightning
    {
        public static Vector3 DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
        public static Vector3 Direction = new Vector3(0.8f, -0.2f, -1);
        public static Vector3 SpecularColor = new Vector3(0.7f, 0.7f, 0.7f);

        public static Matrix LightView;
        public static Matrix LightProjection;
        public static Matrix LightViewProjection;

        public static void DrawGlobalLightning(Effect effect)
        {
            LightView = Matrix.CreateLookAt(Vector3.Zero,
                        Vector3.Zero + GlobalLightning.Direction,
                        Vector3.Up);

            LightProjection = Matrix.CreateOrthographic(1366, 768, 1, 1000);
            LightViewProjection = LightView * LightProjection;

            if (!ToylandSiege.GetInstance().configurationManager.LigthningEnabled)
                return;
            if (effect is BasicEffect)
            {
                (effect as BasicEffect).DirectionalLight0.DiffuseColor = DiffuseColor;
                (effect as BasicEffect).DirectionalLight0.Direction = Direction;
                (effect as BasicEffect).DirectionalLight0.SpecularColor = SpecularColor;

            }
            else if (effect is SkinnedEffect)
            {
                (effect as SkinnedEffect).DirectionalLight0.DiffuseColor = DiffuseColor;
                (effect as SkinnedEffect).DirectionalLight0.Direction = Direction;
                (effect as SkinnedEffect).DirectionalLight0.SpecularColor = SpecularColor;
            }
            else
            {
                Logger.Log.Debug("Unsupported effect: " + effect);
            }
        }
    }
}
