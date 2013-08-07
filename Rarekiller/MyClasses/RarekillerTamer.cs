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
    class RarekillerTamer
    {
        public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        private static Stopwatch Alerttimer = new Stopwatch();

        /// <summary>
        /// Function to Find and Tame Mobs
        /// </summary>
        public void findAndTameMob()
        {
            if (Me.Class != WoWClass.Hunter && !Rarekiller.Settings.TestcaseTamer)
                return;		
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                ((Rarekiller.Settings.TameDefault && Rarekiller.TameableMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                || (Rarekiller.Settings.TameByID && (o.Entry == Convert.ToInt64(Rarekiller.Settings.TameMobID))))
                && !o.IsPet && o.IsTameable)))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (!o.IsDead)
                {
                    Logging.WriteQuiet(Colors.MediumPurple, "Rarekiller: Found a new Pet {0} ID {1}", o.Name, o.Entry);
					Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Unit Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: Find {0} ID {1}')", o.Name, o.Entry);

                    #region don't tame if
                    // Don't tame the Rare if ...
                    if (Me.IsFlying && Me.IsOutdoors && o.IsIndoors)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Unit is Indoors and I fly Outdoors, so blacklist him to prevent Problems");
                        Logging.Write(Colors.MediumPurple, "Rarekiller: You have to place me next to the Spawnpoint, if you want me to hunt this Unit.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.WriteDiagnostic(Colors.MediumPurple, " Part TamerRarekiller: Blacklist Unit for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is Indoors')", o.Name);
                        return;
                    }
                    if (Me.Level < o.Level)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Unit Level is higher then mine, can't tame the Unit.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Unit for 60 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC Level is to high to tame')", o.Name);
                        return;
                    }
                    if (Rarekiller.BlacklistMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: {0} is Member of the BlacklistedMobs.xml", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Unit for 15 Minutes.");
                        return;
                    }
                    if (Rarekiller.DontInteract) return;
                    #endregion

                    //Dismiss Pet
                    if (Me.Class == WoWClass.Hunter && Me.Pet != null)
                    {
                        RarekillerSpells.CastSafe("Dismiss Pet", Me, false);
                        //SpellManager.Cast("Dismiss Pet");
                        Thread.Sleep(3000);
                    }

                    if (Rarekiller.Settings.Alert)
                        Rarekiller.Alert();

                    #region Move to tameable Unit
                    if(!Rarekiller.MoveTo(o, 20, false)) return;
                    if (Me.IsFlying)
                        if (!Rarekiller.DescendToLand(o)) return;
                    Rarekiller.Dismount();
                    #endregion

                    o.Target();
                    if (Me.Class == WoWClass.Hunter)
                    {
                        #region Tame
                        while (!o.IsPet)
                        {
                            if (o.IsDead)
                            {
                                Logging.Write(Colors.MediumPurple, "Rarekiller: Oh no, I accidently killed him !!! ");
                                return;
                            }
                            if (Me.HealthPercent < 10)
                            {
                                Logging.Write(Colors.MediumPurple, "Rarekiller: Health < 10% , Use Feign Death !!! ");
                                if (SpellManager.CanCast("Feign Death"))
                                    RarekillerSpells.CastSafe("Feign Death", Me, false);
                                //SpellManager.Cast("Feign Death");
                                return;
                            }

                            if (!(Me.CastingSpellId == 1515))
                            {
                                WoWMovement.MoveStop();
                                RarekillerSpells.CastSafe("Tame Beast", o, true);
                                //SpellManager.Cast(1515);
                                Logging.Write(Colors.MediumPurple, "Rarekiller: Try to tame Beast {0}", o.Name);
                                Thread.Sleep(500);
                            }
                        }
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Sucessfully tamed Beast {0}", o.Name);
                        #endregion
                    }
                     
                   else
                    {
                        #region Testcase for my Shammi
                        while (!o.IsDead)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller Test: Try to tame Beast", o.Name);
                            Thread.Sleep(1500);
                        }
                        #endregion
                    }
                    
                }
                else if (o.IsPet)
                    return;
                else if (Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags))
                    return;
                else
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Find a Unit, but sadly he's dead or not tameable: {0}", o.Name);
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Unit for 5 Minutes.");
                    return;
                }
            }
        }

        /// <summary>
        /// Function to Find and Alert for Footsteps (Pandaria Rares to Tame)
        /// </summary>
        public void findandfollowFootsteps()
        {
            if (Me.Class != WoWClass.Hunter && !Rarekiller.Settings.TestcaseTamer)
                return;

            #region create a List with Objects in Reach
            // ----------------- Generate a List with all wanted Nests found in Object Manager ---------------------		
            ObjectManager.Update();
            WoWGameObject Footprint = ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                    (o.Entry == 214430) //patrannache footprint
                    || (o.Entry == 214429) //rockhide the immovable footprint
                    || (o.Entry == 214431) //bloodtooth footprint
                    || (o.Entry == 214432) //hexapos footprint
                    || (o.Entry == 214433) //stompy footprint
                    || (o.Entry == 214434) //bristlespine footprint
                    || (o.Entry == 214435) //portent footprint
                    || (o.Entry == 214436) //savage footprint
                    || (o.Entry == 214437) //glimmer footprint
                    || ((o.Entry == 210565) && Rarekiller.Settings.TestcaseTamer) //Testcase for Developer
                )))
                .OrderBy(o => o.Distance).FirstOrDefault();
            #endregion

            List<WoWUnit> RareList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(r => ((r.CreatureRank == Styx.WoWUnitClassificationType.Rare) && r.Level > 85 && !r.IsDead)).OrderBy(r => r.Distance).ToList();

            Logging.Write(Colors.MediumPurple, "Rarekiller: Found a Footprint {0} ID {1}", Footprint.Name, Footprint.Entry);
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Footprint Location: {0} / {1} / {2}", Convert.ToString(Footprint.X), Convert.ToString(Footprint.Y), Convert.ToString(Footprint.Z));
            if (Rarekiller.Settings.LUAoutput)
                Lua.DoString("print('NPCScan: Find {0} ID {1}')", Footprint.Name, Footprint.Entry);

            if (Rarekiller.Settings.Alert)
                Rarekiller.Alert();

            #region don't follow, if ...
            // ----------------- don't collect if Rare Pandaria Elite Around
            if (RareList != null)
            {
                foreach (WoWUnit r in RareList)
                {
                    if (r.Location.Distance(Footprint.Location) < 30)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Can't reach Footprint because there's a Rare Elite around, Blacklist and move on", Footprint.Name);
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: Footprint Elite Rare around')", Footprint.Name);
                        Blacklist.Add(Footprint.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        return;
                    }
                }
            }
            if (Rarekiller.DontInteract) return;
            #endregion

            while (Footprint != null)
            {
                if (Rarekiller.ToonInvalidCombat) return;
                Rarekiller.MoveTo(Footprint, 10, false);
                    
                #region Alert
                if (!Alerttimer.IsRunning)
                {
                    Alerttimer.Reset();
                    Alerttimer.Start();
                }
                // ----------------- Alert ---------------------
                if (Rarekiller.Settings.Alert && Alerttimer.Elapsed.TotalSeconds > 20)
                {
                    Rarekiller.Alert();
                    Alerttimer.Reset();
                    Alerttimer.Start();
                }
                #endregion

                #region create a List with Objects in Reach
                // ----------------- Generate a List with all wanted Nests found in Object Manager ---------------------		
                ObjectManager.Update();
                Footprint = ObjectManager.GetObjectsOfType<WoWGameObject>()
                    .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                        (o.Entry == 214430) //patrannache footprint
                        || (o.Entry == 214429) //rockhide the immovable footprint
                        || (o.Entry == 214431) //bloodtooth footprint
                        || (o.Entry == 214432) //hexapos footprint
                        || (o.Entry == 214433) //stompy footprint
                        || (o.Entry == 214434) //bristlespine footprint
                        || (o.Entry == 214435) //portent footprint
                        || (o.Entry == 214436) //savage footprint
                        || (o.Entry == 214437) //glimmer footprint
                        || ((o.Entry == 210565) && Rarekiller.Settings.TestcaseTamer) //Testcase for Developer
                    )))
                    .OrderBy(o => o.Distance).FirstOrDefault();
                #endregion 
            }
        }
    }
}
