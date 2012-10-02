using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MobileFortressClient.Managers;
using MobileFortressClient.Menus.Customizer;
using MobileFortressClient.Data;
using MobileFortressClient.Messages;

namespace MobileFortressClient.Menus
{
    class ShipCustomizer : BaseMenu
    {
        float introTime = 0;
        const byte fireGroups = 2;

        Texture2D pointerTex;
        Rectangle pointerDimensions;

        UIWeaponSelector Weapon1;
        UIWeaponSelector Weapon2;
        UIWeaponSelector Weapon3;
        UIWeaponSelector Weapon4;
        UIPartSelector Nose;
        UIPartSelector Core;
        UIPartSelector Engine;

        ShipData Ship;

        UIElement DescriptionBox;
        UIStandardButton ConfirmButton;

        enum SelectedPart { Nose, Core, Tail, None }
        SelectedPart selectedPart = SelectedPart.None;
        int selectedWeaponSlot = 0; 

        Matrix shipRotation = Matrix.Identity;

        void ResetWeapons()
        {
            Weapon1.NameBar.color = Color.White;
            Weapon2.NameBar.color = Color.White;
            Weapon3.NameBar.color = Color.White;
            Weapon4.NameBar.color = Color.White;
            Weapon1.Weapon = null;
            Weapon2.Weapon = null;
            Weapon3.Weapon = null;
            Weapon4.Weapon = null;
            Weapon1.Available = false;
            Weapon2.Available = false;
            Weapon3.Available = false;
            Weapon4.Available = false;
        }

        void ResetWeaponColors()
        {
            Weapon1.NameBar.color = Color.White;
            Weapon2.NameBar.color = Color.White;
            Weapon3.NameBar.color = Color.White;
            Weapon4.NameBar.color = Color.White;
        }

        public void MouseOver(UIElement element)
        {
            Resources.Menus.Title.MenuSelect.Play();
        }

        void ResetAvailableWeapons(PartData part)
        {
            Weapon1.Available = false;
            Weapon2.Available = false;
            Weapon3.Available = false;
            Weapon4.Available = false;
            if (part.WeaponSlots != null)
            {
                Weapon1.Available = true;
                if (part.WeaponSlots.Length > 1)
                {
                    Weapon2.Available = true;
                    if (part.WeaponSlots.Length > 2)
                    {
                        Weapon3.Available = true;
                        if (part.WeaponSlots.Length > 3)
                        {
                            Weapon4.Available = true;
                        }
                    }
                    
                }
            }
        }

        void PreviousNose(UIElement element)
        {
            int id = (Ship.NoseID - 1);
            if (id < 0) id = PartData.Noses.Length - 1;
            Ship.SetNose(id);
            Nose.Part = Ship.Nose;
            ResetAvailableWeapons(Ship.Nose);
        }
        void NextNose(UIElement element)
        {
            int id = (Ship.NoseID + 1);
            if (id > PartData.Noses.Length - 1) id = 0;
            Ship.SetNose(id);
            Nose.Part = Ship.Nose;
            ResetAvailableWeapons(Ship.Nose);
        }

        void PreviousCore(UIElement element)
        {
            int id = (Ship.CoreID - 1);
            if (id < 0) id = PartData.Cores.Length - 1;
            Ship.SetCore(id);
            Core.Part = Ship.Core;
            ResetAvailableWeapons(Ship.Core);
        }
        void NextCore(UIElement element)
        {
            int id = (Ship.CoreID + 1);
            if (id > PartData.Cores.Length - 1) id = 0;
            Ship.SetCore(id);
            Core.Part = Ship.Core;
            ResetAvailableWeapons(Ship.Core);
        }

        void PreviousEngine(UIElement element)
        {
            int id = (Ship.EngineID - 1);
            if (id < 0) id = PartData.Engines.Length - 1;
            Ship.SetEngine(id);
            Engine.Part = Ship.Engine;
            ResetAvailableWeapons(Ship.Engine);
        }
        void NextEngine(UIElement element)
        {
            int id = (Ship.EngineID + 1);
            if (id > PartData.Engines.Length - 1) id = 0;
            Ship.SetEngine(id);
            Engine.Part = Ship.Engine;
            ResetAvailableWeapons(Ship.Engine);
        }

        void PreviousWeapon(UIElement element)
        {
            int selectedIndex = 1;
            if (element == Weapon2.UpArrow) selectedIndex = 2;
            if (element == Weapon3.UpArrow) selectedIndex = 3;
            if (element == Weapon4.UpArrow) selectedIndex = 4;
            PartData part = null;
            WeaponData weapon = null;
            Vector3 slot = Vector3.Zero;
            switch (selectedPart)
            {
                case SelectedPart.Nose:
                    part = Ship.Nose;
                    break;
                case SelectedPart.Core:
                    part = Ship.Core;
                    break;
                case SelectedPart.Tail:
                    part = Ship.Engine;
                    break;
                default:
                    break;
            }
            if (part != null && part.WeaponSlots != null && part.WeaponSlots.Length >= selectedWeaponSlot)
            {
                slot = part.WeaponSlots[selectedIndex - 1];
                weapon = Ship.Weapons[slot];
                int nextWeapon = 0;
                bool noWeapon = true;
                if (weapon != null)
                {
                    nextWeapon = weapon.Index - 1;
                    if (nextWeapon < 0)
                        noWeapon = true;
                    else
                        noWeapon = false;
                }
                else
                {
                    nextWeapon = WeaponData.WeaponTypes.Length - 1;
                    noWeapon = false;
                }
                WeaponData newWeapon;
                if (!noWeapon)
                {
                    newWeapon = WeaponData.WeaponTypes[nextWeapon].Copy();
                    Ship.SetWeapon(slot,newWeapon);
                }
                else
                {
                    Ship.SetWeapon(slot,null);
                    newWeapon = null;
                }
                if (noWeapon)
                {
                    if (selectedIndex <= 1)
                        Weapon1.Weapon = null;
                    else if (selectedIndex <= 2)
                        Weapon2.Weapon = null;
                    else if (selectedIndex <= 3)
                        Weapon3.Weapon = null;
                    else
                        Weapon4.Weapon = null;
                }
                else
                {
                    if (selectedIndex <= 1)
                        Weapon1.Weapon = newWeapon;
                    else if (selectedIndex <= 2)
                        Weapon2.Weapon = newWeapon;
                    else if (selectedIndex <= 3)
                        Weapon3.Weapon = newWeapon;
                    else
                        Weapon4.Weapon = newWeapon;
                }
                Resources.Sounds.Latch.Play();
            }
        }

        void NextWeapon(UIElement element)
        {
            int selectedIndex = 1;
            if (element == Weapon2.DownArrow) selectedIndex = 2;
            if (element == Weapon3.DownArrow) selectedIndex = 3;
            if (element == Weapon4.DownArrow) selectedIndex = 4;
            PartData part = null;
            WeaponData weapon = null;
            Vector3 slot = Vector3.Zero;
            switch (selectedPart)
            {
                case SelectedPart.Nose:
                    part = Ship.Nose;
                    break;
                case SelectedPart.Core:
                    part = Ship.Core;
                    break;
                case SelectedPart.Tail:
                    part = Ship.Engine;
                    break;
                default:
                    break;
            }
            if (part != null && part.WeaponSlots != null && part.WeaponSlots.Length >= selectedWeaponSlot)
            {
                slot = part.WeaponSlots[selectedIndex - 1];
                weapon = Ship.Weapons[slot];
                int nextWeapon = 0;
                bool noWeapon = true;
                if (weapon != null)
                {
                    nextWeapon = weapon.Index + 1;
                    if (nextWeapon > WeaponData.WeaponTypes.Length-1)
                        noWeapon = true;
                    else
                        noWeapon = false;
                }
                else
                {
                    nextWeapon = 0;
                    noWeapon = false;
                }
                WeaponData newWeapon;
                if (!noWeapon)
                {
                    newWeapon = WeaponData.WeaponTypes[nextWeapon].Copy();
                    Ship.SetWeapon(slot, newWeapon);
                }
                else
                {
                    Ship.SetWeapon(slot, null);
                    newWeapon = null;
                }
                if (noWeapon)
                {
                    if (selectedIndex <= 1)
                        Weapon1.Weapon = null;
                    else if (selectedIndex <= 2)
                        Weapon2.Weapon = null;
                    else if (selectedIndex <= 3)
                        Weapon3.Weapon = null;
                    else
                        Weapon4.Weapon = null;
                }
                else
                {
                    if (selectedIndex <= 1)
                        Weapon1.Weapon = newWeapon;
                    else if (selectedIndex <= 2)
                        Weapon2.Weapon = newWeapon;
                    else if (selectedIndex <= 3)
                        Weapon3.Weapon = newWeapon;
                    else
                        Weapon4.Weapon = newWeapon;
                }
                Resources.Sounds.Latch.Play();
            }
        }

        void SelectPart(UIElement element)
        {
            if (element == Nose.NameBar)
            {
                selectedPart = SelectedPart.Nose;
                Nose.NameBar.color = Color.Lime;
                Core.NameBar.color = Color.White;
                Engine.NameBar.color = Color.White;
                selectedWeaponSlot = 0;

                ResetWeapons();
                
                if (Ship.Nose.WeaponSlots != null)
                {
                    Weapon1.Weapon = Ship.Weapons[Ship.Nose.WeaponSlots[0]];
                    if(Ship.Nose.WeaponSlots.Length > 1)
                    {
                        Weapon2.Weapon = Ship.Weapons[Ship.Nose.WeaponSlots[1]];
                        if (Ship.Nose.WeaponSlots.Length > 2)
                        {
                            Weapon3.Weapon = Ship.Weapons[Ship.Nose.WeaponSlots[2]];
                            if (Ship.Nose.WeaponSlots.Length > 3)
                            {
                                Weapon4.Weapon = Ship.Weapons[Ship.Nose.WeaponSlots[4]];
                            }
                        }
                    }
                }
                Resources.Menus.Title.MenuSelect.Play();
            }
            else if (element == Core.NameBar)
            {
                selectedPart = SelectedPart.Core;
                Nose.NameBar.color = Color.White;
                Core.NameBar.color = Color.Lime;
                Engine.NameBar.color = Color.White;
                selectedWeaponSlot = 0;
                ResetWeapons();
                if (Ship.Core.WeaponSlots != null)
                {
                    Weapon1.Weapon = Ship.Weapons[Ship.Core.WeaponSlots[0]];
                    if (Ship.Core.WeaponSlots.Length > 1)
                    {
                        Weapon2.Weapon = Ship.Weapons[Ship.Core.WeaponSlots[1]];
                        if (Ship.Core.WeaponSlots.Length > 2)
                        {
                            Weapon3.Weapon = Ship.Weapons[Ship.Core.WeaponSlots[2]];
                            if (Ship.Core.WeaponSlots.Length > 3)
                            {
                                Weapon4.Weapon = Ship.Weapons[Ship.Core.WeaponSlots[3]];
                            }
                        }
                    }
                }
                Resources.Menus.Title.MenuSelect.Play();
            }
            else if(element == Engine.NameBar)
            {
                selectedPart = SelectedPart.Tail;
                Nose.NameBar.color = Color.White;
                Core.NameBar.color = Color.White;
                Engine.NameBar.color = Color.Lime;
                selectedWeaponSlot = 0;
                ResetWeapons();
                if (Ship.Engine.WeaponSlots != null)
                {
                    Weapon1.Weapon = Ship.Weapons[Ship.Engine.WeaponSlots[0]];
                    if (Ship.Engine.WeaponSlots.Length > 1)
                    {
                        Weapon2.Weapon = Ship.Weapons[Ship.Engine.WeaponSlots[1]];
                        if (Ship.Engine.WeaponSlots.Length > 2)
                        {
                            Weapon3.Weapon = Ship.Weapons[Ship.Engine.WeaponSlots[2]];
                            if (Ship.Engine.WeaponSlots.Length > 3)
                            {
                                Weapon4.Weapon = Ship.Weapons[Ship.Engine.WeaponSlots[4]];
                            }
                        }
                    }
                }
                Resources.Menus.Title.MenuSelect.Play();
            }
            else if (element == Weapon1.NameBar)
            {
                if (selectedPart != SelectedPart.None)
                    switch(selectedPart)
                    {
                        case SelectedPart.Nose:
                            if (Ship.Nose.WeaponSlots != null)
                            {
                                selectedWeaponSlot = 1;
                                ResetWeaponColors();
                                Weapon1.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                        case SelectedPart.Core:
                            if (Ship.Core.WeaponSlots != null)
                            {
                                selectedWeaponSlot = 1;
                                ResetWeaponColors();
                                Weapon1.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                        case SelectedPart.Tail:
                            if (Ship.Engine.WeaponSlots != null)
                            {
                                selectedWeaponSlot = 1;
                                ResetWeaponColors();
                                Weapon1.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                    }
            }
            else if (element == Weapon2.NameBar)
            {
                if (selectedPart != SelectedPart.None)
                    switch (selectedPart)
                    {
                        case SelectedPart.Nose:
                            if (Ship.Nose.WeaponSlots != null && Ship.Nose.WeaponSlots.Length >= 2)
                            {
                                selectedWeaponSlot = 2;
                                ResetWeaponColors();
                                Weapon2.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                        case SelectedPart.Core:
                            if (Ship.Core.WeaponSlots != null && Ship.Core.WeaponSlots.Length >= 2)
                            {
                                selectedWeaponSlot = 2;
                                ResetWeaponColors();
                                Weapon2.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                        case SelectedPart.Tail:
                            if (Ship.Engine.WeaponSlots != null && Ship.Engine.WeaponSlots.Length >= 2)
                            {
                                selectedWeaponSlot = 2;
                                ResetWeaponColors();
                                Weapon2.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                    }
            }
            else if (element == Weapon3.NameBar)
            {
                if (selectedPart != SelectedPart.None)
                    switch (selectedPart)
                    {
                        case SelectedPart.Nose:
                            if (Ship.Nose.WeaponSlots != null && Ship.Nose.WeaponSlots.Length >= 3)
                            {
                                selectedWeaponSlot = 3;
                                ResetWeaponColors();
                                Weapon3.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                        case SelectedPart.Core:
                            if (Ship.Core.WeaponSlots != null && Ship.Core.WeaponSlots.Length >= 3)
                            {
                                selectedWeaponSlot = 3;
                                ResetWeaponColors();
                                Weapon3.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                        case SelectedPart.Tail:
                            if (Ship.Engine.WeaponSlots != null && Ship.Engine.WeaponSlots.Length >= 3)
                            {
                                selectedWeaponSlot = 3;
                                ResetWeaponColors();
                                Weapon3.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                    }
            }
            else if (element == Weapon4.NameBar)
            {
                if (selectedPart != SelectedPart.None)
                    switch (selectedPart)
                    {
                        case SelectedPart.Nose:
                            if (Ship.Nose.WeaponSlots != null && Ship.Nose.WeaponSlots.Length >= 3)
                            {
                                selectedWeaponSlot = 4;
                                ResetWeaponColors();
                                Weapon4.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                        case SelectedPart.Core:
                            if (Ship.Core.WeaponSlots != null && Ship.Core.WeaponSlots.Length >= 3)
                            {
                                selectedWeaponSlot = 4;
                                ResetWeaponColors();
                                Weapon4.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                        case SelectedPart.Tail:
                            if (Ship.Engine.WeaponSlots != null && Ship.Engine.WeaponSlots.Length >= 3)
                            {
                                selectedWeaponSlot = 4;
                                ResetWeaponColors();
                                Weapon4.NameBar.color = Color.Lime;
                                Resources.Menus.Title.MenuSelect.Play();
                            }
                            break;
                    }
            }
        }

        void SelectPart()
        {
            if (selectedPart != SelectedPart.None) Resources.Menus.Title.MenuCancel.Play();
            selectedPart = SelectedPart.None;
            Nose.NameBar.color = Color.White;
            Core.NameBar.color = Color.White;
            Engine.NameBar.color = Color.White;
            selectedWeaponSlot = 0;
            ResetWeapons();
        }

        void IncrementWeaponGroup(UIElement element)
        {
            if (element == Weapon1.graphicBox)
            {
                if(Weapon1.Weapon != null) Weapon1.Weapon.fireGroup = (byte)((Weapon1.Weapon.fireGroup+1)%(fireGroups));
            }
            else if (element == Weapon2.graphicBox)
            {
                if (Weapon2.Weapon != null) Weapon2.Weapon.fireGroup = (byte)((Weapon2.Weapon.fireGroup + 1) % (fireGroups));
            }
            else if (element == Weapon3.graphicBox)
            {
                if (Weapon3.Weapon != null) Weapon3.Weapon.fireGroup = (byte)((Weapon3.Weapon.fireGroup + 1) % (fireGroups));
            }
            else
            {
                if (Weapon4.Weapon != null) Weapon4.Weapon.fireGroup = (byte)((Weapon4.Weapon.fireGroup + 1) % (fireGroups));
            }
        }

        void Confirm(UIElement element)
        {
            SendOutputMessage();
        }

        public override void Initialize()
        {
            Viewport viewport = MobileFortressClient.Game.GraphicsDevice.Viewport;

            Weapon1 = new UIWeaponSelector(this, new Point(12, viewport.Height- 256 - 48));
            Weapon2 = new UIWeaponSelector(this, new Point(12, viewport.Height - 192 - 36));
            Weapon3 = new UIWeaponSelector(this, new Point(12, viewport.Height - 128 - 24));
            Weapon4 = new UIWeaponSelector(this, new Point(12, viewport.Height - 64 - 12));
            Weapon1.NameBar.Clicked += SelectPart;
            Weapon2.NameBar.Clicked += SelectPart;
            Weapon3.NameBar.Clicked += SelectPart;
            Weapon4.NameBar.Clicked += SelectPart;
            Weapon1.graphicBox.Clicked += IncrementWeaponGroup;
            Weapon2.graphicBox.Clicked += IncrementWeaponGroup;
            Weapon3.graphicBox.Clicked += IncrementWeaponGroup;
            Weapon4.graphicBox.Clicked += IncrementWeaponGroup;
            Weapon1.UpArrow.Clicked += PreviousWeapon;
            Weapon1.DownArrow.Clicked += NextWeapon;
            Weapon2.UpArrow.Clicked += PreviousWeapon;
            Weapon2.DownArrow.Clicked += NextWeapon;
            Weapon3.UpArrow.Clicked += PreviousWeapon;
            Weapon3.DownArrow.Clicked += NextWeapon;
            Weapon4.UpArrow.Clicked += PreviousWeapon;
            Weapon4.DownArrow.Clicked += NextWeapon;

            Ship = new ShipData(1, 1, 1);

            Nose = new UIPartSelector(this, new Point(375 + 12, viewport.Height - 192 - 36 + 8));
            Nose.Part = Ship.Nose;
            Nose.UpArrow.Clicked += PreviousNose;
            Nose.DownArrow.Clicked += NextNose;
            Nose.NameBar.Clicked += SelectPart;
            Core = new UIPartSelector(this, new Point(375 + 12, viewport.Height - 128 - 24 + 8));
            Core.PartType = "Core";
            Core.Part = Ship.Core;
            Core.UpArrow.Clicked += PreviousCore;
            Core.DownArrow.Clicked += NextCore;
            Core.NameBar.Clicked += SelectPart;
            Engine = new UIPartSelector(this, new Point(375 + 12, viewport.Height - 64 - 12 + 8));
            Engine.PartType = "Engine";
            Engine.Part = Ship.Engine;
            Engine.UpArrow.Clicked += PreviousEngine;
            Engine.DownArrow.Clicked += NextEngine;
            Engine.NameBar.Clicked += SelectPart;

            ConfirmButton = new UIStandardButton(this, new Point(viewport.Width - 198 - 16, 8), "Launch");
            ConfirmButton.hasButton = true;
            ConfirmButton.Clicked += Confirm;
            Manager.Elements.Add(ConfirmButton);

            Texture2D tex = Resources.Menus.Customization.DescriptionScreen;
            DescriptionBox = new UIElement(this, tex,
                new Rectangle(5, 5, tex.Width, tex.Height));

            Manager.ClickAway += SelectPart;

            pointerTex = Resources.Menus.Title.Pointer;
            pointerDimensions = new Rectangle(0, 0, pointerTex.Width, pointerTex.Height);

            Camera.CustomizationSetup(MobileFortressClient.Game,new Vector3(0,1.5f,6));
        }
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyboard, Microsoft.Xna.Framework.Input.MouseState mouse)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            introTime += dt;
            pointerDimensions.Location = new Point(mouse.X, mouse.Y);
            shipRotation = Matrix.CreateRotationY(MathHelper.PiOver4 * introTime);
            Manager.Update(gameTime, mouse);
        }
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Gray);
            DrawShip();
            Viewport viewport = spriteBatch.GraphicsDevice.Viewport;
            Weapon1.Draw(spriteBatch);
            Weapon2.Draw(spriteBatch);
            Weapon3.Draw(spriteBatch);
            Weapon4.Draw(spriteBatch);
            Nose.Draw(spriteBatch);
            Core.Draw(spriteBatch);
            Engine.Draw(spriteBatch);
            DescriptionBox.Draw(spriteBatch);
            ConfirmButton.Draw(spriteBatch);
            PartData part;
            WeaponData weapon;
            switch (selectedPart)
            {
                case SelectedPart.Nose:
                    part = Ship.Nose;
                    if (selectedWeaponSlot > 0)
                        weapon = Ship.Weapons[Ship.Nose.WeaponSlots[selectedWeaponSlot - 1]];
                    else
                        weapon = null;
                    break;
                case SelectedPart.Core:
                    part = Ship.Core;
                    if (selectedWeaponSlot > 0)
                        weapon = Ship.Weapons[Ship.Core.WeaponSlots[selectedWeaponSlot - 1]];
                    else
                        weapon = null;
                    break;
                case SelectedPart.Tail:
                    part = Ship.Engine;
                    if (selectedWeaponSlot > 0)
                        weapon = Ship.Weapons[Ship.Engine.WeaponSlots[selectedWeaponSlot - 1]];
                    else
                        weapon = null;
                    break;
                default:
                    part = null;
                    weapon = null;
                    break;
            }
            if (part != null)
            {
                if (weapon != null)
                {
                    spriteBatch.DrawString(Resources.Menus.SmallDisplayFont, weapon.Description,
                            new Vector2(DescriptionBox.dimensions.X + 32 + 5, DescriptionBox.dimensions.Y + 5),
                            Color.White);
                    string stats = "Weight: " + weapon.Weight + " Ammo: " + weapon.MaxAmmo + " Rate of Fire: " + (1 / weapon.InverseRoF) * 60 + "RPM" + " Reload Time: " + weapon.ReloadTime + "s";
                    spriteBatch.DrawString(Resources.Menus.SmallDisplayFont, stats,
                        new Vector2(DescriptionBox.dimensions.X + 32 + 5, DescriptionBox.dimensions.Y + 16 + 5),
                        Color.White);
                }
                else
                {
                    spriteBatch.DrawString(Resources.Menus.SmallDisplayFont, part.Description,
                            new Vector2(DescriptionBox.dimensions.X + 32 + 5, DescriptionBox.dimensions.Y + 5),
                            Color.White);
                    string stats;
                    if (part == Ship.Core)
                        stats = "Capacity: ";
                    else
                        stats = "Weight: ";
                    stats += part.Weight + " Armor: " + part.Armor + " Equipment Slots: " + part.EquipmentSlots;
                    if (part.WeaponSlots != null) stats += " Weapon Slots: " + part.WeaponSlots.Length;
                    else stats += " Weapon Slots: 0";
                    spriteBatch.DrawString(Resources.Menus.SmallDisplayFont, stats,
                        new Vector2(DescriptionBox.dimensions.X + 32 + 5, DescriptionBox.dimensions.Y + 16 + 5),
                        Color.White);
                }
            }
            string globalStats = "Weight: " + Ship.TotalWeight + "/" + Ship.CapacityWeight + " Armor: " + Ship.TotalArmor + " Forward Vel: " + Ship.Thrust + " Strafe Vel: " + Ship.StrafeVelocity + " Turn Power: " + Ship.Turn;
            spriteBatch.DrawString(Resources.Menus.SmallDisplayFont, globalStats,
                new Vector2(DescriptionBox.dimensions.X + 32 + 5, DescriptionBox.dimensions.Y + DescriptionBox.dimensions.Height - 16 - 5),
                Color.White);
            spriteBatch.Draw(pointerTex, pointerDimensions, Color.White);
        }

        public void DrawShip()
        {
            GraphicsResource noseRes;
            GraphicsResource coreRes;
            GraphicsResource tailRes;
            Ship.SetDrawingResources(out noseRes, out coreRes, out tailRes);

            LightMaterial Material = Resources.gMaterials[0];

            DrawPart(coreRes, coreRes.Effect, Material, Vector3.Up*0.001f, Ship.CoreColor);
            DrawPart(noseRes, noseRes.Effect, Material, Vector3.Forward * 1.5f, Ship.NoseColor);
            DrawPart(tailRes, tailRes.Effect, Material, Vector3.Backward * 1.5f, Ship.TailColor);
            foreach (KeyValuePair<Vector3, WeaponData> kvp in Ship.Weapons)
            {
                if(kvp.Value != null)
                {
                    var res = Resources.GetResource((ushort)(Resources.WeaponIndex + kvp.Value.Index));
                    DrawPart(res, res.Effect, Material, kvp.Key, Ship.WeaponColor);
                }
            }
        }

        void DrawPart(GraphicsResource Part, Effect Effect, LightMaterial Mat, Vector3 Offset, Color Color)
        {
            if (Part.lightmap != null)
            {
                Effect.Parameters["xLightmap"].SetValue(Part.lightmap);
            }
            foreach (ModelMesh mesh in Part.model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Matrix world = mesh.ParentBone.Transform * Matrix.CreateTranslation(Offset) * Part.transform * shipRotation;
                    part.Effect = Effect;
                    Weather.SetStandardEffect(ref Effect, Mat, world);
                    if (Part.texture != null)
                    {
                        Effect.CurrentTechnique = Effect.Techniques["TTexSM2"];
                        Effect.Parameters["xTexture"].SetValue(Part.texture);
                    }
                    else
                        Effect.CurrentTechnique = Effect.Techniques["TSM2"];
                    Effect.Parameters["customColors"].SetValue(true);
                    Effect.Parameters["customColorA"].SetValue(Color.ToVector4());
                }
                mesh.Draw();
            }
            Effect.Parameters["customColors"].SetValue(false); //Don't make gray in the entire world change kthx.
        }

        public void SendOutputMessage()
        {
            MessageWriter.ShipDataOutputMessage(Ship);
        }
    }
}
