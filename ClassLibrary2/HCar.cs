using GTA;
using GTA.Native;
using GTA.UI;
using System;
using System.Drawing;
using WatchDogsTrial;

namespace XGamingM
{
    class HCar
    {
        public Vehicle target;
        private Ped currentPed;
        private float timer;
        private string hack;
        private float lastTime;
        public bool stopHack = false;
        private bool wantedOnTopHack = false;

        public HCar(Vehicle Target, float Timer, string Hack, bool bWantedOnTopHack = false)
        {
            target = Target;
            timer = Timer;
            lastTime = Game.GameTime;
            hack = Hack;
            wantedOnTopHack = bWantedOnTopHack;
        }

        public static void changeSeat(Ped ped)
        {
            if (ped.CurrentVehicle != null)
            {
                if (ped.CurrentVehicle.IsSeatFree(VehicleSeat.RightFront))
                {
                    ped.SetIntoVehicle(ped.CurrentVehicle, VehicleSeat.RightFront);
                }
                else if (ped.CurrentVehicle.IsSeatFree(VehicleSeat.LeftRear))
                {
                    ped.SetIntoVehicle(ped.CurrentVehicle, VehicleSeat.LeftRear);
                }
                else if (ped.CurrentVehicle.IsSeatFree(VehicleSeat.RightRear))
                {
                    ped.SetIntoVehicle(ped.CurrentVehicle, VehicleSeat.RightRear);
                }
                else if (ped.CurrentVehicle.IsSeatFree(VehicleSeat.ExtraSeat1))
                {
                    ped.SetIntoVehicle(ped.CurrentVehicle, VehicleSeat.ExtraSeat1);
                }
                else if (ped.CurrentVehicle.IsSeatFree(VehicleSeat.ExtraSeat2))
                {
                    ped.SetIntoVehicle(ped.CurrentVehicle, VehicleSeat.ExtraSeat2);
                }
                else if (ped.CurrentVehicle.IsSeatFree(VehicleSeat.ExtraSeat3))
                {
                    ped.SetIntoVehicle(ped.CurrentVehicle, VehicleSeat.ExtraSeat3);
                }
                else if (ped.CurrentVehicle.IsSeatFree(VehicleSeat.ExtraSeat4))
                {
                    ped.SetIntoVehicle(ped.CurrentVehicle, VehicleSeat.ExtraSeat4);
                }
            }
        }

        public void ApplyPreHack()
        {
            if (hack == "tirehack")
            {
                VehicleWheelCollection ct = target.Wheels;
                for (int i = 0; i < 6; i++)
                {
                    if (ct[i] != null)
                    {
                        ct[i].Burst();
                    }
                }
            }
            else if (hack == "speedhack")
            {
                target.EngineTorqueMultiplier = 300;
                target.EnginePowerMultiplier = 1000;
                if (target.Driver != null)
                {
                    currentPed = target.Driver;
                    changeSeat(currentPed);
                }
            }
            else if (hack == "speedreverthack")
            {
                target.EngineTorqueMultiplier = 300;
                target.EnginePowerMultiplier = 1000;
                if (target.Driver != null)
                {
                    currentPed = target.Driver;
                    changeSeat(currentPed);
                }
            }
            else if (hack == "turnlefthack")
            {
                if (target.Driver != null)
                {
                    currentPed = target.Driver;
                    changeSeat(currentPed);
                }
            }
            else if (hack == "turnrighthack")
            {
                if (target.Driver != null)
                {
                    currentPed = target.Driver;
                    changeSeat(currentPed);
                }
            }
            else if(hack == "explodehack")
            {
                if (target.Model.IsTrain)
                {
                    Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, target, true);
                    Function.Call(Hash.SET_VEHICLE_CAN_BE_VISIBLY_DAMAGED, target, true);
                    Function.Call(Hash.SET_VEHICLE_CAN_BREAK, target, true);
                    Function.Call(Hash.SET_VEHICLE_STRONG, target, false);

                    Function.Call(Hash.SET_ENTITY_HEALTH, target, .5); // Set the train's health to half of its original value, this is so it takes one explosion rather than two to stop
                    Function.Call(Hash.SET_ENTITY_INVINCIBLE, target, false);
                    Function.Call(Hash.SET_ENTITY_PROOFS, target, true, true, false, false, true, false, false, false);
                    Function.Call(Hash.SET_ENTITY_HAS_GRAVITY, target, true);

                    target.Explode();
                }
                else
                {
                    target.Explode();
                }

                if(wantedOnTopHack == true)
                {
                    Game.Player.DispatchsCops = true;
                    Game.Player.WantedLevel = Game.Player.WantedLevel + 1;
                }

            }
            else if (hack == "burnouthack")
            {
                target.EngineTorqueMultiplier = 300;
                target.EnginePowerMultiplier = 1000;
                if (target.Driver != null)
                {
                    currentPed = target.Driver;
                    changeSeat(currentPed);
                }
            }
            else if (hack == "repairhack")
            {
                target.Repair();
                target.Wash();
                target.EngineHealth = 1000.0F;
                target.IsEngineRunning = true;
                target.IsDriveable = true;
                target.HealthFloat = 1000.0F;
                target.LockStatus = VehicleLockStatus.Unlocked;
            }
            else if (hack == "removehack")
            {
                target.Delete();
            }
            else if (hack == "wantedhack")
            {
                target.IsWanted = true;
            }
            else if (hack == "lockhack")
            {
                target.LockStatus = VehicleLockStatus.CannotEnter;
            }
            else if (hack == "unlockhack")
            {
                target.LockStatus = VehicleLockStatus.Unlocked;
            }
            else if (hack == "stophack" || hack == "stopfirehack")
            {
                if (target.Driver != null)
                {
                    currentPed = target.Driver;
                    changeSeat(currentPed);
                }
            }
        }

        public Vehicle GetVehicle()
        {
            return target;
        }

        public string GetHack()
        {
            return hack;
        }

        public void forceStopHack()
        {
            target.BrakePower = 0.0f;
            target.ThrottlePower = 0.0f;
            target.EngineTorqueMultiplier = 1.0f;
            target.EnginePowerMultiplier = 1.0f;
        }

        public void ApplyForcedHack()
        {
            if (target != null && stopHack == false)
            {
                if (Game.GameTime < lastTime + timer)
                {
                    if (hack == "speedhack")
                    {
                        target.BrakePower = -1.0f;
                        target.ThrottlePower = 1.0f;
                    }
                    else if (hack == "speedreverthack")
                    {
                        target.BrakePower = -1.0f;
                        target.ThrottlePower = -1.0f;
                    }
                    else if (hack == "turnlefthack")
                    {
                        target.SteeringAngle = 22;
                        target.BrakePower = -1.0f;
                        target.ThrottlePower = 0.45f;
                    }
                    else if (hack == "turnrighthack")
                    {
                        target.SteeringAngle = -22;
                        target.BrakePower = -1.0f;
                        target.ThrottlePower = 0.45f;
                    }
                    else if (hack == "burnouthack")
                    {
                        target.BrakePower = 1.0f;
                        target.ThrottlePower = 1.0f;
                    }
                    else if (hack == "stophack")
                    {
                        Point local = new Point((int)GTA.UI.Screen.WorldToScreen(target.Position).X, (int)GTA.UI.Screen.WorldToScreen(target.AbovePosition).Y);
                        CustomSprite carcurHack = new CustomSprite(AppDomain.CurrentDomain.BaseDirectory + "\\XGamingM\\currentlyHacked.png", new Size(38, 38), local);
                        carcurHack.Draw();
                        target.HandlingData.BrakeForce = 10000.0f;
                        target.BrakePower = 1.0f;
                        target.ThrottlePower = 0.0f;
                    }
                    else if (hack == "stopfirehack") {
                        Point local = new Point((int)GTA.UI.Screen.WorldToScreen(target.Position).X, (int)GTA.UI.Screen.WorldToScreen(target.AbovePosition).Y);
                        CustomSprite carcurHack = new CustomSprite(AppDomain.CurrentDomain.BaseDirectory + "\\XGamingM\\currentlyFireHacked.png", new Size(38, 38), local);
                        carcurHack.Draw();
                        target.HandlingData.BrakeForce = 10000.0f;
                        target.BrakePower = 1.0f;
                        target.ThrottlePower = 0.0f;
                        EntityBoneCollection entityBones = target.Bones;
                        int engineBoneId = LivingWorld.getEntityBoneIndexByName(target, "engine");
                        EntityBone engineBone = entityBones[engineBoneId];
                    }
                }
                else
                {
                    if (currentPed != null && currentPed.CurrentVehicle != null)
                    {
                        currentPed.SetIntoVehicle(currentPed.CurrentVehicle, VehicleSeat.Driver);
                        currentPed = null;
                    }
                    stopHack = true;
                }
            }
        }
    }
}
