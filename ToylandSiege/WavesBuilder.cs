
using System;
using System.Linq;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class WavesBuilder
    {
        public void Build()
        {
            ToylandSiege.waveController.AddWave(BuildWave(4, 120));
            ToylandSiege.waveController.AddWave(BuildWave(3, 100));
            ToylandSiege.waveController.AddWave(BuildWave(3, 80));
            ToylandSiege.waveController.AddWave(BuildWave(6, 150));

            ToylandSiege.waveController.SetCurrentWave(ToylandSiege.waveController.Waves.First());
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
