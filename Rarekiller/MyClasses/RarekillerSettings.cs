//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//
//==================================================================
using System;
using System.IO;
using System.Xml;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals.WoWObjects;


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
        public bool DarkSoil = false;
        public bool ObjectsCollector = true;
        public bool Blingtron = false;
        public bool AnotherMansTreasure = false;
        public bool InteractNPC = false;
		public bool Poseidus = true;
        public bool Camel = true;
        public bool Aeonaxx = false;
        // Mists of Pandaria
        public bool Bonobos50828 = false;
        public bool IkIk50836 = false;
        public bool Nanners50840 = false;
        public bool Ferocious50823 = false;
        public bool Scritch50831 = false; 
        public bool Spriggin50830 = false; 
        public bool Yowler50832 = false; 
        public bool Aethis50750 = false;          
        public bool Cournith50768 = false; 
        public bool Eshelon50772 = false; 
        public bool Selena50766 = false; 
        public bool Zai50769 = false;                    
        public bool Sahn50780 = false; 
        public bool Nalash50776 = false; 
        public bool Garlok50739 = false; 
        public bool Kaltik50749 = false;                     
        public bool Lithik50734 = false; 
        public bool Nallak50364 = false; 
        public bool Kraxik50363 = false; 
        public bool Skithik50733 = false;                   
        public bool Torik50388 = false;
        public bool Borginn50341 = false; 
        public bool Kang50349 = false; 
        public bool Gaarn50340 = false;                     
        public bool Karr50347 = false; 
        public bool Kornas50338 = false; 
        public bool Norlaxx50344 = false; 
        public bool Sulikshor50339 = false;                    
        public bool Havak50354 = false; 
        public bool JonnDar50351 = false; 
        public bool Kahtir50355 = false; 
        public bool Krol50356 = false;                    
        public bool Morgrinn50350 = false; 
        public bool Qunas50352 = false; 
        public bool Urgolax50359 = false; 
        public bool AiLi50821 = false;                     
        public bool Ahone50817 = false; 
        public bool AiRan50822 = false; 
        public bool Ruun50816 = false; 
        public bool Nasra50811 = false;                    
        public bool Urobi50808 = false; 
        public bool Yul50820 = false; 
        public bool Arness50787 = false; 
        public bool Moldo50806 = false;                     
        public bool Nessos50789 = false; 
        public bool Omnis50805 = false; 
        public bool Salyin50783 = false; 
        public bool Sarnak50782 = false;                    
        public bool Siltriss50791 = false; 
        public bool Blackhoof51059 = false; 
        public bool Dak50334 = false; 
        public bool Ferdinand51078 = false;                     
        public bool GoKan50331 = false; 
        public bool Korda50332 = false; 
        public bool Lon50333 = false; 
        public bool Yorik50336 = false;

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
        public bool Footprints = false;
        // Slowfall
        public bool UseSlowfall = true;
        public bool Cloak = false;
        public bool Item = false;
        public bool Spell = false;
        public string Falltimer = "500";
        public string SlowfallSpell = "";
        public string SlowfallItem = "";
        // Pullspell
        public bool DefaultPull = true;
        public string Pull = "";
        public string Range = "5";
        public bool Vyragosa = true;
        public bool Blazewing = false;
        //Misc
        public string BlacklistTime = "180";
        public bool BlacklistCheck = true;
        public bool PlayerScan = false;
        public bool Alert = true;
        public bool Wisper = true;
        public bool BNWisper = true;
        public bool Guild = false;
        public bool Keyer = true;
        public bool Shadowmeld = false;
        public string SoundfileWisper = Rarekiller.Soundfile2;
        public string SoundfileGuild = Rarekiller.Soundfile2;
		public string SoundfileFoundRare = Rarekiller.Soundfile;
        public bool LUAoutput = false;
//Some other Stuff
        public Int32 Level = 61;
        public Int64 Blacklist60 = 3600;
        public Int64 Blacklist15 = 900;
        public Int64 Blacklist5 = 300;
        public Int64 Blacklist2 = 120;
        public BlacklistFlags Flags = BlacklistFlags.All;
        public Int32 PullCounter = 0;
        public Int32 MaxPullCounter = 3;
        public ulong GuidCurrentPull = 0;
        public Int64 BlacklistCounter = 0;
 	
// Attentione for Developers		
//Developer Things
        public bool DeveloperBoxActive = false;
        public bool MoPRaresDeveloper = true;
        public bool BETA = true;
//Developer Testcases
        public bool TestCaseDormus = false;
        public bool TestRaptorNest = false;
        public bool TestFigurineInteract = false;
        public bool TestcaseTamer = false;
        public bool ReachedDormusHelperpoint = false;

        // -------------- Load ConfigFile ---------------

        /// <summary>
        /// Loads the Configfile
        /// </summary>
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
                xvar = xml.SelectSingleNode("//Rarekiller/Blingtron");
                if (xvar != null)
                {
                    Blingtron = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Blingtron.ToString());
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
                xvar = xml.SelectSingleNode("//Rarekiller/DarkSoil");
                if (xvar != null)
                {
                    DarkSoil = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + DarkSoil.ToString());
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
                xvar = xml.SelectSingleNode("//Rarekiller/Footprints");
                if (xvar != null)
                {
                    Footprints = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Footprints.ToString());
                }
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
                xvar = xml.SelectSingleNode("//Rarekiller/LUAoutput");
                if (xvar != null)
                {
                    LUAoutput = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + LUAoutput.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/PlayerScan");
                if (xvar != null)
                {
                    PlayerScan = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + PlayerScan.ToString());
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

                xvar = xml.SelectSingleNode("//Rarekiller/Bonobos50828");
                if (xvar != null)
                {
                    Bonobos50828 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Bonobos50828.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/IkIk50836");
                if (xvar != null)
                {
                    IkIk50836 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + IkIk50836.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Nanners50840");
                if (xvar != null)
                {
                    Nanners50840 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Nanners50840.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Ferocious50823");
                if (xvar != null)
                {
                    Ferocious50823 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Ferocious50823.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Scritch50831");
                if (xvar != null)
                {
                    Scritch50831 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Scritch50831.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Spriggin50830");
                if (xvar != null)
                {
                    Spriggin50830 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Spriggin50830.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Yowler50832");
                if (xvar != null)
                {
                    Yowler50832 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Yowler50832.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Aethis50750");
                if (xvar != null)
                {
                    Aethis50750 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Aethis50750.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/Cournith50768");
                if (xvar != null)
                {
                    Cournith50768 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Cournith50768.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Eshelon50772");
                if (xvar != null)
                {
                    Eshelon50772 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Eshelon50772.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Selena50766");
                if (xvar != null)
                {
                    Selena50766 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Selena50766.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Zai50769");
                if (xvar != null)
                {
                    Zai50769 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Zai50769.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Sahn50780");
                if (xvar != null)
                {
                    Sahn50780 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Sahn50780.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Nalash50776");
                if (xvar != null)
                {
                    Nalash50776 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Nalash50776.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Garlok50739");
                if (xvar != null)
                {
                    Garlok50739 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Garlok50739.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Kaltik50749");
                if (xvar != null)
                {
                    Kaltik50749 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Kaltik50749.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/Lithik50734");
                if (xvar != null)
                {
                    Lithik50734 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Lithik50734.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Nallak50364");
                if (xvar != null)
                {
                    Nallak50364 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Nallak50364.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Kraxik50363");
                if (xvar != null)
                {
                    Kraxik50363 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Kraxik50363.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Skithik50733");
                if (xvar != null)
                {
                    Skithik50733 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Skithik50733.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Torik50388");
                if (xvar != null)
                {
                    Torik50388 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Torik50388.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Borginn50341");
                if (xvar != null)
                {
                    Borginn50341 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Borginn50341.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Kang50349");
                if (xvar != null)
                {
                    Kang50349 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Kang50349.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Gaarn50340");
                if (xvar != null)
                {
                    Gaarn50340 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Gaarn50340.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/Karr50347");
                if (xvar != null)
                {
                    Karr50347 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Karr50347.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Kornas50338");
                if (xvar != null)
                {
                    Kornas50338 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Kornas50338.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Norlaxx50344");
                if (xvar != null)
                {
                    Norlaxx50344 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Norlaxx50344.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Sulikshor50339");
                if (xvar != null)
                {
                    Sulikshor50339 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Sulikshor50339.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Havak50354");
                if (xvar != null)
                {
                    Havak50354 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Havak50354.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/JonnDar50351");
                if (xvar != null)
                {
                    JonnDar50351 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + JonnDar50351.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Kahtir50355");
                if (xvar != null)
                {
                    Kahtir50355 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Kahtir50355.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/Krol50356");
                if (xvar != null)
                {
                    Krol50356 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Krol50356.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Morgrinn50350");
                if (xvar != null)
                {
                    Morgrinn50350 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Morgrinn50350.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Qunas50352");
                if (xvar != null)
                {
                    Qunas50352 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Qunas50352.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Urgolax50359");
                if (xvar != null)
                {
                    Urgolax50359 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Urgolax50359.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/AiLi50821");
                if (xvar != null)
                {
                    AiLi50821 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + AiLi50821.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Ahone50817");
                if (xvar != null)
                {
                    Ahone50817 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Ahone50817.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/AiRan50822");
                if (xvar != null)
                {
                    AiRan50822 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + AiRan50822.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Ruun50816");
                if (xvar != null)
                {
                    Ruun50816 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Ruun50816.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/Nasra50811");
                if (xvar != null)
                {
                    Nasra50811 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Nasra50811.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Urobi50808");
                if (xvar != null)
                {
                    Urobi50808 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Urobi50808.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Yul50820");
                if (xvar != null)
                {
                    Yul50820 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Yul50820.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Arness50787");
                if (xvar != null)
                {
                    Arness50787 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Arness50787.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Moldo50806");
                if (xvar != null)
                {
                    Moldo50806 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Moldo50806.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Nessos50789");
                if (xvar != null)
                {
                    Nessos50789 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Nessos50789.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Omnis50805");
                if (xvar != null)
                {
                    Omnis50805 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Omnis50805.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Salyin50783");
                if (xvar != null)
                {
                    Salyin50783 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Salyin50783.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/Sarnak50782");
                if (xvar != null)
                {
                    Sarnak50782 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Sarnak50782.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Siltriss50791");
                if (xvar != null)
                {
                    Siltriss50791 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Siltriss50791.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Blackhoof51059");
                if (xvar != null)
                {
                    Blackhoof51059 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Blackhoof51059.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Dak50334");
                if (xvar != null)
                {
                    Dak50334 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Dak50334.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Ferdinand51078");
                if (xvar != null)
                {
                    Ferdinand51078 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Ferdinand51078.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/GoKan50331");
                if (xvar != null)
                {
                    GoKan50331 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + GoKan50331.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Korda50332");
                if (xvar != null)
                {
                    Korda50332 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Korda50332.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Lon50333");
                if (xvar != null)
                {
                    Lon50333 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Lon50333.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Yorik50336");
                if (xvar != null)
                {
                    Yorik50336 = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDiagnostic("Rarekiller Load: " + xvar.Name + "=" + Yorik50336.ToString());
                }


            }
            catch (Exception e)
            {
                Logging.WriteDiagnostic(Colors.Red, e.Message);
            }
        }

        public void DeactivateMoPRare(WoWUnit o)
        {
            if (o.Entry == 50828)
                Bonobos50828 = false;
            if (o.Entry == 50836)
                IkIk50836 = false;
            if (o.Entry == 50840)
                Nanners50840 = false;
            if (o.Entry == 50823)
                Ferocious50823 = false;
            if (o.Entry == 50831)
                Scritch50831 = false;
            if (o.Entry == 50830)
                Spriggin50830 = false;
            if (o.Entry == 50832)
                Yowler50832 = false;
            if (o.Entry == 50750)
                Aethis50750 = false;
            if (o.Entry == 50768)
                Cournith50768 = false;
            if (o.Entry == 50772)
                Eshelon50772 = false;
            if (o.Entry == 50766)
                Selena50766 = false;
            if (o.Entry == 50769)
                Zai50769 = false;
            if (o.Entry == 50780)
                Sahn50780 = false;
            if (o.Entry == 50776)
                Nalash50776 = false;
            if (o.Entry == 50739)
                Garlok50739 = false;
            if (o.Entry == 50749)
                Kaltik50749 = false;
            if (o.Entry == 50734)
                Lithik50734 = false;
            if (o.Entry == 50364)
                Nallak50364 = false;
            if (o.Entry == 50363)
                Kraxik50363 = false;
            if (o.Entry == 50733)
                Skithik50733 = false;
            if (o.Entry == 50388)
                Torik50388 = false;
            if (o.Entry == 50341)
                Borginn50341 = false;
            if (o.Entry == 50349)
                Kang50349 = false;
            if (o.Entry == 50340)
                Gaarn50340 = false;
            if (o.Entry == 50347)
                Karr50347 = false;
            if (o.Entry == 50338)
                Kornas50338 = false;
            if (o.Entry == 50344)
                Norlaxx50344 = false;
            if (o.Entry == 50339)
                Sulikshor50339 = false;
            if (o.Entry == 50354)
                Havak50354 = false;
            if (o.Entry == 50351)
                JonnDar50351 = false;
            if (o.Entry == 50355)
                Kahtir50355 = false;
            if (o.Entry == 50356)
                Krol50356 = false;
            if (o.Entry == 50350)
                Morgrinn50350 = false;
            if (o.Entry == 50352)
                Qunas50352 = false;
            if (o.Entry == 50359)
                Urgolax50359 = false;
            if (o.Entry == 50821)
                AiLi50821 = false;
            if (o.Entry == 50817)
                Ahone50817 = false;
            if (o.Entry == 50822)
                AiRan50822 = false;
            if (o.Entry == 50816)
                Ruun50816 = false;
            if (o.Entry == 50811)
                Nasra50811 = false;
            if (o.Entry == 50808)
                Urobi50808 = false;
            if (o.Entry == 50820)
                Yul50820 = false;
            if (o.Entry == 50787)
                Arness50787 = false;
            if (o.Entry == 50806)
                Moldo50806 = false;
            if (o.Entry == 50789)
                Nessos50789 = false;
            if (o.Entry == 50805)
                Omnis50805 = false;
            if (o.Entry == 50783)
                Salyin50783 = false;
            if (o.Entry == 50782)
                Sarnak50782 = false;
            if (o.Entry == 50791)
                Siltriss50791 = false;
            if (o.Entry == 51059)
                Blackhoof51059 = false;
            if (o.Entry == 50334)
                Dak50334 = false;
            if (o.Entry == 51078)
                Ferdinand51078 = false;
            if (o.Entry == 50331)
                GoKan50331 = false;
            if (o.Entry == 50332)
                Korda50332 = false;
            if (o.Entry == 50333)
                Lon50333 = false;
            if (o.Entry == 50336)
                Yorik50336 = false;
        }

    }
}
