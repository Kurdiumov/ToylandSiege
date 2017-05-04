using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;


namespace ToylandSiege.GameObjects
{
    public abstract class UnitBase : GameObject
    {
        public float Health { get; set; }
        public float Damage { get; set; }
        public uint ShotDistance { get; set; }
        public float TimeBetweeenShoots { get; set; }

        public string UnitType;

        protected readonly HashSet<Field> FieldsInRange = new HashSet<Field>();
        protected TimeSpan LastShootTime;

        private Field _field;
        public Field Field
        {
            get
            {
                return _field;
            }
            set
            {
                _field = value;
                if (value == null)
                    Logger.Log.Debug("Field set to NULL for " + Type + " " + this);
                else
                    Logger.Log.Debug("New field (" + _field + ") set for " + Type + " " + this);
            }
        }

        public UnitBase()
        {
            Initialize();
        }
        protected abstract override void Initialize();

        public abstract override void Update(GameTime gameTime);

        public void UpdateFieldsInRange()
        {
            if (Field == null)
                return;
            FieldsInRange.Clear();
            foreach (var field in Field.GetNearestFields())
            {
                FieldsInRange.Add(field);
            }

            for (int i = 1; i < ShotDistance; i++)
            {
                int count = FieldsInRange.Count;
                for (int j = 0; j < count; j++)
                {
                    FieldsInRange.ElementAt(j).GetNearestFields().ForEach(fld => FieldsInRange.Add(fld));
                }
            }
        }

        public bool CanShoot(GameTime gameTime)
        {
            if (TimeBetweeenShoots - (gameTime.TotalGameTime - LastShootTime).TotalSeconds <= 0)
                return true;
            return false;
        }

        public void DestroyItself()
        {
            Logger.Log.Debug(Name + " Destroing itself. Health = " + Health);
            if (this is Unit)
            {
                if (Field != null)
                {
                    Field.unit = null;
                }
                Level.GetCurrentLevel().RootGameObject.Childs["Units"].RemoveChild(this);
            }
            else if (this is Enemy)
            {
                if (Field != null)
                {
                    Field.enemy = null;
                }
                
                Field.Spawner.TimerStarted = false;
                Level.GetCurrentLevel().RootGameObject.Childs["Enemies"].RemoveChild(this);
            }
        }

        public void GetDamage(float Damage)
        {
            Health -= Damage;
            Logger.Log.Debug(Name + " got" + Damage +" Damage. Health = " + Health);
            if (this.Health <= 0)
                DestroyItself();
        }

        public void Shoot(GameTime gameTime, UnitBase target)
        {
            if (target == null)
            {
                Logger.Log.Error(this + "cant shoot. Target is Null ");
                return;
            }
            Logger.Log.Debug(this + " shooting. Target:  " + target);
            //TODO:Sound here
            target.GetDamage(Damage);
            LastShootTime = gameTime.TotalGameTime;
        }
    }
}
