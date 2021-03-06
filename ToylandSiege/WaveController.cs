﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using Microsoft.Xna.Framework;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class WaveController
    {
        public List<Wave> Waves = new List<Wave>();
        public Wave CurrentWave { get; private set; }

        public static bool RoundRunning = false;
        public GameTime gameTime;

        public static int WawesCount;
        public static int RoundNumber;
        public WaveController()
        {
            WawesCount = 0;
            RoundNumber = 0;
        }

        public void Update(GameTime gameTime)
        {
            try
            {
                this.gameTime = gameTime;

                if (CurrentWave != null && RoundRunning)
                {
                    if (!ToylandSiege.GetInstance().gameStateManager.IsPaused())
                    {
                        CurrentWave.TimeLeft -= gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    CurrentWave.RefreshLists();
                    if (CurrentWave.TimeLeft <= 0.0 || CurrentWave.UnitsInWave.Count == 0 || CurrentWave.UnitsInWave.All(unit => unit.Field.FinishingTile))
                    {
                        FinishRound();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public void SetCurrentWave(Wave wave)
        {
            CurrentWave = wave;
        }

        public void AddWave(Wave wave)
        {
            WawesCount++;
            Waves.Add(wave);
        }

        public void StartRound()
        {
            RoundNumber++;
            RoundRunning = true;
            CurrentWave.WaveStartedTime = gameTime.TotalGameTime;
            Camera.SetCurrentCamera(Camera.AvailableCameras["FirstPersonCamera"]);
            ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["FirstPerson"]);
        }

        public void FinishRound()
        {
            RoundRunning = false;
            Camera.SetCurrentCamera(Camera.AvailableCameras["StrategicCamera"]);

            CurrentWave.RefreshLists();
            //Should add units which are alive to next wave?
            List<Unit> AliveUnits = new List<Unit>();
            CurrentWave.UnitsInWave.ForEach(unit => AliveUnits.Add(unit));
            CurrentWave.AvailableUnits.ForEach(unit => AliveUnits.Add(unit));

            //Remove Units in Wave
            foreach (Unit unit in CurrentWave.UnitsInWave)
            {
                unit.Field.unit = null;
                unit.Field = null;
                unit.TargetFields.Clear();
                unit.FieldsInWay.Clear();

                Level.GetCurrentLevel().RootGameObject.Childs["Units"].RemoveChild(unit);
            }

            //Remove Wave
            Waves.Remove(CurrentWave);
            CurrentWave = Waves.FirstOrDefault();

            //TODO: Finish game here
            //Maybe throw exception to toyland siege Update method level??
            if (CurrentWave == null && Waves.Count == 0)
            {
                ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["Menu"]);
                return;
            }

            foreach (Unit unit in AliveUnits)
            {
                CurrentWave.AvailableUnits.Add(unit);
            }


            //Set IsPartOfWay to false for each field in board
            foreach (var rows in Level.GetCurrentLevel().RootGameObject.Childs["Board"].Childs.Values)
                foreach (var field in rows.Childs.Values)
                    if (field is Field)
                        (field as Field).IsPartOfWay = false;


            ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["Strategic"]);
        }
    }
}
