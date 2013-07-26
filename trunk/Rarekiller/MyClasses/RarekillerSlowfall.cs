//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//    Credits to highvoltz, bloodlove, SMcCloud, Lofi, ZapMan 
//                and all the brave Testers
//
//==================================================================
using System.Threading;
using System.Linq;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace katzerle
{
    class RarekillerSlowfall
    {
        public static LocalPlayer Me = StyxWoW.Me;
        #region Snowfall Lager
        public WoWItem SnowfallLagerID
        {
            get
            {
				return ObjectManager.GetObjectsOfType<WoWItem>().Where(u => u.Entry == 43472).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }
        public WoWItem SnowfallLagerString
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWItem>().Where(u => Rarekiller.Settings.Item && u.Name == Rarekiller.Settings.SlowfallItem).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }
        #endregion

        public void HelpFalling()
        {
			int UseSlowfall = 1;
            //Slowfall Spell
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
            //Cloak
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
            //Snowfall Lager english
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
                && !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && Rarekiller.Settings.Item && Rarekiller.Settings.SlowfallItem == "Snowfall Lager")
			{
                if (SnowfallLagerID.Usable)
                    SnowfallLagerID.Use(); // or use Item
				Thread.Sleep(300);
				if (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura("Slow Fall"))
				{
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Used Slowfall Ability {0}", Rarekiller.Settings.SlowfallItem);
					return;
				}
			}
            //Snowfall Lager Client Language
            if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
                && !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && Rarekiller.Settings.Item)
            {
                if (SnowfallLagerString.Usable)
                    SnowfallLagerString.Use(); // or use Item
                Thread.Sleep(300); if (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura("Slow Fall"))
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Used Slowfall Ability {0}", Rarekiller.Settings.SlowfallItem);
                    return;
                }
            }

            #region Notfallzauber Levitate, Slow Fall, Snowfall Lager by ID
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
            if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
                 && !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && SnowfallLagerID != null)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Found {0} in Bag", SnowfallLagerID.Name);
				if(SnowfallLagerID.Usable)
                    SnowfallLagerID.Use(); // or use Item
                Thread.Sleep(300);
                if (Me.HasAura("Snowfall Lager"))
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Slowfall: Used Slowfall Ability Snowfall Lager (ID)");
                    return;
                }
            }
            #endregion

            #region Slowfall sucessfull ?
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
            #endregion
            // Slowfall Part End
        }
    }
}
