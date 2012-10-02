using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using MobileFortressClient.Data;
using MobileFortressClient.Physics;
using Microsoft.Xna.Framework.Audio;
using MobileFortressClient.Particles;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;

namespace MobileFortressClient
{
    class Resources
    {
        public static SpriteFont defaultFont;
        public static Boolean isLoaded { get; private set; }
        static Matrix Rotate90 = Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2);
        static Matrix Scale2 = Matrix.CreateScale(2f);

        public const float AudioPositionQuotient = 10;

        public const int ShipPartIndex = 6;
        public const int WeaponIndex = 16;
        public const int ProjectileIndex = 21;

        static GraphicsResource[] gResources = new GraphicsResource[]
        {
            new GraphicsResource(0,"Models/Terrain/cube",Matrix.CreateScale(750,5,750)),
            new GraphicsResource(3,"Models/Projectile/SphericalExplosion","Textures/Explosion"),
            new GraphicsResource(0,"Models/Terrain/BuildingPiece","Textures/Building"),
            new GraphicsResource(0,"Models/Terrain/BuildingPiece","Textures/Building2"),
            new GraphicsResource(0,"Models/Terrain/BuildingPiece","Textures/Frame"),
            new GraphicsResource(0,"Models/Person","Textures/Person"),
            new GraphicsResource(1,"Models/Ship/TexLightCore", "Textures/Part/LightCore"),
            new GraphicsResource(1,"Models/Ship/TexLightNose", "Textures/Part/LightNose"),
            new GraphicsResource(4,"Models/Ship/TexTwinTurbine", "Textures/Part/TwinEngine","Textures/Lightmap/TwinEngineLightmap"),
            new GraphicsResource(1,"Models/Ship/TexCore", "Textures/Part/MedOctCore"),
            new GraphicsResource(1,"Models/Ship/TexSpaceNose", "Textures/Part/SpaceNose"),
            new GraphicsResource(4,"Models/Ship/TexSoloEngine", "Textures/Part/SoloEngine","Textures/Lightmap/SoloEngineLightmap"),
            new GraphicsResource(1,"Models/Ship/TexHeavyCore", "Textures/Part/HeavyCore"),
            new GraphicsResource(4,"Models/Ship/TexRocket", "Textures/Part/RocketEngine","Textures/Lightmap/RocketEngineLightmap"),
            new GraphicsResource(1,"Models/Ship/TexShieldNose", "Textures/Part/ShieldNose"),
            new GraphicsResource(1,"Models/Ship/VNose","Textures/Part/VNose"),
            new GraphicsResource(0,"Models/Weapon/LightMG", "Textures/Weapon/LightMG"),
            new GraphicsResource(0,"Models/Weapon/HeavyMG", "Textures/Weapon/HeavyMG"),
            new GraphicsResource(0,"Models/Weapon/Autocannon", "Textures/Weapon/Autocannon"),
            new GraphicsResource(0,"Models/Weapon/RocketPod", "Textures/Weapon/RocketPod"),
            new GraphicsResource(0,"Models/Weapon/MissileLauncher", "Textures/Weapon/MissileLauncher"),
            new GraphicsResource(0,"Models/Weapon/Bomb","Textures/Weapon/Bomb"),
            new GraphicsResource(0,"Models/Projectile/Missile","Textures/Weapon/Missile"),
            new GraphicsResource(0,"Models/Weapon/Bomb", "Textures/Weapon/Bomb")
        };

        static SpriteResource[] sResources = new SpriteResource[]
        {
            new SpriteResource("Sprites/Particle/bullet"),
            new SpriteResource("Sprites/Particle/engine"),
            new SpriteResource("Sprites/Particle/explosion"),
            new SpriteResource("Sprites/Particle/smoke")
        };

        public static Effect[] gEffects;

        public static LightMaterial[] gMaterials = new LightMaterial[]
        {
            new LightMaterial(Vector3.Zero, new Vector3(.2f,.2f,.2f),new Vector4(.7f,.7f,.7f,1),new Vector3(.9f,.9f,.9f),4),
            new LightMaterial(Vector3.Zero, new Vector3(.1f,.1f,.1f),new Vector4(.5f,.5f,.5f,1),Vector3.One,5),
            new LightMaterial(Vector3.Zero, Vector3.One, Vector4.One)
        };

        public static void Load(ContentManager Content)
        {
            defaultFont = Content.Load<SpriteFont>("Default");
            gEffects = new Effect[]
            {
                Content.Load<Effect>("Diffuse"),
                Content.Load<Effect>("Phong"),
                Content.Load<Effect>("Island"),
                Content.Load<Effect>("NoLighting"),
                Content.Load<Effect>("Lightmap")
            };
            PBullet.Initialize(Content);
            Sounds.Load(Content);
            Menus.Load(Content);
            Island.Load(Content);
            for(int i = 0; i < gResources.Length; i++)
            {
                gResources[i].Load(Content);
            }
            for (int i = 0; i < sResources.Length; i++)
            {
                sResources[i].Load(Content);
            }
            isLoaded = true;
        }

        public static GraphicsResource GetResource(UInt16 index)
        {
            int i = (int)index;
            if (i > gResources.Length - 1) throw new ApplicationException("Resource index out of bounds.");
            return gResources[i];
        }

        public static SpriteResource GetSprite(UInt16 index)
        {
            int i = (int)index;
            if (i > sResources.Length - 1) throw new ApplicationException("Sprite index out of bounds.");
            return sResources[i];
        }

        public static Entity GetEntity(Vector3 position, UInt16 index)
        {
            switch (index)
            {
                case 0: //Terrain
                    return new Box(position, 750, 5, 750);
                case 1: //Building
                    return new Box(position, 30, 16, 30);
                case 2: //UL Core
                    Entity shape = new CompoundBody(
                        new List<CompoundShapeEntry>
                        {
                            new CompoundShapeEntry(new BoxShape(.85f * 2, .85f * 2, 1 * 2),float.MaxValue),
                            new CompoundShapeEntry(new BoxShape(4.4f * 2, 0.05f * 2, 1 * 2),new Vector3(0,0.85f,0))
                        }, 50
                        );
                    shape.Position = position;
                    return shape;
                case 3: //UL Nose
                    return new Box(position, .75f * 2, 1f * 2, .75f * 2, 20);
                case 4: //UL Engine
                    return new Box(position, .6f * 2, .95f * 2, 0.425f * 2, 20);
                case 5: //L Core
                    shape = new CompoundBody(
                        new List<CompoundShapeEntry>
                        {
                            new CompoundShapeEntry(new BoxShape(2, 2, 2),float.MaxValue),
                            new CompoundShapeEntry(new BoxShape(8, 2, 0.2f),new Vector3(0,0,0))
                        }, 50
                        );
                    shape.Position = position;
                    return shape;
                case 6: //L Nose
                    return new Box(position, 1.35f, 1.6f, 1.6f);
                case 7: //L Engine
                    return new Box(position, 1.8f, 1.4f, 1.05f);
                default:
                    return new Box(position, 1, 1, 1);
            }
        }

        public class Sounds
        {
            static Random sRandom = new Random();
            public static SoundEffect Engine;
            public static SoundEffect LightMG;
            public static SoundEffect HeavyMG;
            public static SoundEffect Cannon;
            public static SoundEffect Rocket;
            public static SoundEffect RocketEngine;
            public static SoundEffect[] bulletHit = new SoundEffect[3];
            public static SoundEffect[] shipExplosion = new SoundEffect[3];
            public static SoundEffect[] explosion = new SoundEffect[3];
            public static SoundEffect Reload;

            public static SoundEffect[] WeaponSounds = new SoundEffect[4];

            public static SoundEffect Latch;

            public static SoundEffect LockingOn;
            public static SoundEffect LockedOn;
            public static SoundEffect EnemyLockon;
            public static SoundEffect EnemyMissile;

            public static void Load(ContentManager Content)
            {
                LightMG = Content.Load<SoundEffect>("Sounds/Weapons/LightMG");
                HeavyMG = Content.Load<SoundEffect>("Sounds/Weapons/HeavyMG");
                Cannon = Content.Load<SoundEffect>("Sounds/Weapons/Cannon");
                Rocket = Content.Load<SoundEffect>("Sounds/Weapons/MissileTube");
                WeaponSounds[0] = LightMG;
                WeaponSounds[1] = HeavyMG;
                WeaponSounds[2] = Cannon;
                WeaponSounds[3] = Rocket;

                Engine = Content.Load<SoundEffect>("Sounds/Engine");
                RocketEngine = Content.Load<SoundEffect>("Sounds/Weapons/MissileFlight");
                Reload = Content.Load<SoundEffect>("Sounds/Weapons/Reload");

                Latch = Content.Load<SoundEffect>("Sounds/Latch");
                LockingOn = Content.Load<SoundEffect>("Sounds/Lockons/LockingOn");
                LockedOn = Content.Load<SoundEffect>("Sounds/Lockons/LockedOn");
                EnemyLockon = Content.Load<SoundEffect>("Sounds/Lockons/EnemyLockon");
                EnemyMissile = Content.Load<SoundEffect>("Sounds/Lockons/EnemyMissile");


                for (byte i = 0; i < 3; i++)
                {
                    bulletHit[i] = Content.Load<SoundEffect>("Sounds/Explosions/BulletHitsMetal" + (i + 1));
                    shipExplosion[i] = Content.Load<SoundEffect>("Sounds/Explosions/LargeExplosion" + (i + 1));
                    explosion[i] = Content.Load<SoundEffect>("Sounds/Explosions/MissileExplode" + (i + 1));
                }
            }

            public static SoundEffect BulletHit()
            {
                return Select(bulletHit);
            }
            public static SoundEffect ShipExplosion()
            {
                return Select(shipExplosion);
            }
            public static SoundEffect Explosion()
            {
                return Select(explosion);
            }
            public static SoundEffect Select(SoundEffect[] sounds)
            {
                return sounds[sRandom.Next(0, sounds.Length)];
            }
        }

        public class Menus
        {
            public class Title
            {
                public static Texture2D HalcyonLogo;
                public static Texture2D Tile;
                public static Texture2D TitleBar;
                public static Texture2D LoginButton;
                public static Texture2D OptionsButton;
                public static Texture2D ExitButton;
                public static Texture2D LoginConfirmButton;
                public static Texture2D Pointer;
                public static Texture2D LoginMenu;
                public static Texture2D TextBox;

                public static SoundEffect MenuSelect;
                public static SoundEffect MenuConfirm;
                public static SoundEffect MenuDeny;
                public static SoundEffect MenuCancel;

                public static void Load(ContentManager Content)
                {
                    HalcyonLogo = Content.Load<Texture2D>("Menu/TitleScreen/HalcyonLogo");
                    Tile = Content.Load<Texture2D>("Menu/TitleScreen/Tile");
                    TitleBar = Content.Load<Texture2D>("Menu/TitleScreen/TitleBar");
                    LoginButton = Content.Load<Texture2D>("Menu/TitleScreen/login");
                    OptionsButton = Content.Load<Texture2D>("Menu/TitleScreen/options");
                    ExitButton = Content.Load<Texture2D>("Menu/TitleScreen/exit");
                    LoginConfirmButton = Content.Load<Texture2D>("Menu/TitleScreen/loginkey");
                    Pointer = Content.Load<Texture2D>("Menu/TitleScreen/Pointer");
                    LoginMenu = Content.Load<Texture2D>("Menu/TitleScreen/loginmenu");
                    TextBox = Content.Load<Texture2D>("Menu/TitleScreen/textbox");

                    MenuSelect = Content.Load<SoundEffect>("Menu/TitleScreen/UIselect");
                    MenuConfirm = Content.Load<SoundEffect>("Menu/TitleScreen/UIConfirm");
                    MenuDeny = Content.Load<SoundEffect>("Menu/TitleScreen/UIDeny");
                    MenuCancel = Content.Load<SoundEffect>("Menu/TitleScreen/UICancel");
                }
            }
            public class HUD
            {
                public static Texture2D Crosshairs;
                public static Texture2D Arrows;
                public static Texture2D Reticle;

                public static Texture2D[] WeaponIcon = new Texture2D[6];

                public static Texture2D NoseHealth;
                public static Texture2D CoreHealth;
                public static Texture2D EngineHealth;
                public static void Load(ContentManager Content)
                {
                    Crosshairs = Content.Load<Texture2D>("Menu/ShipHUD/Crosshairs");
                    Arrows = Content.Load<Texture2D>("Menu/ShipHUD/Arrows");
                    Reticle = Content.Load<Texture2D>("Menu/ShipHUD/Reticle");

                    NoseHealth = Content.Load<Texture2D>("Menu/ShipHUD/NoseHealth");
                    CoreHealth = Content.Load<Texture2D>("Menu/ShipHUD/CoreHealth");
                    EngineHealth = Content.Load<Texture2D>("Menu/ShipHUD/EngineHealth");

                    for (byte i = 0; i < WeaponIcon.Length; i++)
                    {
                        WeaponIcon[i] = Content.Load<Texture2D>("Menu/ShipHUD/Weapon"+i);
                    }
                }
            }
            public class Customization
            {
                public static Texture2D WeaponSelector;
                public static Texture2D UpArrow;
                public static Texture2D DownArrow;
                public static Texture2D DescriptionScreen;

                public static void Load(ContentManager Content)
                {
                    WeaponSelector = Content.Load<Texture2D>("Menu/Customization/WeaponSelector");
                    UpArrow = Content.Load<Texture2D>("Menu/Customization/UpArrow");
                    DownArrow = Content.Load<Texture2D>("Menu/Customization/DownArrow");
                    DescriptionScreen = Content.Load<Texture2D>("Menu/Customization/DescriptionScreen");
                }
            }

            public static Texture2D ButtonLeft;
            public static Texture2D ButtonCenter;
            public static Texture2D ButtonRight;

            public static Texture2D DisplayLeft;
            public static Texture2D DisplayCenter;
            public static Texture2D DisplayRight;

            public static SpriteFont ButtonFont;
            public static SpriteFont SmallDisplayFont;
            public static SpriteFont BaroqueScript16;

            public static Texture2D ButtonSquare;

            public static void Load(ContentManager Content)
            {
                Title.Load(Content);
                HUD.Load(Content);
                Customization.Load(Content);  
                ButtonLeft = Content.Load<Texture2D>("Menu/ButtonLeft");
                ButtonCenter = Content.Load<Texture2D>("Menu/ButtonCenter");
                ButtonRight = Content.Load<Texture2D>("Menu/ButtonRight");
                ButtonFont = Content.Load<SpriteFont>("UI");
                DisplayLeft = Content.Load<Texture2D>("Menu/DisplayLeft");
                DisplayCenter = Content.Load<Texture2D>("Menu/DisplayCenter");
                DisplayRight = Content.Load<Texture2D>("Menu/DisplayRight");
                SmallDisplayFont = Content.Load<SpriteFont>("SmallUI");
                ButtonSquare = Content.Load<Texture2D>("Menu/ButtonSquare");
                BaroqueScript16 = Content.Load<SpriteFont>("BaroqueScript");
            }
        }

        public class Island
        {
            public static Texture2D OceanTex;
            public static Texture2D SeafloorTex;
            public static Texture2D PlainTex;
            public static Texture2D SandTex;
            public static Texture2D SnowTex;
            public static void Load(ContentManager Content)
            {
                OceanTex = Content.Load<Texture2D>("Textures/Island/Ocean");
                SeafloorTex = Content.Load<Texture2D>("Textures/Island/Mud");
                PlainTex = Content.Load<Texture2D>("Textures/Island/Ground");
                SandTex = Content.Load<Texture2D>("Textures/Island/Sand");
                SnowTex = Content.Load<Texture2D>("Textures/Island/Snow");
            }
        }

    }
    class GraphicsResource
    {
        public Model model { get; private set; }
        public Texture2D texture { get; private set; }
        public Texture2D lightmap { get; private set; }
        public Matrix transform { get; set; }
        public Effect Effect;
        int effectType;
        String modelFile;
        String textureFile;
        String lightMapFile;

        public void Load(ContentManager Content)
        {
            model = Content.Load<Model>(modelFile);
            Effect = Resources.gEffects[effectType];
            if (textureFile != null) texture = Content.Load<Texture2D>(textureFile);
            if (lightMapFile != null) lightmap = Content.Load<Texture2D>(lightMapFile);
        }
        public GraphicsResource(int effect, String modelfile)
        {
            effectType = effect;
            modelFile = modelfile;
            this.transform = Matrix.Identity;
        }
        public GraphicsResource(int effect, String modelfile, String texturefile)
        {
            effectType = effect;
            modelFile = modelfile;
            textureFile = texturefile;
            this.transform = Matrix.Identity;
        }
        public GraphicsResource(int effect, String modelfile, String texturefile, String lightmap)
        {
            effectType = effect;
            modelFile = modelfile;
            textureFile = texturefile;
            lightMapFile = lightmap;
            this.transform = Matrix.Identity;
        }
        public GraphicsResource(int effect, String modelfile, Matrix transform)
        {
            effectType = effect;
            modelFile = modelfile;
            this.transform = transform;
        }
        public GraphicsResource(int effect, String modelfile, Matrix transform, String texturefile)
        {
            effectType = effect;
            modelFile = modelfile;
            textureFile = texturefile;
            this.transform = transform;
        }
    }

    struct LightMaterial
    {
        public Vector3 Emissive;
        public Vector3 Ambient;
        public Vector4 Diffuse;
        public Vector3 Specular;
        public float Power;

        public LightMaterial(Vector3 emissive, Vector3 ambient, Vector4 diffuse, Vector3 specular, float power)
        {
            Emissive = emissive;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            Power = power;
        }
        public LightMaterial(Vector3 emissive, Vector3 ambient, Vector4 diffuse)
        {
            Emissive = emissive;
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = Vector3.Zero;
            Power = 0;
        }
    }

    class SpriteResource
    {
        public string spriteFile;
        public Texture2D sprite;
        public bool isLoaded { get { return sprite != null; } }
        public SpriteResource(string file)
        {
            spriteFile = file;
        }
        public void Load(ContentManager Content)
        {
            sprite = Content.Load<Texture2D>(spriteFile);
        }
    }
}
