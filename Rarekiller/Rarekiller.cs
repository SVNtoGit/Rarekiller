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
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Net;
using System.Globalization;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins;
using Styx.Pathing;

namespace katzerle
{
	class Rarekiller: HBPlugin
	{
		//Variables
		public static string name { get { return "Rarekiller"; } }
		public override string Name { get { return name; } }
		public override string Author { get { return "katzerle"; } }
		private readonly static Version _version = new Version(3, 0);
		public override Version Version { get { return _version; } }
		public override string ButtonText { get { return "Settings"; } }
		public override bool WantButton { get { return true; } }
		
		public static LocalPlayer Me = StyxWoW.Me;
        public static RarekillerSettings Settings = new RarekillerSettings();
        public static RarekillerSlowfall Slowfall = new RarekillerSlowfall();
        public static RarekillerKiller Killer = new RarekillerKiller();
        public static RarekillerCamel Camel = new RarekillerCamel();
        public static RarekillerTamer Tamer = new RarekillerTamer();
        public static RarekillerCollector RaptorNest = new RarekillerCollector();
        public static RarekillerSecurity Security = new RarekillerSecurity();
		public static RarekillerSpells Spells = new RarekillerSpells();

        private static Stopwatch Checktimer = new Stopwatch();
        public static Random rnd = new Random();

		public static Dictionary<Int32, string> BlacklistMobsList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> TameableMobsList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> CollectObjectsList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> KillMobsList = new Dictionary<Int32, string>();
		
        public static bool hasItBeenInitialized = false; 
        Int32 MoveTimer = 100;
		
        public Rarekiller()
        {
			UpdatePlugin();

            Settings.Load();
            Logging.Write(Colors.MediumPurple, "Rarekiller loaded");
            if (Me.Class != WoWClass.Hunter)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller Part Tamer: I'm no Hunter. Deactivate the Tamer Part");
                Settings.TameByID = false;
                Settings.TameDefault = false;
                Settings.TameMobID = "";
                Settings.Hunteractivated = false;
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
                    _plugin.Enabled = false;
            }


//Alerts for Wisper and Guild
            if (Rarekiller.Settings.Wisper)
            {
                Chat.Whisper += Security.newWhisper;
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
			}
//Tameable Mobs to List
            XmlDocument TameableMobsXML = new XmlDocument();
            string sPath2 = Path.Combine(FolderPath, "config\\TameableMobs.xml");
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
            }
//CollectObjects to List
            XmlDocument CollectObjectsXML = new XmlDocument();
            string sPath3 = Path.Combine(FolderPath, "config\\CollectObjects.xml");
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
            }
//KillMobs to List
            XmlDocument KillMobsXML = new XmlDocument();
            string sPath4 = Path.Combine(FolderPath, "config\\CollectObjects.xml");
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
                Int32 Entry3 = Convert.ToInt32(KillMob.Attributes["Entry"].InnerText);
                string Name3 = KillMob.Attributes["Name"].InnerText;
                KillMobsList.Add(Entry3, Name3);
            }
        }

        public override void Pulse()
		{
			try
			{
                
// ------------ Deactivate if not in Game or in Combat etc
                if (Me == null || !StyxWoW.IsInGame || inCombat)
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
                if (Settings.Keyer && !Checktimer.IsRunning)
                    Checktimer.Start();

// ------------ Part Slowfall if falling down
				if(Me.IsFalling && Settings.UseSlowfall)
				{
					Thread.Sleep(Convert.ToInt32(Rarekiller.Settings.Falltimer));
					if (Me.IsFalling && !Me.IsDead && !Me.IsGhost)
						Slowfall.HelpFalling();
				}
					
				if (!inCombat)
				{

// ------------ Part Camel Figurine
					if (Settings.Camel || Settings.TestFigurineInteract)
						Camel.findAndPickupObject();
                    if (Settings.Camel)
						Camel.findAndKillDormus();

// ------------ Part Raptor Nest
					if (Settings.RaptorNest || Settings.TestRaptorNest)
						RaptorNest.findAndPickupNest();

// ------------ Part The Tamer
					if ((Me.Class == WoWClass.Hunter) && (Rarekiller.Settings.TameDefault || Rarekiller.Settings.TameByID))
					{
						if (Me.HealthPercent > 30)
							Tamer.findAndTameMob();
					}

// ------------ Part Rarekiller						
					if (Settings.MOP || Settings.WOTLK || Settings.BC || Settings.CATA || Settings.TLPD || Settings.LowRAR || Settings.HUNTbyID || Settings.Poseidus)
						Killer.findAndKillMob();

// ------------ Part Security - Keypresser
					if (Settings.Keyer)
					{
						if (!Checktimer.IsRunning || Checktimer.Elapsed.TotalSeconds > MoveTimer)
						{
							Checktimer.Reset();
							Checktimer.Start();
							MoveTimer = rnd.Next(90, 200);
							if(!Me.IsMoving)
								Security.Movearound();
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
