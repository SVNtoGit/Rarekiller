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
    class RarekillerSlowfall
    {
        public static LocalPlayer Me = StyxWoW.Me;

        public void HelpFalling()
        {
			int UseSlowfall = 1;
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell) && !Me.HasAura("Slow Fall") 
				&& !Me.HasAura("Levitate") && Rarekiller.Settings.Spell 
				&& SpellManager.CanCast(Rarekiller.Settings.SlowfallSpell) && SpellManager.HasSpell(Rarekiller.Settings.SlowfallSpell))
			{
				SpellManager.Cast(Rarekiller.Settings.SlowfallSpell);
				Thread.Sleep(300);
				if (Me.HasAura(Rarekiller.Settings.SlowfallSpell))
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Used Slowfall Ability {0}", Rarekiller.Settings.SlowfallSpell);
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && Rarekiller.Settings.Cloak && (Me.Inventory.Equipped.Back.Cooldown == 0))
			{
				Me.Inventory.Equipped.Back.Use();
				Thread.Sleep(300);
				if (Me.HasAura("Parachute") || Me.HasAura("Slow Fall"))
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Used Slowfall Ability Cloak");
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && Rarekiller.Settings.Item)
			{
				Lua.DoString("UseItemByName(\"" + Rarekiller.Settings.SlowfallItem + "\")"); // or use Item
				Thread.Sleep(300);
				if (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura("Slow Fall"))
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Used Slowfall Ability {0}", Rarekiller.Settings.SlowfallItem);
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && SpellManager.CanCast("Slow Fall") && SpellManager.HasSpell("Slow Fall"))
			{
				SpellManager.Cast("Slow Fall");
				Thread.Sleep(300);
				if (Me.HasAura("Slow Fall"))
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Used Slowfall Ability Slow Fall");
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && SpellManager.CanCast("Levitate") && SpellManager.HasSpell("Levitate"))
			{
				SpellManager.Cast("Levitate");
				Thread.Sleep(300);
				if (Me.HasAura("Levitate"))
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Used Slowfall Ability Levitate");
					return;
				}
			}
			
			if (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura(Rarekiller.Settings.SlowfallSpell) || Me.HasAura("Slow Fall") || Me.HasAura("Levitate"))
			{
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Slowfall successfull, Parachute to Ground");
				//Überprüfen:

				if (Me.CurrentTarget != null)
				{
					if (Me.CurrentTarget.IsMe)
						Me.ClearTarget();
					if(!Me.CurrentTarget.IsMe)
						Me.CurrentTarget.Face();
				}
			}
			else
			{
				if (Me.IsDead || Me.IsGhost)
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: I'm dead");
					return;
				}
				if (!Me.IsFalling)
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Don't fall any more");
					return;
				}


				if (UseSlowfall >= 3)
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller Part Slowfall: Slowfall failed 3 Times");
					return;
				}
                Logging.Write(Colors.MediumPurple, "Rarekiller Part Slowfall: Slowfall failed");
				UseSlowfall = UseSlowfall + 1;
				Thread.Sleep(300);
				Rarekiller.Slowfall.HelpFalling();
			}
// Slowfall Part End
        }
    }
}
