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
using System.Windows.Forms;
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
    public partial class RarekillerUI : Form
    {
        public static LocalPlayer Me = StyxWoW.Me;
        
        public RarekillerUI()
        {
            InitializeComponent();

            // Addons
            CBMOP.Checked = Rarekiller.Settings.MOP;
            CBKillList.Checked = Rarekiller.Settings.KillList;
            CBCata.Checked = Rarekiller.Settings.CATA;
            CBWotlk.Checked = Rarekiller.Settings.WOTLK;
            CBBC.Checked = Rarekiller.Settings.BC;
            CBLowRAR.Checked = Rarekiller.Settings.LowRAR;
            CBTLPD.Checked = Rarekiller.Settings.TLPD;
            CBAnotherMansTreasure.Checked = Rarekiller.Settings.AnotherMansTreasure;
            CBInteractNPC.Checked = Rarekiller.Settings.InteractNPC;
			CBPoseidus.Checked = Rarekiller.Settings.Poseidus;
            CBRaptorNest.Checked = Rarekiller.Settings.RaptorNest;
            CBDarkSoil.Checked = Rarekiller.Settings.DarkSoil;
            CBObjects.Checked = Rarekiller.Settings.ObjectsCollector;
            CBCamel.Checked = Rarekiller.Settings.Camel;
            CBAeonaxx.Checked = Rarekiller.Settings.Aeonaxx;
            // Hunt by ID
            CBHuntByID.Checked = Rarekiller.Settings.HUNTbyID;
            TBHuntByID.Text = Rarekiller.Settings.MobID;
            CBBlacklistCheck.Checked = Rarekiller.Settings.BlacklistCheck;
            TBBlacklistTime.Text = Rarekiller.Settings.BlacklistTime;
            //Security
            CBLUAoutput.Checked = Rarekiller.Settings.LUAoutput;
            CBPlayerScan.Checked = Rarekiller.Settings.PlayerScan;
			CBAlert.Checked = Rarekiller.Settings.Alert;
            CBWisper.Checked = Rarekiller.Settings.Wisper;
            CBBNWisper.Checked = Rarekiller.Settings.BNWisper;
            CBGuild.Checked = Rarekiller.Settings.Guild;
            CBKeyer.Checked = Rarekiller.Settings.Keyer;
            CBShadowmeld.Checked = Rarekiller.Settings.Shadowmeld;
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
            CBFootprints.Enabled = Rarekiller.Settings.Hunteractivated;
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
            CBTestcaseTamer.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestRaptorNest.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestCamel.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestRaptorNest.Checked = Rarekiller.Settings.TestRaptorNest;
            CBTestCamel.Checked = Rarekiller.Settings.TestFigurineInteract;
            CBTestcaseTamer.Checked = Rarekiller.Settings.TestcaseTamer;

            //Mists of Pandaria
            CBBonobos50828.Checked = Rarekiller.Settings.Bonobos50828;
            CBIkIk50836.Checked = Rarekiller.Settings.IkIk50836;
            CBNanners50840.Checked = Rarekiller.Settings.Nanners50840;
            CBFerocious50823.Checked = Rarekiller.Settings.Ferocious50823;
            CBScritch50831.Checked = Rarekiller.Settings.Scritch50831; 
            CBSpriggin50830.Checked = Rarekiller.Settings.Spriggin50830; 
            CBYowler50832.Checked = Rarekiller.Settings.Yowler50832; 
            CBAethis50750.Checked = Rarekiller.Settings.Aethis50750;          
            CBCournith50768.Checked = Rarekiller.Settings.Cournith50768; 
            CBEshelon50772.Checked = Rarekiller.Settings.Eshelon50772; 
            CBSelena50766.Checked = Rarekiller.Settings.Selena50766; 
            CBZai50769.Checked = Rarekiller.Settings.Zai50769;                    
            CBSahn50780.Checked = Rarekiller.Settings.Sahn50780; 
            CBNalash50776.Checked = Rarekiller.Settings.Nalash50776;
            CBGarlok50739.Checked = Rarekiller.Settings.Garlok50739;
            CBKaltik50749.Checked = Rarekiller.Settings.Kaltik50749;                     
            CBLithik50734.Checked = Rarekiller.Settings.Lithik50734; 
            CBNallak50364.Checked = Rarekiller.Settings.Nallak50364; 
            CBKraxik50363.Checked = Rarekiller.Settings.Kraxik50363; 
            CBSkithik50733.Checked = Rarekiller.Settings.Skithik50733;                   
            CBTorik50388.Checked = Rarekiller.Settings.Torik50388;
            CBBorginn50341.Checked = Rarekiller.Settings.Borginn50341; 
            CBKang50349.Checked = Rarekiller.Settings.Kang50349; 
            CBGaarn50340.Checked = Rarekiller.Settings.Gaarn50340;                     
            CBKarr50347.Checked = Rarekiller.Settings.Karr50347; 
            CBKornas50338.Checked = Rarekiller.Settings.Kornas50338; 
            CBNorlaxx50344.Checked = Rarekiller.Settings.Norlaxx50344; 
            CBSulikshor50339.Checked = Rarekiller.Settings.Sulikshor50339;                    
            CBHavak50354.Checked = Rarekiller.Settings.Havak50354; 
            CBJonnDar50351.Checked = Rarekiller.Settings.JonnDar50351; 
            CBKahtir50355.Checked = Rarekiller.Settings.Kahtir50355; 
            CBKrol50356.Checked = Rarekiller.Settings.Krol50356;                    
            CBMorgrinn50350.Checked = Rarekiller.Settings.Morgrinn50350; 
            CBQunas50352.Checked = Rarekiller.Settings.Qunas50352; 
            CBUrgolax50359.Checked = Rarekiller.Settings.Urgolax50359; 
            CBAiLi50821.Checked = Rarekiller.Settings.AiLi50821;                     
            CBAhone50817.Checked = Rarekiller.Settings.Ahone50817; 
            CBAiRan50822.Checked = Rarekiller.Settings.AiRan50822; 
            CBRuun50816.Checked = Rarekiller.Settings.Ruun50816;
            CBNasra50811.Checked = Rarekiller.Settings.Nasra50811;                    
            CBUrobi50808.Checked = Rarekiller.Settings.Urobi50808; 
            CBYul50820.Checked = Rarekiller.Settings.Yul50820; 
            CBArness50787.Checked = Rarekiller.Settings.Arness50787; 
            CBMoldo50806.Checked = Rarekiller.Settings.Moldo50806;                     
            CBNessos50789.Checked = Rarekiller.Settings.Nessos50789; 
            CBOmnis50805.Checked = Rarekiller.Settings.Omnis50805; 
            CBSalyin50783.Checked = Rarekiller.Settings.Salyin50783; 
            CBSarnak50782.Checked = Rarekiller.Settings.Sarnak50782;                    
            CBSiltriss50791.Checked = Rarekiller.Settings.Siltriss50791; 
            CBBlackhoof51059.Checked = Rarekiller.Settings.Blackhoof51059; 
            CBDak50334.Checked = Rarekiller.Settings.Dak50334; 
            CBFerdinand51078.Checked = Rarekiller.Settings.Ferdinand51078;                     
            CBGoKan50331.Checked = Rarekiller.Settings.GoKan50331; 
            CBKorda50332.Checked = Rarekiller.Settings.Korda50332; 
            CBLon50333.Checked = Rarekiller.Settings.Lon50333;
            CBYorik50336.Checked = Rarekiller.Settings.Yorik50336;

            CBBlingtron.Checked = Rarekiller.Settings.Blingtron;
            CBFootprints.Checked = Rarekiller.Settings.Footprints && Rarekiller.Settings.BETA;
            CBFootprints.Enabled = Rarekiller.Settings.BETA;
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
            new SoundPlayer(TBSoundfileFoundRare.Text).Play();
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

//Mists of Pandaria
            if (Me.Level < 90)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: Don't kill Pandaria Rares under Level 90");
                CBMOP.Checked = false;
            }


//Pull Spell
            if (!CBPull.Checked && !(SpellManager.HasSpell(TBPull.Text)))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: Don't have your configured Pull Spell - setting to Default");
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
                Logging.Write(Colors.MediumPurple, "Set Range to {0} because of Low-Ranged Customized Spell", Rarekiller.Spells.RangeCheck(TBPull.Text));
            }

// Spellrange Test Default Pull Spell
            if (!CBPull.Checked && (Convert.ToInt64(TBRange.Text) > Convert.ToInt64(Rarekiller.Spells.RangeCheck(Rarekiller.Spells.FastPullspell))))
            {
                TBRange.Text = Rarekiller.Spells.RangeCheck(TBPull.Text);
                Logging.Write(Colors.MediumPurple, "Set Range to {0} because of Low-Ranged Default Spell", Rarekiller.Spells.RangeCheck(TBPull.Text));
            }

            if ((CBMOP.Checked || CBWotlk.Checked || CBBC.Checked || CBCata.Checked || CBLowRAR.Checked || CBHuntByID.Checked)
                && CBPull.Checked && !(SpellManager.HasSpell(Rarekiller.Spells.FastPullspell)))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: Don't have your Pull Spell - please config one");
                CBKillList.Checked = false;
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
                Logging.Write(Colors.MediumPurple, "Rarekiller: Don't have a valid Pull Spell for TLPD - please check your Config");
                CBTLPD.Checked = false;
            }

//Hunt and Tame by ID
            if (CBHuntByID.Checked && (TBHuntByID.Text == ""))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: You must insert the ID of the Unit you want to hunt");
                CBHuntByID.Checked = false;
            }
            if (CBTameByID.Checked && (TBTameID.Text == ""))
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: You must insert the ID of the Unit you want to tame");
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
            if (CBNallak50364.Checked && !CBUseSlowfall.Checked)
                Logging.Write(Colors.MediumPurple, "Rarekiller Warning: You will probably die, hunting the Nal'lak without Slowfall.");
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
            Rarekiller.Settings.KillList = CBKillList.Checked;
            Rarekiller.Settings.CATA = CBCata.Checked;
            Rarekiller.Settings.WOTLK = CBWotlk.Checked;
            Rarekiller.Settings.BC = CBBC.Checked;
            Rarekiller.Settings.LowRAR = CBLowRAR.Checked;
            Rarekiller.Settings.TLPD = CBTLPD.Checked;
            Rarekiller.Settings.AnotherMansTreasure = CBAnotherMansTreasure.Checked;
            Rarekiller.Settings.InteractNPC = CBInteractNPC.Checked;
			Rarekiller.Settings.Poseidus = CBPoseidus.Checked;
            Rarekiller.Settings.RaptorNest = CBRaptorNest.Checked;
            Rarekiller.Settings.DarkSoil = CBDarkSoil.Checked;
            Rarekiller.Settings.ObjectsCollector = CBObjects.Checked;
            Rarekiller.Settings.Blingtron = CBBlingtron.Checked;
            Rarekiller.Settings.Aeonaxx = CBAeonaxx.Checked;

            // Hunt by ID
            Rarekiller.Settings.HUNTbyID = CBHuntByID.Checked;
            Rarekiller.Settings.MobID = TBHuntByID.Text;
            Rarekiller.Settings.BlacklistCheck = CBBlacklistCheck.Checked;
            Rarekiller.Settings.BlacklistTime = TBBlacklistTime.Text;
            Rarekiller.Settings.Footprints = CBFootprints.Checked;
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
            Rarekiller.Settings.PlayerScan = CBPlayerScan.Checked;
            Rarekiller.Settings.LUAoutput = CBLUAoutput.Checked;
            Rarekiller.Settings.Alert = CBAlert.Checked;
            Rarekiller.Settings.Wisper = CBWisper.Checked;
            Rarekiller.Settings.BNWisper = CBBNWisper.Checked;
            Rarekiller.Settings.Guild = CBGuild.Checked;
            Rarekiller.Settings.Keyer = CBKeyer.Checked;
            Rarekiller.Settings.Shadowmeld = CBShadowmeld.Checked;
            Rarekiller.Settings.SoundfileWisper = TBSoundfileWisper.Text;
            Rarekiller.Settings.SoundfileGuild = TBSoundfileGuild.Text;
            Rarekiller.Settings.SoundfileFoundRare = TBSoundfileFoundRare.Text;
            //Developer Box
            Rarekiller.Settings.TestRaptorNest = CBTestRaptorNest.Checked; 
            Rarekiller.Settings.TestFigurineInteract = CBTestCamel.Checked;
            Rarekiller.Settings.TestcaseTamer = CBTestcaseTamer.Checked;

            //Mists of Pandaria
            Rarekiller.Settings.Bonobos50828 = CBBonobos50828.Checked;
            Rarekiller.Settings.IkIk50836 = CBIkIk50836.Checked;
            Rarekiller.Settings.Nanners50840 = CBNanners50840.Checked;
            Rarekiller.Settings.Ferocious50823 = CBFerocious50823.Checked;
            Rarekiller.Settings.Scritch50831 = CBScritch50831.Checked;
            Rarekiller.Settings.Spriggin50830 = CBSpriggin50830.Checked;
            Rarekiller.Settings.Yowler50832 = CBYowler50832.Checked;
            Rarekiller.Settings.Aethis50750 = CBAethis50750.Checked;
            Rarekiller.Settings.Cournith50768 = CBCournith50768.Checked;
            Rarekiller.Settings.Eshelon50772 = CBEshelon50772.Checked;
            Rarekiller.Settings.Selena50766 = CBSelena50766.Checked;
            Rarekiller.Settings.Zai50769 = CBZai50769.Checked;
            Rarekiller.Settings.Sahn50780 = CBSahn50780.Checked;
            Rarekiller.Settings.Nalash50776 = CBNalash50776.Checked;
            Rarekiller.Settings.Garlok50739 = CBGarlok50739.Checked;
            Rarekiller.Settings.Kaltik50749 = CBKaltik50749.Checked;
            Rarekiller.Settings.Lithik50734 = CBLithik50734.Checked;
            Rarekiller.Settings.Nallak50364 = CBNallak50364.Checked;
            Rarekiller.Settings.Kraxik50363 = CBKraxik50363.Checked;
            Rarekiller.Settings.Skithik50733 = CBSkithik50733.Checked;
            Rarekiller.Settings.Torik50388 = CBTorik50388.Checked;
            Rarekiller.Settings.Borginn50341 = CBBorginn50341.Checked;
            Rarekiller.Settings.Kang50349 = CBKang50349.Checked;
            Rarekiller.Settings.Gaarn50340 = CBGaarn50340.Checked;
            Rarekiller.Settings.Karr50347 = CBKarr50347.Checked;
            Rarekiller.Settings.Kornas50338 = CBKornas50338.Checked;
            Rarekiller.Settings.Norlaxx50344 = CBNorlaxx50344.Checked;
            Rarekiller.Settings.Sulikshor50339 = CBSulikshor50339.Checked;
            Rarekiller.Settings.Havak50354 = CBHavak50354.Checked;
            Rarekiller.Settings.JonnDar50351 = CBJonnDar50351.Checked;
            Rarekiller.Settings.Kahtir50355 = CBKahtir50355.Checked;
            Rarekiller.Settings.Krol50356 = CBKrol50356.Checked;
            Rarekiller.Settings.Morgrinn50350 = CBMorgrinn50350.Checked;
            Rarekiller.Settings.Qunas50352 = CBQunas50352.Checked;
            Rarekiller.Settings.Urgolax50359 = CBUrgolax50359.Checked;
            Rarekiller.Settings.AiLi50821 = CBAiLi50821.Checked;
            Rarekiller.Settings.Ahone50817 = CBAhone50817.Checked;
            Rarekiller.Settings.AiRan50822 = CBAiRan50822.Checked;
            Rarekiller.Settings.Ruun50816 = CBRuun50816.Checked;
            Rarekiller.Settings.Nasra50811 = CBNasra50811.Checked;
            Rarekiller.Settings.Urobi50808 = CBUrobi50808.Checked;
            Rarekiller.Settings.Yul50820 = CBYul50820.Checked;
            Rarekiller.Settings.Arness50787 = CBArness50787.Checked;
            Rarekiller.Settings.Moldo50806 = CBMoldo50806.Checked;
            Rarekiller.Settings.Nessos50789 = CBNessos50789.Checked;
            Rarekiller.Settings.Omnis50805 = CBOmnis50805.Checked;
            Rarekiller.Settings.Salyin50783 = CBSalyin50783.Checked;
            Rarekiller.Settings.Sarnak50782 = CBSarnak50782.Checked;
            Rarekiller.Settings.Siltriss50791 = CBSiltriss50791.Checked;
            Rarekiller.Settings.Blackhoof51059 = CBBlackhoof51059.Checked;
            Rarekiller.Settings.Dak50334 = CBDak50334.Checked;
            Rarekiller.Settings.Ferdinand51078 = CBFerdinand51078.Checked;
            Rarekiller.Settings.GoKan50331 = CBGoKan50331.Checked;
            Rarekiller.Settings.Korda50332 = CBKorda50332.Checked;
            Rarekiller.Settings.Lon50333 = CBLon50333.Checked;
            Rarekiller.Settings.Yorik50336 = CBYorik50336.Checked;


            // Rarekiller
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: KillList = {0}", CBKillList.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: CATA = {0}", CBCata.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: WOTLK = {0}", CBWotlk.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: BC = {0}", CBBC.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: LowRare = {0}", CBLowRAR.Checked.ToString());
			// Mount Rares
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: TLPD = {0}", CBTLPD.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Poseidus = {0}", CBPoseidus.Checked.ToString());
            //Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Aeonaxx = {0}", CBAeonaxx.Checked.ToString());
            // Collector
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Camel = {0}", CBCamel.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: RaptorNest = {0}", CBRaptorNest.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: DarkSoil = {0}", CBDarkSoil.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: ObjectsCollect = {0}", CBObjects.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: AnotherMansTreasure = {0}", CBAnotherMansTreasure.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Blingtron = {0}", CBBlingtron.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: InteractNPC = {0}", CBInteractNPC.Checked.ToString());
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
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Footprints = {0}", CBFootprints.Checked.ToString());
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
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Shadowmeld = {0}", CBShadowmeld.Checked.ToString());
			// Alerts
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: PlayerScan = {0}", CBPlayerScan.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: LUAoutput = {0}", CBLUAoutput.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Alert = {0}", CBAlert.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Wisper = {0}", CBWisper.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: BNWisper = {0}", CBBNWisper.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Guild = {0}", CBGuild.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: FileWisper = {0}", TBSoundfileWisper.Text.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: FileGuild = {0}", TBSoundfileGuild.Text.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: FileFound = {0}", TBSoundfileFoundRare.Text.ToString());

            //Mists of Pandaria

            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Mists of Pandaria:");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: MOP = {0}", CBMOP.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Bonobos50828 = {0}", CBBonobos50828.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: IkIk50836 = {0}", CBIkIk50836.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Nanners50840 = {0}", CBNanners50840.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Ferocious50823 = {0}", CBFerocious50823.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Scritch50831 = {0}", CBScritch50831.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Spriggin50830 = {0}", CBSpriggin50830.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Yowler50832 = {0}", CBYowler50832.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Aethis50750 = {0}", CBAethis50750.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Cournith50768 = {0}", CBCournith50768.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Eshelon50772 = {0}", CBEshelon50772.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Selena50766 = {0}", CBSelena50766.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Zai50769 = {0}", CBZai50769.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Sahn50780 = {0}", CBSahn50780.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Nalash50776 = {0}", CBNalash50776.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Garlok50739 = {0}", CBGarlok50739.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Kaltik50749 = {0}", CBKaltik50749.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Lithik50734 = {0}", CBLithik50734.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Nallak50364 = {0}", CBNallak50364.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Kraxik50363 = {0}", CBKraxik50363.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Skithik50733 = {0}", CBSkithik50733.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Torik50388 = {0}", CBTorik50388.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Borginn50341 = {0}", CBBorginn50341.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Kang50349 = {0}", CBKang50349.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Gaarn50340 = {0}", CBGaarn50340.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Karr50347 = {0}", CBKarr50347.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Kornas50338 = {0}", CBKornas50338.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Norlaxx50344 = {0}", CBNorlaxx50344.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Sulikshor50339 = {0}", CBSulikshor50339.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Havak50354 = {0}", CBHavak50354.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: JonnDar50351 = {0}", CBJonnDar50351.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Kahtir50355 = {0}", CBKahtir50355.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Krol50356 = {0}", CBKrol50356.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Morgrinn50350 = {0}", CBMorgrinn50350.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Qunas50352 = {0}", CBQunas50352.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Urgolax50359 = {0}", CBUrgolax50359.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: AiLi50821 = {0}", CBAiLi50821.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Ahone50817 = {0}", CBAhone50817.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: AiRan50822 = {0}", CBAiRan50822.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Ruun50816 = {0}", CBRuun50816.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Nasra50811 = {0}", CBNasra50811.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Urobi50808 = {0}", CBUrobi50808.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Yul50820 = {0}", CBYul50820.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Arness50787 = {0}", CBArness50787.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Moldo50806 = {0}", CBMoldo50806.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Nessos50789 = {0}", CBNessos50789.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Omnis50805 = {0}", CBOmnis50805.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Salyin50783 = {0}", CBSalyin50783.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Sarnak50782 = {0}", CBSarnak50782.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Siltriss50791 = {0}", CBSiltriss50791.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Blackhoof51059 = {0}", CBBlackhoof51059.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Dak50334 = {0}", CBDak50334.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Ferdinand51078 = {0}", CBFerdinand51078.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: GoKan50331 = {0}", CBGoKan50331.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Korda50332 = {0}", CBKorda50332.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Lon50333 = {0}", CBLon50333.Checked.ToString());
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Save: Yorik50336 = {0}", CBYorik50336.Checked.ToString());

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
            element = xml.CreateElement("KillList");
            text = xml.CreateTextNode(CBKillList.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
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
            //let's add another element (child of the root)
            //element = xml.CreateElement("Aeonaxx");
            //text = xml.CreateTextNode(CBAeonaxx.Checked.ToString());
            //element.AppendChild(text);
            //root.AppendChild(element);

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
            element = xml.CreateElement("DarkSoil");
            text = xml.CreateTextNode(CBDarkSoil.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("ObjectsCollector");
            text = xml.CreateTextNode(CBObjects.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("AnotherMansTreasure");
            text = xml.CreateTextNode(CBAnotherMansTreasure.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Blingtron");
            text = xml.CreateTextNode(CBBlingtron.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("InteractNPC");
            text = xml.CreateTextNode(CBInteractNPC.Checked.ToString());
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
            //let's add another element (child of the root)
            element = xml.CreateElement("Footprints");
            text = xml.CreateTextNode(CBFootprints.Checked.ToString());
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
            //let's add another element (child of the root)
            element = xml.CreateElement("Shadowmeld");
            text = xml.CreateTextNode(CBShadowmeld.Checked.ToString());
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
            element = xml.CreateElement("PlayerScan");
            text = xml.CreateTextNode(CBPlayerScan.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("LUAoutput");
            text = xml.CreateTextNode(CBLUAoutput.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
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

            //Mists of Pandaria
            //let's add another element (child of the root)
            element = xml.CreateElement("Bonobos50828");
            text = xml.CreateTextNode(CBBonobos50828.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("IkIk50836");
            text = xml.CreateTextNode(CBIkIk50836.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Nanners50840");
            text = xml.CreateTextNode(CBNanners50840.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Ferocious50823");
            text = xml.CreateTextNode(CBFerocious50823.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Scritch50831");
            text = xml.CreateTextNode(CBScritch50831.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Spriggin50830");
            text = xml.CreateTextNode(CBSpriggin50830.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Yowler50832");
            text = xml.CreateTextNode(CBYowler50832.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Aethis50750");
            text = xml.CreateTextNode(CBAethis50750.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Cournith50768");
            text = xml.CreateTextNode(CBCournith50768.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Eshelon50772");
            text = xml.CreateTextNode(CBEshelon50772.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Selena50766");
            text = xml.CreateTextNode(CBSelena50766.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Zai50769");
            text = xml.CreateTextNode(CBZai50769.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Sahn50780");
            text = xml.CreateTextNode(CBSahn50780.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Nalash50776");
            text = xml.CreateTextNode(CBNalash50776.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Garlok50739");
            text = xml.CreateTextNode(CBGarlok50739.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Kaltik50749");
            text = xml.CreateTextNode(CBKaltik50749.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Lithik50734");
            text = xml.CreateTextNode(CBLithik50734.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Nallak50364");
            text = xml.CreateTextNode(CBNallak50364.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Kraxik50363");
            text = xml.CreateTextNode(CBKraxik50363.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Skithik50733");
            text = xml.CreateTextNode(CBSkithik50733.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Torik50388");
            text = xml.CreateTextNode(CBTorik50388.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Borginn50341");
            text = xml.CreateTextNode(CBBorginn50341.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Kang50349");
            text = xml.CreateTextNode(CBKang50349.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Gaarn50340");
            text = xml.CreateTextNode(CBGaarn50340.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Karr50347");
            text = xml.CreateTextNode(CBKarr50347.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Kornas50338");
            text = xml.CreateTextNode(CBKornas50338.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Norlaxx50344");
            text = xml.CreateTextNode(CBNorlaxx50344.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Sulikshor50339");
            text = xml.CreateTextNode(CBSulikshor50339.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Havak50354");
            text = xml.CreateTextNode(CBHavak50354.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("JonnDar50351");
            text = xml.CreateTextNode(CBJonnDar50351.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Kahtir50355");
            text = xml.CreateTextNode(CBKahtir50355.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Krol50356");
            text = xml.CreateTextNode(CBKrol50356.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Morgrinn50350");
            text = xml.CreateTextNode(CBMorgrinn50350.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Qunas50352");
            text = xml.CreateTextNode(CBQunas50352.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Urgolax50359");
            text = xml.CreateTextNode(CBUrgolax50359.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("AiLi50821");
            text = xml.CreateTextNode(CBAiLi50821.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Ahone50817");
            text = xml.CreateTextNode(CBAhone50817.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("AiRan50822");
            text = xml.CreateTextNode(CBAiRan50822.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Ruun50816");
            text = xml.CreateTextNode(CBRuun50816.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Nasra50811");
            text = xml.CreateTextNode(CBNasra50811.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Urobi50808");
            text = xml.CreateTextNode(CBUrobi50808.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Yul50820");
            text = xml.CreateTextNode(CBYul50820.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Arness50787");
            text = xml.CreateTextNode(CBArness50787.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Moldo50806");
            text = xml.CreateTextNode(CBMoldo50806.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Nessos50789");
            text = xml.CreateTextNode(CBNessos50789.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Omnis50805");
            text = xml.CreateTextNode(CBOmnis50805.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Salyin50783");
            text = xml.CreateTextNode(CBSalyin50783.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Sarnak50782");
            text = xml.CreateTextNode(CBSarnak50782.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //let's add another element (child of the root)
            element = xml.CreateElement("Siltriss50791");
            text = xml.CreateTextNode(CBSiltriss50791.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Blackhoof51059");
            text = xml.CreateTextNode(CBBlackhoof51059.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Dak50334");
            text = xml.CreateTextNode(CBDak50334.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Ferdinand51078");
            text = xml.CreateTextNode(CBFerdinand51078.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("GoKan50331");
            text = xml.CreateTextNode(CBGoKan50331.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Korda50332");
            text = xml.CreateTextNode(CBKorda50332.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Lon50333");
            text = xml.CreateTextNode(CBLon50333.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Yorik50336");
            text = xml.CreateTextNode(CBYorik50336.Checked.ToString());
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

            Close();
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
            CBKillList.Checked = false;
            CBCata.Checked = true;
            CBWotlk.Checked = true;
            CBBC.Checked = true;
            CBLowRAR.Checked = false;
            CBTLPD.Checked = true;
            CBPoseidus.Checked = true;
            CBAeonaxx.Checked = false;
            CBRaptorNest.Checked = true;
            CBDarkSoil.Checked = false;
            CBAnotherMansTreasure.Checked = false;
            CBBlingtron.Checked = false;
            CBInteractNPC.Checked = false;
            CBObjects.Checked = true;
            // Hunt by ID
            CBHuntByID.Checked = false;
            TBHuntByID.Text = "";
            CBBlacklistCheck.Checked = true;
            TBBlacklistTime.Text = "180";
            //Misc
            CBAlert.Checked = true;
            //Security
            CBPlayerScan.Checked = false;
            CBLUAoutput.Checked = false;
            CBWisper.Checked = true;
            CBBNWisper.Checked = true;
            CBGuild.Checked = false;
            CBKeyer.Checked = true;
            CBShadowmeld.Checked = false;
            TBSoundfileWisper.Text = Rarekiller.Soundfile2;
            TBSoundfileGuild.Text = Rarekiller.Soundfile2;
            TBSoundfileFoundRare.Text = Rarekiller.Soundfile;
            //Tamer
            CBTameDefault.Checked = false;
            CBTameByID.Checked = false;
            CBNotKillTameable.Checked = false;
            TBTameID.Text = "";
            CBFootprints.Checked = false;
            CBFootprints.Enabled = Rarekiller.Settings.BETA;
            CBTameDefault.Enabled = Rarekiller.Settings.Hunteractivated;
            CBTameByID.Enabled = Rarekiller.Settings.Hunteractivated;
            TBTameID.Enabled = Rarekiller.Settings.Hunteractivated;
            CBFootprints.Enabled = Rarekiller.Settings.Hunteractivated;
            // Slowfall
            CBUseSlowfall.Checked = true;
            RBCloak.Checked = false;
            CBItem.Checked = false;
            TBSlowfallItem.Text = "";
            CBSpell.Checked = false;
            TBSlowfallSpell.Text = "";
            TBFalltimer.Text = "500";
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
            CBTestcaseTamer.Enabled = false;
            CBTestcaseTamer.Checked = false;

            //Mists of Pandaria
            CBBonobos50828.Checked = false;
            CBIkIk50836.Checked = false;
            CBNanners50840.Checked = false;
            CBFerocious50823.Checked = false;
            CBScritch50831.Checked = false;
            CBSpriggin50830.Checked = false;
            CBYowler50832.Checked = false;
            CBAethis50750.Checked = false;
            CBCournith50768.Checked = false;
            CBEshelon50772.Checked = false;
            CBSelena50766.Checked = false;
            CBZai50769.Checked = false;
            CBSahn50780.Checked = false;
            CBNalash50776.Checked = false;
            CBGarlok50739.Checked = false;
            CBKaltik50749.Checked = false;
            CBLithik50734.Checked = false;
            CBNallak50364.Checked = false;
            CBKraxik50363.Checked = false;
            CBSkithik50733.Checked = false;
            CBTorik50388.Checked = false;
            CBBorginn50341.Checked = false;
            CBKang50349.Checked = false;
            CBGaarn50340.Checked = false;
            CBKarr50347.Checked = false;
            CBKornas50338.Checked = false;
            CBNorlaxx50344.Checked = false;
            CBSulikshor50339.Checked = false;
            CBHavak50354.Checked = false;
            CBJonnDar50351.Checked = false;
            CBKahtir50355.Checked = false;
            CBKrol50356.Checked = false;
            CBMorgrinn50350.Checked = false;
            CBQunas50352.Checked = false;
            CBUrgolax50359.Checked = false;
            CBAiLi50821.Checked = false;
            CBAhone50817.Checked = false;
            CBAiRan50822.Checked = false;
            CBRuun50816.Checked = false;
            CBNasra50811.Checked = false;
            CBUrobi50808.Checked = false;
            CBYul50820.Checked = false;
            CBArness50787.Checked = false;
            CBMoldo50806.Checked = false;
            CBNessos50789.Checked = false;
            CBOmnis50805.Checked = false;
            CBSalyin50783.Checked = false;
            CBSarnak50782.Checked = false;
            CBSiltriss50791.Checked = false;
            CBBlackhoof51059.Checked = false;
            CBDak50334.Checked = false;
            CBFerdinand51078.Checked = false;
            CBGoKan50331.Checked = false;
            CBKorda50332.Checked = false;
            CBLon50333.Checked = false;
            CBYorik50336.Checked = false;

			// Variablen nach Settings übernehmen
            // Addons
            Rarekiller.Settings.MOP = CBMOP.Checked;
            Rarekiller.Settings.KillList = CBKillList.Checked;
            Rarekiller.Settings.CATA = CBCata.Checked;
            Rarekiller.Settings.WOTLK = CBWotlk.Checked;
            Rarekiller.Settings.BC = CBBC.Checked;
            Rarekiller.Settings.LowRAR = CBLowRAR.Checked;
            Rarekiller.Settings.TLPD = CBTLPD.Checked;
			Rarekiller.Settings.Poseidus = CBPoseidus.Checked;
            Rarekiller.Settings.Aeonaxx = CBAeonaxx.Checked;
            Rarekiller.Settings.RaptorNest = CBRaptorNest.Checked;
            Rarekiller.Settings.Blingtron = CBBlingtron.Checked;
            Rarekiller.Settings.DarkSoil = CBDarkSoil.Checked;
            Rarekiller.Settings.InteractNPC = CBInteractNPC.Checked;
            Rarekiller.Settings.AnotherMansTreasure = CBAnotherMansTreasure.Checked;
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
            Rarekiller.Settings.Footprints = CBFootprints.Checked;
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
            Rarekiller.Settings.PlayerScan = CBPlayerScan.Checked;
            Rarekiller.Settings.LUAoutput = CBLUAoutput.Checked;
            Rarekiller.Settings.Alert = CBAlert.Checked;
            Rarekiller.Settings.Wisper = CBWisper.Checked;
            Rarekiller.Settings.BNWisper = CBBNWisper.Checked;
            Rarekiller.Settings.Guild = CBGuild.Checked;
            Rarekiller.Settings.Keyer = CBKeyer.Checked;
            Rarekiller.Settings.Shadowmeld = CBShadowmeld.Checked;
            Rarekiller.Settings.SoundfileWisper = TBSoundfileWisper.Text;
            Rarekiller.Settings.SoundfileGuild = TBSoundfileGuild.Text;
            Rarekiller.Settings.SoundfileFoundRare = TBSoundfileFoundRare.Text;
           //Developer Box
            Rarekiller.Settings.TestRaptorNest = CBTestRaptorNest.Checked; 
            Rarekiller.Settings.TestFigurineInteract = CBTestCamel.Checked;
            Rarekiller.Settings.TestcaseTamer = CBTestcaseTamer.Checked;

            //Mists of Pandaria
            Rarekiller.Settings.Bonobos50828 = CBBonobos50828.Checked;
            Rarekiller.Settings.IkIk50836 = CBIkIk50836.Checked;
            Rarekiller.Settings.Nanners50840 = CBNanners50840.Checked;
            Rarekiller.Settings.Ferocious50823 = CBFerocious50823.Checked;
            Rarekiller.Settings.Scritch50831 = CBScritch50831.Checked;
            Rarekiller.Settings.Spriggin50830 = CBSpriggin50830.Checked;
            Rarekiller.Settings.Yowler50832 = CBYowler50832.Checked;
            Rarekiller.Settings.Aethis50750 = CBAethis50750.Checked;
            Rarekiller.Settings.Cournith50768 = CBCournith50768.Checked;
            Rarekiller.Settings.Eshelon50772 = CBEshelon50772.Checked;
            Rarekiller.Settings.Selena50766 = CBSelena50766.Checked;
            Rarekiller.Settings.Zai50769 = CBZai50769.Checked;
            Rarekiller.Settings.Sahn50780 = CBSahn50780.Checked;
            Rarekiller.Settings.Nalash50776 = CBNalash50776.Checked;
            Rarekiller.Settings.Garlok50739 = CBGarlok50739.Checked;
            Rarekiller.Settings.Kaltik50749 = CBKaltik50749.Checked;
            Rarekiller.Settings.Lithik50734 = CBLithik50734.Checked;
            Rarekiller.Settings.Nallak50364 = CBNallak50364.Checked;
            Rarekiller.Settings.Kraxik50363 = CBKraxik50363.Checked;
            Rarekiller.Settings.Skithik50733 = CBSkithik50733.Checked;
            Rarekiller.Settings.Torik50388 = CBTorik50388.Checked;
            Rarekiller.Settings.Borginn50341 = CBBorginn50341.Checked;
            Rarekiller.Settings.Kang50349 = CBKang50349.Checked;
            Rarekiller.Settings.Gaarn50340 = CBGaarn50340.Checked;
            Rarekiller.Settings.Karr50347 = CBKarr50347.Checked;
            Rarekiller.Settings.Kornas50338 = CBKornas50338.Checked;
            Rarekiller.Settings.Norlaxx50344 = CBNorlaxx50344.Checked;
            Rarekiller.Settings.Sulikshor50339 = CBSulikshor50339.Checked;
            Rarekiller.Settings.Havak50354 = CBHavak50354.Checked;
            Rarekiller.Settings.JonnDar50351 = CBJonnDar50351.Checked;
            Rarekiller.Settings.Kahtir50355 = CBKahtir50355.Checked;
            Rarekiller.Settings.Krol50356 = CBKrol50356.Checked;
            Rarekiller.Settings.Morgrinn50350 = CBMorgrinn50350.Checked;
            Rarekiller.Settings.Qunas50352 = CBQunas50352.Checked;
            Rarekiller.Settings.Urgolax50359 = CBUrgolax50359.Checked;
            Rarekiller.Settings.AiLi50821 = CBAiLi50821.Checked;
            Rarekiller.Settings.Ahone50817 = CBAhone50817.Checked;
            Rarekiller.Settings.AiRan50822 = CBAiRan50822.Checked;
            Rarekiller.Settings.Ruun50816 = CBRuun50816.Checked;
            Rarekiller.Settings.Nasra50811 = CBNasra50811.Checked;
            Rarekiller.Settings.Urobi50808 = CBUrobi50808.Checked;
            Rarekiller.Settings.Yul50820 = CBYul50820.Checked;
            Rarekiller.Settings.Arness50787 = CBArness50787.Checked;
            Rarekiller.Settings.Moldo50806 = CBMoldo50806.Checked;
            Rarekiller.Settings.Nessos50789 = CBNessos50789.Checked;
            Rarekiller.Settings.Omnis50805 = CBOmnis50805.Checked;
            Rarekiller.Settings.Salyin50783 = CBSalyin50783.Checked;
            Rarekiller.Settings.Sarnak50782 = CBSarnak50782.Checked;
            Rarekiller.Settings.Siltriss50791 = CBSiltriss50791.Checked;
            Rarekiller.Settings.Blackhoof51059 = CBBlackhoof51059.Checked;
            Rarekiller.Settings.Dak50334 = CBDak50334.Checked;
            Rarekiller.Settings.Ferdinand51078 = CBFerdinand51078.Checked;
            Rarekiller.Settings.GoKan50331 = CBGoKan50331.Checked;
            Rarekiller.Settings.Korda50332 = CBKorda50332.Checked;
            Rarekiller.Settings.Lon50333 = CBLon50333.Checked;
            Rarekiller.Settings.Yorik50336 = CBYorik50336.Checked;

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

        private void CBCheckAllMoP_Click(object sender, EventArgs e)
        {
            CBBonobos50828.Checked = true;
            CBIkIk50836.Checked = true;
            CBNanners50840.Checked = true;
            CBFerocious50823.Checked = true;
            CBScritch50831.Checked = true;
            CBSpriggin50830.Checked = true;
            CBYowler50832.Checked = true;
            CBAethis50750.Checked = true;
            CBCournith50768.Checked = true;
            CBEshelon50772.Checked = true;
            CBSelena50766.Checked = true;
            CBZai50769.Checked = true;
            CBSahn50780.Checked = true;
            CBNalash50776.Checked = true;
            //CBGarlok50739.Checked = true;
            //CBKaltik50749.Checked = true;
            CBLithik50734.Checked = true;
            //CBNallak50364.Checked = true;
            CBKraxik50363.Checked = true;
            CBSkithik50733.Checked = true;
            CBTorik50388.Checked = true;
            CBBorginn50341.Checked = true;
            CBKang50349.Checked = true;
            CBGaarn50340.Checked = true;
            CBKarr50347.Checked = true;
            CBKornas50338.Checked = true;
            CBNorlaxx50344.Checked = true;
            CBSulikshor50339.Checked = true;
            CBHavak50354.Checked = true;
            CBJonnDar50351.Checked = true;
            CBKahtir50355.Checked = true;
            CBKrol50356.Checked = true;
            CBMorgrinn50350.Checked = true;
            CBQunas50352.Checked = true;
            CBUrgolax50359.Checked = true;
            //CBAiLi50821.Checked = true;
            //CBAhone50817.Checked = true;
            //CBAiRan50822.Checked = true;
            //CBRuun50816.Checked = true;
            //CBNasra50811.Checked = true;
            CBUrobi50808.Checked = true;
            //CBYul50820.Checked = true;
            CBArness50787.Checked = true;
            CBMoldo50806.Checked = true;
            CBNessos50789.Checked = true;
            CBOmnis50805.Checked = true;
            CBSalyin50783.Checked = true;
            CBSarnak50782.Checked = true;
            CBSiltriss50791.Checked = true;
            CBBlackhoof51059.Checked = true;
            CBDak50334.Checked = true;
            CBFerdinand51078.Checked = true;
            CBGoKan50331.Checked = true;
            CBKorda50332.Checked = true;
            CBLon50333.Checked = true;
            CBYorik50336.Checked = true;
        }

        private void CBUncheckAllMoP_Click(object sender, EventArgs e)
        {
            //Mists of Pandaria
            CBBonobos50828.Checked = false;
            CBIkIk50836.Checked = false;
            CBNanners50840.Checked = false;
            CBFerocious50823.Checked = false;
            CBScritch50831.Checked = false;
            CBSpriggin50830.Checked = false;
            CBYowler50832.Checked = false;
            CBAethis50750.Checked = false;
            CBCournith50768.Checked = false;
            CBEshelon50772.Checked = false;
            CBSelena50766.Checked = false;
            CBZai50769.Checked = false;
            CBSahn50780.Checked = false;
            CBNalash50776.Checked = false;
            CBGarlok50739.Checked = false;
            CBKaltik50749.Checked = false;
            CBLithik50734.Checked = false;
            CBNallak50364.Checked = false;
            CBKraxik50363.Checked = false;
            CBSkithik50733.Checked = false;
            CBTorik50388.Checked = false;
            CBBorginn50341.Checked = false;
            CBKang50349.Checked = false;
            CBGaarn50340.Checked = false;
            CBKarr50347.Checked = false;
            CBKornas50338.Checked = false;
            CBNorlaxx50344.Checked = false;
            CBSulikshor50339.Checked = false;
            CBHavak50354.Checked = false;
            CBJonnDar50351.Checked = false;
            CBKahtir50355.Checked = false;
            CBKrol50356.Checked = false;
            CBMorgrinn50350.Checked = false;
            CBQunas50352.Checked = false;
            CBUrgolax50359.Checked = false;
            CBAiLi50821.Checked = false;
            CBAhone50817.Checked = false;
            CBAiRan50822.Checked = false;
            CBRuun50816.Checked = false;
            CBNasra50811.Checked = false;
            CBUrobi50808.Checked = false;
            CBYul50820.Checked = false;
            CBArness50787.Checked = false;
            CBMoldo50806.Checked = false;
            CBNessos50789.Checked = false;
            CBOmnis50805.Checked = false;
            CBSalyin50783.Checked = false;
            CBSarnak50782.Checked = false;
            CBSiltriss50791.Checked = false;
            CBBlackhoof51059.Checked = false;
            CBDak50334.Checked = false;
            CBFerdinand51078.Checked = false;
            CBGoKan50331.Checked = false;
            CBKorda50332.Checked = false;
            CBLon50333.Checked = false;
            CBYorik50336.Checked = false;
        }
    }
}
