using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;

namespace MobileFortressServer.Data
{
    class PartData
    {
        string Name;
        string Description;
        ushort resourceID;
        public Entity Hitbox;
        public float Weight;
        public Vector3[] WeaponSlots;
        public int EquipmentSlots;
        public float Armor;

        //Core
        public int Turn = 0;
        public Vector3 WingVector = Vector3.Zero;

        //Engine
        public int Thrust = 0;
        public int Strafe = 0;

        public static PartData[] Noses = new PartData[]
        {
            new PartData(name:"Basic",
                desc: "The most basic type of nose, with little to no pilot protection.",
                resource: Resources.ShipPartIndex+1, weight: 18, armor: 25, equipmentSlots: 0),
            new PartData(name:"Spacious",
                desc: "Contains space for one passenger and equipment.",
                resource: Resources.ShipPartIndex+4, weight: 25, armor: 45, equipmentSlots: 1),
            new PartData(name:"Shield",
                desc: "Maximum armor at the expense of weight and lack of equipment.",
                resource: Resources.ShipPartIndex+8, weight: 35, armor:70),
            new PartData(name:"Wedge",
                desc: "Carries a centrally aligned weapon.",
                resource: Resources.ShipPartIndex+9, weight: 40, armor: 30, equipmentSlots: 0,
                weapons: new Vector3[]{new Vector3(0,-0.7f,-2.4f)})
        };
        public static PartData[] Cores = new PartData[]
        {
            new PartData(name:"Raised Wing L",
                desc: "Armored dive bombing core. Has only one weapon slot.",
                resource: Resources.ShipPartIndex+0, weight: 55, armor: 225,
                weapons: new Vector3[]{new Vector3(0,1.27f,0)},
                turn: 2, wingheight: 1),
            new PartData(name:"Med Oct",
                desc: "Two weapon slots and good carrying capacity.",
                resource: Resources.ShipPartIndex+3, weight: 70, armor: 125, equipmentSlots: 1,
                weapons: new Vector3[]{new Vector3(-1.36f,-0.93f,0), new Vector3(1.36f,-0.93f,0)},
                turn: 3),
            new PartData(name:"Raised Wing H",
                desc: "Huge capacity and 4 weapon slots. Flies like a brick.",
                resource: Resources.ShipPartIndex+6, weight: 110, armor: 85, equipmentSlots: 2,
                weapons: new Vector3[]{new Vector3(-1.65f,0.48f,0f), new Vector3(1.65f,0.48f,0f),
                new Vector3(-4.27f,0.48f,0f), new Vector3(4.27f,0.48f,0f)}, 
                thrust: -10, strafe: -5, turn:1, wingspan:10, wingheight:1)
        };
        public static PartData[] Engines = new PartData[]
        {
            new PartData(name:"Dual Turbine",
                desc: "Two equipment slots in exchange for limited strafe ability.",
                resource: Resources.ShipPartIndex+2, weight: 30, armor: 75, equipmentSlots: 2, thrust: 35, strafe: 8),
            new PartData(name:"Solo RE",
                desc: "Lighter engine with good strafing, but less thrust.",
                resource: Resources.ShipPartIndex+5, weight: 25, armor: 100, equipmentSlots: 1, thrust: 25, strafe:18),
            new PartData(name:"Rocket",
                desc: "Enormous amounts of forward thrust with no strafing.",
                resource: Resources.ShipPartIndex+7, weight: 35, armor: 60, equipmentSlots: 0, thrust: 60)
        };

        public PartData Copy()
        {
            PartData copy = new PartData(Name, Description, resourceID, Weight, Armor, EquipmentSlots, null, Turn, Thrust, Strafe);
            if (WeaponSlots != null)
            {
                copy.WeaponSlots = new Vector3[WeaponSlots.Length];
                for (int i = 0; i < WeaponSlots.Length; i++)
                    copy.WeaponSlots[i] = WeaponSlots[i];
            }
            copy.Hitbox = Resources.GetEntity(Vector3.Zero, resourceID);
            return copy;
        }

        public PartData(string name, string desc, ushort resource, float weight,
            float armor, int equipmentSlots = 0, Vector3[] weapons = null,
            int turn = 0, int thrust = 0, int strafe = 0, float wingspan = 8f, float wingheight = 0f, float winglength = 2f)
        {
            Name = name;
            Description = desc;
            Weight = weight;
            Armor = armor;
            EquipmentSlots = equipmentSlots;
            resourceID = resource;
            WeaponSlots = weapons;
            Turn = turn;
            Thrust = thrust;
            Strafe = strafe;
            WingVector = new Vector3(wingspan, wingheight, winglength);
        }
    }
}
