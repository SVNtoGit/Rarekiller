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
        /// <summary>
        /// Casts a Spell on a Unit
        /// </summary>
        /// <param name="spellName">The Spellname as String</param>
        /// <param name="Unit">The Enemy</param>
        /// <param name="wait">true and he will wait till the Cast is finished</param>
        /// <returns>true if the cast was sucessfull</returns>
        static public bool CastSafe(string spellName, WoWUnit Unit, bool wait)
        {
            bool SpellSuccess = false;

            if (!SpellManager.HasSpell(spellName))
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I don't have Spell {0}", spellName);
                return false;
            }

            WoWMovement.MoveStop();
            if (Unit != Me)
            {
                Unit.Target();
                if (!Me.IsFacing(Unit))
                {
                    Unit.Face();
                    Thread.Sleep(100);
                }
            }

            if (!SpellManager.CanCast(spellName))
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: cannot cast spell '{0}' yet - cd={1}, gcd={2}, casting={3} ",
                    SpellManager.Spells[spellName].Name,
                    SpellManager.Spells[spellName].Cooldown,
                    SpellManager.GlobalCooldown,
                    Me.IsCasting
                    );
                return false;
            }
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: * Cast Distance: {0}", Unit.Location.Distance(Me.Location).ToString());
            SpellSuccess = SpellManager.Cast(spellName, Unit);
            Logging.Write(Colors.MediumPurple, "Rarekiller: * {0} - {1}", spellName, SpellSuccess);
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Developer Log: Global Cooldown running - Variable in HB {0}", SpellManager.GlobalCooldown);
            if (wait)
            {
                Thread.Sleep(500);
                while (Me.IsCasting)
                {
                    Thread.Sleep(50);
                }
                Thread.Sleep(500);
            }
            return SpellSuccess;
        }

        /// <summary>
        /// Returns the Class spezific Spell of File Rarekiller/config/DefaultInterruptSpells.xml
        /// </summary>
        public string Interrupt
        {
            get
            {
                XmlDocument SpellsXML = new XmlDocument();
                string sPath = Path.Combine(Rarekiller.FolderPath, "config\\DefaultInterruptSpells.xml");
                System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                string ClassMe = Me.Class.ToString();
                string ClassSpell = "None";
                string ReturnSpell = "No Spell available";

                try
                {
                    SpellsXML.Load(fs);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs.Close();
                    return ReturnSpell;
                }
                XmlElement root = SpellsXML.DocumentElement;
                foreach (XmlNode Node in root.ChildNodes)
                {
                    ClassSpell = Node.Attributes["Class"].InnerText;
                    if (ClassMe == ClassSpell)
                    {
                        if (!SpellManager.HasSpell(Node.Attributes["Name"].InnerText))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I don't have Spell {0}", Node.Attributes["Name"].InnerText);
                            continue;
                        }
                        ReturnSpell = Node.Attributes["Name"].InnerText;
                        return ReturnSpell;
                    }
                }
                if(ReturnSpell == "No Spell available")
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Class {0} not Found", ClassMe);
                return ReturnSpell;
            }
        }

        /// <summary>
        /// Returns the Class spezific Spell of File Rarekiller/config/DefaultStunSpells.xml
        /// </summary>
        public string Stun
        {
            get
            {
                XmlDocument SpellsXML = new XmlDocument();
                string sPath = Path.Combine(Rarekiller.FolderPath, "config\\DefaultStunSpells.xml");
                System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                string ClassMe = Me.Class.ToString();
                string ClassSpell = "None";
                string ReturnSpell = "No Spell available";

                try
                {
                    SpellsXML.Load(fs);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs.Close();
                    return ReturnSpell;
                }
                XmlElement root = SpellsXML.DocumentElement;
                foreach (XmlNode Node in root.ChildNodes)
                {
                    ClassSpell = Node.Attributes["Class"].InnerText;
                    if (ClassMe == ClassSpell)
                    {
                        if (!SpellManager.HasSpell(Node.Attributes["Name"].InnerText))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I don't have Spell {0}", Node.Attributes["Name"].InnerText);
                            continue;
                        }
                        ReturnSpell = Node.Attributes["Name"].InnerText;
                        return ReturnSpell;
                    }
                }
                if (ReturnSpell == "No Spell available")
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Class {0} not Found", ClassMe);
                return ReturnSpell;
            }
        }

        /// <summary>
        /// Returns the Class spezific Spell of File Rarekiller/config/DefaultRangedPullspells.xml
        /// </summary>
        public string RangedPullspell
        {
            get
            {
                XmlDocument SpellsXML = new XmlDocument();
                string sPath = Path.Combine(Rarekiller.FolderPath, "config\\DefaultRangedPullspells.xml");
                System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                string ClassMe = Me.Class.ToString();
                string ClassSpell = "None";
                string ReturnSpell = "No Spell available";

                try
                {
                    SpellsXML.Load(fs);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs.Close();
                    return ReturnSpell;
                }
                XmlElement root = SpellsXML.DocumentElement;
                foreach (XmlNode Node in root.ChildNodes)
                {
                    ClassSpell = Node.Attributes["Class"].InnerText;
                    if (ClassMe == ClassSpell)
                    {
                        if (!SpellManager.HasSpell(Node.Attributes["Name"].InnerText))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I don't have Spell {0}", Node.Attributes["Name"].InnerText);
                            continue;
                        }
                        ReturnSpell = Node.Attributes["Name"].InnerText;
                        return ReturnSpell;
                    }
                }
                if (ReturnSpell == "No Spell available")
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Class {0} not Found", ClassMe);
                return ReturnSpell;
            }
        }

        /// <summary>
        /// Returns the Class spezific Spell of File Rarekiller/config/DefaultPullspells.xml
        /// </summary>
        public string FastPullspell
        {
            get
            {
                XmlDocument SpellsXML = new XmlDocument();
                string sPath = Path.Combine(Rarekiller.FolderPath, "config\\DefaultPullspells.xml");
                System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                string ClassMe = Me.Class.ToString();
                string ClassSpell = "None";
                string ReturnSpell = "No Spell available";

                try
                {
                    SpellsXML.Load(fs);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs.Close();
                    return ReturnSpell;
                }
                XmlElement root = SpellsXML.DocumentElement;
                foreach (XmlNode Node in root.ChildNodes)
                {
                    ClassSpell = Node.Attributes["Class"].InnerText;
                    if (ClassMe == ClassSpell)
                    {
                        if (!SpellManager.HasSpell(Node.Attributes["Name"].InnerText))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I don't have Spell {0}", Node.Attributes["Name"].InnerText);
                            continue;
                        }
                        ReturnSpell = Node.Attributes["Name"].InnerText;
                        return ReturnSpell;
                    }
                }
                if (ReturnSpell == "No Spell available")
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Class {0} not Found", ClassMe);
                return ReturnSpell;
            }
        }

        /// <summary>
        /// Returns the Class spezific Spell of File Rarekiller/config/DefaultPullspells.xml
        /// </summary>
        public string RunFastSpell
        {
            get
            {
                XmlDocument SpellsXML = new XmlDocument();
                string sPath = Path.Combine(Rarekiller.FolderPath, "config\\RunFastSpells.xml");
                System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                string ClassMe = Me.Class.ToString();
                string RaceMe = Me.Race.ToString();
                string ClassSpell = "None";
                string ReturnSpell = "No Spell available";

                try
                {
                    SpellsXML.Load(fs);
                    fs.Close();
                }
                catch (Exception e)
                {
                    Logging.WriteDiagnostic(Colors.Red, e.Message);
                    fs.Close();
                    return ReturnSpell;
                }
                XmlElement root = SpellsXML.DocumentElement;
                foreach (XmlNode Node in root.ChildNodes)
                {
                    ClassSpell = Node.Attributes["Class"].InnerText;
                    if (ClassMe == ClassSpell)
                    {
                        if (!SpellManager.HasSpell(Node.Attributes["Name"].InnerText))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I don't have Spell {0}", Node.Attributes["Name"].InnerText);
                            continue;
                        }
                        ReturnSpell = Node.Attributes["Name"].InnerText;
                        return ReturnSpell;
                    }
                    else if (RaceMe == ClassSpell)
                    {
                        if (!SpellManager.HasSpell(Node.Attributes["Name"].InnerText))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: I don't have Spell {0}", Node.Attributes["Name"].InnerText);
                            continue;
                        }
                        ReturnSpell = Node.Attributes["Name"].InnerText;
                        return ReturnSpell;
                    }
                }
                if (ReturnSpell == "No Spell available")
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Class {0} not Found", ClassMe);
                return ReturnSpell;
            }
        }

        /// <summary>
        /// Returns the Safe Range of a Spell
        /// </summary>
        /// <param name="Object">The Spellname as String</param>
        public string RangeCheck (string Spell)
        {
            //get
            //{
            float NewRange;
            if (SpellManager.Spells[Spell].MaxRange > 17 && !SpellManager.Spells[Spell].IsMeleeSpell)
            {
                NewRange = SpellManager.Spells[Spell].MaxRange - 5;
                return NewRange.ToString();
            }
            else if (SpellManager.Spells[Spell].IsMeleeSpell)
                return "3";
            else
                return "7";
        }	
    }    
}
