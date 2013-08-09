//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//                and all the brave Testers
//
//==================================================================

// -- ToDo --

using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.Globalization;
using System.Windows.Media;
using System.Media;
using System.Linq;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins;
using Styx.Pathing;
using Styx.WoWInternals.World;


namespace katzerle
{
	class Rarekiller: HBPlugin
	{
		//Variables
		public static string name { get { return "Rarekiller"; } }
		public override string Name { get { return name; } }
		public override string Author { get { return "katzerle"; } }
		private readonly static Version _version = new Version(4, 6);
		public override Version Version { get { return _version; } }
		public override string ButtonText { get { return "Settings"; } }
		public override bool WantButton { get { return true; } }
		
		public static LocalPlayer Me = StyxWoW.Me;
        public static RarekillerSettings Settings = new RarekillerSettings();
        public static RarekillerSlowfall Slowfall = new RarekillerSlowfall();
        public static RarekillerKiller Killer = new RarekillerKiller();
        public static RarekillerCamel Camel = new RarekillerCamel();
        public static RarekillerTamer Tamer = new RarekillerTamer();
        public static RarekillerCollector Collector = new RarekillerCollector();
        public static RarekillerSecurity Security = new RarekillerSecurity();
		public static RarekillerSpells Spells = new RarekillerSpells();
        public static RarekillerMoPRares MoPRares = new RarekillerMoPRares();

        private static Stopwatch Checktimer = new Stopwatch();
        private static Stopwatch Shadowmeldtimer = new Stopwatch();
        // Developer Thing (ToDo Remove)
        private static Stopwatch DumpAuraTimer = new Stopwatch();
        private static Stopwatch FallTimer = new Stopwatch();

		public static Dictionary<Int32, string> BlacklistMobsList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> TameableMobsList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> CollectObjectsList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> KillMobsList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> InteractNPCList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> AnotherMansTreasureList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> FrostbittenList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> BloodyRareList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> CataRaresList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> TaggedMobsList = new Dictionary<Int32, string>();
		
        public static bool hasItBeenInitialized = false; 
        Int32 MoveTimer = 100;
        public static Random rnd = new Random();
		
        public Rarekiller()
        {
			UpdatePlugin();
            //Settings.Load();
            Logging.WriteQuiet(Colors.MediumPurple, "Rarekiller 4.6 loaded");
            if (Me.Class != WoWClass.Hunter)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: I'm no Hunter. Deactivate the Tamer Part");
                Settings.TameByID = false;
                Settings.TameDefault = false;
                Settings.TameMobID = "";
                Settings.Hunteractivated = false;
                Settings.Footprints = false;
            }
            if (!Settings.BETA)
            {
                Settings.Footprints = false;
                Settings.Blingtron = false;
            }
            if (Me.Race != WoWRace.NightElf)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: I'm no Nightelf. Deactivate Shadowmeld");
                Settings.Shadowmeld = false;
            }

// Developer-Things:
            if (Settings.DeveloperBoxActive)
                Logging.Write(Colors.MediumPurple, "Rarekiller: Developerpart activated");
        }

        static private void BotStopped(EventArgs args)
        {
            Logging.Write(Colors.MediumPurple, "Rarekiller: Had to Blacklist {0} permanently", Settings.BlacklistCounter);

//Alerts for Wisper and Guild
            Chat.Whisper -= Security.newWhisper;
            Lua.Events.DetachEvent("CHAT_MSG_BN_WHISPER", Security.BNWhisper);
            Chat.Guild -= Security.newGuild;
            Chat.Officer -= Security.newOfficer;
                        
// Register the events of the Start/Stop Button in HB
			BotEvents.OnBotStopped -= BotStopped;
// Clear some Lists
            BlacklistMobsList.Clear();
            TameableMobsList.Clear();
            CollectObjectsList.Clear();
            KillMobsList.Clear();
            InteractNPCList.Clear();
            AnotherMansTreasureList.Clear();
            FrostbittenList.Clear();
            BloodyRareList.Clear();
            CataRaresList.Clear();
            TaggedMobsList.Clear();
            hasItBeenInitialized = false;
        }
		
        public override void OnButtonPress()
        {
            ConfigForm.ShowDialog();
        }

        private Form MyForm;
        public Form ConfigForm
        {
            get
            {
                if(MyForm == null)
                    MyForm = new RarekillerUI();
                return MyForm;
            }
        }
		
		/// <summary>
        /// Initialize - Do this once at first Pulse
        /// </summary>
        static public void Initialize()
        {
// Register the events of the Start/Stop Button in HB
			BotEvents.OnBotStopped += BotStopped;

// ------------ Deactivate RareAlerter if Rarekiller is Enabled			
            List<PluginContainer> _pluginList = PluginManager.Plugins;
            foreach (PluginContainer _plugin in _pluginList)
            {
				if (_plugin.Enabled && _plugin.Plugin.Name == "RareAlerter")
				{
					Logging.Write(Colors.MediumPurple, "Rarekiller: RareAlerter deactivated");
                    _plugin.Enabled = false;
				}
            }


//Alerts for Wisper and Guild
            Chat.Whisper += Security.newWhisper;
            Lua.Events.AttachEvent("CHAT_MSG_BN_WHISPER", Security.BNWhisper);
            Chat.Guild += Security.newGuild;
            Chat.Officer += Security.newOfficer;

            string sPath1 = Path.Combine(FolderPath, "config\\BlacklistedMobs.xml");
            FillDictionaryFromFile(BlacklistMobsList, sPath1);

            string sPath2 = Path.Combine(FolderPath, "config\\TameableMobs.xml");
            FillDictionaryFromFile(TameableMobsList, sPath2);

            string sPath3 = Path.Combine(FolderPath, "config\\CollectObjects.xml");
            FillDictionaryFromFile(CollectObjectsList, sPath3);

            string sPath4 = Path.Combine(FolderPath, "config\\KillMobs.xml");
            FillDictionaryFromFile(KillMobsList, sPath4);

            string sPath5 = Path.Combine(FolderPath, "config\\InteractNPC.xml");
            FillDictionaryFromFile(InteractNPCList, sPath5);

            string sPath6 = Path.Combine(FolderPath, "config\\AnotherMansTreasure.xml");
            FillDictionaryFromFile(AnotherMansTreasureList, sPath6);

            string sPath7 = Path.Combine(FolderPath, "config\\Frostbitten.xml");
            FillDictionaryFromFile(FrostbittenList, sPath7);

            string sPath8 = Path.Combine(FolderPath, "config\\BloodyRare.xml");
            FillDictionaryFromFile(BloodyRareList, sPath8);

            string sPath9 = Path.Combine(FolderPath, "config\\CataRares.xml");
            FillDictionaryFromFile(CataRaresList, sPath9);

            string sPath10 = Path.Combine(FolderPath, "config\\TaggedMobs.xml");
            FillDictionaryFromFile(TaggedMobsList, sPath10);
        }



        public override void Pulse()
		{
			try
            {
                #region Plugin deactivated if ...
                if (Rarekiller.ToonInvalid) return;
                if (Battlegrounds.IsInsideBattleground || Me.IsInInstance)
				    return;
                #endregion

                #region Init
                if (!hasItBeenInitialized)
                {
                    Initialize();
                    hasItBeenInitialized = true;
                }
                #endregion

                #region Timer
                if (Settings.Keyer && !Checktimer.IsRunning && !Me.IsMoving)
                    Checktimer.Start();
                if (Settings.Keyer && Checktimer.IsRunning && Me.IsMoving)
                    Checktimer.Reset();
                if (Settings.Shadowmeld && !Shadowmeldtimer.IsRunning && !Me.IsMoving && !Me.HasAura("Shadowmeld"))
                    Shadowmeldtimer.Start();
                if (Settings.Shadowmeld && Shadowmeldtimer.IsRunning && Me.IsMoving)
                    Shadowmeldtimer.Reset();
                // Developer Thing (ToDo Remove)
                if (!DumpAuraTimer.IsRunning && Rarekiller.Settings.MoPRaresDeveloper)
                {
                    DumpAuraTimer.Reset();
                    DumpAuraTimer.Start();
                }


                #endregion

                #region Slowfall
                if (Me.IsFalling && Settings.UseSlowfall && !FallTimer.IsRunning)
                    FallTimer.Start();
                if (!Me.IsFalling && Settings.UseSlowfall && FallTimer.IsRunning)
                    FallTimer.Reset();
                if (Me.IsFalling && Settings.UseSlowfall && FallTimer.ElapsedMilliseconds > Convert.ToInt32(Rarekiller.Settings.Falltimer))
                {
                    FallTimer.Reset();
                    Slowfall.HelpFalling();

                }

                //if (Me.IsFalling && Settings.UseSlowfall)
                //{
                //    Thread.Sleep(Convert.ToInt32(Rarekiller.Settings.Falltimer));
                //    if (Me.IsFalling && !Me.IsDead && !Me.IsGhost)
                //        Slowfall.HelpFalling();
                //}
                #endregion

                if (!Me.Combat)
                {

                    #region Pulse Camel Figurine and NPC Interactor
                    if (Settings.Camel || Settings.TestFigurineInteract || Settings.AnotherMansTreasure || Settings.InteractNPC)
                        Camel.findAndInteractNPC();
                    // --> Dormus' Rage = 93269
                    if (Me.HasAura(93269) || Settings.Camel)
                        Camel.findAndKillDormus();
                    #endregion

                    #region Pulse Object Interactor
                    if (Settings.RaptorNest || Settings.TestRaptorNest || Settings.ObjectsCollector || Settings.AnotherMansTreasure)
                        Collector.findAndPickupObject();
                    #endregion

                    #region Pulse Tamer
                    if (((Me.Class == WoWClass.Hunter) && (Rarekiller.Settings.TameDefault || Rarekiller.Settings.TameByID)) || Rarekiller.Settings.TestcaseTamer)
                    {
                        if (Me.HealthPercent > 30)
                            Tamer.findAndTameMob();
                    }

                    if ((Me.Class == WoWClass.Hunter && Rarekiller.Settings.Footprints) || Rarekiller.Settings.TestcaseTamer)
                        Tamer.findandfollowFootsteps();
                    #endregion

                    #region Pulse Rarekiller
                    if (Settings.KillList || Settings.MOP || Settings.WOTLK || Settings.BC || Settings.CATA || Settings.TLPD || Settings.LowRAR || Settings.HUNTbyID || Settings.Poseidus)
                        Killer.findAndKillMob();
                    #endregion

                    #region Pulse Security
                    if (Settings.Keyer && !Me.IsMoving)
                    {
                        if (Checktimer.Elapsed.TotalSeconds > MoveTimer)
                        {
                            Checktimer.Reset();
                            MoveTimer = rnd.Next(90, 200);
                            Security.Movearound();
                        }
                    }

                    if (Settings.Shadowmeld && !Me.IsMoving)
                    {
                        if (Shadowmeldtimer.Elapsed.TotalSeconds > 5)
                        {
                            Shadowmeldtimer.Reset();
                            if (SpellManager.HasSpell("Shadowmeld") && SpellManager.CanCast("Shadowmeld") && !Me.HasAura("Shadowmeld"))
                            {
                                bool SpellSuccess = RarekillerSpells.CastSafe("Shadowmeld", Me, false);
                                //bool SpellSuccess = SpellManager.Cast("Shadowmeld");
                                Logging.Write(Colors.MediumPurple, "Rarekiller: Shadowmeld activated - {0}", SpellSuccess);
                            }
                        }
                    }
                    #endregion
                }
                else // In Combat with MoPRares / Dormus
                {

                    #region Hozen - working - but needs some luck
                    if (MoPRares.Hozen != null)
                    {
                        // Bananarang
                        if (MoPRares.Hozen.CastingSpellId == 125311)
                            MoPRares.AvoidEnemyCast(MoPRares.Hozen, 0, 50);
                        // Going Bananas
                        else if (MoPRares.Hozen.CastingSpellId == 125363)
                        {
                            while (MoPRares.Hozen.CastingSpellId == 125363 && MoPRares.Hozen.Location.Distance(Me.Location) > 5)
                            {
                                if (Me.IsSwimming)
                                    WoWMovement.ClickToMove(MoPRares.Hozen.Location);
                                else
                                    Navigator.MoveTo(MoPRares.Hozen.Location);
                                Thread.Sleep(80);
                                if (Rarekiller.ToonInvalid) return;
                            }  
                        }
                    }
                    #endregion

                    #region Mogu Sorcerer - working
                    if (MoPRares.MoguSorcerer != null)
                    {
                        //Voidcloud
                        if (Me.HasAura("Voidcloud") && MoPRares.getVoidcloudList != null)
                        {
                            if (MoPRares.getVoidcloudList[0].Distance < (MoPRares.getVoidcloudList[0].Radius * 1.6f))
                                MoPRares.AvoidEnemyAOE(MoPRares.MoguSorcerer, "Voidcloud", 10, MoPRares.getVoidcloudList, 10, 3);
                        }
                    }
                    #endregion

                    #region Saurok - working
                    if (MoPRares.Saurok != null)
                    {
                        if (MoPRares.Saurok.Combat && MoPRares.Saurok.Location.Distance(Me.Location) > 15)
                        {
                            while (MoPRares.Saurok.Location.Distance(Me.Location) > 5)
                            {
                                if (Me.IsSwimming)
                                    WoWMovement.ClickToMove(MoPRares.Saurok.Location);
                                else
                                    Navigator.MoveTo(MoPRares.Saurok.Location);
                                Thread.Sleep(80);
                                if (Rarekiller.ToonInvalid) return;
                            }
                        }
                        if (Me.CurrentTarget != null && Me.CurrentTarget != MoPRares.Saurok && !MoPRares.Saurok.IsDead)
                        {
                            MoPRares.Saurok.Target();
                        }
                    }
                    #endregion

                    #region Jinyu - working - but needs some luck
                    if (MoPRares.Jinyu != null)
                    {
                        // Rain Dance
                        if (MoPRares.Jinyu.CastingSpellId == 124860 && SpellManager.CanCast(Spells.Stun))
                        {
                            RarekillerSpells.CastSafe(Spells.Stun, MoPRares.Jinyu, false);
                            Logging.Write(Colors.MediumPurple, "Rarekiller: * {0}. - Stun", Spells.Stun);
                        }
                        else if (MoPRares.Jinyu.CastingSpellId == 124860)
                        {
                            MoPRares.AvoidEnemyMissiles(MoPRares.Jinyu, 0, 7, true, 10, 5);
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Avoid Missiles");
                        }

                        // Torrent - interrupt
                        else if (MoPRares.Jinyu.CastingSpellId == 124935 && SpellManager.CanCast(Spells.Interrupt))
                        {
                            RarekillerSpells.CastSafe(Spells.Interrupt, MoPRares.Jinyu, false);
                            Logging.Write(Colors.MediumPurple, "Rarekiller: * {0}. - Interrupt", Spells.Interrupt);
                        }
                    }
                    #endregion

                    #region Mogu Warrior - working
                    if (MoPRares.MoguWarrior != null)
                    {
                        while (MoPRares.MoguWarrior.Distance2D <= 15 && MoPRares.MoguWarrior.IsCasting && MoPRares.MoguWarrior.CastingSpellId == 124946 && !Me.IsSafelyBehind(MoPRares.MoguWarrior))
                        {
                            if (Me.IsSwimming)
                                WoWMovement.ClickToMove(MoPRares.getLocationBehindUnit(MoPRares.MoguWarrior));
                            else
                                Navigator.MoveTo(MoPRares.getLocationBehindUnit(MoPRares.MoguWarrior));
                            Thread.Sleep(80);
                            if (Rarekiller.ToonInvalid) return;
                        }
                    }
                    #endregion

                    #region Mantid - working but Index Fehler
                    if (MoPRares.Mantid != null)
                    {
                        // Blade Flurry
                        while (MoPRares.Mantid.Location.Distance(Me.Location) <= 20 && MoPRares.Mantid.IsCasting && MoPRares.Mantid.CastingSpellId == 125370 && !Me.IsSafelyBehind(MoPRares.Mantid))
                        {
                            if (Me.IsSwimming)
                                WoWMovement.ClickToMove(MoPRares.getLocationBehindUnit(MoPRares.Mantid));
                            else
                                Navigator.MoveTo(MoPRares.getLocationBehindUnit(MoPRares.Mantid));
                            Thread.Sleep(80);
                            if (Rarekiller.ToonInvalid) return;
                        }

                        // Tornados
                        if (MoPRares.getTornadoList != null)
                        {
                            if (MoPRares.getTornadoList[0].Distance < 7)
                                MoPRares.AvoidEnemyAOE(MoPRares.Mantid, 27, MoPRares.getTornadoList, 10, 5);
                        }

                    }
                    #endregion

                    #region Pandaren - don't work good
                    if (MoPRares.Pandaren != null)
                    {
                        // Spinning Crane Kick
                        if (MoPRares.Pandaren.CastingSpellId == 125799 && SpellManager.CanCast(Spells.Stun))
                        {
                            RarekillerSpells.CastSafe(Spells.Stun, MoPRares.Pandaren, false);
                            Logging.Write(Colors.MediumPurple, "Rarekiller: * {0}. - Stun", Spells.Stun);
                        }
                        else if (MoPRares.Pandaren.CastingSpellId == 125799 && MoPRares.Pandaren.Location.Distance(Me.Location) < 25)
                        {
                            WoWMovement.Move(WoWMovement.MovementDirection.Backwards);
                            MoPRares.FleeingFromEnemy(MoPRares.Pandaren, 125799, 60, 10, 5);
                        }
                        // Chi Burst
                        else if (MoPRares.Pandaren.Combat && MoPRares.Pandaren.Location.Distance(Me.Location) > 10)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Run to Pandaren because of Chistoß");
                            while (MoPRares.Pandaren.Location.Distance(Me.Location) > 5)
                            {
                                if (Me.IsSwimming)
                                    WoWMovement.ClickToMove(MoPRares.Pandaren.Location);   
                                else
                                    Navigator.MoveTo(MoPRares.Pandaren.Location);
                                Thread.Sleep(80);
                                if (Rarekiller.ToonInvalid) return;
                            }
                        }
                        // Healing Mists
                        else if (MoPRares.Pandaren.CastingSpellId == 125802 && SpellManager.CanCast(Spells.Interrupt))
                        {
                            RarekillerSpells.CastSafe(Spells.Interrupt, MoPRares.Pandaren, false);
                            Logging.Write(Colors.MediumPurple, "Rarekiller: * {0}. - Interrupt", Spells.Interrupt);
                        }

                    }
                    #endregion

                    #region Yaungol - working
                    if (MoPRares.Yaungol != null)
                    {
                        // Yaungol Stomp
                        if (MoPRares.Yaungol.CastingSpellId == 124289 && MoPRares.Yaungol.Location.Distance(Me.Location) < 15)
                            MoPRares.FleeingFromEnemy(MoPRares.Yaungol, 124289, 17, 10, 5);

                        // Bellowing Rage
                        else if (MoPRares.Yaungol.HasAura("Bellowing Rage") && MoPRares.Yaungol.Location.Distance(Me.Location) < 25)
                            MoPRares.FleeingFromEnemy(MoPRares.Yaungol, 0, 30, 10, 5);

                        // Rushing Charge
                        else if (MoPRares.Yaungol.Combat && Me.Location.Distance(MoPRares.Yaungol.Location) > 20)
                        {
                            while (Me.Location.Distance(MoPRares.Yaungol.Location) > 10)
                            {
                                if (Me.IsSwimming)
                                    WoWMovement.ClickToMove(MoPRares.Yaungol.Location);
                                else
                                    Navigator.MoveTo(MoPRares.Yaungol.Location);
                                Thread.Sleep(80);
                                if (Rarekiller.ToonInvalid) return;
                            }
                        }
                    }
                    #endregion

                    #region Dormus Avoid Spit - working
                    if (Camel.Dormus != null)
                    {
                        // Developer Thing (ToDo Remove)
                        if (DumpAuraTimer.Elapsed.TotalSeconds > 5 && Rarekiller.Settings.MoPRaresDeveloper)
                        {
                            DumpAuraTimer.Reset();
                            MoPRares.DumpAOEEffect();
                        }

                        //94967 = Aura Spit
                        if (Me.HasAura(94967))
                            Camel.AvoidSpit(Camel.Dormus);

                        //if (Me.HasAura(94967) && Camel.getSpitList != null && Camel.getSpitList[0].Distance < (Camel.getSpitList[0].Radius * 1.6f))
                        //    MoPRares.AvoidEnemyAOE(Camel.Dormus, 0, 2, 20, Camel.getSpitList, 15, 3);
                    }

                    #endregion
                }
			}
			
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
                Logging.WriteDiagnostic(Colors.Red, e.Message);
			}
		}

        #region Misc Helper Functions
        /// <summary>
        /// Returns the Default Soundfile
        /// </summary>
		static public string Soundfile
        {
            get
            {
				string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\siren.wav");
                return sPath;
            }
        }

        /// <summary>
        /// Returns another Default Soundfile
        /// </summary>
        static public string Soundfile2
        {
            get
            {
                string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\attention.wav");
                return sPath;
            }
        }

        /// <summary>
        /// Returns the Folder of Rarekiller
        /// </summary>
		static public string FolderPath
        {
            get
            {
                string sPath = Process.GetCurrentProcess().MainModule.FileName;
                sPath = Path.GetDirectoryName(sPath);
                sPath = Path.Combine(sPath, "Plugins\\RareKiller\\");
                return sPath;
            }
        }

        /// <summary>
        /// Returns the Honorbuddy Settings Folder
        /// </summary>
        static public string SettingsPath
        {
            get
            {
                string sPath = Process.GetCurrentProcess().MainModule.FileName;
                sPath = Path.GetDirectoryName(sPath);
                sPath = Path.Combine(sPath, "Settings\\");
                return sPath;
            }
        }

        /// <summary>
        /// Alert Function
        /// </summary>
        static public void Alert()
        {
            if(Rarekiller.Settings.Alert)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Make Noise");
                if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                    new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                else if (File.Exists(Rarekiller.Soundfile))
                    new SoundPlayer(Rarekiller.Soundfile).Play();
                else
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: playing Soundfile failes");
            }
        }

        /// <summary>
        /// returns true Me is in Combat or Transport and puts into Logfile
        /// </summary>
        static public bool DontInteract
        {
            get
            {
                if (Me.Combat)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: ... but first I have to finish fighting another Unit.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: First finish combat')");
                    return true;
                }
                if (Me.IsOnTransport)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: ... but I'm on a Transport.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: I'm on Transport')");
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Loots a Lootable Unit
        /// </summary>
        static public bool Loothelper(WoWUnit o)
        {
            if (o.Lootable)
            {
                int loothelper = 0;
                Logging.Write(Colors.MediumPurple, "Rarekiller: Found lootable corpse, move to him");
                if (o.Entry == 50245) Dismount();
                if (o.Entry == 49822 || o.Entry == 50245 || o.IsIndoors)
                {if (!MoveTo(o.Location, 5, true)) return false;}
                else
                {if (!MoveTo(o.Location, 5, false)) return false;}
                if (o.Entry != 50245) Dismount();

                while (loothelper < 3)
                {
                    WoWMovement.MoveStop();
                    o.Interact();
                    Thread.Sleep(1000);
                    Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
                    Thread.Sleep(1000);
                    if (!o.CanLoot)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: successfully looted");
                        Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Unit for 60 Minutes.");
                        return true;
                    }
                    else
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Loot failed, try again");
                        loothelper = loothelper + 1;
                    }

                }
                Logging.Write(Colors.MediumPurple, "Rarekiller: Loot failed 3 Times");
                return false;
            }
            return true;
        }

               /// <summary>
        /// Credits to exemplar.
        /// </summary>
        /// <returns>Z-Coordinates for PoolPoints so we don't jump into the water.</returns>
        static public float getGroundZ(WoWPoint p)
        {
            WoWPoint ground = WoWPoint.Empty;

            GameWorld.TraceLine(new WoWPoint(p.X, p.Y, (p.Z + 100)), new WoWPoint(p.X, p.Y, (p.Z - 5)), GameWorld.CGWorldFrameHitFlags.HitTestGroundAndStructures/* | GameWorld.CGWorldFrameHitFlags.HitTestBoundingModels | GameWorld.CGWorldFrameHitFlags.HitTestWMO*/, out ground);

            if (ground != WoWPoint.Empty)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Ground Z: {0}.", ground.Z);
                return ground.Z;
            }
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Ground Z returned float.MinValue.");
            return float.MinValue;
        }

        /// <summary>
        /// Loads the Config Files
        /// </summary>
        static public void FillDictionaryFromFile(Dictionary<Int32, string> Dictionary, string sPath)
        {
            XmlDocument XMLDokument = new XmlDocument();
            Logging.Write(Colors.MediumPurple, "Rarekiller: Load {0}", sPath);
            if (File.Exists(sPath))
            {
                System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    XMLDokument.Load(fs);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs.Close();
                    return;
                }
                XmlElement root = XMLDokument.DocumentElement;
                foreach (XmlNode Node in root.ChildNodes)
                {
                    Int32 Entry = Convert.ToInt32(Node.Attributes["Entry"].InnerText);
                    string Name = Node.Attributes["Name"].InnerText;
                    Dictionary.Add(Entry, Name);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Add to List - Name: {0} Entry: {1}", Name, Entry);
                }
                Logging.Write(Colors.MediumPurple, "Rarekiller: transfered {0} Elements", Dictionary.Count);
            }
            else
            {
                Dictionary.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File {0} doesn't exist", sPath);
            }
            Logging.Write(Colors.MediumPurple, "-------------------------------------");
        }
        #endregion

        #region Movement Functions
        /// <summary>
        /// Descend Function
        /// </summary>
        static public bool DescendToLand(WoWUnit Unit)
        {
            Logging.Write(Colors.MediumPurple, "Rarekiller: Descend to Land");
            WoWPoint Ground = Me.Location;
            Ground.Z = getGroundZ(Me.Location);
            if (Ground.Z != float.MinValue)
            {
                while (Me.Location.Distance(Ground) > 5)
                {
                    Flightor.MoveTo(Ground);
                    Thread.Sleep(100);
                    if (Rarekiller.ToonInvalidCombat) return false;
                }
            }
            else
            {
                WoWPoint Groundnear = Me.Location;
                Groundnear.Z = Unit.Z + 5;
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Move Groundnear, because getGroundZ returns float.MinValue");
                while (Me.Location.Distance(Groundnear) > 3 && Me.IsFlying)
                {
                    Flightor.MoveTo(Groundnear);
                    Thread.Sleep(100);
                    if (Rarekiller.ToonInvalidCombat) return false;
                }

                //Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: MovementDirection.Descend, because getGroundZ returns float.MinValue");
                //WoWMovement.Move(WoWMovement.MovementDirection.Descend);
                //Thread.Sleep(4000);
                //WoWMovement.MoveStop();
            }
            return true;
        }

        /// <summary>
        /// Dismount Function
        /// </summary>
        static public void Dismount()
        {
            Logging.Write(Colors.MediumPurple, "Rarekiller: Dismount");
            //Dismount
            WoWMovement.MoveStop();
            if (Me.Auras.ContainsKey("Flight Form"))
                Lua.DoString("CancelShapeshiftForm()");
            else if (Me.Mounted)
                Lua.DoString("Dismount()");
            Thread.Sleep(500);
        }

        /// <summary>
        /// Moves to a Unit
        /// </summary>
        static public bool MoveTo(WoWUnit o, Int64 Distance, bool Forceground)
        {
            Logging.Write(Colors.MediumPurple, "Rarekiller: Move to {0}", o.Name);
            Stopwatch BlacklistTimer = new Stopwatch();
            BlacklistTimer.Start();
            while ((o.Location.Distance(Me.Location) > Distance) && !o.IsDead)
            {
                if ((o.Entry == 49822 || Me.IsIndoors || Forceground) && Me.IsSwimming)
                    WoWMovement.ClickToMove(o.Location);
                else if (o.Entry == 49822 || Me.IsIndoors || Forceground)
                    Navigator.MoveTo(o.Location);
                else
                    Flightor.MoveTo(o.Location);
                Thread.Sleep(50);

                if (Rarekiller.ToonInvalidCombat) return false; // && !(o.Entry == 50364)
                if (o.TaggedByOther && !Rarekiller.TaggedMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Unit {0} is tagged by another Player", o.Name);
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Unit for 5 Minutes.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: NPC {0} is tagged')", o.Name);
                    BlacklistTimer.Reset();
                    WoWMovement.MoveStop();
                    return false;
                }

                // ----------------- Security  ---------------------
                if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Can't reach Unit {0}, Blacklist and Move on", o.Name);
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Unit for 5 Minutes.");
                    BlacklistTimer.Reset();
                    WoWMovement.MoveStop();
                    return false;
                }
            }
            BlacklistTimer.Reset();
            WoWMovement.MoveStop();
            if (o.IsDead && !o.CanLoot)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: Unit was killed by another Player");
                Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Unit for 60 Minutes.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Moves to a Unit
        /// </summary>
        static public bool MoveTo(WoWObject o, Int64 Distance, bool ForceGround)
        {
            Logging.Write(Colors.MediumPurple, "Rarekiller: Move to {0}", o.Name);
            Stopwatch BlacklistTimer = new Stopwatch();
            BlacklistTimer.Start();

            while (o.Location.Distance(Me.Location) > Distance)
            {
                if (ForceGround && Me.IsSwimming)
                    WoWMovement.ClickToMove(o.Location);
                else if (ForceGround)
                    Navigator.MoveTo(o.Location);
                else
                    Flightor.MoveTo(o.Location);
                Thread.Sleep(100);
                // ----------------- Security  ---------------------
                if (Rarekiller.ToonInvalidCombat) return false;
                if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Can't reach Object {0}, Blacklist and Move on", o.Name);
                    Blacklist.Add(o.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    BlacklistTimer.Reset();
                    WoWMovement.MoveStop();
                    return false;
                }
            }
            BlacklistTimer.Reset();
            Thread.Sleep(300);
            WoWMovement.MoveStop();
            return true;
        }

        /// <summary>
        /// Fly to a Helperpoint for a special Unit
        /// </summary>
        static public bool MoveTo(WoWPoint Helperpoint, WoWUnit o, Int64 Distance, bool ForceGround)
        {
            Logging.Write(Colors.MediumPurple, "Rarekiller: Move to Helperpoint for {0}", o.Name);
            while (Me.Location.Distance(Helperpoint) > Distance)
            {
                if (ForceGround && Me.IsSwimming)
                    WoWMovement.ClickToMove(Helperpoint);
                else if (ForceGround)
                    Navigator.MoveTo(Helperpoint);
                else
                    Flightor.MoveTo(Helperpoint);
                Thread.Sleep(100);
                if (Rarekiller.ToonInvalidCombat) return false;
                if (o.TaggedByOther && !Rarekiller.TaggedMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Unit {0} is tagged by another Player", o.Name);
                    Blacklist.Add(Me.CurrentTarget.Guid, Rarekiller.Settings.Flags, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Blacklist Unit for 5 Minutes.");
                    if (Rarekiller.Settings.LUAoutput)
                        Lua.DoString("print('NPCScan: NPC {0} is tagged')", o.Name);
                    WoWMovement.MoveStop();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Fly to a Helperpoint
        /// </summary>
        static public bool MoveTo(WoWPoint Helperpoint, Int64 Distance, bool ForceGround)
        {
            Logging.Write(Colors.MediumPurple, "Rarekiller: Move to Helperpoint");
            while (Me.Location.Distance(Helperpoint) > Distance)
            {
                if (ForceGround && Me.IsSwimming)
                    WoWMovement.ClickToMove(Helperpoint);
                else if (ForceGround)
                    Navigator.MoveTo(Helperpoint);
                else
                    Flightor.MoveTo(Helperpoint);
                Thread.Sleep(100);
                if (Rarekiller.ToonInvalidCombat) return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// returns true if Game crashed, Relogwindow appears, the Toon is in Combat or Petbattle or Dead or Ghost etc
        /// </summary>
        static public bool ToonInvalidCombat
        {
            get
            {
                if (Me == null || !StyxWoW.IsInGame || Me.Combat || Me.IsDead || Me.IsGhost || InPetCombat()) return true;
                return false;
            }
        }

        /// <summary>
        /// returns true if Game crashed, Relogwindow appears, the Toon is in Combat or Petbattle or Dead or Ghost etc
        /// </summary>
        static public bool ToonInvalid
        {
            get
            {
                if (Me == null || !StyxWoW.IsInGame || Me.IsDead || Me.IsGhost || InPetCombat()) return true;
                return false;
            }
        }

        /// <summary>
        /// returns true Petbattle
        /// </summary>
        static public bool InPetCombat()
        {
            List<string> cnt = Lua.GetReturnValues("dummy,reason=C_PetBattles.IsTrapAvailable() return dummy,reason");
            if (cnt != null) { if (cnt[1] != "0") return true; }
            return false;
        }

        /// <summary>
        /// Takes a look in the SVN if something new is there and prints a Message in Honorbuddy Window
        /// </summary>
		static public void UpdatePlugin()
		{
			//Here you have to insert your Internetlocation for the SVN
			string Website = "http://katzerls-plugins.googlecode.com/svn/trunk";

            try
            {
                WebClient client = new WebClient();
                IFormatProvider culture = new CultureInfo("fr-FR", true);

                XDocument VersionLatest = XDocument.Load(Website + "/UpdaterRarekiller.xml");
				Logging.WriteDiagnostic(Colors.MediumPurple, "Load Website");
                XDocument VersionCurrent = XDocument.Load(FolderPath + "\\config\\Updater.xml");
				Logging.WriteDiagnostic(Colors.MediumPurple, "Load Updater.xml local");
                DateTime latestTime = DateTime.Parse(VersionLatest.Element("RarekillerUpdater").Element("UpdateTime").Value, culture, DateTimeStyles.NoCurrentDateDefault);
                DateTime currentTime = DateTime.Parse(VersionCurrent.Element("Updater").Element("UpdateTime").Value, culture, DateTimeStyles.NoCurrentDateDefault);
                string Version = VersionLatest.Element("RarekillerUpdater").Element("Version").Value;
                //Compare if the new Updater XML has a newer Date as the current installed one and if yes: Update the Files
                if (latestTime <= currentTime)
                    return;
                Logging.Write(Colors.MediumPurple, "**********************************************");
                Logging.Write(Colors.MediumPurple, "Rarekiller: New Version available {0}", Version);
                Logging.Write(Colors.MediumPurple, "Download it in the Pluginsection of Honorbuddy", Version);
                Logging.Write(Colors.MediumPurple, "Link or via SVN");
                Logging.Write(Colors.MediumPurple, "**********************************************");
            }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
            }
			
		}
    }
}
