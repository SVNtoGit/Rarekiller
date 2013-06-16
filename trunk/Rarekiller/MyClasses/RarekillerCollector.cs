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
        public void findAndPickupObject()
        {

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic("Rarekiller: Scan for Objects to collect");
// ----------------- Generate a List with all wanted Nests found in Object Manager ---------------------		

			ObjectManager.Update();
            List<WoWGameObject> objList = ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (((o.Entry == 202082) && Rarekiller.Settings.TestRaptorNest)
                || ((o.Entry == 202080) && Rarekiller.Settings.TestRaptorNest)
                || ((o.Entry == 202083) && Rarekiller.Settings.TestRaptorNest)
                || ((o.Entry == 202081) && Rarekiller.Settings.TestRaptorNest)
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

				
// ----------------- Underground Object ----------
                if (o.IsIndoors && Me.IsFlying && Me.IsOutdoors && (o.Location.Distance(Me.Location) > 30))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Can't reach Object because it is Indoors and I fly Outdoors {0}, Blacklist and Move on", o.Name);
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    return;
                }
// ----------------- don't collect if Rare Pandaria Elite Around
                if (RareList != null)
                {
                    foreach (WoWUnit r in RareList)
                    {
                        if (r.Location.Distance(o.Location) < 30)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Collector: Can't reach Object because there's a Rare Elite around, Blacklist and move on", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            return;
                        }
                    }
                }

                if (Me.Combat)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: ... but first I have to finish fighting another Mob.");
                    return;
                }
                if (Me.IsOnTransport)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: ... but I'm on a Transport.");
                    return;
                }
				
		        while (Me.IsCasting)
                {
                    Thread.Sleep(100);
                }

// ----------------- Move to Object Part ---------------------
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Move to Object");
                BlacklistTimer.Reset();
                BlacklistTimer.Start();



				while (o.Location.Distance(Me.Location) > 4)
				{
                    if (o.IsIndoors)
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
                        return;
                    }
				}
                BlacklistTimer.Reset();
				Thread.Sleep(300);
				WoWMovement.MoveStop();	
// Collect Nest
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Collector: Nest Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
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
            }
        }
    }
}
