//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//
//==================================================================
using System;
using System.Threading;
using System.IO;
using System.Media;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Helpers;

namespace katzerle
{
    class RarekillerSecurity
    {
        bool LeftRight = true;
        
        public static LocalPlayer Me = StyxWoW.Me;

        /// <summary>
        /// Function to Move around for Anti AFK
        /// </summary>
        public void Movearound()
        {
            Logging.Write(Colors.MediumPurple, "Rarekiller: Anti AFK Move around");
            if (LeftRight)
            {
                KeyboardManager.PressKey('A');
                //WoWMovement.Move(WoWMovement.MovementDirection.TurnLeft);
				Thread.Sleep(100);
				//WoWMovement.MoveStop();
                KeyboardManager.ReleaseKey('A');
                LeftRight = false;
            }
            else
            {
                KeyboardManager.PressKey('D');
				//WoWMovement.Move(WoWMovement.MovementDirection.TurnRight);
                Thread.Sleep(100);
				//WoWMovement.MoveStop();
                KeyboardManager.ReleaseKey('D');
                LeftRight = true;
            }
        }

        /// <summary>
        /// Function to get all Players around a Object
        /// </summary>
        /// <param name="Object">The Object</param>
        /// <returns>true if there are one or more Players around a Object</returns>
        static public bool PlayerAround(WoWObject Object)
        {
            List<WoWPlayer> PlayerList = ObjectManager.GetObjectsOfType<WoWPlayer>()
                .Where(r => !r.IsDead).OrderBy(r => r.Distance).ToList();
            foreach (WoWPlayer r in PlayerList)
            {
                if (Object.Location.Distance(r.Location) < 5)
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// Function to get all Players around a Unit
        /// </summary>
        /// <param name="Object">The Unit</param>
        /// <returns>true if there are one or more Players around a Unit</returns>
        static public bool PlayerAround(WoWUnit Unit)
        {
            List<WoWPlayer> PlayerList = ObjectManager.GetObjectsOfType<WoWPlayer>()
                .Where(r => !r.IsDead).OrderBy(r => r.Distance).ToList();
            foreach (WoWPlayer r in PlayerList)
            {
                if (Unit.Location.Distance(r.Location) < 5)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Makes Noise at Wisper and Logs it
        /// </summary>
        public void newWhisper(Chat.ChatWhisperEventArgs arg)
        {
            //bool IsGM = Lua.GetReturnVal<bool>("if(_G.GMChatFrame_IsGM and _G.GMChatFrame_IsGM("+ arg.Author + ")) then return true; else return false; end", 0); // from WIM Addon; WIM.lua Z:449 - Needs some Work !!
			if (Rarekiller.Settings.Wisper)
            {
				if (File.Exists(Rarekiller.Settings.SoundfileWisper))
                    new SoundPlayer(Rarekiller.Settings.SoundfileWisper).Play();
                else if (File.Exists(Rarekiller.Soundfile2))
                    new SoundPlayer(Rarekiller.Soundfile2).Play();
                else
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: playing Soundfile failes");
				//if(IsGM) //doesn't work !!! 
                //    Logging.Write(Colors.DarkOrange, "Rarekiller: You got a GM Wisper: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
				//else
                    Logging.Write(Colors.Pink, "Rarekiller: You got a Wisper: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }

        /// <summary>
        /// Makes Noise at BNWhisper and Logs it
        /// </summary>
        public void BNWhisper(object sender, LuaEventArgs args)
        {
            object[] Args = args.Args;
            string Message = Args[0].ToString();
            string presenceId = Args[12].ToString();
            string Author = Lua.GetReturnValues(String.Format("return BNGetFriendInfoByID({0})", presenceId))[3];

            if (Rarekiller.Settings.BNWisper)
            {
                if (File.Exists(Rarekiller.Settings.SoundfileWisper))
                    new SoundPlayer(Rarekiller.Settings.SoundfileWisper).Play();
                else if (File.Exists(Rarekiller.Soundfile2))
                    new SoundPlayer(Rarekiller.Soundfile2).Play();
                else
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: playing Soundfile failes");
                Logging.Write(Colors.Aqua, "Rarekiller: You got a BN Wisper: {0}: {1} - Timestamp: {2}: {3}", Author, Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }

        /// <summary>
        /// Makes Noise at Guild Message and Logs it
        /// </summary>
        public void newGuild(Chat.ChatGuildEventArgs arg)
        {
            if (Rarekiller.Settings.Guild)
            {
                if (File.Exists(Rarekiller.Settings.SoundfileGuild))
                    new SoundPlayer(Rarekiller.Settings.SoundfileGuild).Play();
                else if (File.Exists(Rarekiller.Soundfile2))
                    new SoundPlayer(Rarekiller.Soundfile2).Play();
                else
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: playing Soundfile failes");
                Logging.Write(Colors.Lime, "Rarekiller: Guildmessage: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }

        /// <summary>
        /// Makes Noise at Guild Officer Message and Logs it
        /// </summary>
        public void newOfficer(Chat.ChatLanguageSpecificEventArgs arg)
        {
            if (Rarekiller.Settings.Guild)
            {
                if (File.Exists(Rarekiller.Settings.SoundfileGuild))
                    new SoundPlayer(Rarekiller.Settings.SoundfileGuild).Play();
                else if (File.Exists(Rarekiller.Soundfile2))
                    new SoundPlayer(Rarekiller.Soundfile2).Play();
                else
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: playing Soundfile failes");
                Logging.Write(Colors.Lime, "Rarekiller: Officermessage: {0}: {1} - Timestamp: {2}: {3}", arg.Author, arg.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            }
        }
    }
}
