
using System;
using System.Linq;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class WavesBuilder
    {
        public void Build(WaveController WaveController)
        {
            WaveController.AddWave(BuildWave(4, 120));
            WaveController.AddWave(BuildWave(3, 100));
            WaveController.AddWave(BuildWave(3, 80));
            WaveController.AddWave(BuildWave(6, 150));

            WaveController.SetCurrentWave(WaveController.Waves.First());
        }

        public Wave BuildWave(uint units, double time)
        {
            Wave wave = new Wave();

            wave.WaveTime = time;
            for (int i = 0; i < units; i++)
            {
                wave.AddUnit(new Unit());
            }

            return wave;
        }
    }
}
