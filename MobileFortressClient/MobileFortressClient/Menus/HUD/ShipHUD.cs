using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MobileFortressClient.Ships;
using MobileFortressClient.Physics;
using MobileFortressClient.Messages;
using Microsoft.Xna.Framework.Audio;

namespace MobileFortressClient.Menus
{
    class ShipHUD : BaseMenu
    {
        Texture2D Crosshairs;
        Rectangle RCrosshairs;
        Rectangle RCrosshairs2;

        List<ShipObj> nearbyShips;

        Texture2D LockonTex;
        Rectangle LockonDest;
        Rectangle ArrowSource = new Rectangle(0, 0, 16, 16);
        Rectangle ReticleSource = new Rectangle(0, 0, 32, 32);

        UIFPS FrameCounter = new UIFPS();

        UIHealthIndicator NoseHealth;
        UIHealthIndicator CoreHealth;
        UIHealthIndicator EngineHealth;

        List<UIAmmoIndicator> AmmoIndicators = new List<UIAmmoIndicator>();

        public ShipObj Target = null;
        public LockonStatus LockStatus = LockonStatus.NotLocked;
        float lockTime = 0;

        public bool enemyLock = false;
        float enemyLockTime = 0;
        public bool enemyMissile
        {
            set
            {
                if (value)
                    enemyMissileSound.Play();
                else
                    enemyMissileSound.Stop();
                    
            }
        }
        SoundEffectInstance enemyMissileSound;

        public Color HUDColor = Color.Lime;
        public override void Initialize()
        {
            Viewport viewport = MobileFortressClient.Game.GraphicsDevice.Viewport;
            Crosshairs = Resources.Menus.HUD.Crosshairs;
            RCrosshairs = new Rectangle(0, 0, Crosshairs.Width, Crosshairs.Height);
            RCrosshairs2 = new Rectangle(0, 0, Crosshairs.Width/2, Crosshairs.Height/2);
            nearbyShips = Sector.Redria.Ships.GetTable();

            Texture2D tex = Resources.Menus.HUD.NoseHealth;
            Rectangle dim = new Rectangle(viewport.Width - 100 - tex.Width / 2, 35, tex.Width, tex.Height);
            NoseHealth = new UIHealthIndicator(this, tex, dim);
            Manager.Elements.Add(NoseHealth);
            int offset = tex.Height;

            tex = Resources.Menus.HUD.CoreHealth;
            dim = new Rectangle(viewport.Width - 100 - tex.Width / 2, 35 + offset, tex.Width, tex.Height);
            CoreHealth = new UIHealthIndicator(this, tex, dim);
            Manager.Elements.Add(CoreHealth);
            offset += tex.Height;

            tex = Resources.Menus.HUD.EngineHealth;
            dim = new Rectangle(viewport.Width - 100 - tex.Width/2, 35 + offset, tex.Width, tex.Height);
            EngineHealth = new UIHealthIndicator(this, tex, dim);
            Manager.Elements.Add(EngineHealth);

            SetHealth(600);

            enemyMissileSound = Resources.Sounds.EnemyMissile.CreateInstance();
            enemyMissileSound.IsLooped = true;
        }

        public void SetHealth(int n)
        {
            if (Camera.Target != null)
            {
                ShipObj Ship = (ShipObj)Camera.Target;
                float ratio = Ship.ArmorLeft(n);
                NoseHealth.HealthRatio = ratio;
                CoreHealth.HealthRatio = ratio;
                EngineHealth.HealthRatio = ratio;
            }
        }

        public void SetPing(float f)
        {
            FrameCounter.ping = f;
        }

        public void MakeAmmoIndicator(int weaponIcon)
        {
            Viewport viewport = MobileFortressClient.Game.GraphicsDevice.Viewport;
            Texture2D tex = Resources.Menus.HUD.WeaponIcon[weaponIcon];
            Rectangle dim = new Rectangle(12,viewport.Height-48*(AmmoIndicators.Count+1),tex.Width,tex.Height);
            var indicator = new UIAmmoIndicator(this, tex, dim);
            AmmoIndicators.Add(indicator);
            Manager.Elements.Add(indicator);
        }

        public void SetAmmo(int i, int n)
        {
            AmmoIndicators[i].Ammo = n;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyboard, Microsoft.Xna.Framework.Input.MouseState mouse)
        {
            Viewport viewport = MobileFortressClient.Game.GraphicsDevice.Viewport;
            ShipObj Ship = (ShipObj)Camera.Target;
            Vector3 Crosshair = Ship.Entity.WorldTransform.Forward * 70;
            Vector3 ScreenLocation = viewport.Project(Crosshair, Camera.Projection, Camera.View, Matrix.CreateTranslation(Ship.Entity.WorldTransform.Translation));

            RCrosshairs.Location = new Point((int)ScreenLocation.X - Crosshairs.Width/2,(int)ScreenLocation.Y - Crosshairs.Height/2);

            Crosshair = Ship.Entity.WorldTransform.Forward * 35;
            ScreenLocation = viewport.Project(Crosshair, Camera.Projection, Camera.View, Matrix.CreateTranslation(Ship.Entity.WorldTransform.Translation));
            RCrosshairs2.Location = new Point((int)ScreenLocation.X - Crosshairs.Width / 4, (int)ScreenLocation.Y - Crosshairs.Height / 4);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (LockStatus == LockonStatus.Locking)
            {
                lockTime += dt;
                if (lockTime >= 0.2f)
                {
                    lockTime = 0f;
                    Resources.Sounds.LockingOn.Play();
                }
            }
            if (enemyLock)
            {
                enemyLockTime += dt;
                if (enemyLockTime >= Resources.Sounds.EnemyLockon.Duration.TotalSeconds*1.2f)
                {
                    enemyLockTime = 0f;
                    Resources.Sounds.EnemyLockon.Play();
                }
            }

            FrameCounter.Update(gameTime);
            Manager.Update(gameTime, mouse);
        }
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            Viewport viewport = spriteBatch.GraphicsDevice.Viewport;
            ShipObj Ship = (ShipObj)Camera.Target;
            spriteBatch.Draw(Crosshairs, RCrosshairs2, HUDColor);
            spriteBatch.Draw(Crosshairs, RCrosshairs, HUDColor);
            FrameCounter.Draw(spriteBatch);
            #region Targeting
            foreach (ShipObj ship in nearbyShips)
            {
                if (ship == Camera.Target) continue;
                Rectangle LockonSource = ArrowSource;
                float AoA = AngleOfAttack(ship.Position, Camera.Target.Position, Camera.Target.worldTransform.Forward);
                Vector3 SpriteLocation = viewport.Project(ship.Position, Camera.Projection, Camera.View, Matrix.Identity);
                if (AoA > MathHelper.PiOver2 || AoA < -MathHelper.PiOver2)
                {
                    if (SpriteLocation.X < viewport.Width * 0.25f)
                    {
                        LockonTex = Resources.Menus.HUD.Arrows;
                        if (SpriteLocation.Y < 0)
                            LockonDest = new Rectangle(0, 0, 16, 16);
                        else if (SpriteLocation.Y > viewport.Height)
                            LockonDest = new Rectangle(0, viewport.Height - 16, 16, 16);
                        else
                            LockonDest = new Rectangle(0, (int)SpriteLocation.Y - 8, 16, 16);
                        LockonSource.Y = 16;
                    }
                    else if (SpriteLocation.X > viewport.Width * 0.75f)
                    {
                        LockonTex = Resources.Menus.HUD.Arrows;
                        if (SpriteLocation.Y < 0)
                            LockonDest = new Rectangle(viewport.Width - 16, 0, 16, 16);
                        else if (SpriteLocation.Y > viewport.Height)
                            LockonDest = new Rectangle(viewport.Width - 16, viewport.Height - 16, 16, 16);
                        else
                            LockonDest = new Rectangle(viewport.Width - 16, (int)SpriteLocation.Y - 8, 16, 16);
                        LockonSource.X = 16;
                        LockonSource.Y = 16;
                    }
                    else if (SpriteLocation.Y > viewport.Height / 2)
                    {
                        LockonTex = Resources.Menus.HUD.Arrows;
                        LockonDest = new Rectangle((int)SpriteLocation.X - 8, viewport.Height - 16, 16, 16);
                        LockonSource.X = 16;
                    }
                    else
                    {
                        LockonTex = Resources.Menus.HUD.Arrows;
                        LockonDest = new Rectangle((int)SpriteLocation.X - 8, 0, 16, 16);
                    }
                }
                else
                {
                    if (SpriteLocation.X < 0)
                    {
                        LockonTex = Resources.Menus.HUD.Arrows;
                        if (SpriteLocation.Y < 0)
                            LockonDest = new Rectangle(0, 0, 16, 16);
                        else if (SpriteLocation.Y > viewport.Height)
                            LockonDest = new Rectangle(0, viewport.Height - 16, 16, 16);
                        else
                            LockonDest = new Rectangle(0, (int)SpriteLocation.Y - 8, 16, 16);
                        LockonSource.Y = 16;
                    }
                    else if (SpriteLocation.X > viewport.Width)
                    {
                        LockonTex = Resources.Menus.HUD.Arrows;
                        if (SpriteLocation.Y < 0)
                            LockonDest = new Rectangle(viewport.Width - 16, 0, 16, 16);
                        else if (SpriteLocation.Y > viewport.Height)
                            LockonDest = new Rectangle(viewport.Width - 16, viewport.Height - 16, 16, 16);
                        else
                            LockonDest = new Rectangle(viewport.Width - 16, (int)SpriteLocation.Y - 8, 16, 16);
                        LockonSource.X = 16;
                        LockonSource.Y = 16;
                    }

                    else if (SpriteLocation.Y < 0)
                    {
                        LockonTex = Resources.Menus.HUD.Arrows;
                        LockonDest = new Rectangle((int)SpriteLocation.X - 8, 0, 16, 16);
                    }
                    else if (SpriteLocation.Y > viewport.Height)
                    {
                        LockonTex = Resources.Menus.HUD.Arrows;
                        LockonDest = new Rectangle((int)SpriteLocation.X - 8, viewport.Height - 16, 16, 16);
                        LockonSource.X = 16;
                    }
                    else
                    {
                        LockonTex = Resources.Menus.HUD.Reticle;
                        LockonDest = new Rectangle((int)SpriteLocation.X - 16, (int)SpriteLocation.Y - 16, 32, 32);
                        LockonSource = ReticleSource;
                    }
                }
                Color color = HUDColor;
                if (ship == Target)
                {
                    if (LockStatus == LockonStatus.Locking)
                    {
                        color = Color.Gold;
                    }
                    else if (LockStatus == LockonStatus.Locked)
                    {
                        color = Color.Red;
                    }
                }
                spriteBatch.Draw(LockonTex, LockonDest, LockonSource, color);
            }
            #endregion
            Manager.Draw(spriteBatch);
        }

        public static float AngleOfAttack(Vector3 targetPoint, Vector3 position, Vector3 orientation)
        {
            Vector3 difference = targetPoint - position;
            difference.Normalize();
            return (float)Math.Acos(Vector3.Dot(orientation, difference));
        }
    }
}
