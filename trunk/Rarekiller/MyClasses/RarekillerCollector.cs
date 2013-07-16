//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//    Credits to highvoltz, bloodlove, SMcCloud, Lofi, ZapMan 
//                and all the brave Testers
//
//==================================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Pathing;
namespace katzerle
{
    class RarekillerCollector
    {
		public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();

        //ToDo Landing Points for of Underground Objects
        public static WoWPoint LandingPoint213364 = new WoWPoint(2288.31, -1772.671, 238.9393);
        public static WoWPoint LandingPoint214337 = new WoWPoint(2417.861, -2929.056, 8.610151);
        public static WoWPoint LandingPoint213649 = new WoWPoint(178.477, 949.426, 171.0682);
        public static WoWPoint LandingPoint213650 = new WoWPoint(291.5115, 1772.154, 310.9289);
        public static WoWPoint LandingPoint214340 = new WoWPoint(49.96476, -950.9753, 24.16642);
        public static WoWPoint LandingPoint213651 = new WoWPoint(-2998.009, 952.5232, 16.09599);
        public static WoWPoint LandingPoint213750 = new WoWPoint(-421.5852, -366.7636, 108.9386);
        public static WoWPoint LandingPoint214325 = new WoWPoint(805.1631, -193.6357, 407.6493);
        public static WoWPoint LandingPoint213768 = new WoWPoint(2677.461, 1532.559, 644.4728);
        public static WoWPoint LandingPoint213751 = new WoWPoint(2559.742, 275.2169, 496.4695);
        public static WoWPoint LandingPoint213770 = new WoWPoint(3386.12, 1135.976, 665.71);
        public static WoWPoint LandingPoint213793 = new WoWPoint(3501.53, 1570.532, 872.9788);
        public static WoWPoint LandingPoint213769 = new WoWPoint(3029.445, 1674.854, 649.7944);
        public static WoWPoint LandingPoint214438 = new WoWPoint(3525.092, 841.6902, 493.3049);
        public static WoWPoint LandingPoint214407 = new WoWPoint(2581.199, 1817.027, 670.2189);
        public static WoWPoint LandingPoint213956 = new WoWPoint(2188.075, 5209.729, 93.06538);
        public static WoWPoint LandingPoint213956_2 = new WoWPoint(2171.203, 5051.739, 73.98229);
        public static WoWPoint LandingPoint213970 = new WoWPoint(-536.6893, 4760.024, 1.222435);
        public static WoWPoint LandingPoint213970_2 = new WoWPoint(-463.5745, 4761.513, -32.32083);
        public static WoWPoint LandingPoint213362 = new WoWPoint(-986.6257, -2095.295, 4.935998);

        public void findAndPickupObject()
        {

            //bool ForceGround = false;
            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic("Rarekiller: Scan for Objects to collect");
// ----------------- Generate a List with all wanted Nests found in Object Manager ---------------------		

			ObjectManager.Update();
            List<WoWGameObject> objList = ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (((o.Entry == 202082) && Rarekiller.Settings.RaptorNest)
                || ((o.Entry == 202080) && Rarekiller.Settings.RaptorNest)
                || ((o.Entry == 202083) && Rarekiller.Settings.RaptorNest)
                || ((o.Entry == 202081) && Rarekiller.Settings.RaptorNest)
                || ((o.Entry == 210565) && Rarekiller.Settings.DarkSoil)
                || (Rarekiller.AnotherMansTreasureList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.AnotherMansTreasure && o.Entry > 200000)
				|| (Rarekiller.CollectObjectsList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.ObjectsCollector)
                || ((o.Entry == 206195) && Rarekiller.Settings.TestRaptorNest) //Testcase Thundermar Ale Keg
                )))
                .OrderBy(o => o.Distance).ToList();

            List<WoWUnit> RareList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(r => ((r.CreatureRank == Styx.WoWUnitClassificationType.Rare) && r.Level > 85 && !r.IsDead)).OrderBy(r => r.Distance).ToList();

            foreach (WoWGameObject o in objList)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: Find A Object to collect {0} ID {1}", o.Name, o.Entry);
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Object Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));

                if (Rarekiller.Settings.LUAoutput)
                    Lua.DoString("print('NPCScan: Find {0} ID {1}')", o.Name, o.Entry);
				
				
// ----------------- Alert ---------------------
                if (Rarekiller.Settings.Alert)
                {
                    if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                        new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                    else if (File.Exists(Rarekiller.Soundfile))
                        new SoundPlayer(Rarekiller.Soundfile).Play();
                    else
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: playing Soundfile failes");
                }

                if(o.Entry == 213970  || o.Entry == 213362)
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Can't reach Object because it is under Water {0}, Blacklist and Move on", o.Name);
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: Object is under Water')");
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    return;
                }

// ----------------- Underground Object ----------
                //if not ID of Underground Object of Another Mans Treasure --> don't collect
                if (!(o.Entry == 213364 || o.Entry == 214337 || o.Entry == 213649 || o.Entry == 213650 || o.Entry == 214340 || o.Entry == 213651
                    || o.Entry == 213750 || o.Entry == 214325 || o.Entry == 213768 || o.Entry == 213751 || o.Entry == 213770 || o.Entry == 213793
                    || o.Entry == 213769 || o.Entry == 214438 || o.Entry == 214407 || o.Entry == 213956)) // || o.Entry == 213970  || o.Entry == 213362
                {
                    if (o.IsIndoors && Me.IsFlying && Me.IsOutdoors && (o.Location.Distance(Me.Location) > 30))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Can't reach Object because it is Indoors and I fly Outdoors {0}, Blacklist and Move on", o.Name);
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: Object is Indoors')");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        return;
                    }
                }
// ----------------- don't collect if Rare Pandaria Elite Around
                if (RareList != null)
                {
                    foreach (WoWUnit r in RareList)
                    {
                        if (r.Location.Distance(o.Location) < 30)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Can't reach Object because there's a Rare Elite around, Blacklist and move on", o.Name);
                            if (Rarekiller.Settings.LUAoutput)
                                Lua.DoString("print('NPCScan: Object Elite Rare around')", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            return;
                        }
                    }
                }

                if (Rarekiller.Settings.PlayerScan && RarekillerSecurity.PlayerAround(o))
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: There are other Players around, so move on");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: Other Players around')");
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    return;
                }

                if (Me.Combat)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: ... but first I have to finish fighting another Mob.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: First finish combat')");
                    return;
                }
                if (Me.IsOnTransport)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: ... but I'm on a Transport.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: I'm on Transport')");
                    return;
                }
				
		        while (Me.IsCasting)
                {
                    Thread.Sleep(100);
                }

                //ToDo IDs of Underground NPCs
                if (!Rarekiller.Settings.Forceground && (o.Entry == 213364 || o.Entry == 214337 || o.Entry == 213649 || o.Entry == 213650 || o.Entry == 214340 || o.Entry == 213651
                    || o.Entry == 213750 || o.Entry == 214325 || o.Entry == 213768 || o.Entry == 213751 || o.Entry == 213770 || o.Entry == 213793
                    || o.Entry == 213769 || o.Entry == 214438 || o.Entry == 214407 || o.Entry == 213956 || o.Entry == 213970 || o.Entry == 213362))
                {
                    WoWPoint Helperpoint = o.Location;
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Found a Underground NPC {0} so dismount and walk", o.Entry);
                    if (o.Entry == 213364)
                        Helperpoint = LandingPoint213364;
                    if (o.Entry == 214337)
                        Helperpoint = LandingPoint214337;
                    if (o.Entry == 213649)
                        Helperpoint = LandingPoint213649;
                    if (o.Entry == 213650)
                        Helperpoint = LandingPoint213650;
                    if (o.Entry == 214340)
                        Helperpoint = LandingPoint214340;
                    if (o.Entry == 213651)
                        Helperpoint = LandingPoint213651;
                    if (o.Entry == 213750)
                        Helperpoint = LandingPoint213750;
                    if (o.Entry == 214325)
                        Helperpoint = LandingPoint214325;
                    if (o.Entry == 213768)
                        Helperpoint = LandingPoint213768;
                    if (o.Entry == 213751)
                        Helperpoint = LandingPoint213751;
                    if (o.Entry == 213770)
                        Helperpoint = LandingPoint213770;
                    if (o.Entry == 213793)
                        Helperpoint = LandingPoint213793;
                    if (o.Entry == 213769)
                        Helperpoint = LandingPoint213769;
                    if (o.Entry == 214438)
                        Helperpoint = LandingPoint214438;
                    if (o.Entry == 214407)
                        Helperpoint = LandingPoint214407;
                    if (o.Entry == 213956)
                        Helperpoint = LandingPoint213956;
                    if (o.Entry == 213970)
                        Helperpoint = LandingPoint213970;
                    if (o.Entry == 213362)
                        Helperpoint = LandingPoint213362;

                    while (Me.Location.Distance(Helperpoint) > 5)
                    {
                        Flightor.MoveTo(Helperpoint);
                        Thread.Sleep(100);
                    }
                    WoWMovement.MoveStop();
                    
                    //Dismount
                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");
                       

                    Thread.Sleep(300);
                    //if ((o.Entry != 213362) && (o.Entry != 213970))
                    Rarekiller.Settings.Forceground = true;
                }

// ----------------- Move to Object Part ---------------------
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Move to Object");
                BlacklistTimer.Reset();
                BlacklistTimer.Start();



				while (o.Location.Distance(Me.Location) > 4)
				{
                    if (Rarekiller.Settings.Forceground)
                        Navigator.MoveTo(o.Location);
                    else
                        Flightor.MoveTo(o.Location);
					Thread.Sleep(100);
// ----------------- Security  ---------------------
					if (Rarekiller.inCombat) return;
                    if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part MoveTo: Can't reach Object {0}, Blacklist and Move on", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        BlacklistTimer.Reset();
                        WoWMovement.MoveStop();
                        Rarekiller.Settings.Forceground = false;
                        return;
                    }
				}
                BlacklistTimer.Reset();
				Thread.Sleep(300);
				WoWMovement.MoveStop();
// Collect Object
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: Object Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
                if (Me.Auras.ContainsKey("Flight Form"))
                    Lua.DoString("CancelShapeshiftForm()");
                else if (Me.Mounted)
                    Lua.DoString("Dismount()");

                //Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Take a Screen");
                //Lua.DoString("TakeScreenshot()"); 
				Thread.Sleep(1000);
                o.Interact();
                o.Interact();
                o.Interact();
				Thread.Sleep(2000);
				Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
				Thread.Sleep(4000);
                Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Interact with {0} - ID {1}", o.Name, o.Entry);
                Rarekiller.Settings.Forceground = false;
            }
        }
    }
}
