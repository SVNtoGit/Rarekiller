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
    class RarekillerSecurity
    {
        bool LeftRight = true;
        
        public static LocalPlayer Me = StyxWoW.Me; 

        public void Movearound()
        {
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Keypresser: Move around");
            if (LeftRight)
            {
                //KeyboardManager.PressKey('A');
                WoWMovement.Move(WoWMovement.MovementDirection.TurnLeft);
				Thread.Sleep(100);
				WoWMovement.MoveStop();
                //KeyboardManager.ReleaseKey('A');
                LeftRight = false;
            }
            else
            {
                //KeyboardManager.PressKey('D');
				WoWMovement.Move(WoWMovement.MovementDirection.TurnRight);
                Thread.Sleep(100);
				WoWMovement.MoveStop();
                //KeyboardManager.ReleaseKey('D');
                LeftRight = true;
            }
        }

        public void newWhisper(Chat.ChatWhisperEventArgs arg)
        {
            bool IsGM = Lua.GetReturnVal<bool>("if(_G.GMChatFrame_IsGM and _G.GMChatFrame_IsGM("+ arg.Author + ")) then return true; else return false; end", 0); // from WIM Addon; WIM.lua Z:449 - Needs some Work !!
			if (Rarekiller.Settings.Wisper)
            {
				if (File.Exists(Rarekiller.Settings.SoundfileWisper))
                    new SoundPlayer(Rarekiller.Settings.SoundfileWisper).Play();
                else if (File.Exists(Rarekiller.Soundfile))
                    new SoundPlayer(Rarekiller.Soundfile).Play();
                else
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Alert: playing Soundfile failes");
				if(IsGM) //doesn't work !!! 
                    Logging.Write(Colors.DarkOrange, "Rarekiller Part Alert: You got a GM Wisper: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
				else
                    Logging.Write(Colors.Pink, "Rarekiller Part Alert: You got a Wisper: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }
        public void BNWhisper(object sender, LuaEventArgs args)
        {
            object[] Args = args.Args;
            string Message = Args[0].ToString();
            string presenceId = Args[12].ToString();
            string Author = Lua.GetReturnValues(String.Format("return BNGetFriendInfoByID({0})", presenceId))[3];

            if (File.Exists(Rarekiller.Settings.SoundfileWisper))
                new SoundPlayer(Rarekiller.Settings.SoundfileWisper).Play();
            else if (File.Exists(Rarekiller.Soundfile))
                new SoundPlayer(Rarekiller.Soundfile).Play();
            else
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Alert: playing Soundfile failes");
            Logging.Write(Colors.Aqua, "Rarekiller Part Alert: You got a BN Wisper: {0}: {1} - Timestamp: {2}: {3}", Author, Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
        }
        public void newGuild(Chat.ChatGuildEventArgs arg)
        {
            if (Rarekiller.Settings.Guild)
            {
                if (File.Exists(Rarekiller.Settings.SoundfileGuild))
                    new SoundPlayer(Rarekiller.Settings.SoundfileGuild).Play();
                else if (File.Exists(Rarekiller.Soundfile))
                    new SoundPlayer(Rarekiller.Soundfile).Play();
                else
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Alert: playing Soundfile failes");
                Logging.Write(Colors.Lime, "Rarekiller Part Alert: Guildmessage: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }
        public void newOfficer(Chat.ChatLanguageSpecificEventArgs arg)
        {
            if (Rarekiller.Settings.Guild)
            {
                if (File.Exists(Rarekiller.Settings.SoundfileGuild))
                    new SoundPlayer(Rarekiller.Settings.SoundfileGuild).Play();
                else if (File.Exists(Rarekiller.Soundfile))
                    new SoundPlayer(Rarekiller.Soundfile).Play();
                else
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Alert: playing Soundfile failes");
                Logging.Write(Colors.Lime, "Rarekiller Part Alert: Officermessage: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }
		
    }
}
