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
using System.IO;
using System.Xml;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;


namespace katzerle
{

    // -------------- Settings -------------------
    class RarekillerSettings
    {
        public RarekillerSettings()
        {
            if (StyxWoW.Me != null)
                try
                {
                    Load();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                }
        }


        //Default Einstellungen GUI
        // Addons
        public bool MOP = false;
        public bool CATA = true;
        public bool WOTLK = true;
        public bool BC = true;
        public bool LowRAR = false;
        public bool TLPD = true;
        public bool KillList = false;
        public bool RaptorNest = true;
        public bool ObjectsCollector = true;
        public bool AnotherMansTreasure = false;
        public bool InteractNPC = false;
		public bool Poseidus = true;
        public bool Camel = true;
        public bool Aeonaxx = false;
        // Hunt by ID
        public bool HUNTbyID = false;
        public string MobID = "";
        //Tamer

		public bool Hunteractivated = true;
        public bool NotKillTameable = false;
        public bool TameDefault = false;
        public bool TameByID = false;
        public string TameMobID = "";
        public Int32 Tamedistance = 12;
        // Slowfall
        public bool UseSlowfall = true;
        public bool Cloak = false;
        public bool Item = true;
        public bool Spell = false;
        public string Falltimer = "900";
        public string SlowfallSpell = "";
        public string SlowfallItem = "Snowfall Lager";
        // Pullspell
        public bool DefaultPull = true;
        public string Pull = "";
        public string Range = "10";
        public bool Vyragosa = true;
        public bool Blazewing = false;
        //Misc
        public string BlacklistTime = "180";
        public bool BlacklistCheck = true;
        public bool Alert = true;
        public bool Wisper = true;
        public bool BNWisper = true;
        public bool Guild = false;
        public bool Keyer = true;
        public bool Shadowmeld = false;
		public string SoundfileWisper = Rarekiller.Soundfile;
		public string SoundfileGuild = Rarekiller.Soundfile;
		public string SoundfileFoundRare = Rarekiller.Soundfile;
//Some other Stuff
        public Int32 Level = 61;
        public Int64 Blacklist60 = 3600;
        public Int64 Blacklist15 = 900;
        public Int64 Blacklist5 = 300;
        public Int64 Blacklist2 = 120;
        public BlacklistFlags Flags = BlacklistFlags.All;
 	
// Attentione for Developers		
//Developer Things
        public bool DeveloperBoxActive = false;
        public bool DeveloperLogs = false;
//Developer Testcases
        public bool TestRaptorNest = false;
        public bool TestFigurineInteract = false;

        // -------------- Load ConfigFile ---------------
        public void Load()
        {
            //    XmlTextReader reader;
            XmlDocument xml = new XmlDocument();
            XmlNode xvar;
			
            string sPath = Rarekiller.SettingsPath;

            if (!Directory.Exists(sPath))
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Creating Settings directory");
                Directory.CreateDirectory(sPath);
            }

            sPath = Path.Combine(sPath, StyxWoW.Me.RealmName, StyxWoW.Me.Name, "Rarekiller.config");

            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Loading config file");
            if (!File.Exists(sPath))
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: No Special Config - Continuing with Default Config Values");
                return;
            }
            System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            try
            {
                xml.Load(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
                fs.Close();
                return;
            }

            //            xml = new XmlDocument();

            try
            {
                //                xml.Load(reader);
                if (xml == null)
                    return;
                // Load Variables - Addons
                xvar = xml.SelectSingleNode("//Rarekiller/KillList");
                if (xvar != null)
                {
                    KillList = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + KillList.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/MOP");
                if (xvar != null)
                {
                    MOP = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + MOP.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/CATA");
                if (xvar != null)
                {
                    CATA = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + CATA.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/WOTLK");
                if (xvar != null)
                {
                    WOTLK = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + WOTLK.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/BC");
                if (xvar != null)
                {
                    BC = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + BC.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/LowRAR");
                if (xvar != null)
                {
                    LowRAR = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + LowRAR.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/TLPD");
                if (xvar != null)
                {
                    TLPD = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + TLPD.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/InteractNPC");
                if (xvar != null)
                {
                    InteractNPC = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + InteractNPC.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/AnotherMansTreasure");
                if (xvar != null)
                {
                    AnotherMansTreasure = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + AnotherMansTreasure.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/Poseidus");
                if (xvar != null)
                {
                    Poseidus = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Poseidus.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/RaptorNest");
                if (xvar != null)
                {
                    RaptorNest = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + RaptorNest.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/ObjectsCollector");
                if (xvar != null)
                {
                    ObjectsCollector = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + ObjectsCollector.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/HuntByID");
                if (xvar != null)
                {
                    HUNTbyID = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + HUNTbyID.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/MobID");
                if (xvar != null)
                {
                    MobID = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + MobID.ToString());
                }

                // Tamer
                xvar = xml.SelectSingleNode("//Rarekiller/NotKillTameable");
                if (xvar != null)
                {
                    NotKillTameable = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + NotKillTameable.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/TameDefault");
                if (xvar != null)
                {
                    TameDefault = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + TameDefault.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/TameByID");
                if (xvar != null)
                {
                    TameByID = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + TameByID.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/TameMobID");
                if (xvar != null)
                {
                    TameMobID = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + TameMobID.ToString());
                }

                // Load Variables - Slowfall
                xvar = xml.SelectSingleNode("//Rarekiller/UseSlowfall");
                if (xvar != null)
                {
                    UseSlowfall = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + UseSlowfall.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Spell");
                if (xvar != null)
                {
                    Spell = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Spell.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/SlowfallSpell");
                if (xvar != null)
                {
                    SlowfallSpell = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + SlowfallSpell.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Cloak");
                if (xvar != null)
                {
                    Cloak = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Cloak.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Item");
                if (xvar != null)
                {
                    Item = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Item.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/SlowfallItem");
                if (xvar != null)
                {
                    SlowfallItem = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + SlowfallItem.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Falltimer");
                if (xvar != null)
                {
                    Falltimer = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Falltimer.ToString());
                }

                // Load Variables - Pullspell
                xvar = xml.SelectSingleNode("//Rarekiller/DefaultPull");
                if (xvar != null)
                {
                    DefaultPull = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + DefaultPull.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Pull");
                if (xvar != null)
                {
                    Pull = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Pull.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Range");
                if (xvar != null)
                {
                    Range = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Range.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Vyragosa");
                if (xvar != null)
                {
                    Vyragosa = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Vyragosa.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Blazewing");
                if (xvar != null)
                {
                    Blazewing = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Blazewing.ToString());
                }

                // Load Variables - Other Settings
                xvar = xml.SelectSingleNode("//Rarekiller/BlacklistCheck");
                if (xvar != null)
                {
                    BlacklistCheck = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + BlacklistCheck.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/BlacklistTime");
                if (xvar != null)
                {
                    BlacklistTime = Convert.ToString(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + BlacklistTime.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Alert");
                if (xvar != null)
                {
                    Alert = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Alert.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Wisper");
                if (xvar != null)
                {
                    Wisper = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Wisper.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/BNWisper");
                if (xvar != null)
                {
                    BNWisper = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + BNWisper.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/GuildChat");
                if (xvar != null)
                {
                    Guild = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Guild.ToString());
                }
				
				xvar = xml.SelectSingleNode("//Rarekiller/SoundfileFoundRare");
                if (xvar != null)
                {
                    if (Convert.ToString(xvar.InnerText) != "")
                        SoundfileFoundRare = Convert.ToString(xvar.InnerText);
                    else
                        SoundfileFoundRare = Rarekiller.Soundfile;
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + SoundfileFoundRare.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/SoundfileWisper");
                if (xvar != null)
                {
                    if (Convert.ToString(xvar.InnerText) != "")
                        SoundfileWisper = Convert.ToString(xvar.InnerText);
                    else
                        SoundfileWisper = Rarekiller.Soundfile;
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + SoundfileWisper.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/SoundfileGuild");
                if (xvar != null)
                {
                    if (Convert.ToString(xvar.InnerText) != "")
                        SoundfileGuild = Convert.ToString(xvar.InnerText);
                    else
                        SoundfileGuild = Rarekiller.Soundfile;
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + SoundfileGuild.ToString());
                }
				
                xvar = xml.SelectSingleNode("//Rarekiller/Keyer");
                if (xvar != null)
                {
                    Keyer = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Keyer.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Shadowmeld");
                if (xvar != null)
                {
                    Shadowmeld = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Shadowmeld.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Camel");
                if (xvar != null)
                {
                    Camel = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Camel.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/Aeonaxx");
                if (xvar != null)
                {
                    Aeonaxx = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Aeonaxx.ToString());
                }
            }
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
            }
        }

    }
}
