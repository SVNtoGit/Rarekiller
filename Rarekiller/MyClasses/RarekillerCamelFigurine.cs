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
        //public static WoWPoint IndoorNPC1 = new WoWPoint(-11066.67, -2100.342, 175.2816);
        //public static WoWPoint IndoorNPC2 = new WoWPoint(-11066.67, -2100.342, 175.2816);
        //public static WoWPoint IndoorNPC3 = new WoWPoint(-11066.67, -2100.342, 175.2816);
        //public static WoWPoint IndoorNPC4 = new WoWPoint(-11066.67, -2100.342, 175.2816);
        //public static WoWPoint IndoorNPC5 = new WoWPoint(-11066.67, -2100.342, 175.2816);

        public void findAndPickupObject()
        {
			bool ForceGround = false;

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Scan for NPC to Interact");
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                    ((o.Entry == 50409) && Rarekiller.Settings.Camel) || ((o.Entry == 50410) && Rarekiller.Settings.Camel) // 50409 might be the porting Camel Figurine
                    || (Rarekiller.AnotherMansTreasureList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.AnotherMansTreasure && o.Entry < 200000)
                    || (Rarekiller.InteractNPCList.ContainsKey(Convert.ToInt32(o.Entry)) && Rarekiller.Settings.InteractNPC)
                    || ((o.Entry == 48959) && Rarekiller.Settings.TestFigurineInteract) //Testcase rostiger Amboss - Schnotzz Landing
                )))
                .OrderBy(o => o.Distance).ToList();

            List<WoWUnit> RareList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(r => ((r.CreatureRank == Styx.WoWUnitClassificationType.Rare) && r.Level > 85 && !r.IsDead)).OrderBy(r => r.Distance).ToList();

            foreach (WoWUnit o in objList)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller Part NPC: Find {0} ID {1}", o.Name, o.Entry);

				
                if (Rarekiller.Settings.Alert)
                {
                    if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                        new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play(); 
                    else if (File.Exists(Rarekiller.Soundfile))
                        new SoundPlayer(Rarekiller.Soundfile).Play();
                    else
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Camel: playing Soundfile failes");
                }
				
				
// ----------------- Underground ----------
                if (o.IsIndoors && Me.IsFlying && Me.IsOutdoors && (o.Location.Distance(Me.Location) > 30))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part NPC: Can't reach NPC because it is Indoors and I fly Outdoors {0}, Blacklist and Move on", o.Name);
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    return;
                }
// -----------------  don't collect if Rare Pandaria Elite Around
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
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: ... but first I have to finish fighting another Mob.");
                    return;
                }
                if (Me.IsOnTransport)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: ... but I'm on a Transport.");
                    return;
                }
                if (Me.IsCasting)
                {
                    SpellManager.StopCasting();
                    Thread.Sleep(100);
                }
					
				if (Me.IsFlying && ((o.Location.Distance(ProblemCamel1) < 10) || (o.Location.Distance(ProblemCamel2) < 10) || 
						(o.Location.Distance(ProblemCamel3) < 10) || (o.Location.Distance(ProblemCamel4) < 20)))
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Found a Problem NPC {0} so dismount and walk", o.Entry);
					while (o.Location.Distance(Me.Location) > 30)
					{
						if (ForceGround)
							Navigator.MoveTo(o.Location);
						else
							Flightor.MoveTo(o.Location);
						Thread.Sleep(100);
						if (Rarekiller.inCombat) return;
					}
					WoWMovement.MoveStop();
					Thread.Sleep(1000);
					//Descend to Land
					WoWMovement.Move(WoWMovement.MovementDirection.Descend);
					Thread.Sleep(2000);
					WoWMovement.MoveStop();
					//Dismount
                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");

					Thread.Sleep(300);
					ForceGround = true;						
				}

                Logging.Write(Colors.MediumPurple, "Rarekiller Part MoveTo: Move to target");

                BlacklistTimer.Reset();
                BlacklistTimer.Start();


				while (o.Location.Distance(Me.Location) > 4)
				{
					if (ForceGround)
						Navigator.MoveTo(o.Location);
					else
						Flightor.MoveTo(o.Location);
					Thread.Sleep(100);
					if (Rarekiller.inCombat) return;
					if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
					{
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                        Logging.Write(Colors.MediumPurple, "Rarekiller Part NPC: Blacklist NPC for 15 Minutes.");
						BlacklistTimer.Reset();
						WoWMovement.MoveStop();
						return;
					}
				}

                BlacklistTimer.Reset();
                Thread.Sleep(500);
                WoWMovement.MoveStop();

                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: NPC Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part NPC: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
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
                Logging.Write(Colors.MediumPurple, "Rarekiller Part NPC: Interact with NPC - ID {0}", o.Entry);
                if (o.Entry == 50410 || o.Entry == 50409)
					Thread.Sleep(10000);
				else
					Thread.Sleep(1000);
                //64143 = Test
				if (o.Entry == 65552 || o.Entry == 64272 || o.Entry == 64004 || o.Entry == 64191 || o.Entry == 64227)
				{
					Lua.DoString("RunMacroText(\"/click GossipTitleButton1\");");
					Thread.Sleep(1000);
				}
				ForceGround = false;
            }
        }

        public void findAndKillDormus()
        {
            bool CastSuccess = false;

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Scan for Dormus");
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags)) && (o.Entry == 50245))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (!o.IsDead)
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: Find Dormus.");
                    
                    if (Rarekiller.Settings.Alert)
                    {
                        if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                            new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                        else if (File.Exists(Rarekiller.Soundfile))
                            new SoundPlayer(Rarekiller.Soundfile).Play();
                        else
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: playing Soundfile failes");
                    }


                    if (RoutineManager.Current.NeedRest)
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: CC says we need rest - Letting it do it before Fight.");
                        RoutineManager.Current.Rest();
                    }

                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");

                    o.Target();
                    Thread.Sleep(500);

                    while (!Rarekiller.inCombat)
                    {
                        // ------------- Move to Dormus with Klick to Move -------------------					
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: Move to Dormus");

                        while (Me.IsSwimming)
                        {
                            WoWMovement.ClickToMove(o.Location);
                        }
                        WoWMovement.MoveStop();

                        while (o.Location.Distance(Me.Location) > 3)
                        {
                            Navigator.MoveTo(o.Location);
                            Thread.Sleep(100);
                        }
                        WoWMovement.MoveStop();
                        // ------------- pull Dormus  -------------------					
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: Distance: {0}", o.Location.Distance(Me.Location));
						o.Target();
						o.Face();
						Thread.Sleep(100);

                        if (!(Rarekiller.Settings.DefaultPull) && SpellManager.HasSpell(Rarekiller.Settings.Pull))
                            CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Settings.Pull, o, true);
                        else if (SpellManager.HasSpell(Rarekiller.Spells.FastPullspell))
                            CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.FastPullspell, o, false);
                        else
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: I have no Pullspell");
                        if (!CastSuccess && SpellManager.HasSpell("Shoot"))
                            CastSuccess = RarekillerSpells.CastSafe("Shoot", o, true);
                        if (CastSuccess)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: successfully pulled Dormus");
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: Pull Distance: {0}", o.Location.Distance(Me.Location));
                            return;
                        }
                        else if (!CastSuccess && Me.Combat)
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: got Aggro");
                        else
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: Pull Fails --> next try");
                        }
                    }
                }
                else
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: Found lootable {0}, move to him", o.Name);
                    // ----------------- Loot Helper ---------------------
                    if (o.CanLoot)
                    {
                        if (Me.Auras.ContainsKey("Flight Form"))
                            Lua.DoString("CancelShapeshiftForm()");
                        else if (Me.Mounted)
                            Lua.DoString("Dismount()");

                        o.Target();
                        // ------------- Move to Corpse -------------------					
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Dormus: ... move to him to loot");
 						while (o.Location.Distance(Me.Location) > 3)
						{
							Navigator.MoveTo(o.Location);
							Thread.Sleep(100);
						}
						
                        Thread.Sleep(500);
                        WoWMovement.MoveStop();
                        o.Interact();
                        Thread.Sleep(2000);
                        Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
                        Thread.Sleep(4000);
                        if (!o.CanLoot)
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: successfuly looted");
                        else
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part Dormus: Loot failed, try again");

                    }
                    else if (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Find {0}, but sadly he's dead", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                    }
                }
            }
        }
    }
}
