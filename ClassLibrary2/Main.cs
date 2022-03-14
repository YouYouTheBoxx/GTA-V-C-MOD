using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime;
using GTA;
using GTA.Math;
using GTA.UI;
using GTA.Native;
using System.Collections.Generic;
using XGamingM;
using LemonUI.Menus;
using LemonUI;

namespace WatchDogsTrial
{
    public class LivingWorld : Script
    {

        private readonly ObjectPool pool = new ObjectPool();
        private readonly NativeMenu GeneralMenu = new NativeMenu("Hackers", "Hacking nothing!");
        private readonly NativeMenu SubMenuWorld = new NativeMenu("World Overriding", "Override nothing!");
        private readonly NativeMenu SubMenuHack = new NativeMenu("Hack configuration", "Configure nothing!");

        private bool bClearArea = false;
        private bool bWanted4Explosion = true;

        public bool isHacking = false;
        public bool pedMakesErrors = false;
        public bool canHack = false;
        public Vehicle[] arrVehicle;
        public Ped[] CurrentPeds;
        public Vehicle currentveh;
        public Entity trafficEntity;
        List<HCar> allvehs = new List<HCar>();
        public bool setTrafficLightsOff = false;
        public float gctime;
        public float nextT = 0;
        public string nextHack = "traffichack";
        public string carHack = "tirehack";
        public string hacktype = "";
        public bool isCarHack = false;

        private int targetCarTime;
        private int targetPedTime;

        public static uint Clamp(uint value, uint min, uint max)
        {
            uint cvalue = value;
            if (cvalue < min)
            {
                cvalue = min;
            }
            else if (cvalue > max)
            {
                cvalue = max;
            }
            return cvalue;
        }

        private void registerMenu()
        {
            pool.Add(GeneralMenu);
            pool.Add(SubMenuWorld);
            pool.Add(SubMenuHack);

            NativeSubmenuItem sbWorld = GeneralMenu.AddSubMenu(SubMenuWorld);
            sbWorld.Title = "World Overrides";

            NativeCheckboxItem clearArea = new NativeCheckboxItem("Clear Area");
            SubMenuWorld.Add(clearArea);
            clearArea.CheckboxChanged += new EventHandler(clearAreaCheck);

            NativeSubmenuItem sbHack = GeneralMenu.AddSubMenu(SubMenuHack);
            sbHack.Title = "Hack configuration";

            NativeCheckboxItem wantedExplosion = new NativeCheckboxItem("Get Wanted on Explode hack");
            wantedExplosion.Checked = true;
            SubMenuHack.Add(wantedExplosion);
            wantedExplosion.CheckboxChanged += new EventHandler(wantedExplosionCheck);

        }

        private void clearAreaCheck(object sender, System.EventArgs e)
        {
            NativeCheckboxItem checkboxItem = (NativeCheckboxItem)sender;
            if (checkboxItem.Checked == true)
            {
                bClearArea = true;
            }
            else
            {
                bClearArea = false;
            }
        }

        private void wantedExplosionCheck(object sender, System.EventArgs e)
        {
            NativeCheckboxItem checkboxItem = (NativeCheckboxItem)sender;
            if (checkboxItem.Checked == true)
            {
                bWanted4Explosion = true;
            }
            else
            {
                bWanted4Explosion = false;
            }
        }

        public LivingWorld()
        {
            Tick += OnTick;
            KeyUp += OnKeyUp;

            registerMenu();
        }

        public static void pedOverride(Ped ped, bool boverride = false)
        {
            Function.Call(Hash.SET_PED_ALLOW_VEHICLES_OVERRIDE, ped, boverride);
            Function.Call(Hash.KNOCK_PED_OFF_VEHICLE, ped);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.M) {
                GeneralMenu.Open();
            }
            if (e.KeyCode == Keys.R && currentveh != null && canHack)
            {
                if (carHack == "unlockhack")
                {
                    carHack = "speedhack";
                }
                else if (carHack == "speedhack")
                {
                    carHack = "speedreverthack";
                }
                else if (carHack == "speedreverthack")
                {
                    carHack = "turnlefthack";
                }
                else if (carHack == "turnlefthack")
                {
                    carHack = "turnrighthack";
                }
                else if (carHack == "turnrighthack")
                {
                    carHack = "explodehack";
                }
                else if (carHack == "explodehack")
                {
                    carHack = "tirehack";
                }
                else if (carHack == "tirehack")
                {
                    carHack = "burnouthack";
                }
                else if (carHack == "burnouthack")
                {
                    carHack = "repairhack";
                }
                else if (carHack == "repairhack")
                {
                    carHack = "removehack";
                }
                else if (carHack == "removehack")
                {
                    carHack = "wantedhack";
                }
                else if (carHack == "wantedhack")
                {
                    carHack = "lockhack";
                }
                else if (carHack == "lockhack")
                {
                    carHack = "unlockhack";
                }
            }

            if (e.KeyCode == Keys.E && canHack && nextHack == "traffichack" && !isCarHack)
            {
                Ped player = Game.Player.Character;
                arrVehicle = World.GetNearbyVehicles(player, 75); //Get arrays
                CurrentPeds = new Ped[arrVehicle.Length];

                if (arrVehicle != null)
                {
                    for (int i = 0; i < arrVehicle.Length; i++)
                    {
                        /*arrVehicle[i].Wheels[0].Burst();
                        arrVehicle[i].Wheels[1].Burst();
                        arrVehicle[i].Wheels[2].Burst();
                        arrVehicle[i].Wheels[3].Burst();*/
                        arrVehicle[i].EngineTorqueMultiplier = 300;
                        arrVehicle[i].EnginePowerMultiplier = 1000;
                        if (arrVehicle[i].GetPedOnSeat(VehicleSeat.LeftFront) != null)
                        {
                            CurrentPeds[i] = arrVehicle[i].Driver;
                            //pedOverride(arrVehicle[i].GetPedOnSeat(VehicleSeat.LeftFront), false);
                            HCar.changeSeat(CurrentPeds[i]);
                        }
                        gctime = Game.GameTime;
                        //GTA.UI.Screen.StartEffect(GTA.UI.ScreenEffect.ChopVision);
                        //Game.TimeScale = 0.42F;
                        isHacking = true;
                        setTrafficLightsOff = true;
                        hacktype = "external";
                    }
                }
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "tirehack" && isCarHack && currentveh != null)
            {
                HCar tt = new HCar(currentveh, 3600, carHack);
                tt.ApplyPreHack();
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "speedhack" && isCarHack && currentveh != null && allvehs.Find(x => x.target == currentveh) == null)
            {
                if (currentveh.Model.IsTrain)
                {
                    Function.Call(Hash.SET_TRAIN_SPEED, currentveh, Function.Call<float>(Hash.GET_ENTITY_SPEED, currentveh) * 1.75);
                }
                else
                {
                    allvehs.Add(new HCar(currentveh, 3600, carHack));
                    allvehs[allvehs.Count - 1].ApplyPreHack();
                    hacktype = "internal";
                    isHacking = true;
                }
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "speedreverthack" && isCarHack && currentveh != null && allvehs.Find(x => x.target == currentveh) == null)
            {
                if (currentveh.Model.IsTrain)
                {
                    Function.Call(Hash.SET_TRAIN_SPEED, currentveh, Function.Call<float>(Hash.GET_ENTITY_SPEED, currentveh) * -1.75);
                }
                else
                {
                    allvehs.Add(new HCar(currentveh, 3600, carHack));
                    allvehs[allvehs.Count - 1].ApplyPreHack();
                    hacktype = "internal";
                    isHacking = true;
                }
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "turnlefthack" && isCarHack && currentveh != null && allvehs.Find(x => x.target == currentveh) == null)
            {
                allvehs.Add(new HCar(currentveh, 3600, carHack));
                allvehs[allvehs.Count - 1].ApplyPreHack();
                hacktype = "internal";
                isHacking = true;
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "turnrighthack" && isCarHack && currentveh != null && allvehs.Find(x => x.target == currentveh) == null)
            {
                allvehs.Add(new HCar(currentveh, 3600, carHack));
                allvehs[allvehs.Count - 1].ApplyPreHack();
                hacktype = "internal";
                isHacking = true;
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "explodehack" && isCarHack && currentveh != null)
            {
                HCar tt = new HCar(currentveh, 3600, carHack, bWanted4Explosion);
                tt.ApplyPreHack();
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "burnouthack" && isCarHack && currentveh != null)
            {
                currentveh.IsEngineRunning = false;
                allvehs.Add(new HCar(currentveh, 28800, carHack));
                currentveh.IsEngineRunning = true;
                allvehs[allvehs.Count - 1].ApplyPreHack();
                hacktype = "internal";
                isHacking = true;
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "repairhack" && isCarHack && currentveh != null)
            {
                HCar tt = new HCar(currentveh, 3600, carHack);
                tt.ApplyPreHack();
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "removehack" && isCarHack && currentveh != null)
            {
                HCar tt = new HCar(currentveh, 3600, carHack);
                tt.ApplyPreHack();
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "wantedhack" && isCarHack && currentveh != null)
            {
                HCar tt = new HCar(currentveh, 3600, carHack);
                tt.ApplyPreHack();
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "lockhack" && isCarHack && currentveh != null)
            {
                HCar tt = new HCar(currentveh, 3600, carHack);
                tt.ApplyPreHack();
            }
            else if (e.KeyCode == Keys.E && canHack && carHack == "unlockhack" && isCarHack && currentveh != null)
            {
                HCar tt = new HCar(currentveh, 3600, carHack);
                tt.ApplyPreHack();
            }

            /*if(e.KeyCode == Keys.M)
            {
                createPickupObject(WeaponHash.Bottle, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, true, -1152075764);
            }*/

        }

        public static float Clamp(float value, float min, float max)
        {
            float cvalue = value;
            if (cvalue < min)
            {
                cvalue = min;
            }
            else if (cvalue > max)
            {
                cvalue = max;
            }
            return cvalue;
        }

        float Lerp(float firstFloat, float secondFloat, float by)
        {
            return Clamp(firstFloat + by,0, secondFloat);
        }

        private void createCamera(Vector3 position, Vector3 rotation, float fov, Vehicle veh, Vector3 offset)
        {
            Camera cam = World.CreateCamera(position, rotation, fov);
            cam.AttachTo(veh, offset);
        }

        private void executeHacks()
        {
            Ped player = Game.Player.Character;
            Entity[] objects = World.GetNearbyEntities(player.Position, 75);
            Entity[] lights = World.GetNearbyEntities(player.Position, 50);
            //Prop[] etprops = World.GetNearbyProps(Game.Player.Character.Position, 30);
            int nbprops = 0;
            int carsee = 0;

            //Function.Call(Hash.DISABLE_CONTROL_ACTION, 0, 69, true);

            RaycastResult rayTraffic = VectorConv.RotationToDirection(player, IntersectFlags.Objects);
            if (rayTraffic.HitEntity != null && (rayTraffic.HitEntity.Model.Hash == 1043035044 || rayTraffic.HitEntity.Model.Hash == -655644382 || rayTraffic.HitEntity.Model.Hash == 862871082 || rayTraffic.HitEntity.Model.Hash == 865627822) && rayTraffic.HitEntity != trafficEntity)
            {
                trafficEntity = rayTraffic.HitEntity;
            }
            if(trafficEntity != null && trafficEntity.IsOnScreen)
            {
                Point local = new Point((int)GTA.UI.Screen.WorldToScreen(trafficEntity.LeftPosition).X, (int)GTA.UI.Screen.WorldToScreen(trafficEntity.AbovePosition).Y);
                CustomSprite currentHack = new CustomSprite(AppDomain.CurrentDomain.BaseDirectory + "\\XGamingM\\" + nextHack + ".png", new Size(100, 38), local);
                currentHack.Draw();
                canHack = true;
                isCarHack = false;
                nbprops += 1;
            }

            if (nbprops == 0) { canHack = false; }
            if (nextHack == "traffichack" && hacktype == "external")
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (setTrafficLightsOff && isHacking)
                    {
                        if (objects[i].Model.Hash == 1043035044 || objects[i].Model.Hash == -655644382 || objects[i].Model.Hash == 862871082)
                        {
                            Function.Call(Hash.SET_ENTITY_TRAFFICLIGHT_OVERRIDE, objects[i], 0);
                        }
                    }
                }
                setTrafficLightsOff = false;
            }

            if (arrVehicle != null && hacktype == "external")
            {
                if (nextHack == "traffichack")
                {
                    if (Game.GameTime < gctime + 10200)
                    {
                        if (isHacking)
                        {
                            for (int i = 0; i < arrVehicle.Length; i++)
                            {
                                arrVehicle[i].BrakePower = -1.0f;
                                arrVehicle[i].ThrottlePower = 1.0f;
                                //arrVehicle[i].ForwardSpeed = Lerp(arrVehicle[i].Speed, 175, 0.25F);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < arrVehicle.Length; i++)
                        {
                            if (isHacking)
                            {
                                Game.TimeScale = 1;
                                if (CurrentPeds != null)
                                {
                                    if (CurrentPeds[i] != null && arrVehicle[i] != null)
                                    {
                                        CurrentPeds[i].SetIntoVehicle(arrVehicle[i], VehicleSeat.Driver);
                                    }
                                }
                            }
                        }
                        //GTA.UI.Screen.StopEffect(GTA.UI.ScreenEffect.ChopVision);
                        isHacking = false;
                        hacktype = "";
                    }
                }
            }

            RaycastResult rayVehicles = VectorConv.RotationToDirection(player, IntersectFlags.Everything);

            if (rayVehicles.DidHit)
            {
                if (rayVehicles.HitEntity != null && rayVehicles.HitEntity.EntityType == EntityType.Vehicle)
                {
                    Vehicle nveh = (Vehicle)rayVehicles.HitEntity;
                    if (nveh != null && nveh != currentveh)
                    {
                        currentveh = nveh;
                    }
                }
            }

            if (currentveh != null && currentveh.IsOnScreen)
            {
                Game.DisableControlThisFrame(GTA.Control.VehicleCinCam);
                Point local = new Point((int)GTA.UI.Screen.WorldToScreen(currentveh.Position).X, (int)GTA.UI.Screen.WorldToScreen(currentveh.Position).Y);
                CustomSprite carcurHack = new CustomSprite(AppDomain.CurrentDomain.BaseDirectory + "\\XGamingM\\" + carHack + ".png", new Size(100, 38), local);
                carcurHack.Draw();
                isCarHack = true;
                canHack = true;
                carsee += 1;
            }

            if (carsee == 0 && nbprops == 0) { canHack = false; currentveh = null; isCarHack = false; };
            if (carsee == 0) { isCarHack = false; };

            foreach (HCar hall in allvehs)
            {
                if ((isHacking == true && hacktype == "internal") || pedMakesErrors == true)
                {
                    if (hall.stopHack == true || (hall.GetVehicle() == currentveh && !hall.GetHack().Contains(carHack) && canHack == true))
                    {
                        hall.forceStopHack();
                        allvehs.Remove(hall);
                        break;
                    }
                    else
                    {
                        hall.ApplyForcedHack();
                    }

                }
            }

            if (allvehs.Count == 0)
            {
                isHacking = false;
                hacktype = "";
            }
        }

        private void makeLivingWorld()
        {
            WeightedChanceExecutor pedExecutions = new WeightedChanceExecutor(
                            new WeightedChanceParam(() =>
                            {
                                Ped[] nPeds = World.GetNearbyPeds(Game.Player.Character, 20);

                                if (nPeds.Length >= 1)
                                {
                                    int iRand = new Random().Next(0, nPeds.Length - 1);
                                    Ped selectedPed = nPeds[iRand];

                                    foreach (Ped ped in World.GetNearbyPeds(selectedPed, 20))
                                    {
                                        if (ped != Game.Player.Character)
                                        {
                                            Ped closePed = ped;
                                            setPedAttackAnotherPed(selectedPed, closePed);
                                            setPedAttackAnotherPed(closePed, selectedPed);
                                        }
                                    }
                                }
                            }, 2), //25% chance (since 25 + 25 + 50 = 100)
                            new WeightedChanceParam(() =>
                            {
                                Ped[] nPeds = World.GetNearbyPeds(Game.Player.Character, 20);

                                if (nPeds.Length >= 1)
                                {
                                    int iRand = new Random().Next(0, nPeds.Length - 1);
                                    Ped selectedPed = nPeds[iRand];
                                    foreach (Ped ped in World.GetNearbyPeds(selectedPed, 20))
                                    {
                                        if (ped != Game.Player.Character)
                                        {
                                            Ped closePed = ped;

                                            selectedPed.Weapons.RemoveAll();
                                            selectedPed.Weapons.Give(WeaponHash.Molotov, 50, true, true);
                                            Vehicle[] nVehicles = World.GetNearbyVehicles(selectedPed, 50);
                                            if (nVehicles.Length >= 1)
                                            {
                                                int vehRand = new Random().Next(0, nVehicles.Length - 1);
                                                ShootAtCoordinates(selectedPed, nVehicles[vehRand].Position.X, nVehicles[vehRand].Position.Y, nVehicles[vehRand].Position.Z, 3600, FiringPattern.FullAuto);
                                            }
                                        }
                                    }
                                }
                            }, 99) //25% chance
            );

            WeightedChanceExecutor vehicleExecutions = new WeightedChanceExecutor(
                            new WeightedChanceParam(() =>
                            {
                                Vehicle[] nVehicles = World.GetNearbyVehicles(Game.Player.Character, 50);
                                if (nVehicles.Length >= 1)
                                {
                                    int iRand = new Random().Next(0, nVehicles.Length - 1);
                                    Vehicle selectedVehicle = nVehicles[iRand];
                                    if (selectedVehicle.Speed >= 7)
                                    {
                                        int directionRand = new Random().Next(0, 1);
                                        string sHack = (directionRand == 0 ? "turnlefthack" : "turnrighthack");
                                        allvehs.Add(new HCar(selectedVehicle, 3600, sHack));
                                        allvehs[allvehs.Count - 1].ApplyPreHack();
                                        pedMakesErrors = true;
                                    }
                                }
                            }, 15),
                            new WeightedChanceParam(() =>
                            {
                                Vehicle[] nVehicles = World.GetNearbyVehicles(Game.Player.Character, 50);
                                if (nVehicles.Length >= 1)
                                {
                                    int iRand = new Random().Next(0, nVehicles.Length - 1);
                                    Vehicle selectedVehicle = nVehicles[iRand];
                                    if (selectedVehicle.Speed >= 7)
                                    {
                                        /*allvehs.Add(new HCar(selectedVehicle, 7200, "stopfirehack"));
                                        allvehs[allvehs.Count - 1].ApplyPreHack();
                                        pedMakesErrors = true;*/
                                        SetEntityOnFire(selectedVehicle);
                                    }
                                }
                            }, 15),
                            new WeightedChanceParam(() =>
                            {
                            Vehicle[] nVehicles = World.GetNearbyVehicles(Game.Player.Character, 50);
                                if (nVehicles.Length >= 1)
                                {
                                    int iRand = new Random().Next(0, nVehicles.Length - 1);
                                    Vehicle selectedVehicle = nVehicles[iRand];
                                    VehicleWheelCollection ct = selectedVehicle.Wheels;
                                    int wheelRand = new Random().Next(0, 6);
                                    if (ct[wheelRand] != null)
                                    {
                                        ct[wheelRand].Burst();
                                    }
                                }
                            }, 2), //50% chance
                            new WeightedChanceParam(() =>
                            {
                                
                            }, 53),
                            new WeightedChanceParam(() =>
                            {
                                Vehicle[] nVehicles = World.GetNearbyVehicles(Game.Player.Character, 50);
                                if (nVehicles.Length >= 1)
                                {
                                    int iRand = new Random().Next(0, nVehicles.Length - 1);
                                    Vehicle selectedVehicle = nVehicles[iRand];
                                    if (selectedVehicle.Speed >= 7)
                                    {
                                        allvehs.Add(new HCar(selectedVehicle, 3600, "stophack"));
                                        allvehs[allvehs.Count - 1].ApplyPreHack();
                                        pedMakesErrors = true;
                                    }
                                }
                            }, 15) //25% chance
            );

            if(Game.GameTime >= targetCarTime)
            {
                vehicleExecutions.Execute();
                targetCarTime = 0;
            }

            if(Game.GameTime >= targetPedTime)
            {
                pedExecutions.Execute();
                targetPedTime = 0;
            }
        }

        private void clearArea(Ped player, float radius)
        {
            if (bClearArea)
            {
                Vehicle[] vehicles = World.GetNearbyVehicles(player, radius);
                foreach (Vehicle vehicle in vehicles)
                {
                    if (!vehicle.PreviouslyOwnedByPlayer)
                    {
                        vehicle.Delete();
                    }
                }
            }
        }

        public static void createPickupObject(WeaponHash pickupHash, float x, float y, float z, bool placeOnGround, int modelHash)
        {
            Function.Call<object>(Hash.CREATE_PORTABLE_PICKUP, pickupHash, x, y, z, placeOnGround, modelHash);
        }

        public static void setPedAttackAnotherPed(Ped ped, Ped targetPed, int p2 = 0, int p3 = 16)
        {
            Function.Call(Hash.TASK_COMBAT_PED, ped, targetPed, p2, p3);
        }

        public static void setVehicleOutOfControl(Vehicle vehicle, bool killDriver, bool explodeOnImpact)
        {
            Function.Call(Hash.SET_VEHICLE_OUT_OF_CONTROL, vehicle, killDriver, explodeOnImpact);
        }

        public static void setFire(float X, float Y, float Z, int maxChildren, bool isGasFire)
        {
            Function.Call(Hash.START_SCRIPT_FIRE, X, Y, Z, maxChildren, isGasFire);
        }

        public static void SetEntityOnFire(Entity entity)
        {
            Function.Call(Hash.START_ENTITY_FIRE, entity);
        }

        public static int getEntityBoneIndexByName(Vehicle vehicle, string boneName)
        {
            return Function.Call<int>(Hash.GET_ENTITY_BONE_INDEX_BY_NAME, vehicle, boneName);
        }

        public static void ShootAtCoordinates(Ped ped, float x, float y, float z, int duration, FiringPattern firingPattern)
        {
            Function.Call(Hash.TASK_SHOOT_AT_ENTITY, ped, x, y, z, duration, firingPattern);
        }

        private void OnTick(object sender, EventArgs e)
        {

            if(targetCarTime == 0)
            {
                targetCarTime = Game.GameTime + 250;
            }

            if (targetPedTime == 0)
            {
                targetPedTime = Game.GameTime + 10;
            }

            executeHacks();
            makeLivingWorld();
            clearArea(Game.Player.Character, 200);
            pool.Process();
        }
    }
}