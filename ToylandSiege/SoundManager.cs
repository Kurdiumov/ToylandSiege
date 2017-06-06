using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace ToylandSiege
{
    //TODO: Calculate Sound Volume relying on camera postion and distance
    public class SoundManager
    {
        public static Dictionary<string, SoundEffect > LoadedSounds = new Dictionary<string, SoundEffect>();

        public void Initialize()
        {
            LoadedSounds.Add("SniperShootSound", ToylandSiege.GetInstance().Content.Load<SoundEffect>("Sounds/SniperShootSound"));
            LoadedSounds.Add("DefenderShootSound", ToylandSiege.GetInstance().Content.Load<SoundEffect>("Sounds/DefenderShootSound"));
            LoadedSounds.Add("StandartShootSound", ToylandSiege.GetInstance().Content.Load<SoundEffect>("Sounds/StandartShootSound"));
            LoadedSounds.Add("SoldierShootSound", ToylandSiege.GetInstance().Content.Load<SoundEffect>("Sounds/SoldierShootSound"));
            LoadedSounds.Add("TankShootSound", ToylandSiege.GetInstance().Content.Load<SoundEffect>("Sounds/TankShootSound"));
            LoadedSounds.Add("ScoutShootSound", ToylandSiege.GetInstance().Content.Load<SoundEffect>("Sounds/ScoutShootSound"));

            LoadedSounds.Add("EnemyDeathSound", ToylandSiege.GetInstance().Content.Load<SoundEffect>("Sounds/EnemyDeathSound"));
            LoadedSounds.Add("UnitDeathSound", ToylandSiege.GetInstance().Content.Load<SoundEffect>("Sounds/UnitDeathSound"));

        }

        public static SoundEffect Get(string soundName)
        {
            return LoadedSounds[soundName];
        }

        public static void PlaySound(string soundName, float Volume = 1)
        {
            PlaySound(LoadedSounds[soundName], Volume);
        }

        public static void PlaySound(SoundEffect soundEffect, float Volume = 1)
        {
            if (soundEffect == null)
            {
                throw new ArgumentNullException(
                    "SoundEffect",
                    "You cannot play a sound effect that has nothing loaded!");
            }
            var SoundEffectInstance = soundEffect.CreateInstance();
            SoundEffectInstance.IsLooped = false;
            SoundEffectInstance.Volume = Volume;
            SoundEffectInstance.Play();
        }
        
        public static void PlayLoopSound(SoundEffect soundEffect, float Volume)
        {
            if (soundEffect == null)
            {
                throw new ArgumentNullException(
                    "SoundEffect",
                    "You cannot play a sound effect that has nothing loaded!");
            }

            var SoundEffectInstance = soundEffect.CreateInstance();
            SoundEffectInstance.IsLooped = true;
            SoundEffectInstance.Volume = Volume;
            SoundEffectInstance.Play();
        }
    }
}
