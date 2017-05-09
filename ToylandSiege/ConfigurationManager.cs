using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using ToylandSiege.GameState;

namespace ToylandSiege
{
    public class ConfigurationManager
    {
        public readonly string PathToConfigFile = "application.config.json";
        private readonly JObject Configuration;

        public bool GodModeEnabled;
        public bool IsFullScreen;
        public bool FPSEnabled;
        public bool MouseVisible;
        public bool PickingEnabled;
        public bool LigthningEnabled;
        public bool DebugDraw;

        public int HeightResolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        public int WidthResolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

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
            IsFullScreenEnabled();
            GetResolution();
            IsFPSEnabled();
            IsMouseVisible();
            IsPickingEnabled();
            IsLightningEnabled();
            IsDebugDrawEnabled();            
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
        
        public void InitGameStates()
        {
            var gameStateManager = ToylandSiege.GetInstance().gameStateManager;

            for (int i = 0; i < Configuration.GetValue("GameStates").Count(); i++)
            {
                var state = Configuration.GetValue("GameStates")[i].Value<string>();

                switch (state.ToLower())
                {
                    case "godmode":
                        if (!GodModeEnabled)
                            throw new ArgumentException("GodMode is not Enabled in configuration file!");
                        gameStateManager.AddGameState("GodMode", new GodMode());
                        Logger.Log.Debug("GameState added:  GodMode");
                        break;
                    case "firstperson":
                        gameStateManager.AddGameState("FirstPerson", new FirstPerson());
                        Logger.Log.Debug("GameState added:  FirstPerson");
                        break;
                    case "strategic":
                        gameStateManager.AddGameState("Strategic", new Strategic());
                        Logger.Log.Debug("GameState added:  Strategic");
                        break;
                    case "menu":
                        gameStateManager.AddGameState("Menu", new Menu());
                        Logger.Log.Debug("GameState added:  Menu");
                        break;
                    case "paused":
                        gameStateManager.AddGameState("Paused", new Paused());
                        Logger.Log.Debug("GameState added:  Paused");
                        break;
                    default:
                        throw new ArgumentException("Not supported game state " + state);
                }
            }

            //Set starting game state
            if (JSONHelper.ValueExist("StartingGameState", Configuration))
                gameStateManager.SetNewGameState(gameStateManager.AvailableGameStates[JSONHelper.GetValue("StartingGameState", Configuration)]);
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

        public bool IsDebugDrawEnabled()
        {
            if (JSONHelper.ValueExist("DebugDraw", Configuration))
                DebugDraw = JSONHelper.ToBool(JSONHelper.GetValue("DebugDraw", Configuration));

            if (DebugDraw)
                Logger.Log.Debug("Debug draw Enabled");
            else
                Logger.Log.Debug("Debug Draw Disabled");

            return DebugDraw;
        }
    }
}
