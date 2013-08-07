//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
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

        #region Landingpoints for Underground Objects
        //Landing Points for of Underground Objects
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
        public static WoWPoint LandingPoint213970 = new WoWPoint(-536.6893, 4760.024, 1.222435);
        public static WoWPoint LandingPoint213362 = new WoWPoint(-986.6257, -2095.295, 4.935998);
        #endregion

        /// <summary>
        /// Function Find and Pickup Objects
        /// </summary>
        public void findAndPickupObject()
        {

            #region create a List with Objects in Reach
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
            #endregion

            foreach (WoWGameObject o in objList)
            {
                Logging.WriteQuiet(Colors.MediumPurple, "Rarekiller: Find A Object to collect {0} ID {1}", o.Name, o.Entry);
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Object Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                if (Rarekiller.Settings.LUAoutput)
                    Lua.DoString("print('NPCScan: Find {0} ID {1}')", o.Name, o.Entry);

                if (Rarekiller.Settings.Alert)
                    Rarekiller.Alert();

                #region don't collect, if ...
                if (o.Entry == 213970  || o.Entry == 213362)
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Can't reach Object because it is under Water {0}, Blacklist and Move on", o.Name);
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: Object is under Water')");
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    return;
                }

// ----------------- Underground Object ----------
                //if not ID of known Underground Object of Another Mans Treasure --> don't collect
                if (!(o.Entry == 213364 || o.Entry == 214337 || o.Entry == 213649 || o.Entry == 213650 || o.Entry == 214340 || o.Entry == 213651
                    || o.Entry == 213750 || o.Entry == 214325 || o.Entry == 213768 || o.Entry == 213751 || o.Entry == 213770 || o.Entry == 213793
                    || o.Entry == 213769 || o.Entry == 214438 || o.Entry == 214407 || o.Entry == 213956)) // || o.Entry == 213970  || o.Entry == 213362
                {
                    if (o.IsIndoors && Me.IsFlying && Me.IsOutdoors && (o.Location.Distance(Me.Location) > 30))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Can't reach Object because it is Indoors and I fly Outdoors {0}, Blacklist and Move on", o.Name);
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
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Can't reach Object because there's a Rare Elite around, Blacklist and move on", o.Name);
                            if (Rarekiller.Settings.LUAoutput)
                                Lua.DoString("print('NPCScan: Object Elite Rare around')", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            return;
                        }
                    }
                }

                if (Rarekiller.Settings.PlayerScan && RarekillerSecurity.PlayerAround(o))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: There are other Players around, so move on");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: Other Players around')");
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    return;
                }
                if (Rarekiller.DontInteract) return;
                #endregion

                #region fly to Helperpoint if known Underground Object
                //ToDo IDs of Underground NPCs
                if (Me.IsFlying && (o.Entry == 213364 || o.Entry == 214337 || o.Entry == 213649 || o.Entry == 213650 || o.Entry == 214340 || o.Entry == 213651
                    || o.Entry == 213750 || o.Entry == 214325 || o.Entry == 213768 || o.Entry == 213751 || o.Entry == 213770 || o.Entry == 213793
                    || o.Entry == 213769 || o.Entry == 214438 || o.Entry == 214407 || o.Entry == 213956 || o.Entry == 213970 || o.Entry == 213362))
                {
                    WoWPoint Helperpoint = o.Location;
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

                    if (!Rarekiller.MoveTo(Helperpoint, 5, false)) return;
                    Rarekiller.Dismount();
                }
                #endregion

                #region Move to Object
                // ----------------- Move to Object Part ---------------------
                if (o.Entry == 213364 || o.Entry == 214337 || o.Entry == 213649 || o.Entry == 213650 || o.Entry == 214340 || o.Entry == 213651
                    || o.Entry == 213750 || o.Entry == 214325 || o.Entry == 213768 || o.Entry == 213751 || o.Entry == 213770 || o.Entry == 213793
                    || o.Entry == 213769 || o.Entry == 214438 || o.Entry == 214407 || o.Entry == 213956 || o.Entry == 213970 || o.Entry == 213362)
                { if (!Rarekiller.MoveTo(o, 4, true)) return; }
                else
                { if (!Rarekiller.MoveTo(o, 4, false)) return; }
                #endregion

                    // Collect Object
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Object Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
                Rarekiller.Dismount();

                o.Interact();
                o.Interact();
                o.Interact();
				Thread.Sleep(1000);
				Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
				Thread.Sleep(1000);
                Logging.Write(Colors.MediumPurple, "Rarekiller: Interact with {0} - ID {1}", o.Name, o.Entry);
            }
        }
    }
}
