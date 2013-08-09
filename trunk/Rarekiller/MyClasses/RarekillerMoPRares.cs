//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//
//==================================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Windows.Media;

using Styx;
using Styx.Common;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Pathing;
using Styx.Helpers;
using Styx.CommonBot;


namespace katzerle
{
    class RarekillerMoPRares
    {
        public static LocalPlayer Me = StyxWoW.Me;

        #region Units
        public WoWUnit Hozen
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50828 || o.Entry == 50836 || o.Entry == 50840 || o.Entry == 50823 ||
                    o.Entry == 50831 || o.Entry == 50830 || o.Entry == 50832) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Jinyu
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50750 || o.Entry == 50768 || o.Entry == 50772 || o.Entry == 50766 ||
                    o.Entry == 50769 || o.Entry == 50780 || o.Entry == 50776) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Mantid
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50739 || o.Entry == 50749 || o.Entry == 50734 || o.Entry == 50364 ||
                    o.Entry == 50363 || o.Entry == 50733 || o.Entry == 50388) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit MoguSorcerer
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50341 || o.Entry == 50349 || o.Entry == 50340 || o.Entry == 50347 ||
                    o.Entry == 50338 || o.Entry == 50344 || o.Entry == 50339) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit MoguWarrior
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50354 || o.Entry == 50351 || o.Entry == 50355 || o.Entry == 50356 ||
                    o.Entry == 50350 || o.Entry == 50352 || o.Entry == 50359) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Pandaren
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50821 || o.Entry == 50817 || o.Entry == 50822 || o.Entry == 50816 ||
                    o.Entry == 50811 || o.Entry == 50808 || o.Entry == 50820) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Saurok
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 50787 || o.Entry == 50806 || o.Entry == 50789 || o.Entry == 50805 ||
                    o.Entry == 50783 || o.Entry == 50782 || o.Entry == 50791) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }

        public WoWUnit Yaungol
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (
                    o.Entry == 51059 || o.Entry == 50334 || o.Entry == 51078 || o.Entry == 50331 ||
                    o.Entry == 50332 || o.Entry == 50333 || o.Entry == 50336) && o.Distance < 30 && !o.IsDead).OrderBy(u => u.Distance).FirstOrDefault();
            }
        }
		#endregion

        #region Dump Developer IDs
        /// <summary>
        /// Dumps IDs of AOE Pools to the Logfile
        /// </summary>
        public void DumpAOEEffect()
        {
            ObjectManager.Update();
            List<WoWDynamicObject> AOEList = ObjectManager.GetObjectsOfType<WoWDynamicObject>()
                .Where(o => o.Distance < 10)
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWDynamicObject o in AOEList)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller - Developer AOE Dump: Unit {0}, AOE {1} ID {2}", StyxWoW.Me.CurrentTarget.Name, o.Name, o.Entry);
            }
        }

        /// <summary>
        /// Dumps IDs of everything around to the Logfile
        /// </summary>
        public void DumpJinyuMissiles()
        {
            ObjectManager.Update();

            foreach (WoWMissile o in WoWMissile.AllMissiles)
            {
                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller - Developer AOE Dump: Unit {0}, Missile ID {1}, SpellVisualID {2}, Impact Location {3}", StyxWoW.Me.CurrentTarget.Name, o.SpellId, o.SpellVisualId, o.ImpactPosition);
            }
        }
        #endregion

        #region Avoid Enemy AOE Behaviors
        /// <summary>
        /// Avoids AOE Pools
        /// </summary> 
        /// <param name="Enemy">The Enemy</param>
        /// <param name="Aura">The Aura Name or "No Aura" if no Aura Debuff</param>
        /// <param name="MinDistToEnemy">Minimum Distance to the Enemy</param>
        /// <param name="Objects">A List of AOE Pools</param>
        /// <param name="TraceStep">How many Checks in one Round</param>
        /// <param name="ScanDistance">Distance of the Scans to each other</param>
        public void AvoidEnemyAOE(WoWUnit Enemy, string Aura, int MinDistToPools, List<WoWDynamicObject> Objects, int TraceStep, int ScanDistance)
        {
            if (Objects == null)
            { Logging.Write(Colors.MediumPurple, "Rarekiller: no Pools found .."); return; }
            Logging.Write(Colors.MediumPurple, "Rarekiller: found {0} AOE Pools! start RayCast ..", Objects.Count);
            int MaxDistToMove = MinDistToPools * 2;

            Lua.DoString("RunMacroText(\"/stopcasting\");");
            // get save location
            WoWPoint newP = getSaveLocationSingular(Enemy, Objects, MinDistToPools, MaxDistToMove, TraceStep, ScanDistance, Aura);
            //WoWPoint newP = getSaveLocationBrodie(Me.Location, Objects, MinDistToPools, MaxDistToMove, TraceStep);
            if (newP == WoWPoint.Empty)
            {
                // no save location found, move 2sec Strafe Left
                WoWMovement.Move(WoWMovement.MovementDirection.StrafeLeft, TimeSpan.FromSeconds(2));
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            else
            {
                // move to save location
                while (StyxWoW.Me.Location.Distance(newP) > 4)
                {
                    if (Me.Class != WoWClass.Mage || Me.Class != WoWClass.Monk)
                    {
                        if (!Me.HasAura(Rarekiller.Spells.RunFastSpell) && SpellManager.CanCast(Rarekiller.Spells.RunFastSpell) && newP.Distance(Me.Location) > 12)
                            RarekillerSpells.CastSafe(Rarekiller.Spells.RunFastSpell, Me, false);
                    }

                    if (Me.IsSwimming)
                        WoWMovement.ClickToMove(newP);
                    else
                        Navigator.MoveTo(newP);
                    Thread.Sleep(80);
                    if (Rarekiller.ToonInvalid) return;

                    if (Aura != "No Aura" && !Me.HasAura(Aura))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Don't have {0} any more", Aura);
                        WoWMovement.MoveStop();
                        return;
                    }
                }
            }
            Logging.Write(Colors.MediumPurple, "Rarekiller: reached save Location");
            WoWMovement.MoveStop();
        }

        /// <summary>
        /// Avoids Units
        /// </summary> 
        /// <param name="Enemy">The Enemy</param>
        /// <param name="SpellID">the Spell ID</param>
        /// <param name="MinDistToEnemy">Minimum Distance to the Enemy</param>
        /// <param name="Units">A List of Units</param>
        /// <param name="TraceStep">How many Checks in one Round</param>
        /// <param name="ScanDistance">Distance of the Scans to each other</param>
        public void AvoidEnemyAOE(WoWUnit Enemy, int MinDistToUnit, List<WoWUnit> Units, int TraceStep, int ScanDistance)
        {
            if (Units == null)
            { Logging.Write(Colors.MediumPurple, "Rarekiller: no Tornados found .."); return; }
            Logging.Write(Colors.MediumPurple, "Rarekiller: found {0} Units to avoid! start RayCast ..", Units.Count);
            int MaxDistToMove = MinDistToUnit * 2;

            Lua.DoString("RunMacroText(\"/stopcasting\");");
            // get save location
            WoWPoint newP = getSaveLocationSingular(Enemy, Units, MinDistToUnit, MaxDistToMove, TraceStep, ScanDistance);
            //WoWPoint newP = getSaveLocationBrodie(Me.Location, Units, MinDistToUnit, MaxDistToMove, TraceStep);
            if (newP == WoWPoint.Empty)
            {
                // no save location found, move 2sec Strafe Left
                WoWMovement.Move(WoWMovement.MovementDirection.StrafeLeft, TimeSpan.FromSeconds(2));
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            else
            {
                // move to save location
                while (StyxWoW.Me.Location.Distance(newP) > 5)
                {
                    if (Me.Class != WoWClass.Mage || Me.Class != WoWClass.Monk)
                    {
                        if (!Me.HasAura(Rarekiller.Spells.RunFastSpell) && SpellManager.CanCast(Rarekiller.Spells.RunFastSpell) && newP.Distance(Me.Location) > 12)
                            RarekillerSpells.CastSafe(Rarekiller.Spells.RunFastSpell, Me, false);
                    }
                    
                    if (Me.IsSwimming)
                        WoWMovement.ClickToMove(newP);
                    else
                        Navigator.MoveTo(newP);
                    Thread.Sleep(80);
                    if (Rarekiller.ToonInvalid) return;
                    WoWUnit Check = Units.OrderBy(u => u.Distance).FirstOrDefault();
                    if (Check.Location.Distance(Me.Location) > 40)
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: Far enough away from {0}", Check.Name);
                        WoWMovement.MoveStop();
                        return;
                    }
                }
            }
            Logging.Write(Colors.MediumPurple, "Rarekiller: reached save Location");
            WoWMovement.MoveStop();
        }

        /// <summary>
        /// Avoids Missiles Impact Points
        /// </summary>
        /// <param name="Enemy">The Enemy</param>
        /// <param name="SpellID">the Spell ID</param>
        /// <param name="MinDistToEnemy">Minimum Distance to the Enemy</param>
        /// <param name="Missile">Simply use true</param>
        /// <param name="TraceStep">How many Checks in one Round</param>
        /// <param name="ScanDistance">Distance of the Scans to each other</param>
        public void AvoidEnemyMissiles(WoWUnit Enemy, int SpellID, int MinDistToMissiles, bool Missile, int TraceStep, int ScanDistance)
        {
            foreach (WoWMissile o in WoWMissile.AllMissiles)
            {
                if (WoWMissile.AllMissiles == null)
                { Logging.Write(Colors.MediumPurple, "Rarekiller: no Missiles found .."); return; }
                Logging.Write(Colors.MediumPurple, "Rarekiller: found Missiles! start RayCast ..");

                //int MinDistToMissiles = 9;
                int MaxDistToMove = MinDistToMissiles * 2;

                Lua.DoString("RunMacroText(\"/stopcasting\");");
                // get save location
                WoWPoint newP = getSaveLocationSingular(Enemy, Missile, MinDistToMissiles, MaxDistToMove, TraceStep, ScanDistance, SpellID);
                //WoWPoint newP = getSaveLocationBrodie(Me.Location, Missile, MinDistToMissiles, MaxDistToMove, TraceStep);
                if (newP == WoWPoint.Empty)
                {
                    // no save location found, move 2sec Strafe Left
                    WoWMovement.Move(WoWMovement.MovementDirection.StrafeLeft, TimeSpan.FromSeconds(2));
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
                }
                else
                {
                    // move to save location
                    while (StyxWoW.Me.Location.Distance(newP) > 4 && WoWMissile.AllMissiles != null)
                    {
                        if (Me.Class != WoWClass.Mage || Me.Class != WoWClass.Monk)
                        {
                            if (!Me.HasAura(Rarekiller.Spells.RunFastSpell) && SpellManager.CanCast(Rarekiller.Spells.RunFastSpell) && newP.Distance(Me.Location) > 12)
                                RarekillerSpells.CastSafe(Rarekiller.Spells.RunFastSpell, Me, false);
                        }
                        
                        if (Me.IsSwimming)
                            WoWMovement.ClickToMove(newP);
                        else
                            Navigator.MoveTo(newP);
                        Thread.Sleep(80);
                        if (Rarekiller.ToonInvalid) return;
                        if (SpellID != 0 && (!Enemy.IsCasting || Enemy.CastingSpellId != SpellID))
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller: {0} don't cast any more", Enemy.Name);
                            WoWMovement.MoveStop();
                            return;
                        }
                    }
                }
                Logging.Write(Colors.MediumPurple, "Rarekiller: reached save Location");
                WoWMovement.MoveStop();
            }
        }

        /// <summary>
        /// Logging Diagnostic things
        /// </summary>
        private bool wlog(WoWDynamicObject obj)
        { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: add pool - dis2D: {0}", obj.Distance2D); return true; }

        /// <summary>
        /// Logging Diagnostic things
        /// </summary>
        private bool wlog(WoWUnit unit)
        { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: add pool - dis2D: {0}", unit.Distance2D); return true; }

        /// <summary>
        /// Get a List of AOE Pools around
        /// </summary>
        /// <returns>Returns a List of AOE Pools around</returns>
        public List<WoWDynamicObject> getVoidcloudList
        {
            get
            {
                ObjectManager.Update();
                return (from lp in ObjectManager.GetObjectsOfType<WoWDynamicObject>()
                        orderby lp.Distance2D ascending
                        where lp.Entry == 125241
                        where wlog(lp)
                        select lp).ToList();
            }
        }

        /// <summary>
        /// Get a List of Tornados around
        /// </summary>
        /// <returns>Returns a List of Tornados around</returns>
        public List<WoWUnit> getTornadoList
        {
            get
            {
                ObjectManager.Update();
                List<WoWUnit> Tornados = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Entry == 64267) && u.Distance < 40 && !u.IsDead).OrderBy(o => o.Distance).ToList();
                return Tornados;
            }
        }
        #endregion

        #region Avoid Enemy Cast
        /// <summary>
        /// Get a Location Behind the Enemy
        /// </summary>
        /// <param name="Unit">The Enemy</param>
        /// <returns>WoWPoint behind the Enemy</returns>
        public WoWPoint getLocationBehindUnit(WoWUnit Unit)
        {
            return Unit.Location.RayCast(Unit.Rotation + WoWMathHelper.DegreesToRadians(180), 8f);
        }

        /// <summary>
        /// Runs away from an Enemy 
        /// </summary>
        /// <param name="Enemy">the Enemy</param>
        /// <param name="SpellID">the Spell ID</param>
        /// <param name="MinDistToEnemy">Minimum Distance to Enemy</param>
        /// <param name="TraceStep">How many Checks in one Round</param>
        /// <param name="ScanDistance">Distance of the Scans to each other</param>
        public void FleeingFromEnemy(WoWUnit Enemy, int SpellID, int MinDistToEnemy, int TraceStep, int ScanDistance)
        {
            int MaxDistToMove = MinDistToEnemy * 2;
            Logging.Write(Colors.MediumPurple, "Rarekiller: Fleeing from {0}", Enemy.Name);
            
            Lua.DoString("RunMacroText(\"/stopcasting\");");
            // get save location
            WoWPoint newP = getSaveFleeingLocationSingular(Enemy, MinDistToEnemy, MaxDistToMove, TraceStep, ScanDistance, SpellID);
            //WoWPoint newP = getSaveFleeingLocationBrodie(Me.Location, Enemy, MinDistToEnemy, MaxDistToMove, TraceStep);
            if (newP == WoWPoint.Empty)
            {
                // no save location found, move 2sec Forward
                if(Me.IsFacing(Enemy))
                    Navigator.MoveTo(getLocationBehindUnit(Me));
                WoWMovement.Move(WoWMovement.MovementDirection.Forward, TimeSpan.FromSeconds(2));
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            else
            {
                // move to save location
                while (StyxWoW.Me.Location.Distance(newP) > 4)
                {
                    if (Me.Class == WoWClass.Mage || Me.Class == WoWClass.Monk)
                    {
                        if (SpellManager.CanCast(Rarekiller.Spells.RunFastSpell) && Me.IsFacing(newP) && newP.Distance(Me.Location) > 20)
                            RarekillerSpells.CastSafe(Rarekiller.Spells.RunFastSpell, Me, false);
                    }
                    else
                    {
                        if (!Me.HasAura(Rarekiller.Spells.RunFastSpell) && SpellManager.CanCast(Rarekiller.Spells.RunFastSpell) && newP.Distance(Me.Location) > 15)
                            RarekillerSpells.CastSafe(Rarekiller.Spells.RunFastSpell, Me, false);
                    }

                    if (Me.IsSwimming)
                        WoWMovement.ClickToMove(newP);
                    else
                        Navigator.MoveTo(newP);
                    Thread.Sleep(80);
                    if (Rarekiller.ToonInvalid) return;
                    if ((SpellID != 0 && (!Enemy.IsCasting || Enemy.CastingSpellId != SpellID)) || ((Enemy == Yaungol) && SpellID == 0 && !Enemy.HasAura("Bellowing Rage")))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: {0} don't cast any more", Enemy.Name);
                        WoWMovement.MoveStop();
                        return;
                    }
                    if (Me.Location.Distance(Enemy.Location) > (MinDistToEnemy + 10))
                    {
                        Logging.Write(Colors.MediumPurple, "Rarekiller: reached save Location");
                        WoWMovement.MoveStop();
                        return;
                    }
                }
            }
            Logging.Write(Colors.MediumPurple, "Rarekiller: reached save Location");
            WoWMovement.MoveStop();
        }
        /// <summary>
        /// this behavior will move the bot StrafeRight/StrafeLeft only if enemy is casting and we needed to move!
        /// Credits to BarryDurex.
        /// </summary>
        /// <param name="EnemyAttackRadius">EnemyAttackRadius or 0 for move Behind</param>
        public void AvoidEnemyCast(WoWUnit Unit, float EnemyAttackRadius, float SaveDistance)
        {
            Logging.Write(Colors.MediumPurple, "Rarekiller: Fleeing from {0}", Unit.Name);
            if (!StyxWoW.Me.IsFacing(Unit))
            { Unit.Face(); Thread.Sleep(300); }

            float BehemothRotation = getPositive(Unit.RotationDegrees);
            float invertEnemyRotation = getInvert(BehemothRotation);

            WoWMovement.MovementDirection move = WoWMovement.MovementDirection.None;

            if (getPositive(StyxWoW.Me.RotationDegrees) > invertEnemyRotation)
            { move = WoWMovement.MovementDirection.StrafeRight; }
            else
            { move = WoWMovement.MovementDirection.StrafeLeft; }

            while (Unit.Distance2D <= SaveDistance && Unit.IsCasting && ((EnemyAttackRadius == 0 && !StyxWoW.Me.IsSafelyBehind(Unit)) ||
                (EnemyAttackRadius != 0 && Unit.IsSafelyFacing(StyxWoW.Me, EnemyAttackRadius)) || Unit.Distance2D <= 2))
            {
                WoWMovement.Move(move);
                Unit.Face();
                if (Rarekiller.ToonInvalid) return;
            }
            WoWMovement.MoveStop();
        }
        #endregion

        #region RayCast - Code from Kite Function of Singular
        /// <summary>
        /// Get a Save Location away from a List of AoE Pools
        /// </summary>
        /// <param name="Enemy">The Enemy</param>
        /// <param name="badObjects">A List of AOE Pools</param>
        /// <param name="minDist">Minimum Distance to the AOE Pools</param>
        /// <param name="maxDist">Maximum Distance to the AOE Pools</param>
        /// <param name="TraceStep">How many Checks in one Round</param>
        /// <param name="ScanDistance">Distance of the Scans to each other</param>
        /// <returns>Hopefully a save Point or WoWPoint.Empty</returns>
        private WoWPoint getSaveLocationSingular(WoWUnit Enemy, List<WoWDynamicObject> badObjects, int minDist, int maxDist, int TraceStep, int ScanDistance, string Aura)
        {
            try
            {
                DateTime startFind = DateTime.Now;
                bool checkForEnemysAround = true;
                int countPointsChecked = 0;
                int countFailToPointNav = 0;
                int countFailSafe = 0;
                double furthestNearMobDistSqr = 0f;
                bool ChooseSafestAvailable = true;
                WoWPoint pFurthest = WoWPoint.Empty;
                WoWPoint pMe = Me.GetTraceLinePos();

                //Line of Sight
                int countFailToPointLoS = 0;
                int countFailToMobLoS = 0;
                bool CheckLineOfSight = true;
                bool CheckLineOfSightEnemy = true;

                WoWPoint pSavePoint = new WoWPoint();
                List<WoWPoint> mobLocations = AllEnemyMobLocationsToCheck(Enemy);
                float arcIncrement = ((float)Math.PI * 2) / TraceStep;
                double minSafeDistSqr = 15 * 15;

                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));
               
                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: search near {0:F0}d @ {1:F1} yds for Unit free area", WoWMathHelper.RadiansToDegrees(_PIx2), minDist);
                for (int arcIndex = 0; arcIndex < TraceStep; arcIndex++)
                {
                    float checkFacing = WoWMathHelper.DegreesToRadians((float)new Random().Next(1, 360));

                    for (float distFromOrigin = minDist; distFromOrigin <= maxDist; distFromOrigin += ScanDistance)
                    {
                        countPointsChecked++;
                        if (Rarekiller.ToonInvalid) return Me.Location;
                        if (Aura != "No Aura" && !Me.HasAura(Aura))
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Don't have {0} any more", Aura);
                            WoWMovement.MoveStop();
                            return Me.Location;
                        }

                        pSavePoint = pMe.RayCast(checkFacing, distFromOrigin);
                        pSavePoint.Z = Rarekiller.getGroundZ(pSavePoint);

                        if (pSavePoint.Z == float.MinValue)
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed getGroundZ for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointNav++;
                            continue;
                        }
                        
                        if (Navigator.GeneratePath(StyxWoW.Me.Location, pSavePoint).Length == 0 || StyxWoW.Me.Location.Distance2D(pSavePoint) < 1 || !Navigator.CanNavigateFully(Me.Location, pSavePoint))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed navigation check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointNav++;
                            continue;
                        }

                        if (badObjects.FirstOrDefault(_obj => _obj.Location.Distance2D(pSavePoint) <= minDist) != null)
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed, Bad Object around degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailSafe++;
                            continue;
                        }

                        // Check Line of Sight
                        if (CheckLineOfSight && !Styx.WoWInternals.World.GameWorld.IsInLineOfSight(pMe, pSavePoint))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Failed line of sight check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointLoS++;
                            continue;
                        }

                        if (CheckLineOfSightEnemy && !Styx.WoWInternals.World.GameWorld.IsInLineOfSpellSight(pSavePoint, Enemy.GetTraceLinePos()))
                        {
                            if (!Styx.WoWInternals.World.GameWorld.IsInLineOfSight(pSavePoint, Enemy.GetTraceLinePos()))
                            {
                                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Failed Unit line of sight check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                                countFailToMobLoS++;
                                continue;
                            }
                        }

                        if (checkForEnemysAround)
                        {
                            WoWPoint ptNearest = NearestMobLoc(pSavePoint, mobLocations);
                            if (ptNearest == WoWPoint.Empty)
                            {
                                if (furthestNearMobDistSqr < minSafeDistSqr)
                                {
                                    furthestNearMobDistSqr = minSafeDistSqr;
                                    pFurthest = pSavePoint;     // set best available if others fail
                                }
                            }
                            else
                            {
                                double mobDistSqr = pSavePoint.Distance2DSqr(ptNearest);
                                if (furthestNearMobDistSqr < mobDistSqr)
                                {
                                    furthestNearMobDistSqr = mobDistSqr;
                                    pFurthest = pSavePoint;     // set best available if others fail
                                }
                                if (mobDistSqr <= minSafeDistSqr)
                                {
                                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Safe Location failed, Hostile Mobs around degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                                    countFailSafe++;
                                    continue;
                                }
                            }
                        }

                        Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: Found Unit-free location ({0:F1} yd radius) at degrees={1:F1} dist={2:F1} on point check# {3} at {4}, {5}, {6}", 15, WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin, countPointsChecked, pSavePoint.X, pSavePoint.Y, pSavePoint.Z);
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                        return pSavePoint;
                    }
                }

                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: No Unit-free location ({0:F1} yd radius) found within {1:F1} yds ({2} checked, {3} nav, {4} not safe, LoS {5}, Enemy LoS {6})", 15, maxDist, countPointsChecked, countFailToPointNav, countFailSafe, countFailToPointLoS, countFailToMobLoS);
                if (ChooseSafestAvailable && pFurthest != WoWPoint.Empty)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: choosing best available spot in {0:F1} yd radius where closest Unit is {1:F1} yds", 15, Math.Sqrt(furthestNearMobDistSqr));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                    return ChooseSafestAvailable ? pFurthest : WoWPoint.Empty;
                }

                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                return WoWPoint.Empty;
            }
            catch (Exception ex)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: {0}", ex.Message); }

            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No valid points returned by RayCast ...");
            return WoWPoint.Empty;
        }

        /// <summary>
        /// Get a Save Location away from a List of Units
        /// </summary>
        /// <param name="Enemy">The Enemy</param>
        /// <param name="badUnit">A List of Units</param>
        /// <param name="minDist">Minimum Distance to the Units</param>
        /// <param name="maxDist">Maximum Distance to the Units</param>
        /// <param name="TraceStep">How many Checks in one Round</param>
        /// <param name="ScanDistance">Distance of the Scans to each other</param>
        /// <returns>Hopefully a save Point or WoWPoint.Empty</returns>
        private WoWPoint getSaveLocationSingular(WoWUnit Enemy, List<WoWUnit> badUnit, int minDist, int maxDist, int TraceStep, int ScanDistance)
        {
            try
            {
                bool checkForEnemysAround = true;
                DateTime startFind = DateTime.Now;
                int countPointsChecked = 0;
                int countFailToPointNav = 0;
                int countFailSafe = 0;
                double furthestNearMobDistSqr = 0f;
                bool ChooseSafestAvailable = true;
                WoWPoint pFurthest = WoWPoint.Empty;
                WoWPoint pMe = Me.GetTraceLinePos();

                //Line of Sight
                int countFailToPointLoS = 0;
                int countFailToMobLoS = 0;
                bool CheckLineOfSight = true;
                bool CheckLineOfSightEnemy = true;

                WoWPoint pSavePoint = new WoWPoint();
                List<WoWPoint> mobLocations = AllEnemyMobLocationsToCheck(Enemy);
                float arcIncrement = ((float)Math.PI * 2) / TraceStep;
                double minSafeDistSqr = 15 * 15;

                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));

                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: search near {0:F0}d @ {1:F1} yds for Unit free area", WoWMathHelper.RadiansToDegrees(_PIx2), minDist);
                for (int arcIndex = 0; arcIndex < TraceStep; arcIndex++)
                {
                    float checkFacing = WoWMathHelper.DegreesToRadians((float)new Random().Next(1, 360));

                    for (float distFromOrigin = minDist; distFromOrigin <= maxDist; distFromOrigin += ScanDistance)
                    {
                        countPointsChecked++;
                        if (Rarekiller.ToonInvalid) return Me.Location;
                        WoWUnit Check = badUnit.OrderBy(u => u.Distance).FirstOrDefault();
                        if (Check.Location.Distance(Me.Location) > 20)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller: Far enough away from {0}", Check.Name);
                            WoWMovement.MoveStop();
                            return Me.Location;
                        }

                        pSavePoint = pMe.RayCast(checkFacing, distFromOrigin);
                        pSavePoint.Z = Rarekiller.getGroundZ(pSavePoint);

                        if (pSavePoint.Z == float.MinValue)
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed getGroundZ for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointNav++;
                            continue;
                        }
                        
                        if (Navigator.GeneratePath(StyxWoW.Me.Location, pSavePoint).Length == 0 || StyxWoW.Me.Location.Distance2D(pSavePoint) < 1 || !Navigator.CanNavigateFully(Me.Location, pSavePoint))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed navigation check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointNav++;
                            continue;
                        }

                        if (badUnit.FirstOrDefault(_obj => _obj.Location.Distance2D(pSavePoint) <= minDist) != null)
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed, Bad Unit around degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailSafe++;
                            continue;
                        }

                        // Check Line of Sight
                        if (CheckLineOfSight && !Styx.WoWInternals.World.GameWorld.IsInLineOfSight(pMe, pSavePoint))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Failed line of sight check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointLoS++;
                            continue;
                        }

                        if (CheckLineOfSightEnemy && !Styx.WoWInternals.World.GameWorld.IsInLineOfSpellSight(pSavePoint, Enemy.GetTraceLinePos()))
                        {
                            if (!Styx.WoWInternals.World.GameWorld.IsInLineOfSight(pSavePoint, Enemy.GetTraceLinePos()))
                            {
                                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Failed Unit line of sight check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                                countFailToMobLoS++;
                                continue;
                            }
                        }

                        if (checkForEnemysAround)
                        {
                            WoWPoint ptNearest = NearestMobLoc(pSavePoint, mobLocations);
                            if (ptNearest == WoWPoint.Empty)
                            {
                                if (furthestNearMobDistSqr < minSafeDistSqr)
                                {
                                    furthestNearMobDistSqr = minSafeDistSqr;
                                    pFurthest = pSavePoint;     // set best available if others fail
                                }
                            }
                            else
                            {
                                double mobDistSqr = pSavePoint.Distance2DSqr(ptNearest);
                                if (furthestNearMobDistSqr < mobDistSqr)
                                {
                                    furthestNearMobDistSqr = mobDistSqr;
                                    pFurthest = pSavePoint;     // set best available if others fail
                                }
                                if (mobDistSqr <= minSafeDistSqr)
                                {
                                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Safe Location failed, Hostile Mobs around degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                                    countFailSafe++;
                                    continue;
                                }
                            }
                        }

                        Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: Found Unit-free location ({0:F1} yd radius) at degrees={1:F1} dist={2:F1} on point check# {3} at {4}, {5}, {6}", 15, WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin, countPointsChecked, pSavePoint.X, pSavePoint.Y, pSavePoint.Z);
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                        return pSavePoint;
                    }
                }

                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No Unit-free location ({0:F1} yd radius) found within {1:F1} yds ({2} checked, {3} nav, {4} not safe, LoS {5}, Enemy LoS {6})", 15, maxDist, countPointsChecked, countFailToPointNav, countFailSafe, countFailToPointLoS, countFailToMobLoS);
                if (ChooseSafestAvailable && pFurthest != WoWPoint.Empty)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: choosing best available spot in {0:F1} yd radius where closest Unit is {1:F1} yds", 15, Math.Sqrt(furthestNearMobDistSqr));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                    return ChooseSafestAvailable ? pFurthest : WoWPoint.Empty;
                }

                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                return WoWPoint.Empty;
            }
            catch (Exception ex)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: {0}", ex.Message); }

            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No valid points returned by RayCast ...");
            return WoWPoint.Empty;
        }

        /// <summary>
        /// Get a Save Location away from the Impact Point of the Missiles
        /// </summary>
        /// <param name="Enemy">The Enemy</param>
        /// <param name="Missile">simply use true</param>
        /// <param name="minDist">Minimum Distance to the Impact Point</param>
        /// <param name="maxDist">Maximum Distance to the Impact Point</param>
        /// <param name="TraceStep">How many Checks in one Round</param>
        /// <param name="ScanDistance">Distance of the Scans to each other</param>
        /// <returns>Hopefully a save Point or WoWPoint.Empty</returns>
        private WoWPoint getSaveLocationSingular(WoWUnit Enemy, bool Missile, int minDist, int maxDist, int TraceStep, int ScanDistance, int SpellID)
        {
            try
            {
                bool checkForEnemysAround = true; 
                DateTime startFind = DateTime.Now;
                int countPointsChecked = 0;
                int countFailToPointNav = 0;
                int countFailSafe = 0;
                double furthestNearMobDistSqr = 0f;
                bool ChooseSafestAvailable = true;
                WoWPoint pFurthest = WoWPoint.Empty;
                WoWPoint pMe = Me.GetTraceLinePos();

                //Line of Sight
                int countFailToPointLoS = 0;
                int countFailToMobLoS = 0;
                bool CheckLineOfSight = true;
                bool CheckLineOfSightEnemy = true;
                
                WoWPoint pSavePoint = new WoWPoint();
                List<WoWPoint> mobLocations = AllEnemyMobLocationsToCheck(Enemy);
                float arcIncrement = ((float)Math.PI * 2) / TraceStep;
                double minSafeDistSqr = 15 * 15;

                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));
                
                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: search near {0:F0}d @ {1:F1} yds for Unit free area", WoWMathHelper.RadiansToDegrees(_PIx2), minDist);
                for (int arcIndex = 0; arcIndex < TraceStep; arcIndex++)
                {
                    float checkFacing = WoWMathHelper.DegreesToRadians((float)new Random().Next(1, 360));

                    for (float distFromOrigin = minDist; distFromOrigin <= maxDist; distFromOrigin += ScanDistance)
                    {
                        countPointsChecked++;
                        if (Rarekiller.ToonInvalid) return Me.Location;
                        if (SpellID != 0 && (!Enemy.IsCasting || Enemy.CastingSpellId != SpellID))
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller: {0} don't cast any more", Enemy.Name);
                            WoWMovement.MoveStop();
                            return Me.Location;
                        }

                        pSavePoint = pMe.RayCast(checkFacing, distFromOrigin);
                        pSavePoint.Z = Rarekiller.getGroundZ(pSavePoint);

                        if (pSavePoint.Z == float.MinValue)
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed getGroundZ for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointNav++;
                            continue;
                        }

                        if (Navigator.GeneratePath(StyxWoW.Me.Location, pSavePoint).Length == 0 || StyxWoW.Me.Location.Distance2D(pSavePoint) < 1 || !Navigator.CanNavigateFully(Me.Location, pSavePoint))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Safe Location failed navigation check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointNav++;
                            continue;
                        }

                        if (WoWMissile.AllMissiles.FirstOrDefault(_obj => _obj.ImpactPosition.Distance2D(pSavePoint) <= minDist) != null)
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed, Missile Impact Point around degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailSafe++;
                            continue;
                        }

                        // Check Line of Sight
                        if (CheckLineOfSight && !Styx.WoWInternals.World.GameWorld.IsInLineOfSight(pMe, pSavePoint))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Failed line of sight check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointLoS++;
                            continue;
                        }

                        if (CheckLineOfSightEnemy && !Styx.WoWInternals.World.GameWorld.IsInLineOfSpellSight(pSavePoint, Enemy.GetTraceLinePos()))
                        {
                            if (!Styx.WoWInternals.World.GameWorld.IsInLineOfSight(pSavePoint, Enemy.GetTraceLinePos()))
                            {
                                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Failed Unit line of sight check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                                countFailToMobLoS++;
                                continue;
                            }
                        }

                        if (checkForEnemysAround)
                        {
                            WoWPoint ptNearest = NearestMobLoc(pSavePoint, mobLocations);
                            if (ptNearest == WoWPoint.Empty)
                            {
                                if (furthestNearMobDistSqr < minSafeDistSqr)
                                {
                                    furthestNearMobDistSqr = minSafeDistSqr;
                                    pFurthest = pSavePoint;     // set best available if others fail
                                }
                            }
                            else
                            {
                                double mobDistSqr = pSavePoint.Distance2DSqr(ptNearest);
                                if (furthestNearMobDistSqr < mobDistSqr)
                                {
                                    furthestNearMobDistSqr = mobDistSqr;
                                    pFurthest = pSavePoint;     // set best available if others fail
                                }
                                if (mobDistSqr <= minSafeDistSqr)
                                {
                                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Safe Location failed, Hostile Mobs around degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                                    countFailSafe++;
                                    continue;
                                }
                            }
                        }

                        Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: Found Unit-free location ({0:F1} yd radius) at degrees={1:F1} dist={2:F1} on point check# {3} at {4}, {5}, {6}", 15, WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin, countPointsChecked, pSavePoint.X, pSavePoint.Y, pSavePoint.Z);
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                        return pSavePoint;
                    }
                }

                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No Unit-free location ({0:F1} yd radius) found within {1:F1} yds ({2} checked, {3} nav, {4} not safe, LoS {5}, Enemy LoS {6})", 15, maxDist, countPointsChecked, countFailToPointNav, countFailSafe, countFailToPointLoS, countFailToMobLoS);
                if (ChooseSafestAvailable && pFurthest != WoWPoint.Empty)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: choosing best available spot in {0:F1} yd radius where closest Unit is {1:F1} yds", 15, Math.Sqrt(furthestNearMobDistSqr));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                    return ChooseSafestAvailable ? pFurthest : WoWPoint.Empty;
                }

                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                return WoWPoint.Empty;
            }
            catch (Exception ex)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: {0}", ex.Message); }

            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No valid points returned by RayCast ...");
            return WoWPoint.Empty;
        }

        /// <summary>
        /// Get a Save Location away from the Enemy
        /// </summary>
        /// <param name="Enemy">The Enemy</param>
        /// <param name="minDist">Minimum Distance to the Enemy</param>
        /// <param name="maxDist">Maximum Distance to the Enemy</param>
        /// <param name="TraceStep">How many Checks in one Round</param>
        /// <param name="ScanDistance">Distance of the Scans to each other</param>
        /// <returns>Hopefully a save Point or WoWPoint.Empty</returns>
        public WoWPoint getSaveFleeingLocationSingular(WoWUnit Enemy, int minDist, int maxDist, int TraceStep, int ScanDistance, int SpellID)
        {
            try
            {
                bool checkForEnemysAround = true;
                DateTime startFind = DateTime.Now;
                int countPointsChecked = 0;
                int countFailToPointNav = 0;
                int countFailSafe = 0;
                double furthestNearMobDistSqr = 0f;
                bool ChooseSafestAvailable = true;
                WoWPoint pFurthest = WoWPoint.Empty;
                WoWPoint pEnemy = Enemy.GetTraceLinePos();

                //Line of Sight
                int countFailToPointLoS = 0;
                int countFailToMobLoS = 0;
                bool CheckLineOfSight = true;
                bool CheckLineOfSightEnemy = true;

                WoWPoint pSavePoint = new WoWPoint();
                List<WoWPoint> mobLocations = AllEnemyMobLocationsToCheck(Enemy);
                float arcIncrement = ((float)Math.PI * 2) / TraceStep;
                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));
                double minSafeDistSqr = 15 * 15;

                float baseDestinationFacing = Enemy == null ?
                                                Me.RenderFacing + (float)Math.PI
                                                : Styx.Helpers.WoWMathHelper.CalculateNeededFacing(Enemy.Location, Me.Location);

                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: search near {0:F0}d @ {1:F1} yds for Unit free area", WoWMathHelper.RadiansToDegrees(baseDestinationFacing), minDist);
                for (int arcIndex = 0; arcIndex < TraceStep; arcIndex++)
                {
                    float checkFacing = WoWMathHelper.DegreesToRadians((float)new Random().Next(1, 360));
                    if (Enemy.Entry == 50821 || Enemy.Entry == 50817 || Enemy.Entry == 50822 || Enemy.Entry == 50816 ||
                            Enemy.Entry == 50811 || Enemy.Entry == 50808 || Enemy.Entry == 50820)
                    {
                        checkFacing = baseDestinationFacing;
                        if ((arcIndex & 1) == 0)
                            checkFacing += arcIncrement * (arcIndex >> 1);
                        else
                            checkFacing -= arcIncrement * ((arcIndex >> 1) + 1);
                    }

                    for (float distFromOrigin = minDist; distFromOrigin <= maxDist; distFromOrigin += ScanDistance)
                    {
                        countPointsChecked++;
                        if (Rarekiller.ToonInvalid) return Me.Location;
                        if ((SpellID != 0 && (!Enemy.IsCasting || Enemy.CastingSpellId != SpellID)) || ((Enemy == Yaungol) && SpellID == 0 && !Enemy.HasAura("Bellowing Rage")))
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller: {0} don't cast any more", Enemy.Name);
                            WoWMovement.MoveStop();
                            return Me.Location;
                        }

                        pSavePoint = pEnemy.RayCast(checkFacing, distFromOrigin);
                        pSavePoint.Z = Rarekiller.getGroundZ(pSavePoint);

                        if (pSavePoint.Z == float.MinValue)
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed getGroundZ for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointNav++;
                            continue;
                        }

                        if (Navigator.GeneratePath(StyxWoW.Me.Location, pSavePoint).Length == 0 || StyxWoW.Me.Location.Distance2D(pSavePoint) < 1 || !Navigator.CanNavigateFully(Me.Location, pSavePoint))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Location failed navigation check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointNav++;
                            continue;
                        }

                        // Check Line of Sight
                        if (CheckLineOfSight && !Styx.WoWInternals.World.GameWorld.IsInLineOfSight(Me.GetTraceLinePos(), pSavePoint))
                        {
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Failed line of sight check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                            countFailToPointLoS++;
                            continue;
                        }

                        if (CheckLineOfSightEnemy && !Styx.WoWInternals.World.GameWorld.IsInLineOfSpellSight(pSavePoint, Enemy.GetTraceLinePos()))
                        {
                            if (!Styx.WoWInternals.World.GameWorld.IsInLineOfSight(pSavePoint, Enemy.GetTraceLinePos()))
                            {
                                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Failed Unit line of sight check for degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                                countFailToMobLoS++;
                                continue;
                            }
                        }

                        if (checkForEnemysAround)
                        {
                            WoWPoint ptNearest = NearestMobLoc(pSavePoint, mobLocations);
                            if (ptNearest == WoWPoint.Empty)
                            {
                                if (furthestNearMobDistSqr < minSafeDistSqr)
                                {
                                    furthestNearMobDistSqr = minSafeDistSqr;
                                    pFurthest = pSavePoint;     // set best available if others fail
                                }
                            }
                            else
                            {
                                double mobDistSqr = pSavePoint.Distance2DSqr(ptNearest);
                                if (furthestNearMobDistSqr < mobDistSqr)
                                {
                                    furthestNearMobDistSqr = mobDistSqr;
                                    pFurthest = pSavePoint;     // set best available if others fail
                                }
                                if (mobDistSqr <= minSafeDistSqr)
                                {
                                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Safe Location failed, Hostile Mobs around degrees={0:F1} dist={1:F1}", WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin);
                                    countFailSafe++;
                                    continue;
                                }
                            }
                        }

                        Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: Found Unit-free location ({0:F1} yd radius) at degrees={1:F1} dist={2:F1} on point check# {3} at {4}, {5}, {6}", 15, WoWMathHelper.RadiansToDegrees(checkFacing), distFromOrigin, countPointsChecked, pSavePoint.X, pSavePoint.Y, pSavePoint.Z);
                        Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                        return pSavePoint;
                    }
                }

                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No Unit-free location ({0:F1} yd radius) found within {1:F1} yds ({2} checked, {3} nav, {4} not safe, LoS {5}, Enemy LoS {6})", 15, maxDist, countPointsChecked, countFailToPointNav, countFailSafe, countFailToPointLoS, countFailToMobLoS);
                if (ChooseSafestAvailable && pFurthest != WoWPoint.Empty)
                {
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: choosing best available spot in {0:F1} yd radius where closest Unit is {1:F1} yds", 15, Math.Sqrt(furthestNearMobDistSqr));
                    Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                    return ChooseSafestAvailable ? pFurthest : WoWPoint.Empty;
                }

                Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                return WoWPoint.Empty;
            }
            catch (Exception ex)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: {0}", ex.Message); }


            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No valid points returned by RayCast ...");
            return WoWPoint.Empty;

        }
        #endregion

        #region Raycast - Code from MoP Daylies
        /// <summary>
        /// Get a Save Location away from a List of AoE Pools
        /// </summary>
        /// <param name="Location">My Location</param>
        /// <param name="badObjects">A List of AOE Pools</param>
        /// <param name="minDist">Minimum Distance to the AOE Pools</param>
        /// <param name="maxDist">Maximum Distance to the AOE Pools</param>
        /// <param name="traceStep">How many Checks in one Round</param>
        /// <returns>Hopefully a save Point or WoWPoint.Empty</returns>
        private WoWPoint getSaveLocationBrodie(WoWPoint Location, List<WoWDynamicObject> badObjects, int minDist, int maxDist, int traceStep)
        {
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Looking for save Location around {0}.", Location);
            DateTime startFind = DateTime.Now;
            int countPointsChecked = 0;

            try
            {
                //float _PIx2 = 3.14159f * 2f;
                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));
                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: search for Unit free area");
                
                for (int i = 0, x = minDist; i < traceStep && x < maxDist; i++)
                {
                    countPointsChecked++;
                    WoWPoint p = Location.RayCast((i * _PIx2) / traceStep, x);

                    p.Z = Rarekiller.getGroundZ(p);

                    if (p.Z != float.MinValue && StyxWoW.Me.Location.Distance2D(p) > 1 &&
                        (badObjects.FirstOrDefault(_obj => _obj.Location.Distance2D(p) <= minDist) == null) &&
                        //(ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Location.Distance2D(p) < 20 && u.IsAlive && !u.Combat) == null) &&
                        (Navigator.GeneratePath(StyxWoW.Me.Location, p).Length != 0 && StyxWoW.Me.Location.Distance2D(p) > 1 && Navigator.CanNavigateFully(Me.Location, p))
                        )
                    {
                        if (getHighestSurroundingSlope(p) < 1.2f)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: Moving to {0}. Distance: {1}", p, Location.Distance(p));
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Checked {0} Points", countPointsChecked);
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                            return p;
                        }
                    }

                    if (i == (traceStep - 1))
                    {
                        i = 0;
                        x++;
                    }
                }
            }
            catch (Exception ex)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: {0}", ex.Message); }


            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No valid points returned by RayCast ...");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Checked {0} Points", countPointsChecked);
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
            return WoWPoint.Empty;

        }

        /// <summary>
        /// Get a Save Location away from a List of Units
        /// </summary>
        /// <param name="Location">My Location</param>
        /// <param name="badUnit">A List of Units</param>
        /// <param name="minDist">Minimum Distance to the Units</param>
        /// <param name="maxDist">Maximum Distance to the Units</param>
        /// <param name="traceStep">How many Checks in one Round</param>
        /// <returns>Hopefully a save Point or WoWPoint.Empty</returns>
        private WoWPoint getSaveLocationBrodie(WoWPoint Location, List<WoWUnit> badUnit, int minDist, int maxDist, int traceStep)
        {
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Looking for save Location around {0}.", Location);
            DateTime startFind = DateTime.Now;
            int countPointsChecked = 0;

            try
            {
                //float _PIx2 = 3.14159f * 2f;
                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));
                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: search for Unit free area");
                
                for (int i = 0, x = minDist; i < traceStep && x < maxDist; i++)
                {
                    countPointsChecked++;
                    WoWPoint p = Location.RayCast((i * _PIx2) / traceStep, x);

                    p.Z = Rarekiller.getGroundZ(p);

                    if (p.Z != float.MinValue && StyxWoW.Me.Location.Distance2D(p) > 1 &&
                        (badUnit.FirstOrDefault(_obj => _obj.Location.Distance2D(p) <= minDist) == null) &&
                        //(ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Location.Distance2D(p) < 20 && u.IsAlive && !u.Combat) == null) &&
                        (Navigator.GeneratePath(StyxWoW.Me.Location, p).Length != 0 && StyxWoW.Me.Location.Distance2D(p) > 1 && Navigator.CanNavigateFully(Me.Location, p))
                        )
                    {
                        if (getHighestSurroundingSlope(p) < 1.2f)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: Moving to {0}. Distance: {1}", p, Location.Distance(p));
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Checked {0} Points", countPointsChecked);
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                            return p;
                        }
                    }

                    if (i == (traceStep - 1))
                    {
                        i = 0;
                        x++;
                    }
                }
            }
            catch (Exception ex)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: {0}", ex.Message); }


            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No valid points returned by RayCast ...");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Checked {0} Points", countPointsChecked);
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
            return WoWPoint.Empty;

        }

        /// <summary>
        /// Get a Save Location away from the Impact Point of the Missiles
        /// </summary>
        /// <param name="Location">My Location</param>
        /// <param name="Missile">simply use true</param>
        /// <param name="minDist">Minimum Distance to the Impact Point</param>
        /// <param name="maxDist">Maximum Distance to the Impact Point</param>
        /// <param name="traceStep">How many Checks in one Round</param>
        /// <returns>Hopefully a save Point or WoWPoint.Empty</returns>
        private WoWPoint getSaveLocationBrodie(WoWPoint Location, bool Missile, int minDist, int maxDist, int traceStep)
        {
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Looking for save Location around {0}.", Location);
            DateTime startFind = DateTime.Now;
            int countPointsChecked = 0;

            try
            {
                //float _PIx2 = 3.14159f * 2f;
                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));
                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: search for Unit free area");
                
                for (int i = 0, x = minDist; i < traceStep && x < maxDist; i++)
                {
                    countPointsChecked++;
                    WoWPoint p = Location.RayCast((i * _PIx2) / traceStep, x);

                    p.Z = Rarekiller.getGroundZ(p);

                    if (p.Z != float.MinValue && StyxWoW.Me.Location.Distance2D(p) > 1 &&
                        (WoWMissile.AllMissiles.FirstOrDefault(_obj => _obj.ImpactPosition.Distance2D(p) <= minDist) == null) &&
                        //(ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Location.Distance2D(p) < 20 && u.IsAlive && !u.Combat) == null) &&
                        (Navigator.GeneratePath(StyxWoW.Me.Location, p).Length != 0 && StyxWoW.Me.Location.Distance2D(p) > 1 && Navigator.CanNavigateFully(Me.Location, p))
                        )
                    {
                        if (getHighestSurroundingSlope(p) < 1.2f)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: Moving to {0}. Distance: {1}", p, Location.Distance(p));
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Checked {0} Points", countPointsChecked);
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                            return p;
                        }
                    }

                    if (i == (traceStep - 1))
                    {
                        i = 0;
                        x++;
                    }
                }
            }
            catch (Exception ex)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: {0}", ex.Message); }


            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No valid points returned by RayCast ...");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Checked {0} Points", countPointsChecked);
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
            return WoWPoint.Empty;

        }

        /// <summary>
        /// Get a Save Location away from the Enemy
        /// </summary>
        /// <param name="Location">My Location</param>
        /// <param name="Enemy">The Enemy</param>
        /// <param name="minDist">Minimum Distance to the Enemy</param>
        /// <param name="maxDist">Maximum Distance to the Enemy</param>
        /// <param name="traceStep">How many Checks in one Round</param>
        /// <returns>Hopefully a save Point or WoWPoint.Empty</returns>
        private WoWPoint getSaveFleeingLocationBrodie(WoWPoint Location, WoWUnit Enemy, int minDist, int maxDist, int traceStep)
        {
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Looking for save Location around {0}.", Location);
            DateTime startFind = DateTime.Now;
            int countPointsChecked = 0;

            try
            {
                float _PIx2 = ((float)new Random().Next(1, 80) * (1.248349f + (float)new Random().NextDouble()));
                //float _PIx2 = 0;
                Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: search for Unit free area");
                
                for (int i = 0, x = minDist; i < traceStep && x < maxDist; i++)
                {
                    countPointsChecked++;
                    WoWPoint p = Enemy.Location.RayCast((i * _PIx2) / traceStep, x);

                    p.Z = Rarekiller.getGroundZ(p);

                    if (p.Z != float.MinValue && StyxWoW.Me.Location.Distance2D(p) > 1 &&
                        (Enemy.Location.Distance2D(p) >= minDist) &&
                        //(ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Location.Distance2D(p) < 20 && u.IsAlive && !u.Combat) == null) &&
                        (Navigator.GeneratePath(StyxWoW.Me.Location, p).Length != 0 && StyxWoW.Me.Location.Distance2D(p) > 1 && Navigator.CanNavigateFully(Me.Location, p))
                        )
                    {
                        if (getHighestSurroundingSlope(p) < 1.2f)
                        {
                            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: Moving to {0}. Distance: {1}", p, Location.Distance(p));
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Checked {0} Points", countPointsChecked);
                            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
                            return p;
                        }
                    }

                    if (i == (traceStep - 1))
                    {
                        i = 0;
                        x++;
                    }
                }
            }
            catch (Exception ex)
            { Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: {0}", ex.Message); }


            Logging.Write(Colors.MediumPurple, "Rarekiller RayCast: No valid points returned by RayCast ...");
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: Checked {0} Points", countPointsChecked);
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller RayCast: processing took {0:F0} ms", (DateTime.Now - startFind).TotalMilliseconds);
            return WoWPoint.Empty;

        }
        #endregion

        #region MoP Rares Helper Functions
        /// <summary>
        /// Returns a List of all Enemys around me exept the Enemy
        /// </summary>
        public List<WoWPoint> AllEnemyMobLocationsToCheck(WoWUnit Enemy)
        {
            return (from u in AllEnemyMobs
                        where u != Enemy && u.Distance2DSqr < (65 * 65) && !u.Combat
                        select u.Location).ToList();
        }

        /// <summary>
        /// Returns a List of all alive and hostile Enemys around me
        /// </summary>
        public static IEnumerable<WoWUnit> AllEnemyMobs
        {
            get
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => !o.IsDead && o.IsHostile && !o.Combat).OrderBy(u => u.Distance).ToList();

            }
        }

        /// <summary>
        /// Returns the Nearest Unit Location to a Point
        /// </summary>
        public static WoWPoint NearestMobLoc(WoWPoint p, IEnumerable<WoWPoint> mobLocs)
        {
            if (!mobLocs.Any())
                return WoWPoint.Empty;

            return mobLocs.OrderBy(u => u.Distance2DSqr(p)).First();
        }

        /// <summary>
        /// Credits to funkescott.
        /// </summary>
        /// <returns>Highest slope of surrounding terrain, returns 100 if the slope can't be determined</returns>
        private float getHighestSurroundingSlope(WoWPoint p)
        {
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller: Sloapcheck on Point: {0}", p);
            float _PIx2 = 3.14159f * 2f;
            float highestSlope = -100;
            float slope = 0;
            int traceStep = 15;
            float range = 0.5f;
            WoWPoint p2;
            for (int i = 0; i < traceStep; i++)
            {
                p2 = p.RayCast((i * _PIx2) / traceStep, range);
                p2.Z = Rarekiller.getGroundZ(p2);
                slope = Math.Abs(getSlope(p, p2));
                if (slope > highestSlope)
                {
                    highestSlope = (float)slope;
                }
            }
            Logging.WriteDiagnostic(Colors.MediumPurple, "Rarekiller Sloapcheck: Highslope {0}", highestSlope);
            return Math.Abs(highestSlope);
        }

        /// <summary>
        /// Credits to funkescott.
        /// </summary>
        /// <param name="p1">from WoWPoint</param>
        /// <param name="p2">to WoWPoint</param>
        /// <returns>Return slope from WoWPoint to WoWPoint.</returns>
        private float getSlope(WoWPoint p1, WoWPoint p2)
        {
            float rise = p2.Z - p1.Z;
            float run = (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

            return rise / run;
        }

        /// <summary>
        /// If a number(float) is smaller then 180 add 180 to her, else minus 180 (for circle calculation)
        /// </summary>
        private float getInvert(float f)
        {
            if (f < 180)
                return (f + 180);
            //else if (f >= 180)
            return (f - 180);
        }

        /// <summary>
        /// If a number(float) is smaller then 0 add 360 to her (for circle calculation)
        /// </summary>
        private float getPositive(float f)
        {
            if (f < 0)
                return (f + 360);
            return f;
        }
        #endregion
    }
}
