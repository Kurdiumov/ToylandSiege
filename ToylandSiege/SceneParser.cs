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
            string Type = JSONHelper.GetValue("Type", currentGameObject);

            switch (Type)
            {
                case "TerrainObject":
                    return ParseTerrainObject(currentGameObject, parent);
                case "Camera":
                    return ParseCameraObject(currentGameObject, parent);
                case "Unit":
                case "Enemy":
                    return ParseUnitObject(currentGameObject, parent);
                case "Group":
                    return ParseGroupObject(currentGameObject, parent);
                default:
                    throw new InvalidEnumArgumentException("Unknown type " + Type);
            }
        }

        private TerrainObject ParseTerrainObject(JObject currentGameObject, GameObject parent = null)
        {
            var name = JSONHelper.GetValue("Name", currentGameObject);
            var model = JSONHelper.GetValue("Model", currentGameObject);
            var IsEnabled = JSONHelper.ToBool(JSONHelper.GetValue("isEnabled", currentGameObject));


            var mod = ToylandSiege.GetToylandSiege().Content.Load<Model>(model);
            var terrainObj = new TerrainObject(name, mod);
            terrainObj.IsEnabled = IsEnabled;
            terrainObj.IsStatic = true;
            terrainObj.Type = JSONHelper.GetValue("Type", currentGameObject);
            terrainObj.Parent = parent;

            if (JSONHelper.ValueExist("Position", currentGameObject))
                terrainObj.Position = JSONHelper.ParseVector3(currentGameObject.GetValue("Position"));

            if (JSONHelper.ValueExist("Rotation", currentGameObject))
                terrainObj.Rotation = JSONHelper.ParseVector3(currentGameObject.GetValue("Rotation"));

            if (JSONHelper.ValueExist("Scale", currentGameObject))
                terrainObj.Scale = JSONHelper.ParseVector3(currentGameObject.GetValue("Scale"));
            terrainObj.CreateTransformationMatrix();

            if (JSONHelper.ValueExist("Child", currentGameObject))
            {  for (int i = 0; i < currentGameObject.GetValue("Child").Count(); i++)
                {
                    terrainObj.AddChild(ParseGameObject(currentGameObject.GetValue("Child")[i].ToObject<JObject>(), terrainObj));    
                }
            }
            
            return terrainObj;
        }

        private Camera ParseCameraObject(JObject currentGameObject, GameObject parent = null)
        {
            var name = JSONHelper.GetValue("Name", currentGameObject);
            var camera = new Camera(name);

            camera.Type  = JSONHelper.GetValue("Type", currentGameObject);
            camera.Model = null;

            if (JSONHelper.ToBool(JSONHelper.GetValue("CurrentCamera", currentGameObject)))
                Camera.SetCurrentCamera(camera);

            camera.IsEnabled = JSONHelper.ToBool(JSONHelper.GetValue("isEnabled", currentGameObject));
            camera.IsCollidable = false;


            //Camera position
            if (JSONHelper.ValueExist("Position", currentGameObject))
                camera.Position = JSONHelper.ParseVector3(currentGameObject.GetValue("Position"));

            //Camera Direction
            if (JSONHelper.ValueExist("Direction", currentGameObject))
                camera.Direction = JSONHelper.ParseVector3(currentGameObject.GetValue("Direction"));

            //Camera Up Vector
            if (JSONHelper.ValueExist("Up", currentGameObject))
                camera.Up = JSONHelper.ParseVector3(currentGameObject.GetValue("UpVector"));

            //Camera Speed
            if (JSONHelper.ValueExist("Speed", currentGameObject))
                camera.Speed = float.Parse(JSONHelper.GetValue("Speed", currentGameObject ));

            //Projection Matrix
            float NearDistance = float.Parse(JSONHelper.GetValue("NearPlaneDistance", currentGameObject));
            float FarDistance = float.Parse(JSONHelper.GetValue("FarPlaneDistance", currentGameObject));
            float Angle = float.Parse(JSONHelper.GetValue("Angle", currentGameObject));

            camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Angle),
                ToylandSiege.GetToylandSiege().GraphicsDevice.Viewport.AspectRatio, NearDistance, FarDistance);


            return camera;
        }

        private UnitBase ParseUnitObject(JObject currentGameObject, GameObject parent = null)
        {
            var name = JSONHelper.GetValue("Name", currentGameObject);
            var model = JSONHelper.GetValue("Model", currentGameObject);
            var type = JSONHelper.GetValue("Type", currentGameObject);
            var IsEnabled = JSONHelper.ToBool(JSONHelper.GetValue("isEnabled", currentGameObject));
            var health =float.Parse(JSONHelper.GetValue("Health", currentGameObject));
            var unitType = JSONHelper.GetValue("UnitType", currentGameObject);

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

            unitObj.IsStatic = false;
            unitObj.Parent = parent;

            if (JSONHelper.ValueExist("Position", currentGameObject))
                unitObj.Position = JSONHelper.ParseVector3(currentGameObject.GetValue("Position"));

            if (JSONHelper.ValueExist("Rotation", currentGameObject))
                unitObj.Rotation = JSONHelper.ParseVector3(currentGameObject.GetValue("Rotation"));

            if (JSONHelper.ValueExist("Scale", currentGameObject))
                unitObj.Scale = JSONHelper.ParseVector3(currentGameObject.GetValue("Scale"));
            unitObj.CreateTransformationMatrix();

            if (JSONHelper.ValueExist("Child", currentGameObject))
            {
                for (int i = 0; i < currentGameObject.GetValue("Child").Count(); i++)
                {
                    unitObj.AddChild(ParseGameObject(currentGameObject.GetValue("Child")[i].ToObject<JObject>(), unitObj));
                }
            }

            // TODO: Load from a config file?
            unitObj.BType = GameObject.BoundingType.Sphere;
            unitObj.CreateBoundingSphereForModel();

            return unitObj;
        }

        private Group ParseGroupObject(JObject currentGameObject, GameObject parent = null)
        {
            var name = JSONHelper.GetValue("Name", currentGameObject);
            var IsEnabled = JSONHelper.ToBool(JSONHelper.GetValue("isEnabled", currentGameObject));


            var terrainObj = new Group(name);
            terrainObj.IsEnabled = IsEnabled;
            terrainObj.IsStatic = true;
            terrainObj.Type = JSONHelper.GetValue("Type", currentGameObject);
            terrainObj.Parent = parent;
             
            if (JSONHelper.ValueExist("Position", currentGameObject))
                terrainObj.Position = JSONHelper.ParseVector3(currentGameObject.GetValue("Position"));

            if (JSONHelper.ValueExist("Rotation", currentGameObject))
                terrainObj.Rotation = JSONHelper.ParseVector3(currentGameObject.GetValue("Rotation"));

            if (JSONHelper.ValueExist("Scale", currentGameObject))
                terrainObj.Scale = JSONHelper.ParseVector3(currentGameObject.GetValue("Scale"));
            terrainObj.CreateTransformationMatrix();

            if (JSONHelper.ValueExist("Child", currentGameObject))
            {
                for (int i = 0; i < currentGameObject.GetValue("Child").Count(); i++)
                {
                    terrainObj.AddChild(ParseGameObject(currentGameObject.GetValue("Child")[i].ToObject<JObject>(), terrainObj));
                }
            }

            return terrainObj;
        }

    }
}