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
using System.Windows.Forms;
using System.Media;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;

namespace katzerle
{
    public partial class RarekillerUI : Form
    {
        public RarekillerUI()
        {
            InitializeComponent();

            // Addons
            CBMOP.Checked = Rarekiller.Settings.MOP;
            CBCata.Checked = Rarekiller.Settings.CATA;
            CBWotlk.Checked = Rarekiller.Settings.WOTLK;
            CBBC.Checked = Rarekiller.Settings.BC;
            CBLowRAR.Checked = Rarekiller.Settings.LowRAR;
            CBTLPD.Checked = Rarekiller.Settings.TLPD;
			CBPoseidus.Checked = Rarekiller.Settings.Poseidus;
            CBRaptorNest.Checked = Rarekiller.Settings.RaptorNest;
            CBObjects.Checked = Rarekiller.Settings.ObjectsCollector;
            CBCamel.Checked = Rarekiller.Settings.Camel;
            // Hunt by ID
            CBHuntByID.Checked = Rarekiller.Settings.HUNTbyID;
            TBHuntByID.Text = Rarekiller.Settings.MobID;
            CBBlacklistCheck.Checked = Rarekiller.Settings.BlacklistCheck;
            TBBlacklistTime.Text = Rarekiller.Settings.BlacklistTime;
            //Security
			CBAlert.Checked = Rarekiller.Settings.Alert;
            CBWisper.Checked = Rarekiller.Settings.Wisper;
            CBBNWisper.Checked = Rarekiller.Settings.BNWisper;
            CBGuild.Checked = Rarekiller.Settings.Guild;
            CBKeyer.Checked = Rarekiller.Settings.Keyer;
			TBSoundfileWisper.Text = Rarekiller.Settings.SoundfileWisper;
            TBSoundfileGuild.Text = Rarekiller.Settings.SoundfileGuild;
            TBSoundfileFoundRare.Text = Rarekiller.Settings.SoundfileFoundRare;
            //Tamer
            CBTameDefault.Checked = Rarekiller.Settings.TameDefault;
            CBTameByID.Checked = Rarekiller.Settings.TameByID;
            CBNotKillTameable.Checked = Rarekiller.Settings.NotKillTameable;
            TBTameID.Text = Rarekiller.Settings.TameMobID;
            CBTameDefault.Enabled = Rarekiller.Settings.Hunteractivated;
            CBTameByID.Enabled = Rarekiller.Settings.Hunteractivated;
            TBTameID.Enabled = Rarekiller.Settings.Hunteractivated;
            // Slowfall
            CBUseSlowfall.Checked = Rarekiller.Settings.UseSlowfall;
            RBCloak.Checked = Rarekiller.Settings.Cloak;
            CBItem.Checked = Rarekiller.Settings.Item;
            TBSlowfallItem.Text = Rarekiller.Settings.SlowfallItem;
            CBSpell.Checked = Rarekiller.Settings.Spell;
            TBSlowfallSpell.Text = Rarekiller.Settings.SlowfallSpell;
            TBFalltimer.Text = Rarekiller.Settings.Falltimer;
            // Pullspell
            CBPull.Checked = Rarekiller.Settings.DefaultPull;
            CBPull2.Checked = !Rarekiller.Settings.DefaultPull;
            TBPull.Text = Rarekiller.Settings.Pull;
			TBRange.Text = Rarekiller.Settings.Range;
            CBVyragosa.Checked = Rarekiller.Settings.Vyragosa;
            CBBlazewing.Checked = Rarekiller.Settings.Blazewing;
            //Developer Box
            CBTestRaptorNest.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestCamel.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestRaptorNest.Checked = Rarekiller.Settings.TestRaptorNest;
            CBTestCamel.Checked = Rarekiller.Settings.TestFigurineInteract;
        }
		
        private void RBItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBItem.Checked)
            {
                TBSlowfallItem.Text = "";
                TBSlowfallItem.Enabled = false;
            }
            else
            {
                TBSlowfallItem.Text = Rarekiller.Settings.SlowfallItem;
                TBSlowfallItem.Enabled = true;
            }

        }
        private void CBSpell_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBSpell.Checked)
            {
                TBSlowfallSpell.Text = "";
                TBSlowfallSpell.Enabled = false;
            }
            else
            {
                TBSlowfallSpell.Text = Rarekiller.Settings.SlowfallSpell;
                TBSlowfallSpell.Enabled = true;
            }
        }

        private void CBHuntByID_CheckedChanged(object sender, EventArgs e)
        {
            if (CBHuntByID.Checked)
            {
                TBHuntByID.Enabled = true;
                TBHuntByID.Text = Rarekiller.Settings.MobID;
            }
            else
            {
                TBHuntByID.Enabled = false;
                TBHuntByID.Text = "";
            }
        }

        private void CBTameID_CheckedChanged(object sender, EventArgs e)
        {
            if (CBTameByID.Checked)
            {
                TBTameID.Enabled = true;
                TBTameID.Text = Rarekiller.Settings.TameMobID;
            }
            else
            {
                TBTameID.Enabled = false;
                TBTameID.Text = "";
            }

        }

        private void BAlertTest_Click(object sender, EventArgs e)
        {
            new SoundPlayer(Rarekiller.Soundfile).Play();
        }

        private void CBUseSlowfall_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBUseSlowfall.Checked)
            {
                CBSpell.Checked = false;
                CBItem.Checked = false;
				RBCloak.Checked = false;
                CBItem.Enabled = false;
                CBSpell.Enabled = false;
                RBCloak.Enabled = false; 
                TBSlowfallSpell.Text = "";
                TBSlowfallSpell.Enabled = false;
                TBSlowfallItem.Text = "";
                TBSlowfallItem.Enabled = false;
            }
            else
            {
				RBCloak.Checked = Rarekiller.Settings.Cloak;
				CBItem.Checked = Rarekiller.Settings.Item;
				TBSlowfallItem.Text = Rarekiller.Settings.SlowfallItem;
				CBSpell.Checked = Rarekiller.Settings.Spell;
				TBSlowfallSpell.Text = Rarekiller.Settings.SlowfallSpell;
                CBItem.Enabled = true;
                CBSpell.Enabled = true;
                RBCloak.Enabled = true; 
                TBSlowfallSpell.Enabled = true;
                TBSlowfallItem.Enabled = true;
            }
        }

        private void BSave_Click(object sender, EventArgs e)
        {
//----------------- Save Configfile and set Settings ---------------- 
            XmlDocument xml;
            XmlElement root;
            XmlElement element;
            XmlText text;
            XmlComment xmlComment;

// ---------- Check Plausibility of the Config ----------------------------

//Pull Spell
            if (!CBPull.Checked && !(SpellManager.HasSpell(TBPull.Text)))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller Part Spells: Don't have your configured Pull Spell - setting to Default");
                CBPull.Checked = true;
                CBPull2.Checked = false;
                TBPull.Text = "";
            }
            if (!CBPull.Checked && (TBPull.Text == ""))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: You must insert Pullspell");
                CBPull.Checked = true;
                CBPull2.Checked = false;
            }

// Spellrange Test Customized Pull Spell
            if (!CBPull.Checked && (Convert.ToInt64(TBRange.Text) > Convert.ToInt64(Rarekiller.Spells.RangeCheck(TBPull.Text))))
            {
                TBRange.Text = Rarekiller.Spells.RangeCheck(TBPull.Text);
                Logging.WriteDiagnostic(Colors.MediumPurple, "Set Range to {0} because of Low-Ranged Customized Spell", Rarekiller.Spells.RangeCheck(TBPull.Text));
            }

// Spellrange Test Default Pull Spell
            if (!CBPull.Checked && (Convert.ToInt64(TBRange.Text) > Convert.ToInt64(Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell))))
            {
                TBRange.Text = Rarekiller.Spells.RangeCheck(TBPull.Text);
                Logging.WriteDiagnostic(Colors.MediumPurple, "Set Range to {0} because of Low-Ranged Default Spell", Rarekiller.Spells.RangeCheck(TBPull.Text));
            }

            if ((CBMOP.Checked || CBWotlk.Checked || CBBC.Checked || CBCata.Checked || CBLowRAR.Checked || CBHuntByID.Checked)
                && CBPull.Checked && !(SpellManager.HasSpell(Rarekiller.Spells.FastPullspell)))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller Part Spells: Don't have your Pull Spell - please config one");
                CBWotlk.Checked = false;
                CBBC.Checked = false;
                CBCata.Checked = false;
                CBMOP.Checked = false;
                CBHuntByID.Checked = false;
                CBLowRAR.Checked = false;
            }
            if (CBTLPD.Checked && CBPull.Checked
                && !(SpellManager.HasSpell(Rarekiller.Spells.FastPullspell)))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller Part Spells: Don't have a valid Pull Spell for TLPD - please check your Config");
                CBTLPD.Checked = false;
            }

//Hunt and Tame by ID
            if (CBHuntByID.Checked && (TBHuntByID.Text == ""))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: You must insert the ID of the Mob you want to hunt");
                CBHuntByID.Checked = false;
            }
            if (CBTameByID.Checked && (TBTameID.Text == ""))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: You must insert the ID of the Mob you want to tame");
                CBTameByID.Checked = false;
            }


            //Slowfall
            if (CBItem.Checked && (TBSlowfallItem.Text == ""))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: You must insert a Slowfall Item");
                CBItem.Checked = false;
            }
            if (CBSpell.Checked && (TBSlowfallSpell.Text == ""))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: You must insert a Slowfall Spell");
                CBSpell.Checked = false;
            }
            if (CBUseSlowfall.Checked && CBSpell.Checked && !(SpellManager.HasSpell(TBSlowfallSpell.Text)))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: Don't have your Slowfall Spell - please check your Config");
                CBSpell.Checked = false;
                TBSlowfallSpell.Text = "";
            }
            if (CBTLPD.Checked && !CBUseSlowfall.Checked)
                Logging.Write(Colors.MediumPurple, "Rarekiller Warning: You will probably die, hunting the TLPD without Slowfall.");
            if (CBWotlk.Checked && CBVyragosa.Checked && !CBUseSlowfall.Checked)
                Logging.Write(Colors.MediumPurple, "Rarekiller Warning: You will probably die, hunting Vyragosa without Slowfall. Please Check - don't kill Vyragosa");
            if (TBFalltimer.Text == "")
                TBFalltimer.Text = "10";
//Diverses
         if (CBBlacklistCheck.Checked && (TBBlacklistTime.Text == ""))
            {
                TBBlacklistTime.Text = "180";
                Logging.Write(Colors.MediumPurple, "Rarekiller: Set Blacklist Time to Default");
            }


// Variablen nach Settings übernehmen
            // Addons
            Rarekiller.Settings.MOP = CBMOP.Checked;
            Rarekiller.Settings.CATA = CBCata.Checked;
            Rarekiller.Settings.WOTLK = CBWotlk.Checked;
            Rarekiller.Settings.BC = CBBC.Checked;
            Rarekiller.Settings.LowRAR = CBLowRAR.Checked;
            Rarekiller.Settings.TLPD = CBTLPD.Checked;
			Rarekiller.Settings.Poseidus = CBPoseidus.Checked;
            Rarekiller.Settings.RaptorNest = CBRaptorNest.Checked;
            Rarekiller.Settings.ObjectsCollector = CBObjects.Checked;
            // Hunt by ID
            Rarekiller.Settings.HUNTbyID = CBHuntByID.Checked;
            Rarekiller.Settings.MobID = TBHuntByID.Text;
            Rarekiller.Settings.BlacklistCheck = CBBlacklistCheck.Checked;
            Rarekiller.Settings.BlacklistTime = TBBlacklistTime.Text;
            //Tamer
            Rarekiller.Settings.TameDefault = CBTameDefault.Checked;
            Rarekiller.Settings.TameByID = CBTameByID.Checked;
            Rarekiller.Settings.NotKillTameable = CBNotKillTameable.Checked;
            Rarekiller.Settings.TameMobID = TBTameID.Text;
            // Slowfall
            Rarekiller.Settings.UseSlowfall = CBUseSlowfall.Checked;
            Rarekiller.Settings.Cloak = RBCloak.Checked;
            Rarekiller.Settings.Item = CBItem.Checked;
            Rarekiller.Settings.SlowfallItem = TBSlowfallItem.Text;
            Rarekiller.Settings.Spell = CBSpell.Checked;
            Rarekiller.Settings.SlowfallSpell = TBSlowfallSpell.Text;
            Rarekiller.Settings.Falltimer = TBFalltimer.Text;
            // Pullspell
            Rarekiller.Settings.DefaultPull = CBPull.Checked;
            Rarekiller.Settings.Pull = TBPull.Text;
            Rarekiller.Settings.Range = TBRange.Text;
            Rarekiller.Settings.Vyragosa = CBVyragosa.Checked;
            Rarekiller.Settings.Blazewing = CBBlazewing.Checked;
            Rarekiller.Settings.Camel = CBCamel.Checked;
            //Alert etc
            Rarekiller.Settings.Alert = CBAlert.Checked;
            Rarekiller.Settings.Wisper = CBWisper.Checked;
            Rarekiller.Settings.BNWisper = CBBNWisper.Checked;
            Rarekiller.Settings.Guild = CBGuild.Checked;
            Rarekiller.Settings.Keyer = CBKeyer.Checked;
            Rarekiller.Settings.SoundfileWisper = TBSoundfileWisper.Text;
            Rarekiller.Settings.SoundfileGuild = TBSoundfileGuild.Text;
            Rarekiller.Settings.SoundfileFoundRare = TBSoundfileFoundRare.Text;
            //Developer Box
            Rarekiller.Settings.TestRaptorNest = CBTestRaptorNest.Checked; 
            Rarekiller.Settings.TestFigurineInteract = CBTestCamel.Checked;

            // Rarekiller
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: MOP = {0}", CBMOP.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: CATA = {0}", CBCata.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: WOTLK = {0}", CBWotlk.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: BC = {0}", CBBC.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: LowRare = {0}", CBLowRAR.Checked.ToString());
			// Mount Rares
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: TLPD = {0}", CBTLPD.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Poseidus = {0}", CBPoseidus.Checked.ToString());
			// Collector
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Camel = {0}", CBCamel.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: RaptorNest = {0}", CBRaptorNest.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: ObjectsCollect = {0}", CBObjects.Checked.ToString());
			// Problem Mobs
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Vyragosa = {0}", CBVyragosa.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Blazewing = {0}", CBBlazewing.Checked.ToString());
            // Hunt by ID
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: HuntByID = {0}", CBHuntByID.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: MobID = {0}", TBHuntByID.Text.ToString());
            // Tamer
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: TameDefault = {0}", CBTameDefault.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: TameByID = {0}", CBTameByID.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: NotKillTameable = {0}", CBNotKillTameable.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: TameID = {0}", TBTameID.Text.ToString());
            // Pullspell
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: DefaultPull = {0}", CBPull.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Spell = {0}", TBPull.Text.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Range = {0}", TBRange.Text.ToString());
            // Slowfall
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: UseSlowfall = {0}", CBUseSlowfall.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Cloak = {0}", RBCloak.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: UseSlowfallItem = {0}", CBItem.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: SlowfallItem = {0}", TBSlowfallItem.Text.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: UseSlowfallSpell = {0}", CBSpell.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: SlowfallSpell = {0}", TBSlowfallSpell.Text.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Falltimer = {0}", TBFalltimer.Text.ToString());
			// Miscs
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: BlacklistCheck = {0}", CBBlacklistCheck.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: BlacklistTime = {0}", TBBlacklistTime.Text.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: MoveAround = {0}", CBKeyer.Checked.ToString());
			// Alerts
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Alert = {0}", CBAlert.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Wisper = {0}", CBWisper.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: BNWisper = {0}", CBBNWisper.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Guild = {0}", CBGuild.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: FileWisper = {0}", TBSoundfileWisper.Text.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: FileGuild = {0}", TBSoundfileGuild.Text.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: FileFound = {0}", TBSoundfileFoundRare.Text.ToString());

// ---------- Save ----------------------------			

            string sPath = Rarekiller.SettingsPath;

            if (!Directory.Exists(sPath))
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Creating config directory");
                Directory.CreateDirectory(sPath);
            }

            sPath = Path.Combine(sPath, StyxWoW.Me.RealmName, StyxWoW.Me.Name, "Rarekiller.config");

            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Saving config file");
            Logging.Write(Colors.MediumPurple, "Rarekiller: Settings Saved");
            xml = new XmlDocument();
            XmlDeclaration dc = xml.CreateXmlDeclaration("1.0", "utf-8", null);
            xml.AppendChild(dc);

            xmlComment = xml.CreateComment(
                "=======================================================================\n" +
                ".CONFIG  -  This is the Config File For Rarekiller\n\n" +
                "XML file containing settings to customize in the Rarekiller Plugin\n" +
                "It is STRONGLY recommended you use the Configuration UI to change this\n" +
                "file instead of direct changein it here.\n" +
                "========================================================================");

            //let's add the root element
            root = xml.CreateElement("Rarekiller");
            root.AppendChild(xmlComment);

			//Rarekiller
            //let's add another element (child of the root)
            element = xml.CreateElement("MOP");
            text = xml.CreateTextNode(CBMOP.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("CATA");
            text = xml.CreateTextNode(CBCata.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("WOTLK");
            text = xml.CreateTextNode(CBWotlk.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("BC");
            text = xml.CreateTextNode(CBBC.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("LowRAR");
            text = xml.CreateTextNode(CBLowRAR.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
			//Rare Mounts
            //let's add another element (child of the root)
            element = xml.CreateElement("TLPD");
            text = xml.CreateTextNode(CBTLPD.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("Poseidus");
            text = xml.CreateTextNode(CBPoseidus.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

			//Collector
            //let's add another element (child of the root)
            element = xml.CreateElement("Camel");
            text = xml.CreateTextNode(CBCamel.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("RaptorNest");
            text = xml.CreateTextNode(CBRaptorNest.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("ObjectsCollector");
            text = xml.CreateTextNode(CBObjects.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
			//Problem Mobs
            //let's add another element (child of the root)
            element = xml.CreateElement("Blazewing");
            text = xml.CreateTextNode(CBBlazewing.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);			
			//let's add another element (child of the root)
            element = xml.CreateElement("Vyragosa");
            text = xml.CreateTextNode(CBVyragosa.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

			//Hunt by ID
            //let's add another element (child of the root)
            element = xml.CreateElement("HuntByID");
            text = xml.CreateTextNode(CBHuntByID.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("MobID");
            text = xml.CreateTextNode(TBHuntByID.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
            //Tamer
            //let's add another element (child of the root)
            element = xml.CreateElement("NotKillTameable");
            text = xml.CreateTextNode(CBNotKillTameable.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("TameDefault");
            text = xml.CreateTextNode(CBTameDefault.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("TameByID");
            text = xml.CreateTextNode(CBTameByID.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("TameMobID");
            text = xml.CreateTextNode(TBTameID.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
			//Pullspell
            //let's add another element (child of the root)
            element = xml.CreateElement("DefaultPull");
            text = xml.CreateTextNode(CBPull.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Pull");
            text = xml.CreateTextNode(TBPull.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Range");
            text = xml.CreateTextNode(TBRange.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

			//Slowfall
            //let's add another element (child of the root)
            element = xml.CreateElement("UseSlowfall");
            text = xml.CreateTextNode(CBUseSlowfall.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Spell");
            text = xml.CreateTextNode(CBSpell.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("SlowfallSpell");
            text = xml.CreateTextNode(TBSlowfallSpell.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Cloak");
            text = xml.CreateTextNode(RBCloak.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Item");
            text = xml.CreateTextNode(CBItem.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("SlowfallItem");
            text = xml.CreateTextNode(TBSlowfallItem.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Falltimer");
            text = xml.CreateTextNode(TBFalltimer.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

			//Miscs
			//let's add another element (child of the root)
            element = xml.CreateElement("Keyer");
            text = xml.CreateTextNode(CBKeyer.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

			//Blacklist Mobs
			//let's add another element (child of the root)
            element = xml.CreateElement("BlacklistCheck");
            text = xml.CreateTextNode(CBBlacklistCheck.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("BlacklistTime");
            text = xml.CreateTextNode(TBBlacklistTime.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //Alerts
			//let's add another element (child of the root)
            element = xml.CreateElement("Alert");
            text = xml.CreateTextNode(CBAlert.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Wisper");
            text = xml.CreateTextNode(CBWisper.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("BNWisper");
            text = xml.CreateTextNode(CBBNWisper.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("GuildChat");
            text = xml.CreateTextNode(CBGuild.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("SoundfileFoundRare");
            text = xml.CreateTextNode(TBSoundfileFoundRare.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("SoundfileWisper");
            text = xml.CreateTextNode(TBSoundfileWisper.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("SoundfileGuild");
            text = xml.CreateTextNode(TBSoundfileGuild.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
					
            xml.AppendChild(root);

            System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.Create,
                                                               System.IO.FileAccess.Write);
            try
            {
                xml.Save(fs);
                fs.Close();
            }
            catch (Exception np)
            {
                Logging.WriteDiagnostic(Colors.Red, np.Message);
            }
        }

        private void BSoundfileFoundRare_Click(object sender, EventArgs e)
        {
            string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\");
            
            var loadSoundfileFoundRare = new OpenFileDialog
            {
                Filter = "WAVE File (*.wav)|*.wav|WAVE File (*.wave)|*.wave",
                Title = "Select wav file to load",
                InitialDirectory = sPath
             };

            if (loadSoundfileFoundRare.ShowDialog() == DialogResult.OK)
            {
                string fileName1 = loadSoundfileFoundRare.FileName;
                if (!string.IsNullOrEmpty(fileName1))
                {
                    TBSoundfileFoundRare.Text = fileName1;
                }
            }
        }

        private void BSoundfileWisper_Click(object sender, EventArgs e)
        {
            string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\");

            var loadSoundfileWisper = new OpenFileDialog
            {
                Filter = "WAVE File (*.wav)|*.wav|WAVE File (*.wave)|*.wave",
                Title = "Select wav file to load",
                InitialDirectory = sPath
            };

            if (loadSoundfileWisper.ShowDialog() == DialogResult.OK)
            {
                string fileName2 = loadSoundfileWisper.FileName;
                if (!string.IsNullOrEmpty(fileName2))
                {
                    TBSoundfileWisper.Text = fileName2;
                }
            }
        }

        private void BSoundfileGuild_Click(object sender, EventArgs e)
        {
            string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\");

            var loadSoundfileGuild = new OpenFileDialog
            {
                Filter = "WAVE File (*.wav)|*.wav|WAVE File (*.wave)|*.wave",
                Title = "Select wav file to load",
                InitialDirectory = sPath
            };

            if (loadSoundfileGuild.ShowDialog() == DialogResult.OK)
            {
                string fileName3 = loadSoundfileGuild.FileName;
                if (!string.IsNullOrEmpty(fileName3))
                {
                    TBSoundfileGuild.Text = fileName3;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Addons
            CBMOP.Checked = false;
            CBCata.Checked = true;
            CBWotlk.Checked = true;
            CBBC.Checked = true;
            CBLowRAR.Checked = false;
            CBTLPD.Checked = true;
            CBPoseidus.Checked = true;
            CBRaptorNest.Checked = true;
            CBObjects.Checked = true;
            // Hunt by ID
            CBHuntByID.Checked = false;
            TBHuntByID.Text = "";
            CBBlacklistCheck.Checked = true;
            TBBlacklistTime.Text = "180";
            //Misc
            CBAlert.Checked = true;
            //Security
            CBWisper.Checked = true;
            CBBNWisper.Checked = true;
            CBGuild.Checked = false;
            CBKeyer.Checked = true;
            TBSoundfileWisper.Text = Rarekiller.Soundfile;
            TBSoundfileGuild.Text = Rarekiller.Soundfile;
            TBSoundfileFoundRare.Text = Rarekiller.Soundfile;
            //Tamer
            CBTameDefault.Checked = false;
            CBTameByID.Checked = false;
            CBNotKillTameable.Checked = false;
            TBTameID.Text = "";
            CBTameDefault.Enabled = Rarekiller.Settings.Hunteractivated;
            CBTameByID.Enabled = Rarekiller.Settings.Hunteractivated;
            TBTameID.Enabled = Rarekiller.Settings.Hunteractivated;
            // Slowfall
            CBUseSlowfall.Checked = true;
            RBCloak.Checked = false;
            CBItem.Checked = true;
            TBSlowfallItem.Text = "Snowfall Lager";
            CBSpell.Checked = false;
            TBSlowfallSpell.Text = "";
            TBFalltimer.Text = "900";
            // Pullspell
            CBPull.Checked = true;
            CBPull2.Checked = false;
            TBPull.Text = "";
            TBRange.Text = "10";
            CBVyragosa.Checked = true;
            CBBlazewing.Checked = false;
            CBCamel.Checked = true;
            //Developer Box
            CBTestRaptorNest.Enabled = false;
            CBTestCamel.Enabled = false;
            CBTestRaptorNest.Checked = false;
            CBTestCamel.Checked = false;
			
			// Variablen nach Settings übernehmen
            // Addons
            Rarekiller.Settings.MOP = CBMOP.Checked;
            Rarekiller.Settings.CATA = CBCata.Checked;
            Rarekiller.Settings.WOTLK = CBWotlk.Checked;
            Rarekiller.Settings.BC = CBBC.Checked;
            Rarekiller.Settings.LowRAR = CBLowRAR.Checked;
            Rarekiller.Settings.TLPD = CBTLPD.Checked;
			Rarekiller.Settings.Poseidus = CBPoseidus.Checked;
            Rarekiller.Settings.RaptorNest = CBRaptorNest.Checked;
            Rarekiller.Settings.ObjectsCollector = CBObjects.Checked;
            // Hunt by ID
            Rarekiller.Settings.HUNTbyID = CBHuntByID.Checked;
            Rarekiller.Settings.MobID = TBHuntByID.Text;
            Rarekiller.Settings.BlacklistCheck = CBBlacklistCheck.Checked;
            Rarekiller.Settings.BlacklistTime = TBBlacklistTime.Text;
            //Tamer
            Rarekiller.Settings.TameDefault = CBTameDefault.Checked;
            Rarekiller.Settings.TameByID = CBTameByID.Checked;
            Rarekiller.Settings.NotKillTameable = CBNotKillTameable.Checked;
            Rarekiller.Settings.TameMobID = TBTameID.Text;
            // Slowfall
            Rarekiller.Settings.UseSlowfall = CBUseSlowfall.Checked;
            Rarekiller.Settings.Cloak = RBCloak.Checked;
            Rarekiller.Settings.Item = CBItem.Checked;
            Rarekiller.Settings.SlowfallItem = TBSlowfallItem.Text;
            Rarekiller.Settings.Spell = CBSpell.Checked;
            Rarekiller.Settings.SlowfallSpell = TBSlowfallSpell.Text;
            Rarekiller.Settings.Falltimer = TBFalltimer.Text;
            // Pullspell
            Rarekiller.Settings.DefaultPull = CBPull.Checked;
            Rarekiller.Settings.Pull = TBPull.Text;
            Rarekiller.Settings.Range = TBRange.Text;
            Rarekiller.Settings.Vyragosa = CBVyragosa.Checked;
            Rarekiller.Settings.Blazewing = CBBlazewing.Checked;
            Rarekiller.Settings.Camel = CBCamel.Checked;
            //Alert etc
            Rarekiller.Settings.Alert = CBAlert.Checked;
            Rarekiller.Settings.Wisper = CBWisper.Checked;
            Rarekiller.Settings.BNWisper = CBBNWisper.Checked;
            Rarekiller.Settings.Guild = CBGuild.Checked;
            Rarekiller.Settings.Keyer = CBKeyer.Checked;
            Rarekiller.Settings.SoundfileWisper = TBSoundfileWisper.Text;
            Rarekiller.Settings.SoundfileGuild = TBSoundfileGuild.Text;
            Rarekiller.Settings.SoundfileFoundRare = TBSoundfileFoundRare.Text;
           //Developer Box
            Rarekiller.Settings.TestRaptorNest = CBTestRaptorNest.Checked; 
            Rarekiller.Settings.TestFigurineInteract = CBTestCamel.Checked;
        }

        private void CBPull_CheckedChanged_1(object sender, EventArgs e)
        {
            if (!CBPull.Checked)
            {
                CBPull2.Checked = true;
                TBPull.Enabled = true;
                TBPull.Text = Rarekiller.Settings.Pull;
            }
            else
            {
                CBPull2.Checked = false;
                TBPull.Text = "";
                TBPull.Enabled = false;
            }
        }

        private void CBPull2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (!CBPull2.Checked)
            {
                CBPull.Checked = true;
                TBPull.Enabled = false;
                TBPull.Text = "";
            }
            else
            {
                CBPull.Checked = false;
                TBPull.Text = Rarekiller.Settings.Pull;
                TBPull.Enabled = true;
            }
        }

    }
}
