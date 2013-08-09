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
using Styx.CommonBot.Routines;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Pathing;

namespace katzerle
{
    class RarekillerCamel
    {
        public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        public static WoWPoint ProblemCamel1 = new WoWPoint(-8906.634, 312.6967, 349.2024);
        public static WoWPoint ProblemCamel2 = new WoWPoint(-9900.116, 461.3653, 45.62226);
        public static WoWPoint ProblemCamel3 = new WoWPoint(-10697.69, 1045.757, 24.125);
        public static WoWPoint ProblemCamel4 = new WoWPoint(-11066.67, -2100.342, 175.2816);
        public static WoWPoint DormusPoint = new WoWPoint(-5726.439, 673.6976, 163.293);
        public static WoWPoint LandingPoint64227 = new WoWPoint(2359.229, 2484.88, 686.5128);

        #region Unit Dormus
        /// <summary>
        /// returns the WoWUnit Dormus ID 50245
        /// </summary>
        public WoWUnit Dormus
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Entry == 50245) && !u.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        /// <summary>
        /// returns a List of Dormus Camel Spit Pools ID 94967
        /// </summary>		
		public List<WoWDynamicObject> getSpitList
        {
            get
            {
                ObjectManager.Update();
                return (from lp in ObjectManager.GetObjectsOfType<WoWDynamicObject>()
                        orderby lp.Distance2D ascending
                        where lp.Entry == 94967
                        where wlog(lp)
                        select lp).ToList();
            }
        }

        /// <summary>
        /// Logging Diagnostic things
        /// </summary>
        private bool wlog(WoWDynamicObject obj)
        { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: add spit pool - dis2D: {0}", obj.Distance2D); return true; }

		
        #endregion

        /// <summary>
        /// Function to Find and Interact with NPCs
        /// </summary>
        public void findAndInteractNPC()
        {
            #region create a List with NPCs in reach
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                    ((o.Entry == 50409) && Rarekiller.Settings.Camel) || ((o.Entry == 50410) && Rarekiller.Settings.Camel) // 50409 might be the porting Camel Figurine
                    || (Rarekiller.AnotherMansTreasureList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.AnotherMansTreasure && o.Entry < 200000)
                    || (Rarekiller.InteractNPCList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.InteractNPC)
                    || (o.Entry == 43929 && Rarekiller.Settings.Blingtron && o.QuestGiverStatus == QuestGiverStatus.AvailableRepeatable) //ToDo - Is Quest completed 31752
                    || ((o.Entry == 48959) && Rarekiller.Settings.TestFigurineInteract) //Testcase rostiger Amboss - Schnotzz Landing
                )))
                .OrderBy(o => o.Distance).ToList();
            List<WoWUnit> RareList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(r => ((r.CreatureRank == Styx.WoWUnitClassificationType.Rare) && r.Level > 85 && !r.IsDead)).OrderBy(r => r.Distance).ToList();
            #endregion

            bool Forceground = false;

            foreach (WoWUnit o in objList)
            {
                Logging.WriteQuiet(Colors.MediumPurple, "Rarekiller: Find NPC {0} ID {1}", o.Name, o.Entry);
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: NPC Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                if (Rarekiller.Settings.LUAoutput)
                    Lua.DoString("print('NPCScan: Find {0} ID {1}')", o.Name, o.Entry);

                if (Rarekiller.Settings.Alert)
                    Rarekiller.Alert();

                #region don't collect if ...
                // ----------------- Underground ----------
                //if not ID of Underground NPCs of Another Mans Treasure --> don't collect
                if (!(o.Entry == 64227))
                {
                    if (o.IsIndoors && Me.IsFlying && Me.IsOutdoors && (o.Location.Distance(Me.Location) > 30))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Can't reach NPC because it is Indoors and I fly Outdoors {0}, Blacklist and Move on", o.Name);
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is Indoors')", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        return;
                    }
                }
// -----------------  don't collect if Rare Pandaria Elite Around
                if (RareList != null)
                {
                    foreach (WoWUnit r in RareList)
                    {
                        if (r.Location.Distance(o.Location) < 30)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Can't reach NPC because there's a Rare Elite around, Blacklist and move on", o.Name);
                            if (Rarekiller.Settings.LUAoutput)
                                Lua.DoString("print('NPCScan: NPC Elite Rare around')", o.Name);
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

                #region Move to Helperpoint if found known Figurine under a Tent
                if (Me.IsFlying && ((o.Location.Distance(ProblemCamel1) < 10) || (o.Location.Distance(ProblemCamel2) < 10) ||
                        (o.Location.Distance(ProblemCamel3) < 10) || (o.Location.Distance(ProblemCamel4) < 20)))
                {
                    WoWPoint HelperPointCamel = o.Location;
                    HelperPointCamel.X = HelperPointCamel.X + 20;
                    HelperPointCamel.Z = HelperPointCamel.Z + 5;
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Found a Problem Figurine {0} so dismount and walk", o.Entry);
                    if (!Rarekiller.MoveTo(HelperPointCamel, 3, false)) return;
                    //if (!Rarekiller.DescendToLand(o)) return;
                    Rarekiller.Dismount();
                    Forceground = true;
                }
                #endregion

                #region Move to Helperpoint if known Underground NPC
                if (Me.IsFlying && o.Entry == 64227)
                {
                    if(!Rarekiller.MoveTo(LandingPoint64227, 5, false)) return;
                    Rarekiller.Dismount();
                    Forceground = true;
                }
                #endregion

                #region Move to NPC
                if (Me.IsFlying && Forceground)
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Failure in Programming. Fly again to Helperpoint and Dismount", o.Entry);
                    return;
                }
                if(!Rarekiller.MoveTo(o, 4, Forceground)) return;
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: NPC Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
                Rarekiller.Dismount();
                #endregion

                o.Interact();
                o.Interact();
                o.Interact();
                Logging.Write(Colors.MediumPurple, "Rarekiller: Interact with NPC - ID {0}", o.Entry);
                Thread.Sleep(300);
				
                //Another Mans Treasure NPCs
				if (o.Entry == 65552 || o.Entry == 64272 || o.Entry == 64004 || o.Entry == 64191 || o.Entry == 64227)
				{
                    Thread.Sleep(1000);
                    Lua.DoString("RunMacroText(\"/click GossipTitleButton1\");");
					Thread.Sleep(1000);
				}
                //Blingtron
                if (o.Entry == 43929)
                {
                    Thread.Sleep(1000);
                    Lua.DoString("SelectGossipAvailableQuest(GetNumGossipAvailableQuests())");
                    Thread.Sleep(1000);
                    Lua.DoString("RunMacroText(\"/click QuestFrameCompleteQuestButton\")");
                    Thread.Sleep(1000);
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                }
            }
        }

        /// <summary>
        /// Function to Find and Kill Dormus
        /// </summary>
        public void findAndKillDormus()
        {
            bool CastSuccess = false;
			//Testcases --> 51756 = Blutelfenjunge --> 52008 = Resortangestellter --> 50245 = Dormus
            int IDDormus = 50245;
            //Testcases --> 6346 = Fear Ward --> 974 = Earthsschild --> Dormus' Rage = 93269
            int IDDormusAura = 93269;

            ObjectManager.Update();
            WoWUnit Dormus = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == IDDormus).OrderBy(u => u.Distance).FirstOrDefault();

            #region Move to Dormus Helperpoint
            while (Me.HasAura(IDDormusAura) && Dormus == null && !Me.Combat && Me.Location.Distance(DormusPoint) > 5 && Me.Location.Distance(DormusPoint) < 500)
            {
                WoWMovement.ClickToMove(DormusPoint);
                Logging.Write(Colors.MediumPurple, "Rarekiller: Dormus not in sight, Swim to Dormus Helper Point");
                Thread.Sleep(500);

                ObjectManager.Update();
                Dormus = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == IDDormus).OrderBy(u => u.Distance).FirstOrDefault();
            }
            #endregion

            if (Dormus != null)
            {
                if (!Dormus.IsDead)
                {
                    Logging.WriteQuiet(Colors.MediumPurple, "Rarekiller: Find {0}", Dormus.Name);

                    if (Rarekiller.Settings.Alert)
                        Rarekiller.Alert();

                    Dormus.Target();
					
					#region Move to Dormus
					// ------------- Move to Dormus with Klick to Move -------------------		

                    while (DormusPoint.Distance(Me.Location) > 5 && Me.HasAura(IDDormusAura) && !Dormus.IsDead && !Rarekiller.Settings.ReachedDormusHelperpoint)
					{
						WoWMovement.ClickToMove(DormusPoint);
						Thread.Sleep(100);
						Logging.Write(Colors.MediumPurple, "Rarekiller: Move out of Water to Helperpoint");
                        if (Rarekiller.ToonInvalidCombat) return;
					}
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Reached Dormus Helperpoint");
                    Rarekiller.Settings.ReachedDormusHelperpoint = true;

                    while (Dormus.Location.Distance(Me.Location) > 3 && Me.HasAura(IDDormusAura) && !Dormus.IsDead)
					{
                        if (Navigator.CanNavigateFully(Me.Location, Dormus.Location))
                            Navigator.MoveTo(Dormus.Location);
                        else
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Navigation Issue, use Click to Move");
                            WoWMovement.ClickToMove(Dormus.Location);
                        }
						Thread.Sleep(100);
						Logging.Write(Colors.MediumPurple, "Rarekiller: Move to Dormus");
                        if (Rarekiller.ToonInvalidCombat) return;
					}
					WoWMovement.MoveStop();
					#endregion

					#region Pull Dormus
					// ------------- pull Dormus  -------------------					
					Logging.Write(Colors.MediumPurple, "Rarekiller: Distance: {0}", Dormus.Location.Distance(Me.Location));
					Dormus.Target();
					Dormus.Face();
					Thread.Sleep(100);


                    if (!(Rarekiller.Settings.DefaultPull) && SpellManager.HasSpell(Rarekiller.Settings.Pull))
                        CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Settings.Pull, Dormus, false);
                        //CastSuccess = SpellManager.Cast(Rarekiller.Settings.Pull, Dormus);
                    else if (SpellManager.HasSpell(Rarekiller.Spells.FastPullspell))
                        CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.FastPullspell, Dormus, false);
                        //CastSuccess = SpellManager.Cast(Rarekiller.Spells.FastPullspell, Dormus);
					else
						Logging.Write(Colors.MediumPurple, "Rarekiller: I have no Pullspell");
                    if (!CastSuccess && SpellManager.HasSpell("Shoot"))
                        CastSuccess = RarekillerSpells.CastSafe("Shoot", Dormus, true);
                        //CastSuccess = SpellManager.Cast("Shoot", Dormus);
					if (CastSuccess)
					{
						Logging.Write(Colors.MediumPurple, "Rarekiller: successfully pulled Dormus");
						Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Pull Distance: {0}", Dormus.Location.Distance(Me.Location));
						return;
					}
					else if (!CastSuccess && Me.Combat)
						Logging.Write(Colors.MediumPurple, "Rarekiller: got Aggro");
					else
					{
						Logging.Write(Colors.MediumPurple, "Rarekiller: Pull Fails --> next try");
					}
					#endregion
                }
                else if (!Blacklist.Contains(Dormus.Guid, Rarekiller.Settings.Flags))
                {
                    if (Dormus.CanLoot)
                        if (!Rarekiller.Loothelper(Dormus)) return;

                    if (!Blacklist.Contains(Dormus.Guid, Rarekiller.Settings.Flags))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0}, but sadly he's dead", Dormus.Name);
                        Blacklist.Add(Dormus.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Unit for 60 Minutes.");
                    }
                }
            }
        }

        /// <summary>
        /// Function to Avoid Camel Spit
        /// </summary>
        public void AvoidSpit(WoWUnit Unit)
        {
            if (!StyxWoW.Me.IsFacing(Unit))
            { Unit.Face(); Thread.Sleep(100); }
			Logging.Write(Colors.MediumPurple, "Rarekiller: Avoid Camel Spit");
            //94967 = Aura Spit
            while (Me.HasAura(94967))
            {
                WoWMovement.Move(WoWMovement.MovementDirection.StrafeRight);
                if (Rarekiller.ToonInvalid) return;
            }
            WoWMovement.MoveStop();
            Unit.Face();
			Logging.Write(Colors.MediumPurple, "Rarekiller: successfully avoided Camel Spit");
        }
    }
}
