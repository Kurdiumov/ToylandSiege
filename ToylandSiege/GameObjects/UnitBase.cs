using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ToylandSiege.GameObjects
{
    public abstract class UnitBase : GameObject
    {
        public float MaxHealth { get; set; }
        public float Damage { get; set; }
        public uint ShotDistance { get; set; }
        public float TimeBetweeenShoots { get; set; }

        public string UnitType;

        protected readonly HashSet<Field> FieldsInRange = new HashSet<Field>();
        protected TimeSpan LastShootTime;

        private Texture2D HealthBar;

        private float _health;
        public float Health
        {
            get { return _health; }
            set
            {
                _health = value;
                HealthBar = RecreateHealthRectangle();
            }
        }

        private Field _field;
        public Field Field
        {
            get { return _field; }
            set
            {
                _field = value;
                if (value == null)
                    Logger.Log.Debug("Field set to NULL for " + Type + " " + this);
                else
                    Logger.Log.Debug("New field (" + _field + ") set for " + Type + " " + this);
            }
        }

        SpriteBatch spriteBatch;
        BasicEffect basicEffect;
        SpriteFont spriteFont;
        public UnitBase()
        {
            Initialize();
            spriteBatch = new SpriteBatch(ToylandSiege.GetInstance().GraphicsDevice);
            spriteFont = ToylandSiege.GetInstance().Content.Load<SpriteFont>("Fonts/FPS");
            basicEffect = new BasicEffect(ToylandSiege.GetInstance().GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
            };
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
            if(this is Unit)
                SoundManager.PlaySound("UnitDeathSound", 0.8f);
            else if (this is Enemy)
                SoundManager.PlaySound("EnemyDeathSound", 0.8f);
            else
                Logger.Log.Debug(Name + " Death sound not supported. Unrecognized Unit type");
            

            Logger.Log.Debug(Name + " Destroing itself. Health = " + Health);
            if (this is Unit)
            {
                foreach (var field in (this as Unit).FieldsInWay)
                    field.IsPartOfWay = false;

                if (Field != null)
                {
                    Field.IsPartOfWay = false;
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
            HealthBar = RecreateHealthRectangle();
            Logger.Log.Debug(Name + " got" + Damage + " Damage. Health = " + Health);
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
            _PlayShootSound();
            target.GetDamage(Damage);
            LastShootTime = gameTime.TotalGameTime;
        }

        private void _PlayShootSound()
        {
            //TODO: Remove try-catch block when all sound will be loaded in SoundManager
            try
            {
                switch (UnitType.ToLower())
                {
                    case "sniper":
                        SoundManager.PlaySound("SniperShootSound", 0.5f);
                        break;
                    case "defender":
                        SoundManager.PlaySound("DefenderShootSound", 0.8f);
                        break;
                    case "standart":
                        SoundManager.PlaySound("StandartShootSound", 0.5f);
                        break;
                    case "soldier":
                        SoundManager.PlaySound("SoldierShootSound", 0.5f);
                        break;
                    case "tank":
                        SoundManager.PlaySound("TankShootSound", 0.5f);
                        break;
                    case "scout":
                        SoundManager.PlaySound("ScoutShootSound", 0.5f);
                        break;
                    default:
                        throw new ArgumentException("Type not supported + " + UnitType);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
        }

        public void DrawHealthBar()
        {
            Vector3 rectanglePosition = Position + new Vector3(-5f, 7, -1);
            basicEffect.World = Matrix.CreateConstrainedBillboard(rectanglePosition, rectanglePosition - Camera.GetCurrentCamera().Direction, Vector3.Down, Camera.GetCurrentCamera().Direction, null);
            basicEffect.View = Camera.GetCurrentCamera().ViewMatrix;
            basicEffect.Projection = Camera.GetCurrentCamera().ProjectionMatrix;

            spriteBatch.Begin(0, null, null, DepthStencilState.Default, RasterizerState.CullCounterClockwise, basicEffect);
            spriteBatch.Draw(HealthBar, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        private Texture2D RecreateHealthRectangle()
        {
            int width = 6;
            Texture2D texture = new Texture2D(ToylandSiege.GetInstance().GraphicsDevice, width, 1);

            int coef = (int)MaxHealth / width;
            var data = new Color[width];
            for (int i = 0; i < data.Length; ++i)
            {
                if (i * coef >= Health)
                    data[i] = Color.Red;
                else
                    data[i] = Color.Green;
            }

            texture.SetData(data);
            return texture;
        }

        public override void Draw()
        {
            DrawHealthBar();
            ToylandSiege.GetInstance().GraphicsDevice.BlendState = BlendState.Opaque;
            ToylandSiege.GetInstance().GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ToylandSiege.GetInstance().GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw();
        }
    }
}