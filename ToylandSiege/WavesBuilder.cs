
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
        public void Build(WaveController WaveController)
        {
            WaveController.AddWave(BuildWave(3, 120));
            WaveController.AddWave(BuildWave(5, 100));
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
                wave.AddUnit(new Unit()
                {
                    Name = "Unit"+i,
                    Model = ToylandSiege.GetInstance().Content.Load<Model>("1234"),
                    Type = "Unit",
                    IsEnabled = false,
                    Health = 100,
                    UnitType = "BasicType",
                    AnimationPlayer = new AnimationPlayer(ToylandSiege.GetInstance().Content.Load<Model>("1234").Tag as SkinningData),
                    Clips = new Dictionary<string, AnimationClip>() { {"Take 001", (ToylandSiege.GetInstance().Content.Load<Model>("1234").Tag as SkinningData).AnimationClips["Take 001"]} },
                    Scale = new Vector3(0.1f,0.1f,0.1f)
            });
                wave.AvailableUnits.Last().AnimationPlayer.StartClip(wave.AvailableUnits.Last().Clips.First().Value);
            }

            return wave;
        }
    }
}
