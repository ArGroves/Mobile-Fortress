using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MobileFortressServer.Data
{
    class WeaponData
    {
        public float Weight;
        public float InverseRoF;
        public ushort MaxAmmo;
        public ushort CurrentAmmo;
        public ProjectileData Projectile;
        public ushort ProjectileID;
        public float ReloadTime;

        public float Cooldown = 0;

        public bool UseLockon = false;
        public float LockonRadius = 0;
        public float LockonTime = 0;
        public float CurrentLockonTime = 0;

        public byte fireGroup = 0;

        public static WeaponData[] WeaponTypes = new WeaponData[]
        {
            //Light MG
            new WeaponData(weight: 2, iRoF: 0.09f, ammo: 75, reloadtime: 4, projectile: 0),
            //Heavy MG
            new WeaponData(weight: 7, iRoF: 0.16f, ammo: 35, reloadtime: 4, projectile: 1),
            //20mm Cannon
            new WeaponData(weight: 18, iRoF: 0.35f, ammo: 10, reloadtime: 10, projectile: 2),
            //Rocket
            new WeaponData(weight: 15, iRoF: 0.3f, ammo: 16, reloadtime: 20, projectile: 3),
            //Small Missile
            new WeaponData(weight: 22, iRoF: 0.6f, ammo: 4, reloadtime: 30, projectile: 4,
                lockRadius: MathHelper.ToRadians(30), lockTime: 3),
            //Torpedo
            new WeaponData(weight: 25, iRoF: 0f, ammo: 1, reloadtime: 120, projectile: 5)
        };

        public WeaponData(float weight, float iRoF, ushort ammo, float reloadtime, ushort projectile)
        {
            Weight = weight;
            InverseRoF = iRoF;
            CurrentAmmo = MaxAmmo = ammo;
            ReloadTime = reloadtime;
            ProjectileID = projectile;
            Projectile = BulletData.ProjectileTable[projectile];
        }
        public WeaponData(float weight, float iRoF, ushort ammo, float reloadtime, ushort projectile,
            float lockRadius, float lockTime)
        {
            Weight = weight;
            InverseRoF = iRoF;
            CurrentAmmo = MaxAmmo = ammo;
            ReloadTime = reloadtime;
            ProjectileID = projectile;
            Projectile = BulletData.ProjectileTable[projectile];
            UseLockon = true;
            LockonRadius = lockRadius;
            LockonTime = lockTime;
        }

        public WeaponData Copy()
        {
            var copy = new WeaponData(Weight, InverseRoF, MaxAmmo, ReloadTime, ProjectileID);
            copy.fireGroup = 0;
            if (UseLockon)
                copy.UseLockon = true;
                copy.LockonRadius = LockonRadius;
                copy.LockonTime = LockonTime;
            return copy;
        }
    }
}
