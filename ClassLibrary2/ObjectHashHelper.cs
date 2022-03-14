using System;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using XGamingM;

namespace ObjectHashHelper
{
    public class ObjectHashHelper : Script
    {
        public Entity currLoc;
        public float by = 0;
        public bool interp = false;
        public float gtime = 0;
        public Vector3 asTarget;
        public ObjectHashHelper()
        {
            Tick += OnTick;
            KeyUp += OnKeyUp;
        }

        public static float Lerp(float start, float end, float amount)
        {
            return start + (end - start) * amount;
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            /*if (e.KeyCode == Keys.E && currLoc != null && currLoc.Model.Hash == -1286880215)
            {
            
            }*/
            if(e.KeyCode == Keys.E && currLoc != null && (currLoc.Model.Hash == 1286392437 || currLoc.Model.Hash == -1447681559 || currLoc.Model.Hash == 1801655140 || currLoc.Model.Hash == -1184516519 || currLoc.Model.Hash == 1230099731 || currLoc.Model.Hash == -2036241356 || currLoc.Model.Hash == -43433986) && interp == false)
            {
                //currLoc.Position = currLoc.Position + (currLoc.RightVector * 1);
                interp = true;
                if (currLoc.Model.Hash == 1801655140 || currLoc.Model.Hash == -1184516519 || currLoc.Model.Hash == 1230099731 || currLoc.Model.Hash == -43433986)
                {
                    asTarget = new Vector3(currLoc.Rotation.X, Lerp(currLoc.Rotation.Y, -90, by), currLoc.Rotation.Z);
                }
                else
                {
                    asTarget = currLoc.Position + (currLoc.RightVector * 6.75F);
                }
                gtime = Game.GameTime;
            }
        }

        public void OnTick(object sender, EventArgs e)
        {
            RaycastResult ent = VectorConv.RotationToDirection(Game.Player.Character, IntersectFlags.Objects);
            if (ent.HitEntity != null)
            {
                if (ent.HitEntity.IsRendered)
                {

                    PointF scv = GTA.UI.Screen.WorldToScreen(ent.HitEntity.AbovePosition);
                    TextElement txt = new TextElement("Object Hash : " + ent.HitEntity.Model.Hash.ToString(), scv, 0.5F, Color.Crimson);
                    txt.Draw();

                    PointF scs = GTA.UI.Screen.WorldToScreen(ent.HitEntity.AbovePosition);
                    scs.Y -= 20;
                    TextElement txv = new TextElement("Object Type : " + ent.HitEntity.EntityType.ToString(), scs, 0.5F, Color.Crimson);
                    txv.Draw();

                    

                    if(ent.HitEntity.Model.Hash == 1801655140 || ent.HitEntity.Model.Hash == 1286392437 || ent.HitEntity.Model.Hash == -1447681559 || ent.HitEntity.Model.Hash == -1184516519 || ent.HitEntity.Model.Hash == 1230099731 || ent.HitEntity.Model.Hash == -2036241356 || ent.HitEntity.Model.Hash == -43433986)
                    {
                        Point local = new Point((int)GTA.UI.Screen.WorldToScreen(ent.HitEntity.Position).X, (int)GTA.UI.Screen.WorldToScreen(ent.HitEntity.Position).Y);
                        CustomSprite OpenDoorsHack = new CustomSprite(AppDomain.CurrentDomain.BaseDirectory + "\\XGamingM\\opendoorshack.png", new Size(100, 38), local);
                        OpenDoorsHack.Draw();
                    }

                    if (!interp)
                    {
                        currLoc = ent.HitEntity;
                    }

                }
            }


            if (interp && Game.GameTime > gtime + 25)
            {
                if (currLoc.Model.Hash == 1286392437 || currLoc.Model.Hash == -1447681559 || currLoc.Model.Hash == -2036241356 || currLoc.Model.Hash == -43433986)
                {
                    currLoc.Position = Vector3.Lerp(currLoc.Position, (currLoc.Position + (currLoc.RightVector * 6.75F)), by);
                }
                else if (currLoc.Model.Hash == 1801655140 || currLoc.Model.Hash == -1184516519 || currLoc.Model.Hash == 1230099731)
                {
                    currLoc.Rotation = new Vector3(currLoc.Rotation.X, Lerp(currLoc.Rotation.Y, -90, by), currLoc.Rotation.Z);
                }
                by = by + 0.0001F;
                gtime = Game.GameTime;
                
            }
            if (currLoc != null && (((currLoc.Model.Hash == 1286392437 || currLoc.Model.Hash == -1447681559 || currLoc.Model.Hash == -2036241356 || currLoc.Model.Hash == -43433986) && World.GetDistance(currLoc.Position, asTarget) < float.Parse("0.5")) || ((currLoc.Model.Hash == 1801655140 || currLoc.Model.Hash == -1184516519 || currLoc.Model.Hash == 1230099731) && (currLoc.Rotation.Y <= -89.5 && currLoc.Rotation.Y >= -90))))
            {
                currLoc = null;
                interp = false;
                by = 0;
            }
        }
    }
}
