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
using System.Threading;
using System.IO;
using System.Xml;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace katzerle
{
    class RarekillerSpells
    {
        public static LocalPlayer Me = StyxWoW.Me;

		// ------------ Spell Functions
        static public bool CastSafe(string spellName, WoWUnit Unit, bool wait)
        {
            bool SpellSuccess = false;
            if (!SpellManager.HasSpell(spellName))
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Spells: I don't have Spell {0}", spellName);
                return false;
            }

            WoWMovement.MoveStop();
            Unit.Target();
            Unit.Face();
            while (SpellManager.GlobalCooldown)
            {
                Thread.Sleep(10);
            }

            if (!SpellManager.CanCast(spellName))
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Spells: cannot cast spell '{0}' yet - cd={1}, gcd={2}, casting={3} ",
                    SpellManager.Spells[spellName].Name,
                    SpellManager.Spells[spellName].Cooldown,
                    SpellManager.GlobalCooldown,
                    Me.IsCasting
                    );
                return false;
            }
            Logging.Write(Colors.MediumPurple, "Rarekiller Part Spells: * Cast Distance: {0}", Unit.Location.Distance(Me.Location).ToString());
            SpellSuccess = SpellManager.Cast(spellName);
            

			if (SpellSuccess)
                Logging.Write(Colors.MediumPurple, "Rarekiller Part Spells: * {0}.", spellName);	
            if (wait)
            {
                while (SpellManager.GlobalCooldown || Me.IsCasting)
                {
                    Thread.Sleep(100);
                }
            }

            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Spells: Spell successfull? {0}.", SpellSuccess);
            return SpellSuccess;
        }

        public string FastPullspell
        {

            get
            {
                XmlDocument SpellsXML = new XmlDocument();
                string sPath = Path.Combine(Rarekiller.FolderPath, "config\\DefaultPullspells.xml");
                System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                string ClassMe = Me.Class.ToString();
                string ClassSpell = "None";

                try
                {
                    SpellsXML.Load(fs);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs.Close();
                    return "NoSpell";
                }
                XmlElement root = SpellsXML.DocumentElement;
                foreach (XmlNode Node in root.ChildNodes)
                {
                    ClassSpell = Node.Attributes["Class"].InnerText;
                    if (ClassMe == ClassSpell)
                    {
                        if (!SpellManager.HasSpell(Node.Attributes["Name"].InnerText))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Part Spells: I don't have Spell {0}", Node.Attributes["Name"].InnerText);
                            return "NoSpell";
                        }
                        return Node.Attributes["Name"].InnerText;
                    }
                }
                Logging.WriteDiagnostic(Colors.MediumPurple, "Class {0} not Found", ClassMe);
                return "NoSpell";


                //if (SpellManager.HasSpell("Shadow Word: Pain"))
                //    return "Shadow Word: Pain";
                //else if (SpellManager.HasSpell("Fire Blast"))
                //    return "Fire Blast";
                //else if (SpellManager.HasSpell("Heroic Throw"))
                //    return "Heroic Throw";
                //else if (SpellManager.HasSpell("Arcane Shot"))
                //    return "Arcane Shot";
                //else if (SpellManager.HasSpell("Sinister Strike"))
                //    return "Sinister Strike";
                //else if (SpellManager.HasSpell("Icy Touch"))
                //    return "Icy Touch";
                //else if (SpellManager.HasSpell("Crusader Strike"))
                //    return "Crusader Strike";
                //else if (SpellManager.HasSpell("Earth Shock"))
                //    return "Earth Shock";
                //else if (SpellManager.HasSpell("Corruption"))
                //    return "Corruption";
                //else if (SpellManager.HasSpell("Moonfire"))
                //    return "Moonfire";
                //else if (SpellManager.HasSpell("Jab"))
                //    return "Jab";
                //else
                //    return "no Spell";
            }
        }

        public string RangeCheck (string Spell)
        {
            //get
            //{
            float NewRange;
            if (SpellManager.CanCast(Spell) && SpellManager.Spells[Spell].MaxRange > 17 && !SpellManager.Spells[Spell].IsMeleeSpell)
            {
                NewRange = SpellManager.Spells[Spell].MaxRange - 10;
                return NewRange.ToString();
            }
            else if (SpellManager.Spells[Spell].IsMeleeSpell)
                return "3";
            else
                return "7";


                //XmlDocument SpellsXML = new XmlDocument();
                //string sPath = Path.Combine(Rarekiller.FolderPath, "config\\DefaultPullspells.xml");
                //System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                //string ClassMe = Me.Class.ToString();
                //string ClassSpell = "None";

                //try
                //{
                //    SpellsXML.Load(fs);
                //    fs.Close();
                //}
                //catch (Exception e)
                //{
                //    Logging.WriteDiagnostic(Colors.Red, e.Message);
                //    fs.Close();
                //    return "Range";
                //}
                //XmlElement root = SpellsXML.DocumentElement;
                //foreach (XmlNode Node in root.ChildNodes)
                //{
                //    ClassSpell = Node.Attributes["Class"].InnerText;
                //    if (ClassMe == ClassSpell)
                //        return Node.Attributes["Range"].InnerText;
                //}


                //return "NoSpell";
                //if (SpellManager.HasSpell("Shadow Word: Pain"))
                //    return "5";
                //else if (SpellManager.HasSpell("Fire Blast"))
                //    return "5";
                //else if (SpellManager.HasSpell("Heroic Throw"))
                //    return "5";
                //else if (SpellManager.HasSpell("Arcane Shot"))
                //    return "5";
                //else if (SpellManager.HasSpell("Sinister Strike"))
                //    return "3";
                //else if (SpellManager.HasSpell("Icy Touch"))
                //    return "5";
                //else if (SpellManager.HasSpell("Crusader Strike"))
                //    return "3";
                //else if (SpellManager.HasSpell("Earth Shock"))
                //    return "5";
                //else if (SpellManager.HasSpell("Corruption"))
                //    return "5";
                //else if (SpellManager.HasSpell("Moonfire"))
                //    return "5";
                //else if (SpellManager.HasSpell("Jab"))
                //    return "3";
                //else
                //    return "no Spell";
            //}
        }
		
    }    
}
