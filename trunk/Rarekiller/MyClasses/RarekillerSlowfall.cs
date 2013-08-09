//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
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
				return ObjectManager.GetObjectsOfType<WoWItem>().Where(u => u.Entry == 43472 && Me.BagItems.Contains(u)).OrderBy(u => u.Distance).FirstOrDefault();
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

        /// <summary>
        /// Activates Slow Fall and checks if it was successfull
        /// </summary>
        public void HelpFalling()
        {
			int UseSlowfall = 1;
            //Slowfall Spell
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell) && !Me.HasAura("Slow Fall") 
				&& !Me.HasAura("Levitate") && Rarekiller.Settings.Spell 
				&& SpellManager.HasSpell(Rarekiller.Settings.SlowfallSpell))
			{
                RarekillerSpells.CastSafe(Rarekiller.Settings.SlowfallSpell, Me, false);
                //SpellManager.Cast(Rarekiller.Settings.SlowfallSpell);
				Thread.Sleep(200);
				if (Me.HasAura(Rarekiller.Settings.SlowfallSpell))
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Used Slowfall Ability {0}", Rarekiller.Settings.SlowfallSpell);
					return;
				}
			}
            //Cloak
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && Rarekiller.Settings.Cloak && (Me.Inventory.Equipped.Back.Cooldown == 0))
			{
                if (Me.Inventory.Equipped.Back.Usable)
                {
                    Me.Inventory.Equipped.Back.Use();
                    Thread.Sleep(200);
                }
				if (Me.HasAura("Parachute") || Me.HasAura("Slow Fall"))
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Used Slowfall Ability Cloak");
					return;
				}
			}
            //Snowfall Lager english
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
                && !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && Rarekiller.Settings.Item && Rarekiller.Settings.SlowfallItem == "Snowfall Lager")
			{
                if (SnowfallLagerID.Usable)
                {
                    SnowfallLagerID.Use(); // or use Item
                    Thread.Sleep(200);
                }
				if (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura("Slow Fall"))
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Used Slowfall Ability {0}", Rarekiller.Settings.SlowfallItem);
					return;
				}
			}
            //Snowfall Lager Client Language
            if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
                && !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && Rarekiller.Settings.Item)
            {
                if (SnowfallLagerString != null)
                {
                    if (SnowfallLagerString.Usable)
                    {
                        SnowfallLagerString.Use(); // or use Item
                        Thread.Sleep(200);
                    }
                    if (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura("Slow Fall"))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Used Slowfall Ability {0}", Rarekiller.Settings.SlowfallItem);
                        return;
                    }
                }
            }

            #region Notfallzauber Levitate, Slow Fall, Snowfall Lager by ID
            if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && SpellManager.HasSpell("Slow Fall"))
			{
                RarekillerSpells.CastSafe("Slow Fall", Me, false);
                //SpellManager.Cast("Slow Fall");
				Thread.Sleep(200);
				if (Me.HasAura("Slow Fall"))
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Used Slowfall Ability Slow Fall");
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && SpellManager.HasSpell("Levitate"))
			{
                RarekillerSpells.CastSafe("Levitate", Me, false);
                //SpellManager.Cast("Levitate");
				Thread.Sleep(200);
				if (Me.HasAura("Levitate"))
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Used Slowfall Ability Levitate");
					return;
				}
			}
            if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell)
                 && !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && SnowfallLagerID != null)
            {
                Logging.Write(Colors.MediumPurple, "Rarekiller: Found {0} in Bag", SnowfallLagerID.Name);
                if (SnowfallLagerID.Usable)
                {
                    SnowfallLagerID.Use(); // or use Item
                    Thread.Sleep(200);
                }
                if (Me.HasAura("Snowfall Lager"))
                {
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Used Slowfall Ability Snowfall Lager (ID)");
                    return;
                }
            }
            #endregion

            #region Slowfall sucessfull ?
            if (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura(Rarekiller.Settings.SlowfallSpell) || Me.HasAura("Slow Fall") || Me.HasAura("Levitate"))
			{
                Logging.Write(Colors.MediumPurple, "Rarekiller: Slowfall successfull, Parachute to Ground");
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
                    Logging.Write(Colors.MediumPurple, "Rarekiller: I'm dead");
					return;
				}
				if (!Me.IsFalling)
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Don't fall any more");
					return;
				}


				if (UseSlowfall >= 3)
				{
                    Logging.Write(Colors.MediumPurple, "Rarekiller: Slowfall failed 3 Times");
					return;
				}
                Logging.Write(Colors.MediumPurple, "Rarekiller: Slowfall failed");
				UseSlowfall = UseSlowfall + 1;
				Rarekiller.Slowfall.HelpFalling();
            }
            #endregion
            // Slowfall Part End
        }
    }
}
