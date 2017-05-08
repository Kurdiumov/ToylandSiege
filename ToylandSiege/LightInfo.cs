using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ToylandSiege
{
    public class LightInfo
    {
        //Ambient
        public Vector4 AmbientColor = new Vector4(1, 1, 1, 1);
        public float AmbientIntensity = 0.2f;

        //Diffuse
        public Vector3 DiffuseLightDirection = new Vector3(0, 0, -1);
        public Vector4 DiffuseColor = new Vector4(1, 1, 1, 1);
        public float DiffuseIntensity = 1.0f;

        //Specular
        public  float Shininess = 0;
        public Vector4 SpecularColor = new Vector4(1, 1, 1, 1);
        public float SpecularIntensity = 0;
        public Vector3 ViewVector = new Vector3(0, 0, -1);
    }
}
