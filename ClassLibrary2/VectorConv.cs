using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XGamingM
{
    public class VectorConv
    {

        public static RaycastResult RotationToDirection(Entity Player, IntersectFlags intersectFlags)
        {
            Vector3 camPos = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_COORD);
            Vector3 camRot = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_ROT);

            double x = (Math.PI/180) * Convert.ToDouble(camRot.X);
            double y = (Math.PI/180) * Convert.ToDouble(camRot.Y);
            double z = (Math.PI/180) * Convert.ToDouble(camRot.Z);

            Vector3 adjustedRotation = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));

            x = -Math.Sin(adjustedRotation.Z) * Math.Abs(Math.Cos(adjustedRotation.X));
		    y = Math.Cos(adjustedRotation.Z) * Math.Abs(Math.Cos(adjustedRotation.X));
		    z = Math.Sin(adjustedRotation.X);

            Vector3 direction = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));

            x = camPos.X + direction.X * 300; 
		    y = camPos.Y + direction.Y * 300;
            z = camPos.Z + direction.Z * 300;

            Vector3 destination = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));

            RaycastResult rayResult = World.Raycast(camPos, destination, intersectFlags, Player);

            return rayResult;
        }

        internal static Vehicle getNearestVehicle()
        {
            Vector3 camPos = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_COORD);
            Vector3 camRot = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_ROT);

            double x = (Math.PI / 180) * Convert.ToDouble(camRot.X);
            double y = (Math.PI / 180) * Convert.ToDouble(camRot.Y);
            double z = (Math.PI / 180) * Convert.ToDouble(camRot.Z);

            Vector3 adjustedRotation = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));

            x = -Math.Sin(adjustedRotation.Z) * Math.Abs(Math.Cos(adjustedRotation.X));
            y = Math.Cos(adjustedRotation.Z) * Math.Abs(Math.Cos(adjustedRotation.X));
            z = Math.Sin(adjustedRotation.X);

            Vector3 direction = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));

            x = camPos.X + direction.X * 50;
            y = camPos.Y + direction.Y * 50;
            z = camPos.Z + direction.Z * 50;

            Vector3 destination = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));

            return World.GetClosestVehicle(destination, 20);
        }
    }
}
