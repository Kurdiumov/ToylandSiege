using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ToylandSiege
{
    public class ConfigurationManager
    {
        public readonly string PathToConfigFile = "application.config.json";
        private readonly JObject Configuration;

        public bool GodModeEnabled;
        public  State GameState = State.FirstPerson;
        public bool IsFullScreen;
        public bool FPSEnabled;
        public bool MouseVisible;
        public bool PickingEnabled;
        public bool LigthningEnabled;

        public int HeightResolution = 600;
        public int WidthResolution = 800;

        public ConfigurationManager()
        {
            Logger.Log.Debug("Parsing configuration from " + PathToConfigFile + " file");

            Configuration = JObject.Parse(File.ReadAllText(PathToConfigFile)).GetValue("config").ToObject<JObject>();

            if (Configuration == null)
                throw new ArgumentNullException(PathToConfigFile + " does not contain configuration key");

            ReadAllProperties();
        }

        private void ReadAllProperties()
        {
            IsGodModeEnabled();
            GetGameState();
            IsFullScreenEnabled();
            GetResolution();
            IsFPSEnabled();
            IsMouseVisible();
            IsPickingEnabled();
            IsLightningEnabled();
        }

        public bool IsGodModeEnabled()
        {
            if (JSONHelper.ValueExist("GodModeEnabled", Configuration))
                GodModeEnabled = JSONHelper.ToBool(JSONHelper.GetValue("GodModeEnabled", Configuration));

            if(GodModeEnabled)
                Logger.Log.Debug("GodMode Enabled");
            else
                Logger.Log.Debug("GodMode Disabled");
            
            return GodModeEnabled;
        }

        public bool IsFullScreenEnabled()
        {
            if (JSONHelper.ValueExist("IsFullScreen", Configuration))
                IsFullScreen = JSONHelper.ToBool(JSONHelper.GetValue("IsFullScreen", Configuration));

            if (IsFullScreen)
                Logger.Log.Debug("FullScreen Enabled");
            else
                Logger.Log.Debug("FullScreen Disabled");

            return IsFullScreen;
        }

        public State GetGameState()
        {
            if (!JSONHelper.ValueExist("GameState", Configuration))
                return State.FirstPerson;

            var state = JSONHelper.GetValue("GameState", Configuration);

            switch (state.ToLower())
            {
                case "godmode":
                    if (!GodModeEnabled)
                        throw new ArgumentException("GodMode is not Enabled in configuration file!");
                    GameState = State.GodMode;
                    Logger.Log.Debug("Starting GameState:  GodMode");
                    break;
                case "firstperson":
                    GameState = State.FirstPerson;
                    Logger.Log.Debug("Starting GameState:  FirstPerson");
                    break;
                case "strategic":
                    GameState = State.Strategic;
                    Logger.Log.Debug("Starting GameState:  Strategic");
                    break;
                case "menu":
                    GameState = State.Menu;
                    Logger.Log.Debug("Starting GameState:  Menu");
                    break;
                case "paused":
                    GameState = State.Paused;
                    Logger.Log.Debug("Starting GameState:  Paused");
                    break;
                default:
                    throw new ArgumentException("Not supported game state " + state);
            }
            return GameState;
        }

        private void GetResolution()
        {
            if (JSONHelper.ValueExist("HeightResolution", Configuration))
                HeightResolution = Int32.Parse(JSONHelper.GetValue("HeightResolution", Configuration));

            if (JSONHelper.ValueExist("WidthResolution", Configuration))
                WidthResolution = Int32.Parse(JSONHelper.GetValue("WidthResolution", Configuration));

            Logger.Log.Debug("Setting resolution " + WidthResolution + " x " + HeightResolution);
        }

        public bool IsFPSEnabled()
        {
            if (JSONHelper.ValueExist("FPSEnabled", Configuration))
                FPSEnabled = JSONHelper.ToBool(JSONHelper.GetValue("FPSEnabled", Configuration));

            if (FPSEnabled)
                Logger.Log.Debug("FPS Enabled");
            else
                Logger.Log.Debug("FPS Disabled");

            return FPSEnabled;
        }

        public bool IsMouseVisible()
        {
            if (JSONHelper.ValueExist("MouseVisible", Configuration))
                MouseVisible = JSONHelper.ToBool(JSONHelper.GetValue("MouseVisible", Configuration));

            if (MouseVisible)
                Logger.Log.Debug("Mouse visibility Enabled");
            else
                Logger.Log.Debug("Mouse visibility Disabled");

            return MouseVisible;
        }

        public bool IsPickingEnabled()
        {
            if (JSONHelper.ValueExist("RayPickingEnabled", Configuration))
                PickingEnabled = JSONHelper.ToBool(JSONHelper.GetValue("RayPickingEnabled", Configuration));

            if (PickingEnabled)
                Logger.Log.Debug("Picking Enabled");
            else
                Logger.Log.Debug("Picking Disabled");

            return FPSEnabled;
        }


        public bool IsLightningEnabled()
        {
            if (JSONHelper.ValueExist("LightningEnabled", Configuration))
                LigthningEnabled = JSONHelper.ToBool(JSONHelper.GetValue("LightningEnabled", Configuration));

            if (LigthningEnabled)
                Logger.Log.Debug("Lightning Enabled");
            else
                Logger.Log.Debug("Lightning Disabled");

            return LigthningEnabled;
        }

    }
    
}
