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
    class RarekillerKiller
    {

        public static LocalPlayer Me = StyxWoW.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        public void findAndKillMob()
        {
            bool CastSuccess = false;
			int loothelper = 0;            

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Scan for Rare Mob");
// ----------------- Generate a List with all wanted Rares found in Object Manager ---------------------		
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags) && (
                (Rarekiller.Settings.CATA && (Rarekiller.CataRaresList.ContainsKey(Convert.ToInt32(o.Entry))))
                || (Rarekiller.Settings.Poseidus && ((o.Entry == 50005)	// Poseidus
                        || (o.Entry == 9999999)))			// Platzhalter
                || (Rarekiller.Settings.TLPD && ((o.Entry == 32491)	// Timelost Protodrake
                        || (o.Entry == 32630)))			// Vyragosa
                || (Rarekiller.Settings.WOTLK && (Rarekiller.FrostbittenList.ContainsKey(Convert.ToInt32(o.Entry))))
                || (Rarekiller.Settings.BC && (Rarekiller.BloodyRareList.ContainsKey(Convert.ToInt32(o.Entry))))
                || (Rarekiller.Settings.KillList && (Rarekiller.KillMobsList.ContainsKey(Convert.ToInt32(o.Entry)))) //Kill Mobs from List
                || (Rarekiller.Settings.KillList && o.TaggedByOther && (Rarekiller.TaggedMobsList.ContainsKey(Convert.ToInt32(o.Entry)))) //Kill Tagged Mobs from List
                || ((o.Level == 86 || o.Level == 87 || o.Level == 88 || o.Level == 89 || o.Level == 90) && Rarekiller.Settings.MOP && (o.CreatureRank == Styx.WoWUnitClassificationType.Rare)) // every single Pandaren Rare Mob is hunted
                || ((o.Level < Rarekiller.Settings.Level) && Rarekiller.Settings.LowRAR && (o.CreatureRank == Styx.WoWUnitClassificationType.Rare)) // every single Rare Mob < Level 61 is hunted	
                || (Rarekiller.Settings.HUNTbyID && (o.Entry == Convert.ToInt64(Rarekiller.Settings.MobID)))				// Hunt special IDs 
                )))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (!o.IsDead && !o.IsPet)
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Find a hunted Mob called {0} ID {1}", o.Name, o.Entry);
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: Find {0} ID {1}')", o.Name, o.Entry);

// Don't kill the Rare if ...
                    if (o.TaggedByOther && !Rarekiller.TaggedMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Find {0}, but he's tagged by another Player", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is tagged')", o.Name);
                        return;
                    }


                    if (Rarekiller.Settings.NotKillTameable && o.IsTameable) // ... I want to tame him :)
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Pulse Tamer");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is tameable, don't kill')", o.Name);
                        Rarekiller.Tamer.findAndTameMob();
					}

                    if (o.Level == 86 || o.Level == 87 || o.Level == 88 || o.Level == 89 || o.Level == 90)
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Note: a Pandarenmop is hard to kill");

                        

                    if (Me.IsFlying && Me.IsOutdoors && o.IsIndoors)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Mob is Indoors and I fly Outdoors, so blacklist him to prevent Problems");
                        Logging.Write(Colors.MediumPurple, "Rarekiller: You have to place me next to the Spawnpoint, if you want me to hunt this Mob.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is Indoors')", o.Name);
                        return;
                    }
                    
					if (o.Level > (Me.Level + 4)) // ... 4 Levels higher them me
					{
                        Logging.Write(Colors.MediumPurple, "Rarekiller: His Level is 5 over mine, better not to kill him.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
						return;
					}
					if (o.IsFriendly) // ... is Friendly
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Find {0}, but he's friendly", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: NPC is friendly')", o.Name);
						return;
					}
					if ((o.Entry == 32630) && !Rarekiller.Settings.Vyragosa) // ... my Settings say don't kill Vyragosa
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Config says: don't kill Vyragosa.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: don't kill NPC')", o.Name);
						return;
					}
					if ((o.Entry == 50057) && !Rarekiller.Settings.Blazewing) // ... my Settings say don't kill Blazewing
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Config says: don't kill Blazewing.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: don't kill NPC')", o.Name);
						return;
					}
					if ((o.Entry == 596) || (o.Entry == 599) || Me.IsInInstance)
					{
					// ... Instance Mobs, don't run wild in Instances
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Don't run wild because of RareMobs in Instances.");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
						return;
					}

					if(Rarekiller.BlacklistMobsList.ContainsKey(Convert.ToInt32(o.Entry))) 
					// ... Mob is Blacklisted in Rarekiller/config/BlacklistedMobs.xml
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: {0} is Member of the BlacklistedMobs.xml", o.Name);
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 15 Minutes.");
						return;
					}

                    if (Me.Combat) // ... I'm in combat
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: ... but first I have to finish fighting another one.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: First finish combat')");
						return;
					}
					if (Me.IsOnTransport) // ... I'm on transport
					{
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: ... but I'm on a Transport.");
                        if (Rarekiller.Settings.LUAoutput)
                            Lua.DoString("print('NPCScan: I'm on Transport')");
						return;
					}
						
// ----------------- Alert ---------------------
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Make Noise");
					if (Rarekiller.Settings.Alert)
					{
						if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
							new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
						else if (File.Exists(Rarekiller.Soundfile))
							new SoundPlayer(Rarekiller.Soundfile).Play();
						else
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: playing Soundfile failes");
					}

                    if (Me.IsCasting)
                    {
                        SpellManager.StopCasting();
                        Thread.Sleep(100);
                    }

// ----------------- Move to Mob Part ---------------------	
                    if (!(Me.Pet == null) && ((Me.Class == WoWClass.Hunter) || (Me.Class == WoWClass.Warlock)))
                        Lua.DoString(string.Format("RunMacroText(\"/petpassive\")"), 0);

                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Move to target");
                    BlacklistTimer.Reset();
                    BlacklistTimer.Start();
					
					// ----------------- Hunting Flying Mobs with Groundmount Mode ---------------------
// Spellrange Test Default Pull Spell
                    if (Rarekiller.Settings.DefaultPull && (Convert.ToInt64(Rarekiller.Settings.Range) > Convert.ToInt64(Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell))))
                    {
                        Rarekiller.Settings.Range = Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell);
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Set Range to {0} because of Low-Ranged Default Spell", Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell));
                    }

                    // Spellrange Test Customized Pull Spell
                    if (!Rarekiller.Settings.DefaultPull && (Convert.ToInt64(Rarekiller.Settings.Range) > Convert.ToInt64(Rarekiller.Spells.RangeCheck(Rarekiller.Settings.Pull))))
                    {
                        Rarekiller.Settings.Range = Rarekiller.Spells.RangeCheck(Rarekiller.Settings.Pull);
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Set Range to {0} because of Low-Ranged Customized Spell", Rarekiller.Spells.RangeCheck(Rarekiller.Settings.Pull));
                    }

                    while ((o.Location.Distance(Me.Location) > Convert.ToInt64(Rarekiller.Settings.Range)) && !o.IsDead)
					{
                        if (o.Entry == 49822 || Me.IsIndoors)
							Navigator.MoveTo(o.Location);
						else
							Flightor.MoveTo(o.Location);
						Thread.Sleep(50);

                        if (o.TaggedByOther && !Rarekiller.TaggedMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Find {0}, but he's tagged by another Player", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
                            if (Rarekiller.Settings.LUAoutput)
                                Lua.DoString("print('NPCScan: NPC {0} is tagged')", o.Name);
                            BlacklistTimer.Reset();
                            WoWMovement.MoveStop();
                            return;
                        }

						// ----------------- Security  ---------------------
						if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
						{
                            Logging.Write(Colors.MediumPurple, "Rarekiller Part MoveTo: Can't reach Mob {0}, Blacklist and Move on", o.Name);
                            Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 5 Minutes.");
							BlacklistTimer.Reset();
							WoWMovement.MoveStop();
							return;
						}
					}
					BlacklistTimer.Reset();
                    
                    if (o.IsDead && !o.CanLoot)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Mob was killed by another Player");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                        return;
                    }

                    o.Target();
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Pull at Mob Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: ... my Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Target is Flying - {0}", o.IsFlying);

// ----------------- Pull Part --------------------
					WoWMovement.MoveStop();

                    if (!(Rarekiller.Settings.DefaultPull) && SpellManager.HasSpell(Rarekiller.Settings.Pull))
                        CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Settings.Pull, o, false);
                    else if (SpellManager.HasSpell(Rarekiller.Spells.FastPullspell))
                        CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.FastPullspell, o, false);
                    else
                    {
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I have no valid Pullspell - set Range to 3 for next try");
                        Rarekiller.Settings.Range = "3";
                        findAndKillMob();
                    }

                    if (CastSuccess)
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Successfully pulled {0}", o.Name);
                    else
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Pull fails - set Range to 3 for next try to initiate Aggro", o.Name);
                        Rarekiller.Settings.Range = "3";
                    }

                    if (!(Me.Pet == null) && ((Me.Class == WoWClass.Hunter) || (Me.Class == WoWClass.Warlock)))
                        Lua.DoString(string.Format("RunMacroText(\"/petdefensive\")"), 0);

                    Thread.Sleep(100);
					WoWMovement.MoveStop();
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Use Quick Slowfall: {0} Mob: {1}", Me.IsFalling, o.Name);
                    if (Me.IsFalling && Rarekiller.Settings.UseSlowfall && ((o.Entry == 29753) || (o.Entry == 32491) || (o.Entry == 32630) || (o.Entry == 33687)))
					{
						Thread.Sleep(500);
						Rarekiller.Slowfall.HelpFalling();
					}
					if(Me.CurrentTarget != o)
						o.Target();
					o.Face();
                    return;					
                }
                else if (o.IsDead)
                {
                    if (o.CanLoot)
                    {
// ----------------- Loot Helper for all killed Rare Mobs ---------------------
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Found lootable corpse, move to him");

                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part MoveTo: Take Screen and Move to target");
                        Lua.DoString("TakeScreenshot()");
                        Thread.Sleep(300);
                        while (o.Location.Distance(Me.Location) > 5)
						{
                            if (o.Entry == 49822 || o.IsIndoors)
								Navigator.MoveTo(o.Location);
							else
								Flightor.MoveTo(o.Location);
							Thread.Sleep(100);
							if (Rarekiller.inCombat) return;
						}
						WoWMovement.MoveStop();

                        if (Me.Auras.ContainsKey("Flight Form"))
                            Lua.DoString("CancelShapeshiftForm()");
                        else if (Me.Mounted)
                            Lua.DoString("Dismount()");

                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Take another Screen");
                        Lua.DoString("TakeScreenshot()");
                        Thread.Sleep(300); 
                        while (loothelper < 3)
                        {
                            Thread.Sleep(500);
                            WoWMovement.MoveStop();
                            o.Interact();
                            Thread.Sleep(2000);
                            Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
                            Thread.Sleep(4000);
                            if (!o.CanLoot)
                            {
                                Logging.Write(Colors.MediumPurple, "Rarekiller: successfully looted");
                                Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                                Logging.Write(Colors.MediumPurple, "Rarekiller: Blacklist Mob for 60 Minutes.");
                                return;
                            }
                            else
                            {
                                Logging.Write(Colors.MediumPurple, "Rarekiller: Loot failed, try again");
                                loothelper = loothelper + 1;
                            }
                            
                        }
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Loot failed 3 Times");
                    }

                    if (!Blacklist.Contains(o.Guid, Rarekiller.Settings.Flags))
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
