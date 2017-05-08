
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class WavesBuilder
    {
        public void BuildTutorial(WaveController WaveController)
        {
            Dictionary<string, string> Wave1 = new Dictionary<string, string>();
            Wave1.Add("Unit1", "Soldier");
            WaveController.AddWave(BuildWave(Wave1, 120));

            Dictionary<string, string> Wave2 = new Dictionary<string, string>();
            Wave2.Add("Unit2", "Scout");
            WaveController.AddWave(BuildWave(Wave2, 120));

            Dictionary<string, string> Wave3 = new Dictionary<string, string>();
            Wave3.Add("Unit3", "Tank");

            WaveController.AddWave(BuildWave(Wave3, 120));

            WaveController.SetCurrentWave(WaveController.Waves.First());
        }


        public void BuildLevel1(WaveController WaveController)
        {
            Dictionary<string, string> Wave1 = new Dictionary<string, string>();
            Wave1.Add("Unit1", "Soldier");
            Wave1.Add("Unit2", "Tank");
            Wave1.Add("Unit3", "Scout");
            WaveController.AddWave(BuildWave(Wave1, 120));

            Dictionary<string, string> Wave2 = new Dictionary<string, string>();
            Wave2.Add("Unit4", "Soldier");
            Wave2.Add("Unit5", "Soldier");
            Wave2.Add("Unit6", "Soldier");
            Wave2.Add("Unit7", "Scout");
            WaveController.AddWave(BuildWave(Wave2, 120));

            Dictionary<string, string> Wave3 = new Dictionary<string, string>();
            Wave3.Add("Unit8", "Tank");
            Wave3.Add("Unit9", "Scout");
            Wave3.Add("Unit10", "Tank");
            WaveController.AddWave(BuildWave(Wave3, 120));

            WaveController.SetCurrentWave(WaveController.Waves.First());
        }

        public Wave BuildWave(Dictionary<string, string> units, double time)
        {
            Wave wave = new Wave();

            wave.WaveTime = time;
            foreach (var unit in units)
                wave.AddUnit(UnitBuilder.BuildUnit(unit.Key, unit.Value));
            
            return wave;
        }
    }
}
