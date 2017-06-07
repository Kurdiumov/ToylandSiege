using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using ToylandSiege.GameObjects;

namespace ToylandSiege
{
    public class UnitBuilder
    {
        public static Unit BuildUnit(string Name, string Type)
        {
            switch (Type.ToLower())
            {
                case "soldier":
                    return _BuildSoldierUnit(Name);
                case "tank":
                    return _BuildTankUnit(Name);
                case "scout":
                    return _BuildScoutUnit(Name);
                default:
                    throw new ArgumentException("Unsupported unit type " + Type);
            }
        }

        private static Unit _BuildSoldierUnit(string Name)
        {
            Unit unit = new Unit();
            unit.Name = Name;
            unit.Model = ToylandSiege.GetInstance().Content.Load<Model>("Soldier");
            unit.Type = "Unit";
            unit.IsEnabled = false;
            unit.Health = 100;
            unit.MaxHealth = 100;
            unit.Damage = 25;
            unit.ShotDistance = 3;
            unit.TimeBetweeenShoots = 3;
            unit.Speed = 5;
            unit.UnitType = "Soldier";
            
            var skinningData = ToylandSiege.GetInstance().Content.Load<Model>("Soldier").Tag as SkinningData;
            unit.AnimationPlayer = new AnimationPlayer(skinningData);
            unit.Clips = new Dictionary<string, AnimationClip>();
            unit.Clips.Add("walking", skinningData.AnimationClips["walking"]);
            unit.Clips.Add("standing", new AnimationClip(skinningData.AnimationClips["walking"], skinningData.AnimationClips["standing"]));
            unit.Clips.Add("crouch", new AnimationClip(skinningData.AnimationClips["standing"], skinningData.AnimationClips["crouch"]));
            unit.Clips.Add("crouching", new AnimationClip(skinningData.AnimationClips["crouch"], skinningData.AnimationClips["crouching"]));
            unit.Clips.Add("standup", new AnimationClip(skinningData.AnimationClips["crouching"], skinningData.AnimationClips["standup"]));

            unit.AnimationPlayer.StartClip(unit.Clips.Values.ElementAt(1));

            unit.Position = Vector3.Zero;
            unit.Scale = new Vector3(0.22f, 0.22f, 0.22f);
            unit.Rotation = new Vector3(0f, 0, 0);

            Logger.Log.Debug("Soldier unit created");
            return unit;
        }

        private static Unit _BuildScoutUnit(string Name)
        {
            Unit unit = new Unit();
            unit.Name = Name;
            unit.Model = ToylandSiege.GetInstance().Content.Load<Model>("Soldier");
            unit.Type = "Unit";
            unit.IsEnabled = false;
            unit.Health = 50;
            unit.MaxHealth = 50;
            unit.Damage = 10;
            unit.ShotDistance = 3;
            unit.TimeBetweeenShoots = 1;
            unit.Speed = 8;
            unit.UnitType = "Scout";
            
            var skinningData = ToylandSiege.GetInstance().Content.Load<Model>("Soldier").Tag as SkinningData;
            unit.AnimationPlayer = new AnimationPlayer(skinningData);
            unit.Clips = new Dictionary<string, AnimationClip>();
            unit.Clips.Add("walking", skinningData.AnimationClips["walking"]);
            unit.Clips.Add("standing", new AnimationClip(skinningData.AnimationClips["walking"], skinningData.AnimationClips["standing"]));
            unit.Clips.Add("crouch", new AnimationClip(skinningData.AnimationClips["standing"], skinningData.AnimationClips["crouch"]));
            unit.Clips.Add("crouching", new AnimationClip(skinningData.AnimationClips["crouch"], skinningData.AnimationClips["crouching"]));
            unit.Clips.Add("standup", new AnimationClip(skinningData.AnimationClips["crouching"], skinningData.AnimationClips["standup"]));

            unit.AnimationPlayer.StartClip(unit.Clips.Values.ElementAt(1));

            unit.Position = Vector3.Zero;
            unit.Scale = new Vector3(0.22f, 0.22f, 0.22f);
            unit.Rotation = new Vector3(0f, 0, 0);

            Logger.Log.Debug("Scout unit created");
            return unit;
        }

        private static Unit _BuildTankUnit(string Name)
        {
            Unit unit = new Unit();
            unit.Name = Name;
            unit.Model = ToylandSiege.GetInstance().Content.Load<Model>("Soldier");
            unit.Type = "Unit";
            unit.IsEnabled = false;
            unit.Health = 200;
            unit.MaxHealth = 200;
            unit.Damage = 40;
            unit.ShotDistance = 3;
            unit.TimeBetweeenShoots = 3;
            unit.Speed = 2;
            unit.UnitType = "Tank";
            
            var skinningData = ToylandSiege.GetInstance().Content.Load<Model>("Soldier").Tag as SkinningData;
            unit.AnimationPlayer = new AnimationPlayer(skinningData);
            unit.Clips = new Dictionary<string, AnimationClip>();
            unit.Clips.Add("walking", skinningData.AnimationClips["walking"]);
            unit.Clips.Add("standing", new AnimationClip(skinningData.AnimationClips["walking"], skinningData.AnimationClips["standing"]));
            unit.Clips.Add("crouch", new AnimationClip(skinningData.AnimationClips["standing"], skinningData.AnimationClips["crouch"]));
            unit.Clips.Add("crouching", new AnimationClip(skinningData.AnimationClips["crouch"], skinningData.AnimationClips["crouching"]));
            unit.Clips.Add("standup", new AnimationClip(skinningData.AnimationClips["crouching"], skinningData.AnimationClips["standup"]));

            unit.AnimationPlayer.StartClip(unit.Clips.Values.ElementAt(1));

            unit.Position = Vector3.Zero;
            unit.Scale = new Vector3(0.22f, 0.22f, 0.22f);
            unit.Rotation = new Vector3(0f, 0, 0);

            Logger.Log.Debug("Tank unit created");
            return unit;
        }
    }
}
