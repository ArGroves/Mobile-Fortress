using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileFortressClient.Data
{
    class WeaponData
    {
        public float Weight;
        public float InverseRoF;
        public ushort MaxAmmo;
        public ProjectileData Projectile;
        public ushort ProjectileID;
        public float ReloadTime;

        public string Description;
        public string Name;
        public int Index;
        public byte fireGroup = 0;

        public bool Draw = true;

        public static WeaponData[] WeaponTypes = new WeaponData[]
        {
            new WeaponData(index:0, name: "Light MG",
                desc:"Light, direct-fire automatic weapon." ,
                weight: 2, iRoF: 0.09f, ammo: 75, reloadtime: 4, projectile: 0),
            new WeaponData(index:1, name: "Heavy MG",
                desc:"Heavier and more powerful than it's light counterpart, but less ammo.",
                weight: 7, iRoF: 0.16f, ammo: 35, reloadtime: 4, projectile: 1),
            new WeaponData(index:2, name: "20mm Cannon",
                desc:"A powerful yet slow-firing automatic cannon." ,
                weight: 18, iRoF: 0.35f, ammo: 10, reloadtime: 10, projectile: 2),
            new WeaponData(index:3, name: "Rocket Pod",
                desc:"Dumb-fire explosive rockets with a large splash radius.",
                weight: 15, iRoF: 0.6f, ammo: 16, reloadtime: 20, projectile: 3),
            new WeaponData(index:4, name: "Small Missiles",
                desc: "Homing missiles that track a target.",
                weight: 22, iRoF: 0.6f, ammo: 4, reloadtime: 30, projectile: 3),
            new WeaponData(index:5, name: "Torpedo",
                desc: "Single shot anti-fortress torpedo. Make it count.",
                weight: 25, iRoF: 1f, ammo: 1, reloadtime: 120, projectile: 4)
        };

        public WeaponData(int index, string name, string desc, float weight, float iRoF, ushort ammo, float reloadtime, ushort projectile)
        {
            Index = index;
            Name = name;
            Description = desc;
            Weight = weight;
            InverseRoF = iRoF;
            MaxAmmo = ammo;
            ReloadTime = reloadtime;
            ProjectileID = projectile;
            Projectile = ProjectileData.ProjectileTable[projectile];
        }

        public WeaponData Copy()
        {
            var copied = new WeaponData(Index,Name,Description,Weight,InverseRoF,MaxAmmo,ReloadTime,ProjectileID);
            copied.fireGroup = 0;
            return copied;
        }
    }
}
