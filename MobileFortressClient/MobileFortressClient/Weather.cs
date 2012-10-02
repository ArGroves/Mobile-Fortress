using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileFortressClient
{
    class Weather
    {
        public static Vector3 SunPosition = new Vector3(0,-0.8f,-0.2f);
        public static Vector3 SunColor = new Vector3(.8f, .8f, .8f);
        public static float FogThickness = 0;
        public static Vector3 FogColor = new Vector3(0, 0.8f, 1);

        public static Color SkyColor = new Color(0, 0.8f, 1);

        public static void SetStandardEffect(ref Effect effect, LightMaterial material, Matrix world)
        {
            effect.Parameters["enableFog"].SetValue(true);
            effect.Parameters["fogStart"].SetValue(450f - FogThickness);
            effect.Parameters["fogEnd"].SetValue(700f - FogThickness);
            effect.Parameters["fogColor"].SetValue(FogColor);
            effect.Parameters["ViewPosition"].SetValue(Camera.Position);
            effect.Parameters["LightDir"].SetValue(SunPosition);
            effect.Parameters["LightColor"].SetValue(SunColor);

            effect.Parameters["materialEmissive"].SetValue(material.Emissive);
            effect.Parameters["materialAmbient"].SetValue(material.Ambient);
            effect.Parameters["materialDiffuse"].SetValue(material.Diffuse);
            effect.Parameters["materialSpecular"].SetValue(material.Specular);
            effect.Parameters["materialSpecPower"].SetValue(material.Power);

            effect.Parameters["World"].SetValue(world);
            effect.Parameters["WorldIT"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            effect.Parameters["ViewProjection"].SetValue(Camera.View * Camera.Projection);
        }

        public static void NightSettings()
        {
            SunPosition = new Vector3(0, -0.4f, -0.6f); //The moon in this case.
            SunColor = new Vector3(0.05f, 0.15f, 0.2f);
            SkyColor = new Color(0.06f, 0.06f, 0.15f);
            FogColor = SkyColor.ToVector3();
        }
        public static void DaySettings()
        {
            SunPosition = new Vector3(0,-0.8f,-0.2f);
            SunColor = new Vector3(.8f, .8f, .8f);
            SkyColor = new Color(0, 0.8f, 1);
            FogColor = SkyColor.ToVector3();
        }
        public static void Toggle()
        {
            if (SunColor.Y < 0.35f)
            {
                DaySettings();
            }
            else
            {
                NightSettings();
            }
        }
    }
}
