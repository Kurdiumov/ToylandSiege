using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class EnemyBuilder
    {
        public static Enemy BuildEnemy(string Name, string Type)
        {
            switch (Type.ToLower())
            {
                case "standart":
                    Spawner.EnemyCount++;
                    return _BuildStandart(Name);
                case "sniper":
                    Spawner.EnemyCount++;
                    return _BuildSniper(Name);
                case "defender":
                    Spawner.EnemyCount++;
                    return _BuildDefender(Name);
                default:
                    throw new ArgumentException("Unsupported unit type " + Type);
            }
        }

        private static Enemy _BuildStandart(string Name)
        {
            Enemy unit = new Enemy();
            unit.Name = Name;
            unit.Model = ToylandSiege.GetInstance().Content.Load<Model>("EnemyUnits/BlockmanDefender");
            unit.Type = "Enemy";
            unit.IsEnabled = false;
            unit.Health = 75;
            unit.MaxHealth = 75;
            unit.Damage = 25;
            unit.ShotDistance = 3;
            unit.TimeBetweeenShoots = 3;
            unit.UnitType = "Standart";
            
            var skinningData = ToylandSiege.GetInstance().Content.Load<Model>("EnemyUnits/BlockmanDefender").Tag as SkinningData;
            unit.AnimationPlayer = new AnimationPlayer(skinningData);
            unit.Clips = new Dictionary<string, AnimationClip>() { {"shoot", skinningData.AnimationClips["Take 001"]} };
            /*
            unit.Clips.Add("walking", skinningData.AnimationClips["walking"]);
            unit.Clips.Add("standing", new AnimationClip(skinningData.AnimationClips["walking"], skinningData.AnimationClips["standing"]));
            unit.Clips.Add("crouch", new AnimationClip(skinningData.AnimationClips["standing"], skinningData.AnimationClips["crouch"]));
            unit.Clips.Add("crouching", new AnimationClip(skinningData.AnimationClips["crouch"], skinningData.AnimationClips["crouching"]));
            unit.Clips.Add("standup", new AnimationClip(skinningData.AnimationClips["crouching"], skinningData.AnimationClips["standup"]));
            */
            unit.AnimationPlayer.StartClip(unit.Clips.Values.ElementAt(0));

            unit.Position = Vector3.Zero;
            unit.Scale = new Vector3(0.05f, 0.05f, 0.05f);
            unit.Rotation = new Vector3(0f, 0, 0);

            Logger.Log.Debug("Standart enemy created");
            return unit;
        }

        private static Enemy _BuildSniper(string Name)
        {
            Enemy unit = new Enemy();
            unit.Name = Name;
            unit.Model = ToylandSiege.GetInstance().Content.Load<Model>("Units/Soldier");
            unit.Type = "Enemy";
            unit.IsEnabled = false;
            unit.Health = 50;
            unit.MaxHealth = 50;
            unit.Damage = 60;
            unit.ShotDistance = 5;
            unit.TimeBetweeenShoots = 6;
            unit.UnitType = "Sniper";
            
            var skinningData = ToylandSiege.GetInstance().Content.Load<Model>("Units/Soldier").Tag as SkinningData;

            unit.AnimationPlayer = new AnimationPlayer(skinningData);
            unit.Clips = new Dictionary<string, AnimationClip>();
            unit.Clips.Add("walking", skinningData.AnimationClips["walking"]);
            unit.Clips.Add("standing", new AnimationClip(skinningData.AnimationClips["walking"], skinningData.AnimationClips["standing"]));
            unit.Clips.Add("crouch", new AnimationClip(skinningData.AnimationClips["standing"], skinningData.AnimationClips["crouch"]));
            unit.Clips.Add("crouching", new AnimationClip(skinningData.AnimationClips["crouch"], skinningData.AnimationClips["crouching"]));
            unit.Clips.Add("standup", new AnimationClip(skinningData.AnimationClips["crouching"], skinningData.AnimationClips["standup"]));

            unit.AnimationPlayer.StartClip(unit.Clips.Values.ElementAt(1));

            unit.Position = Vector3.Zero;
            unit.Scale = new Vector3(0.05f, 0.05f, 0.05f);
            unit.Rotation = new Vector3(0f, 0, 0);

            Logger.Log.Debug("Sniper enemy created");
            return unit;
        }

        private static Enemy _BuildDefender(string Name)
        {
            Enemy unit = new Enemy();
            unit.Name = Name;
            unit.Model = ToylandSiege.GetInstance().Content.Load<Model>("Units/Soldier");
            unit.Type = "Enemy";
            unit.IsEnabled = false;
            unit.Health = 100;
            unit.MaxHealth = 100;
            unit.Damage = 20;
            unit.ShotDistance = 1;
            unit.TimeBetweeenShoots = 1;
            unit.UnitType = "Defender";

            var skinningData = ToylandSiege.GetInstance().Content.Load<Model>("Units/Soldier").Tag as SkinningData;
            unit.AnimationPlayer = new AnimationPlayer(skinningData);
            unit.Clips = new Dictionary<string, AnimationClip>();
            unit.Clips.Add("walking", skinningData.AnimationClips["walking"]);
            unit.Clips.Add("standing", new AnimationClip(skinningData.AnimationClips["walking"], skinningData.AnimationClips["standing"]));
            unit.Clips.Add("crouch", new AnimationClip(skinningData.AnimationClips["standing"], skinningData.AnimationClips["crouch"]));
            unit.Clips.Add("crouching", new AnimationClip(skinningData.AnimationClips["crouch"], skinningData.AnimationClips["crouching"]));
            unit.Clips.Add("standup", new AnimationClip(skinningData.AnimationClips["crouching"], skinningData.AnimationClips["standup"]));

            unit.AnimationPlayer.StartClip(unit.Clips.Values.ElementAt(1));

            unit.Position = Vector3.Zero;
            unit.Scale = new Vector3(0.05f, 0.05f, 0.05f);
            unit.Rotation = new Vector3(0f, 0, 0);

            Logger.Log.Debug("Defender enemy created");
            return unit;
        }
    }
}
