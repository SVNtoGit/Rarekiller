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
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.Globalization;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins;

namespace katzerle
{
	class Rarekiller: HBPlugin
	{
		//Variables
		public static string name { get { return "Rarekiller"; } }
		public override string Name { get { return name; } }
		public override string Author { get { return "katzerle"; } }
		private readonly static Version _version = new Version(3, 4);
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

        private static Stopwatch Checktimer = new Stopwatch();
        private static Stopwatch Shadowmeldtimer = new Stopwatch();
        public static Random rnd = new Random();

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
		
        public Rarekiller()
        {
			UpdatePlugin();

            Settings.Load();
            Logging.Write(Colors.MediumPurple, "Rarekiller 3.4 loaded");
            if (Me.Class != WoWClass.Hunter)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Tamer: I'm no Hunter. Deactivate the Tamer Part");
                Settings.TameByID = false;
                Settings.TameDefault = false;
                Settings.TameMobID = "";
                Settings.Hunteractivated = false;
            }
            if (Me.Race != WoWRace.NightElf)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I'm no Nightelf. Deactivate Shadowmeld");
                Settings.Shadowmeld = false;
            }

// Developer-Things:
            if (Settings.DeveloperBoxActive)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Developerpart activated");
			if (Settings.DeveloperLogs)
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Developer Logspam activated");

            
        }

        static private void BotStopped(EventArgs args)
        {
//Alerts for Wisper and Guild
            if (Rarekiller.Settings.Wisper)
            {
                Chat.Whisper -= Security.newWhisper;
            }
            if (Rarekiller.Settings.BNWisper)
            {
                Lua.Events.DetachEvent("CHAT_MSG_BN_WHISPER", Security.BNWhisper);
            }
            if (Rarekiller.Settings.Guild)
            {
                Chat.Guild -= Security.newGuild;
                Chat.Officer -= Security.newOfficer;
            }
                        
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
            if (Rarekiller.Settings.Wisper)
            {
                Chat.Whisper += Security.newWhisper;
            }
            if (Rarekiller.Settings.BNWisper)
            {
                Lua.Events.AttachEvent("CHAT_MSG_BN_WHISPER", Security.BNWhisper);
            }
            if (Rarekiller.Settings.Guild)
            {
                Chat.Guild += Security.newGuild;
                Chat.Officer += Security.newOfficer;
            }
//Blacklisted Mobs to List

            XmlDocument BlacklistMobsXML = new XmlDocument();
            string sPath = Path.Combine(FolderPath, "config\\BlacklistedMobs.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load BlacklistedMobs.xml");
            if (File.Exists(sPath))
            {
                System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    BlacklistMobsXML.Load(fs);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs.Close();
                    return;
                }
                XmlElement root = BlacklistMobsXML.DocumentElement;
                foreach (XmlNode Node in root.ChildNodes)
                {
                    Int32 Entry = Convert.ToInt32(Node.Attributes["Entry"].InnerText);
                    string Name = Node.Attributes["Name"].InnerText;
                    BlacklistMobsList.Add(Entry, Name);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name, Entry);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: BlacklistedMobs.xml loaded");
            }
            else
            {
                BlacklistMobsList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/BlacklistedMobs.xml doesn't exist");
            }
//Tameable Mobs to List
            XmlDocument TameableMobsXML = new XmlDocument();
            string sPath2 = Path.Combine(FolderPath, "config\\TameableMobs.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load TameableMobs.xml");
            if (File.Exists(sPath2))
            {
                System.IO.FileStream fs2 = new System.IO.FileStream(@sPath2, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    TameableMobsXML.Load(fs2);
                    fs2.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs2.Close();
                    return;
                }
                XmlElement root2 = TameableMobsXML.DocumentElement;
                foreach (XmlNode TameMob in root2.ChildNodes)
                {
                    Int32 Entry2 = Convert.ToInt32(TameMob.Attributes["Entry"].InnerText);
                    string Name2 = TameMob.Attributes["Name"].InnerText;
                    TameableMobsList.Add(Entry2, Name2);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name2, Entry2);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: TameableMobs.xml loaded");
            }
            else
            {
                TameableMobsList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/TameableMobs.xml doesn't exist");
            }

//CollectObjects to List
            XmlDocument CollectObjectsXML = new XmlDocument();
            string sPath3 = Path.Combine(FolderPath, "config\\CollectObjects.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load CollectObjects.xml");
            if (File.Exists(sPath3))
            {
                System.IO.FileStream fs3 = new System.IO.FileStream(@sPath3, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    CollectObjectsXML.Load(fs3);
                    fs3.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs3.Close();
                    return;
                }
                XmlElement root3 = CollectObjectsXML.DocumentElement;
                foreach (XmlNode CollectObject in root3.ChildNodes)
                {
                    Int32 Entry3 = Convert.ToInt32(CollectObject.Attributes["Entry"].InnerText);
                    string Name3 = CollectObject.Attributes["Name"].InnerText;
                    CollectObjectsList.Add(Entry3, Name3);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name3, Entry3);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: CollectObjects.xml loaded");
            }
            else
            {
                CollectObjectsList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/CollectObjects.xml doesn't exist");
            }

//KillMobs to List
            XmlDocument KillMobsXML = new XmlDocument();
            string sPath4 = Path.Combine(FolderPath, "config\\KillMobs.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load KillMobs.xml");
            if (File.Exists(sPath4))
            {
                System.IO.FileStream fs4 = new System.IO.FileStream(@sPath4, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    KillMobsXML.Load(fs4);
                    fs4.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs4.Close();
                    return;
                }
                XmlElement root4 = KillMobsXML.DocumentElement;
                foreach (XmlNode KillMob in root4.ChildNodes)
                {
                    Int32 Entry4 = Convert.ToInt32(KillMob.Attributes["Entry"].InnerText);
                    string Name4 = KillMob.Attributes["Name"].InnerText;
                    KillMobsList.Add(Entry4, Name4);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name4, Entry4);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: KillMobs.xml loaded");
            }
            else
            {
                KillMobsList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/KillMobs.xml doesn't exist");
            }

//InteractNPC to List
            XmlDocument InteractNPCXML = new XmlDocument();
            string sPath5 = Path.Combine(FolderPath, "config\\InteractNPC.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load InteractNPC.xml");
            if (File.Exists(sPath5))
            {
                System.IO.FileStream fs5 = new System.IO.FileStream(@sPath5, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    InteractNPCXML.Load(fs5);
                    fs5.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs5.Close();
                    return;
                }
                XmlElement root5 = InteractNPCXML.DocumentElement;
                foreach (XmlNode InteractNPC in root5.ChildNodes)
                {
                    Int32 Entry5 = Convert.ToInt32(InteractNPC.Attributes["Entry"].InnerText);
                    string Name5 = InteractNPC.Attributes["Name"].InnerText;
                    InteractNPCList.Add(Entry5, Name5);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name5, Entry5);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: InteractNPC.xml loaded");
            }
            else
            {
                InteractNPCList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/InteractNPC.xml doesn't exist");
            }

//AnotherMansTreasure to List
            XmlDocument AnotherMansTreasureXML = new XmlDocument();
            string sPath6 = Path.Combine(FolderPath, "config\\AnotherMansTreasure.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load AnotherMansTreasure.xml");
            if (File.Exists(sPath6))
            {
                System.IO.FileStream fs6 = new System.IO.FileStream(@sPath6, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    AnotherMansTreasureXML.Load(fs6);
                    fs6.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs6.Close();
                    return;
                }
                XmlElement root6 = AnotherMansTreasureXML.DocumentElement;
                foreach (XmlNode AnotherMansTreasure in root6.ChildNodes)
                {
                    Int32 Entry6 = Convert.ToInt32(AnotherMansTreasure.Attributes["Entry"].InnerText);
                    string Name6 = AnotherMansTreasure.Attributes["Name"].InnerText;
                    AnotherMansTreasureList.Add(Entry6, Name6);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name6, Entry6);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: AnotherMansTreasure.xml loaded");
            }
            else
            {
                AnotherMansTreasureList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/AnotherMansTreasure.xml doesn't exist");
            }

//Frostbitten to List
            XmlDocument FrostbittenXML = new XmlDocument();
            string sPath7 = Path.Combine(FolderPath, "config\\Frostbitten.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load Frostbitten.xml");
            if (File.Exists(sPath7))
            {
                System.IO.FileStream fs7 = new System.IO.FileStream(@sPath7, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    FrostbittenXML.Load(fs7);
                    fs7.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs7.Close();
                    return;
                }
                XmlElement root7 = FrostbittenXML.DocumentElement;
                foreach (XmlNode Frostbitten in root7.ChildNodes)
                {
                    Int32 Entry7 = Convert.ToInt32(Frostbitten.Attributes["Entry"].InnerText);
                    string Name7 = Frostbitten.Attributes["Name"].InnerText;
                    FrostbittenList.Add(Entry7, Name7);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name7, Entry7);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Frostbitten.xml loaded");
            }
            else
            {
                FrostbittenList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/Frostbitten.xml doesn't exist");
            }

//BloodyRare to List
            XmlDocument BloodyRareXML = new XmlDocument();
            string sPath8 = Path.Combine(FolderPath, "config\\BloodyRare.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load BloodyRare.xml");
            if (File.Exists(sPath8))
            {
                System.IO.FileStream fs8 = new System.IO.FileStream(@sPath8, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    BloodyRareXML.Load(fs8);
                    fs8.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs8.Close();
                    return;
                }
                XmlElement root8 = BloodyRareXML.DocumentElement;
                foreach (XmlNode BloodyRare in root8.ChildNodes)
                {
                    Int32 Entry8 = Convert.ToInt32(BloodyRare.Attributes["Entry"].InnerText);
                    string Name8 = BloodyRare.Attributes["Name"].InnerText;
                    BloodyRareList.Add(Entry8, Name8);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name8, Entry8);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: BloodyRare.xml loaded");
            }
            else
            {
                BloodyRareList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/BloodyRare.xml doesn't exist");
            }

//CataRares to List
            XmlDocument CataRaresXML = new XmlDocument();
            string sPath9 = Path.Combine(FolderPath, "config\\CataRares.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load CataRares.xml");
            if (File.Exists(sPath9))
            {
                System.IO.FileStream fs9 = new System.IO.FileStream(@sPath9, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    CataRaresXML.Load(fs9);
                    fs9.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs9.Close();
                    return;
                }
                XmlElement root9 = CataRaresXML.DocumentElement;
                foreach (XmlNode CataRare in root9.ChildNodes)
                {
                    Int32 Entry9 = Convert.ToInt32(CataRare.Attributes["Entry"].InnerText);
                    string Name9 = CataRare.Attributes["Name"].InnerText;
                    CataRaresList.Add(Entry9, Name9);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name9, Entry9);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: CataRares.xml loaded");
            }
            else
            {
                CataRaresList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/CataRares.xml doesn't exist");
            }

//TaggedMobs to List
            XmlDocument TaggedMobsXML = new XmlDocument();
            string sPath10 = Path.Combine(FolderPath, "config\\TaggedMobs.xml");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Load TaggedMobs.xml");
            if (File.Exists(sPath10))
            {
                System.IO.FileStream fs10 = new System.IO.FileStream(@sPath10, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                try
                {
                    TaggedMobsXML.Load(fs10);
                    fs10.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs10.Close();
                    return;
                }
                XmlElement root10 = TaggedMobsXML.DocumentElement;
                foreach (XmlNode TaggedMob in root10.ChildNodes)
                {
                    Int32 Entry10 = Convert.ToInt32(TaggedMob.Attributes["Entry"].InnerText);
                    string Name10 = TaggedMob.Attributes["Name"].InnerText;
                    TaggedMobsList.Add(Entry10, Name10);
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Name: {0} Entry: {1}", Name10, Entry10);
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: TaggedMobs.xml loaded");
            }
            else
            {
                TaggedMobsList.Add(99999999, "DummyMob");
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: File Rarekiller/config/TaggedMobs.xml doesn't exist");
            }
        }



        public override void Pulse()
		{
			try
			{
                
// ------------ Deactivate if not in Game or in Combat etc
                if (Me == null || !StyxWoW.IsInGame || Me.IsDead || Me.IsGhost || InPetCombat())
                    return;

// ------------ Deactivate Plugin in BGs and Inis
                if (Battlegrounds.IsInsideBattleground || Me.IsInInstance)
				return;
				
// ------------ Part Init
                if (!hasItBeenInitialized)
                {
                    Initialize();
                    hasItBeenInitialized = true;
                }

// ------------ Start the Timer for Anti-AFK
                if (Settings.Keyer && !Checktimer.IsRunning && !Me.IsMoving)
                    Checktimer.Start();
                if (Settings.Keyer && Checktimer.IsRunning && Me.IsMoving)
                    Checktimer.Reset();
                if (Settings.Shadowmeld && !Shadowmeldtimer.IsRunning && !Me.IsMoving && !Me.HasAura("Shadowmeld"))
                    Shadowmeldtimer.Start();
                if (Settings.Shadowmeld && Shadowmeldtimer.IsRunning && Me.IsMoving)
                    Shadowmeldtimer.Reset();

// ------------ Part Slowfall if falling down
				if(Me.IsFalling && Settings.UseSlowfall)
				{
					Thread.Sleep(Convert.ToInt32(Rarekiller.Settings.Falltimer));
					if (Me.IsFalling && !Me.IsDead && !Me.IsGhost)
						Slowfall.HelpFalling();
				}
					
				if (!Me.Combat) //ToDo
				{

// ------------ Part Camel Figurine
					if (Settings.Camel || Settings.TestFigurineInteract || Settings.AnotherMansTreasure || Settings.InteractNPC)
						Camel.findAndPickupObject();
                    if (Settings.Camel)
						Camel.findAndKillDormus();

// ------------ Part Raptor Nest
                    if (Settings.RaptorNest || Settings.TestRaptorNest || Settings.ObjectsCollector || Settings.AnotherMansTreasure)
						Collector.findAndPickupObject();

// ------------ Part The Tamer
					if ((Me.Class == WoWClass.Hunter) && (Rarekiller.Settings.TameDefault || Rarekiller.Settings.TameByID))
					{
						if (Me.HealthPercent > 30)
							Tamer.findAndTameMob();
					}

// ------------ Part Rarekiller						
                    if (Settings.KillList || Settings.MOP || Settings.WOTLK || Settings.BC || Settings.CATA || Settings.TLPD || Settings.LowRAR || Settings.HUNTbyID || Settings.Poseidus)
						Killer.findAndKillMob();

// ------------ Part Security - Keypresser
                    if (Settings.Keyer && !Me.IsMoving)
					{
                        if (Checktimer.Elapsed.TotalSeconds > MoveTimer)
						{
                            Checktimer.Reset();
							MoveTimer = rnd.Next(90, 200);
							Security.Movearound();
						}
					}

// ------------ Part Shadowmeld
                    if (Settings.Shadowmeld && !Me.IsMoving)
                    {
                        if (Shadowmeldtimer.Elapsed.TotalSeconds > 5)
                        {
                            Shadowmeldtimer.Reset();
                            if (SpellManager.HasSpell("Shadowmeld") && SpellManager.CanCast("Shadowmeld") && !Me.HasAura("Shadowmeld"))
                            {
                                bool SpellSuccess = SpellManager.Cast("Shadowmeld");
                                Logging.Write(Colors.MediumPurple, "Rarekiller: Shadowmeld activated - {0}", SpellSuccess);
                            }
                        }
                    }
				}
			}
			
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
                Logging.WriteDiagnostic(Colors.Red, e.Message);
			}
		}

// ------------ Misc Functions
		static public string Soundfile
        {
            get
            {
				string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\siren.wav");
                return sPath;
            }
        }
		
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
		
//Update Function (deactivated
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
//Function In Combat
        static public bool inCombat
        {
            get
            {
                if (Me == null || !StyxWoW.IsInGame || Me.Combat || Me.IsDead || Me.IsGhost || InPetCombat()) return true;
                return false;
            }
        }

        static public bool InPetCombat()
        {
            List<string> cnt = Lua.GetReturnValues("dummy,reason=C_PetBattles.IsTrapAvailable() return dummy,reason");

            if (cnt != null) { if (cnt[1] != "0") return true; }
            return false;
        }



	}
}
