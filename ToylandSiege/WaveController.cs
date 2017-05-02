using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public WaveController()
        {
            
        }

        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            if (CurrentWave != null)
            {
                CurrentWave.TimeLeft = CurrentWave.WaveTime - (gameTime.TotalGameTime - CurrentWave.WaveStartedTime).TotalSeconds;
            }

            if (RoundRunning)
            {
                if(CurrentWave.TimeLeft <= 0.0)
                {
                    FinishRound();
                }
            }
        }

        public void SetCurrentWave(Wave wave)
        {
            CurrentWave = wave;
        }

        public void AddWave(Wave wave)
        {
            Waves.Add(wave);
        }

        public void StartRound()
        {
            RoundRunning = true;
            CurrentWave.WaveStartedTime = gameTime.TotalGameTime;
            Camera.SetCurrentCamera(Camera.AvailableCameras["FirstPersonCamera"]);
            ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["FirstPerson"]);
        }

        public void FinishRound()
        {
            RoundRunning = false;
            Camera.SetCurrentCamera(Camera.AvailableCameras["StrategicCamera"]);
            ToylandSiege.GetInstance().gameStateManager.SetNewGameState(ToylandSiege.GetInstance().gameStateManager.AvailableGameStates["Strategic"]);

            //Remove Wave
            Waves.Remove(CurrentWave);
            CurrentWave = Waves.FirstOrDefault();
            //TODO: Finish game here
            //Maybe throw exception to toyland siege Update method level??
            if(CurrentWave == null && Waves.Count == 0)
                throw new TimeoutException("Game Finished!");
        }
    }
}
