using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class SceneParser
    {
        private readonly string _filePath = "Scene.json";

        public SceneParser()
        {
        }

        public GameObject Parse(string LevelName)
        {
            Logger.Log.Debug("Parsing " + LevelName + " from " + _filePath + " file");
            RootGameObject RootObject = new RootGameObject();

            JObject LevelObject = JObject.Parse(File.ReadAllText(_filePath)).GetValue(LevelName).ToObject<JObject>();

            if (LevelObject == null)
                throw new ArgumentNullException(_filePath + " does not containt key " + LevelName);

            RootObject.Name = LevelObject.GetValue("Name").ToString();
            RootObject.Type = LevelObject.GetValue("Type").ToString();



            for (int i = 0; i < LevelObject.GetValue("Child").Count(); i++)
            {
                RootObject.AddChild(ParseGameObject(LevelObject.GetValue("Child")[i] as JObject));
            }

            Logger.Log.Debug("File " + _filePath + "parsed successfully");
            return RootObject;
        }

        private GameObject ParseGameObject(JObject currentGameObject)
        {
            string Type = GetValue("Type", currentGameObject);

            switch (Type)
            {
                case "TerrainObject":
                    return ParseTerrainObject(currentGameObject);
                case "Camera":
                    return ParseCameraObject(currentGameObject);
                default:
                    throw new InvalidEnumArgumentException("Unknown type " + Type);
            }
        }

        private TerrainObject ParseTerrainObject(JObject currentGameObject)
        {
            var name = GetValue("Name", currentGameObject);
            var model = GetValue("Model", currentGameObject);
            var IsEnabled = ToBool(GetValue("isEnabled", currentGameObject));

            var mod = ToylandSiege.GetToylandSiege().Content.Load<Model>(model);
            var terrainObj = new TerrainObject(name, mod);
            terrainObj.IsEnabled = IsEnabled;
            terrainObj.IsStatic = true;
            terrainObj.Type = GetValue("Type", currentGameObject);
            

            if (ValueExist("Position", currentGameObject))
                terrainObj.Position = ParseVector3(currentGameObject.GetValue("Position"));

            if (ValueExist("Rotation", currentGameObject))
                terrainObj.Rotation = ParseVector3(currentGameObject.GetValue("Rotation"));

            if (ValueExist("Scale", currentGameObject))
                terrainObj.Scale = ParseVector3(currentGameObject.GetValue("Scale"));

            if (ValueExist("Child", currentGameObject))
            {  for (int i = 0; i < currentGameObject.GetValue("Child").Count(); i++)
                {
                    terrainObj.AddChild(ParseGameObject(currentGameObject.GetValue("Child")[i].ToObject<JObject>()));    
                }
            }

            return terrainObj;
        }

        private Camera ParseCameraObject(JObject currentGameObject)
        {
            var name = GetValue("Name", currentGameObject);
            var camera = new Camera(name);

            camera.Type  = GetValue("Type", currentGameObject);
            camera.Model = null;

            if (ToBool(GetValue("CurrentCamera", currentGameObject)))
                Camera.SetCurrentCamera(camera);

            camera.IsEnabled = ToBool(GetValue("isEnabled", currentGameObject));

            //Camera target
            if (ValueExist("CameraTarget", currentGameObject))
                camera.CamTarget = ParseVector3(currentGameObject.GetValue("CameraTarget"));

            //Camera position
            if (ValueExist("Position", currentGameObject))
                camera.Position = ParseVector3(currentGameObject.GetValue("Position"));
            
            //Projection Matrix
            float NearDistance = float.Parse(GetValue("NearPlaneDistance", currentGameObject));
            float FarDistance = float.Parse(GetValue("FarPlaneDistance", currentGameObject));
            float Angle = float.Parse(GetValue("Angle", currentGameObject));

            camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Angle),
                ToylandSiege.GetToylandSiege().GraphicsDevice.Viewport.AspectRatio, NearDistance, FarDistance);

            //View matrix
            if (ValueExist("ViewMatrix", currentGameObject))
            {
                camera.ViewMatrix = Matrix.CreateLookAt(camera.Position, camera.CamTarget,
                    ParseVector3(currentGameObject.GetValue("ViewMatrix")));
            }

            //WorldMatrix
            camera.WorldMatrix = Matrix.CreateWorld(camera.CamTarget, Vector3.Forward, Vector3.Up);

            return camera;
        }

        #region Helpers

        private Vector3 ParseVector3(JToken currentObject)
        {
            List<float> arr = new List<float>();
            foreach (var item in currentObject)
            {

                float result = 0;
                if (!float.TryParse(item.ToString(), out result))
                {
                    Logger.Log.Error("Error while parsing " + currentObject.ToString() + " (" + item.ToString() + ") Replacing value with 0");
                }
                arr.Add(result);
            }

            if (arr.Count == 3)
            {
                return new Vector3(arr[0], arr[1], arr[2]);
            }
            throw new ArgumentException("Arguments count should equal 3");
        }

        private string GetValue(string value, JObject obj)
        {
            if (ValueExist(value, obj))
                return obj.GetValue(value).ToString();
            throw new ArgumentException("Value " + value + " does not exist in current context. " + obj.ToString());
        }

        private bool ValueExist(string value, JObject obj)
        {
            try
            {
                var exists = !String.IsNullOrEmpty(obj.GetValue(value).ToString());
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool ToBool(string value)
        {
            if (value == "True")
                return true;
            return false;
        }
        #endregion
    }
}