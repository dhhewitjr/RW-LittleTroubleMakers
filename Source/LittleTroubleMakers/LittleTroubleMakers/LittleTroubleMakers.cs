using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;
using RimWorld.Planet;

namespace LittleTroubleMakers
{
    public class LittleTroubleMakers
    {
        [StaticConstructorOnStartup]
        static class TroubleMakers
        {
            public static double m_fBaseRoll     = 1.0 / 500_000;
            public static double m_fRaidRoll     = 1.0 / 2;
            public static double m_fGoodWillRoll = 1.0 / 2;
            public static int m_nGoodWillRange   = 10;

            /*
             * Assume 50t/s @ 1x Speed
             * List<Faction> factions = (List<Faction>)Find.FactionManager.AllFactions;
             * D:\Steam Games\steamapps\common\RimWorld
             */
            static TroubleMakers()
            {
                Log.Message("Teemo my Beemo");
                var harmony = new Harmony("stw.troublemakers");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            [HarmonyPatch]
            static class JobDriver_Radiotalking_Patch
            {
                [HarmonyTargetMethod]
                static MethodBase TargetForPatch() =>
                    AccessTools.GetDeclaredMethods(typeof(JobDriver_Radiotalking)).Find(mi => 
                        mi.HasAttribute<CompilerGeneratedAttribute>() && mi.ReturnType == typeof(void));

                [HarmonyPostfix]
                static void Postfix()
                {
                    Random rng = new Random();
                    int eventRoll = rng.Next(100);                    

                    if (eventRoll <= 100 * TroubleMakers.m_fBaseRoll)
                    {
                        HandleEvent(rng.Next(100), ref rng);
                    }
                }

                static void HandleEvent(int typeRoll, ref Random rng)
                {
                    if (typeRoll < (int)100 * TroubleMakers.m_fRaidRoll)
                    {
                        SummonRaidWithMessage(ref rng);
                    }
                    else
                    {
                        AdjustGoodWill(rng.Next(0, 2) == 0, ref rng);
                    }
                }

                static void SummonRaidWithMessage(ref Random rng)
                {
                    List<Faction> factions = (List<Faction>)Find.FactionManager.AllFactions;
                    
                    int factionIndx = rng.Next(factions.Count);
                    while (!factions[factionIndx].AllyOrNeutralTo(Faction.OfPlayer))
                    {
                        factionIndx = rng.Next(factions.Count);
                    }

                    Faction raidingFaction = factions[factionIndx];

                    IncidentWorker_RaidEnemy raidIncident = new IncidentWorker_RaidEnemy();
                    Map currentMap = Find.CurrentMap;

                    if (currentMap == null) return;
                    IncidentParms raidParams = StorytellerUtility.DefaultParmsNow(
                        incCat: IncidentCategoryDefOf.ThreatBig,
                        target: currentMap
                        );

                    raidParams.forced = true;
                    raidParams.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
                    raidParams.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                    raidParams.customLetterText =
                            "Little Timmy prank called " + raidingFaction.Name + "'s leader\n" +
                            raidingFaction.leader.Name.ToStringFull + "'s replied with \"talk shit, get hit\"" +
                            "and is sending a raid!";
                    raidParams.faction = raidingFaction;
                    raidParams.points = StorytellerUtility.DefaultThreatPointsNow(currentMap) * 0.75f;

                    Log.Message("What is Null? --> raidParams " + (raidParams == null).ToString());
                    Log.Message("What is Null? --> raidIncident " + (raidIncident == null).ToString());

                    try
                    {
                        Find.Storyteller.incidentQueue.Add(
                            IncidentDefOf.RaidEnemy,
                            Find.TickManager.TicksGame + 1000,
                            raidParams
                            );
                    }
                    catch (Exception)
                    { }
                }

                static void AdjustGoodWill(bool positive, ref Random rng)
                {
                    int goodWillDelta = (positive ? 1 : -1) * rng.Next(1, m_nGoodWillRange + 1);
                    List<Faction> factions = (List<Faction>)Find.FactionManager.AllFactions;
                    
                    int factionIndx = rng.Next(factions.Count);
                    while (!factions[factionIndx].CanChangeGoodwillFor(Faction.OfPlayer, goodWillDelta))
                    {
                        factionIndx = rng.Next(factions.Count);
                    }
                    Faction targetGoodWillChangeFaction = factions[factionIndx];

                    targetGoodWillChangeFaction.ChangeGoodwill_Debug(Faction.OfPlayer, goodWillDelta);
                    StandardLetter notifLetter = new StandardLetter();
                    notifLetter.Label = "Radio Talking";
                    notifLetter.Text = "A Child has prank called " + targetGoodWillChangeFaction.Name + "!\n";
                    notifLetter.Text += positive ?
                        // Positive Message
                        "Luckily, they found it cute and increased their GoodWill by " + goodWillDelta :
                        // Negative Message
                        "They did not find it amusing... GoodWill has decreased by " + -1 * goodWillDelta;
                    Find.LetterStack.ReceiveLetter(
                        "Prank Call!",
                        "A Child has prank called " + targetGoodWillChangeFaction.Name + "!\n" + (positive ?
                            "Luckily, they found it cute and increased their GoodWill by " + goodWillDelta :
                            "They did not find it amusing... GoodWill has decreased by " + -1 * goodWillDelta),
                        positive ? LetterDefOf.PositiveEvent : LetterDefOf.NegativeEvent,
                        new GlobalTargetInfo()
                        );
                }
            }
        }
    }
}

