﻿using System;
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
            unit.Model = ToylandSiege.GetInstance().Content.Load<Model>("1234");
            unit.Type = "Enemy";
            unit.IsEnabled = false;
            unit.Health = 75;
            unit.Damage = 25;
            unit.ShotDistance = 3;
            unit.TimeBetweeenShoots = 3;
            unit.UnitType = "Standart";

            var skinningData = ToylandSiege.GetInstance().Content.Load<Model>("1234").Tag as SkinningData;
            unit.AnimationPlayer = new AnimationPlayer(skinningData);
            unit.Clips = new Dictionary<string, AnimationClip>()
            {
                {
                    "Take 001",
                    skinningData.AnimationClips["Take 001"]
                }
            };
            unit.AnimationPlayer.StartClip(unit.Clips.Values.First());

            unit.Scale = new Vector3(0.08f, 0.08f, 0.08f);
            unit.Rotation = new Vector3(1.59f, 0, 0);

            Logger.Log.Debug("Standart enemy created");
            return unit;
        }

        private static Enemy _BuildSniper(string Name)
        {
            Enemy unit = new Enemy();
            unit.Name = Name;
            unit.Model = ToylandSiege.GetInstance().Content.Load<Model>("1234");
            unit.Type = "Enemy";
            unit.IsEnabled = false;
            unit.Health = 50;
            unit.Damage = 60;
            unit.ShotDistance = 5;
            unit.TimeBetweeenShoots = 6;
            unit.UnitType = "Sniper";

            var skinningData = ToylandSiege.GetInstance().Content.Load<Model>("1234").Tag as SkinningData;
            unit.AnimationPlayer = new AnimationPlayer(skinningData);
            unit.Clips = new Dictionary<string, AnimationClip>()
            {
                {
                    "Take 001",
                    skinningData.AnimationClips["Take 001"]
                }
            };
            unit.AnimationPlayer.StartClip(unit.Clips.Values.First());

            unit.Scale = new Vector3(0.08f, 0.08f, 0.08f);
            unit.Rotation = new Vector3(1.59f, 0, 0);

            Logger.Log.Debug("Sniper enemy created");
            return unit;
        }

        private static Enemy _BuildDefender(string Name)
        {
            Enemy unit = new Enemy();
            unit.Name = Name;
            unit.Model = ToylandSiege.GetInstance().Content.Load<Model>("1234");
            unit.Type = "Enemy";
            unit.IsEnabled = false;
            unit.Health = 100;
            unit.Damage = 20;
            unit.ShotDistance = 1;
            unit.TimeBetweeenShoots = 1;
            unit.UnitType = "Defender";

            var skinningData = ToylandSiege.GetInstance().Content.Load<Model>("1234").Tag as SkinningData;
            unit.AnimationPlayer = new AnimationPlayer(skinningData);
            unit.Clips = new Dictionary<string, AnimationClip>()
            {
                {
                    "Take 001",
                    skinningData.AnimationClips["Take 001"]
                }
            };
            unit.AnimationPlayer.StartClip(unit.Clips.Values.First());

            unit.Scale = new Vector3(0.08f, 0.08f, 0.08f);
            unit.Rotation = new Vector3(1.59f, 0, 0);

            Logger.Log.Debug("Defender enemy created");
            return unit;
        }
    }
}
