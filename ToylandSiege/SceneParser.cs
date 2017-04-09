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

        private GameObject ParseGameObject(JObject currentGameObject, GameObject parent = null)
        {
            string Type = GetValue("Type", currentGameObject);

            switch (Type)
            {
                case "TerrainObject":
                    return ParseTerrainObject(currentGameObject, parent);
                case "Camera":
                    return ParseCameraObject(currentGameObject, parent);
                case "Unit":
                case "Enemy":
                    return ParseUnitObject(currentGameObject, parent);
                default:
                    throw new InvalidEnumArgumentException("Unknown type " + Type);
            }
        }

        private TerrainObject ParseTerrainObject(JObject currentGameObject, GameObject parent = null)
        {
            var name = GetValue("Name", currentGameObject);
            var model = GetValue("Model", currentGameObject);
            var IsEnabled = ToBool(GetValue("isEnabled", currentGameObject));

            var mod = ToylandSiege.GetToylandSiege().Content.Load<Model>(model);
            var terrainObj = new TerrainObject(name, mod);
            terrainObj.IsEnabled = IsEnabled;
            terrainObj.IsStatic = true;
            terrainObj.Type = GetValue("Type", currentGameObject);
            terrainObj.Parent = parent;

            if (ValueExist("Position", currentGameObject))
                terrainObj.Position = ParseVector3(currentGameObject.GetValue("Position"));

            if (ValueExist("Rotation", currentGameObject))
                terrainObj.Rotation = ParseVector3(currentGameObject.GetValue("Rotation"));

            if (ValueExist("Scale", currentGameObject))
                terrainObj.Scale = ParseVector3(currentGameObject.GetValue("Scale"));
            terrainObj.CreateTransformationMatrix();

            if (ValueExist("Child", currentGameObject))
            {  for (int i = 0; i < currentGameObject.GetValue("Child").Count(); i++)
                {
                    terrainObj.AddChild(ParseGameObject(currentGameObject.GetValue("Child")[i].ToObject<JObject>(), terrainObj));    
                }
            }
            
            return terrainObj;
        }

        private Camera ParseCameraObject(JObject currentGameObject, GameObject parent = null)
        {
            var name = GetValue("Name", currentGameObject);
            var camera = new Camera(name);

            camera.Type  = GetValue("Type", currentGameObject);
            camera.Model = null;

            if (ToBool(GetValue("CurrentCamera", currentGameObject)))
                Camera.SetCurrentCamera(camera);

            camera.IsEnabled = ToBool(GetValue("isEnabled", currentGameObject));


            //Camera position
            if (ValueExist("Position", currentGameObject))
                camera.Position = ParseVector3(currentGameObject.GetValue("Position"));

            //Camera Direction
            if (ValueExist("Direction", currentGameObject))
                camera.Direction = ParseVector3(currentGameObject.GetValue("Direction"));

            //Camera Up Vector
            if (ValueExist("Up", currentGameObject))
                camera.Up = ParseVector3(currentGameObject.GetValue("UpVector"));

            //Camera Speed
            if (ValueExist("Speed", currentGameObject))
                camera.Speed = float.Parse(GetValue("Speed", currentGameObject ));

            //Projection Matrix
            float NearDistance = float.Parse(GetValue("NearPlaneDistance", currentGameObject));
            float FarDistance = float.Parse(GetValue("FarPlaneDistance", currentGameObject));
            float Angle = float.Parse(GetValue("Angle", currentGameObject));

            camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Angle),
                ToylandSiege.GetToylandSiege().GraphicsDevice.Viewport.AspectRatio, NearDistance, FarDistance);


            return camera;
        }

        private UnitBase ParseUnitObject(JObject currentGameObject, GameObject parent = null)
        {
            var name = GetValue("Name", currentGameObject);
            var model = GetValue("Model", currentGameObject);
            var type = GetValue("Type", currentGameObject);
            var IsEnabled = ToBool(GetValue("isEnabled", currentGameObject));
            var health =float.Parse(GetValue("Health", currentGameObject));
            var unitType = GetValue("UnitType", currentGameObject);

            var mod = ToylandSiege.GetToylandSiege().Content.Load<Model>(model);

            UnitBase unitObj;
            if (type == "Unit")
                unitObj = new Unit();
            else if (type == "Enemy")
                unitObj = new Enemy();
            else
                throw new ArgumentException("Unknown unit type " + type);

            unitObj.Model = mod;
            unitObj.Name = name;
            unitObj.Type = type;
            unitObj.IsEnabled = IsEnabled;
            unitObj.Health = health;
            unitObj.UnitType = unitType;

            unitObj.IsStatic = true;
            unitObj.Parent = parent;

            if (ValueExist("Position", currentGameObject))
                unitObj.Position = ParseVector3(currentGameObject.GetValue("Position"));

            if (ValueExist("Rotation", currentGameObject))
                unitObj.Rotation = ParseVector3(currentGameObject.GetValue("Rotation"));

            if (ValueExist("Scale", currentGameObject))
                unitObj.Scale = ParseVector3(currentGameObject.GetValue("Scale"));
            unitObj.CreateTransformationMatrix();

            if (ValueExist("Child", currentGameObject))
            {
                for (int i = 0; i < currentGameObject.GetValue("Child").Count(); i++)
                {
                    unitObj.AddChild(ParseGameObject(currentGameObject.GetValue("Child")[i].ToObject<JObject>(), unitObj));
                }
            }

            return unitObj;
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