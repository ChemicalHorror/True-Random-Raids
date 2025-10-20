using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Random = UnityEngine.Random;

namespace TrueRandomRaids {

    [StaticConstructorOnStartup]
    public static class TRI {
        public static RaidStrategyDef finalStrategy;
        private static readonly Dictionary<string, RaidStrategyDef> strategyCache;
        private static readonly Dictionary<string, PawnsArrivalModeDef> arrivalModeCache;

        static TRI() {
            Harmony harmony = new Harmony("ISee.TrueRandomIncidents");
            foreach (Type allSubclass in GenTypes.AllSubclasses(typeof(IncidentWorker))) {
                MethodInfo method = allSubclass.GetMethod("TryExecuteWorker", BindingFlags.Instance | BindingFlags.NonPublic);
                if (method != (MethodInfo)null && method.DeclaringType == allSubclass) {
                    harmony.Patch((MethodBase)method, new HarmonyMethod(typeof(TRI), "ApplyThreatPoints", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
                    harmony.Patch((MethodBase)method, new HarmonyMethod(typeof(TRI), "ModifyRaidEnemy", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
                    harmony.Patch((MethodBase)method, (HarmonyMethod)null, new HarmonyMethod(typeof(TRI), "DummyMethod", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
                }
            }
            TRI.strategyCache = new Dictionary<string, RaidStrategyDef>()
            {
      {
        "ImmediateAttack",
        DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttack", true)
      },
      {
        "ImmediateAttackSmart",
        DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttackSmart", true)
      },
      {
        "StageThenAttack",
        DefDatabase<RaidStrategyDef>.GetNamed("StageThenAttack", true)
      },
      {
        "EmergeFromWater",
        DefDatabase<RaidStrategyDef>.GetNamed("EmergeFromWater", true)
      },
      {
        "ImmediateAttackBreaching",
        DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttackBreaching", true)
      },
      {
        "ImmediateAttackBreachingSmart",
        DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttackBreachingSmart", true)
      },
      {
        "ImmediateAttackSappers",
        DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttackSappers", true)
      },
      {
        "Siege",
        DefDatabase<RaidStrategyDef>.GetNamed("Siege", true)
      }
    };
            TRI.arrivalModeCache = new Dictionary<string, PawnsArrivalModeDef>()
            {
      {
        "EdgeDrop",
        DefDatabase<PawnsArrivalModeDef>.GetNamed("EdgeDrop", true)
      },
      {
        "EdgeWalkIn",
        DefDatabase<PawnsArrivalModeDef>.GetNamed("EdgeWalkIn", true)
      },
      {
        "CenterDrop",
        DefDatabase<PawnsArrivalModeDef>.GetNamed("CenterDrop", true)
      },
      {
        "RandomDrop",
        DefDatabase<PawnsArrivalModeDef>.GetNamed("RandomDrop", true)
      },
      {
        "EdgeDropGroups",
        DefDatabase<PawnsArrivalModeDef>.GetNamed("EdgeDropGroups", true)
      },
      {
        "EdgeWalkInGroups",
        DefDatabase<PawnsArrivalModeDef>.GetNamed("EdgeWalkInGroups", true)
      },
      {
        "EmergeFromWater",
        DefDatabase<PawnsArrivalModeDef>.GetNamed("EmergeFromWater", true)
      }
    };
        }

        private static void ApplyThreatPoints(IncidentWorker __instance, ref IncidentParms parms) {
            TrueRandomRaidsSettings settings = LoadedModManager.GetMod<TrueRandomRaidsMod>().GetSettings<TrueRandomRaidsSettings>();
            if (settings.isThreat)
                return;
            settings.isThreat = true;
            if (settings.EnableRandomRaidPoints || settings.EnableVanillaRandomRaidPoints) {
                if (settings.EnableRandomIncidentPoints) {
                    Log.Message($"[TrueRandomRaids] \"Random Incident Points\" enabled. Modifying threat points for all viable incidents; selected incident: {__instance.def}.");
                    TRI.ApplyThreatPointsHelperMethod(ref parms);
                }
                else {
                    if (settings.EnableRandomIncidentPoints)
                        return;
                    Log.Message($"[TrueRandomRaids] \"Random Incident Points\" disabled. Modifying threat points only for incidents of type: RaidEnemy; selected incident: {__instance.def}");
                    if (((Def)__instance.def).defName == "RaidEnemy")
                        TRI.ApplyThreatPointsHelperMethod(ref parms);
                    else if (((Def)__instance.def).defName != "RaidEnemy")
                        Log.Message($"[TrueRandomRaids] Using default threat points: {parms.points}");
                }
            }
            else {
                if (settings.EnableRandomRaidPoints || settings.EnableVanillaRandomRaidPoints)
                    return;
                Log.Message($"[TrueRandomRaids] Both threat points methods are disabled. Incidents: {__instance.def}.");
                Log.Message($"[TrueRandomRaids] Using default threat points: {parms.points}");
            }
        }

        private static void ModifyRaidEnemy(IncidentWorker __instance, ref IncidentParms parms) {
            TrueRandomRaidsSettings settings = LoadedModManager.GetMod<TrueRandomRaidsMod>().GetSettings<TrueRandomRaidsSettings>();
            if (settings.isRaid)
                return;
            settings.isRaid = true;
            if (((Def)__instance.def).defName == "RaidEnemy") {
                if (settings.EnableRandomRaids) {
                    Log.Message("[TrueRandomRaids] \"True Random Raids\" enabled. Using modified raid strategy and arrival mode.");
                    TRI.ModifyRaidEnemyHelperMethod(__instance, ref parms);
                }
                else {
                    if (settings.EnableRandomRaids)
                        return;
                    Log.Message("[TrueRandomRaids] \"True Random Raids\" disabled. Using default raid strategy and arrival mode.");
                }
            }
            else if (!(((Def)__instance.def).defName != "RaidEnemy"))
                ;
        }

        public static void DummyMethod() {
            TrueRandomRaidsSettings settings = LoadedModManager.GetMod<TrueRandomRaidsMod>().GetSettings<TrueRandomRaidsSettings>();
            settings.isThreat = false;
            settings.isRaid = false;
        }

        private static void ModifyRaidEnemyHelperMethod(
          IncidentWorker __instance,
          ref IncidentParms parms) {
            TrueRandomRaidsSettings settings = LoadedModManager.GetMod<TrueRandomRaidsMod>().GetSettings<TrueRandomRaidsSettings>();
            if (!(((Def)__instance.def).defName == "RaidEnemy"))
                return;
            parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
            Log.Message("[TrueRandomRaids] Initially selected faction: " + parms.faction.Name);
            // declare with named tuple elements
            List<(RaidStrategyDef strategy, float weight)> source1 = new List<(RaidStrategyDef strategy, float weight)>();

            if (settings.ImmediateAttack_Chance > 0.0)
                source1.Add((TRI.strategyCache["ImmediateAttack"], settings.ImmediateAttack_Chance));

            if (settings.ImmediateAttackSmart_Chance > 0.0)
                source1.Add((TRI.strategyCache["ImmediateAttackSmart"], settings.ImmediateAttackSmart_Chance));

            if (settings.StageThenAttack_Chance > 0.0)
                source1.Add((TRI.strategyCache["StageThenAttack"], settings.StageThenAttack_Chance));

            if (settings.EmergeFromWater_Chance > 0.0)
                source1.Add((TRI.strategyCache["EmergeFromWater"], settings.EmergeFromWater_Chance));

            if (settings.ImmediateAttackBreaching_Chance > 0.0)
                source1.Add((TRI.strategyCache["ImmediateAttackBreaching"], settings.ImmediateAttackBreaching_Chance));

            if (settings.ImmediateAttackBreachingSmart_Chance > 0.0)
                source1.Add((TRI.strategyCache["ImmediateAttackBreachingSmart"], settings.ImmediateAttackBreachingSmart_Chance));

            if (settings.ImmediateAttackSappers_Chance > 0.0)
                source1.Add((TRI.strategyCache["ImmediateAttackSappers"], settings.ImmediateAttackSappers_Chance));

            if (settings.Siege_Chance > 0.0)
                source1.Add((TRI.strategyCache["Siege"], settings.Siege_Chance));

            if (parms.points <= 700.0) {
                Log.Message($"[TrueRandomRaids] Rolled less than 700 threat points ({parms.points}), restricting strategies to \"Immediate Attack\" and \"Stage Then Attack\".");

                // filtering now works since tuple has named element "strategy"
                source1 = source1
                    .Where(a => a.strategy == TRI.strategyCache["ImmediateAttack"]
                             || a.strategy == TRI.strategyCache["StageThenAttack"])
                    .ToList();

                if (source1.Count == 0) {
                    Log.Message("[TrueRandomRaids] No valid raid strategies left, forcing default strategy \"Immediate Attack\".");
                    source1.Add((TRI.strategyCache["ImmediateAttack"], 1f));
                }
            }
            int num1 = 0;
            foreach ((RaidStrategyDef _, float num2) in source1)
                num1 += Mathf.RoundToInt(num2 * 100f);
            Log.Message($"[TrueRandomRaids] Max value based on selected strategies: {num1}");
            int num3 = Random.Range(0, num1);
            int num4 = 0;
            RaidStrategyDef raidStrategyDef1 = (RaidStrategyDef)null;
            foreach ((RaidStrategyDef raidStrategyDef2, float num5) in source1) {
                num4 += Mathf.RoundToInt(num5 * 100f);
                Log.Message($"[TrueRandomRaids] Checking strategy: {((Def)raidStrategyDef2).defName} with cumulative chance: {num4}");
                if (num3 < num4) {
                    raidStrategyDef1 = raidStrategyDef2;
                    TRI.finalStrategy = raidStrategyDef1;
                    Log.Message($"[TrueRandomRaids] Selected strategy: {((Def)raidStrategyDef1).defName} (random value: {num3}, threshold: {num4})");
                    break;
                }
            }
            if (raidStrategyDef1 != null) {
                parms.raidStrategy = raidStrategyDef1;
            }
            else {
                parms.raidStrategy = TRI.finalStrategy;
                Log.Message($"[TrueRandomRaids] No valid raid strategies left, forcing stored strategy {((Def)parms.raidStrategy).defName}.");
            }
            if (((Def)parms.raidStrategy).defName == "ImmediateAttack" || ((Def)parms.raidStrategy).defName == "ImmediateAttackSmart") {
                if (((Def)parms.faction.def).defName == "Mechanoid") {
                    Log.Message($"[TrueRandomRaids] Mechanoids cannot use {((Def)parms.raidStrategy).defName}. Rerolling faction...");
                    for (int index = 0; ((Def)parms.faction.def).defName == "Mechanoid" && index < 100; ++index)
                        parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
                    if (((Def)parms.faction.def).defName == "Mechanoid")
                        Log.Warning("[TrueRandomRaids] Failed to find a valid faction after 100 attempts! Keeping Mechanoids and probably skipping a raid.");
                    else
                        Log.Message($"[TrueRandomRaids] Selected faction: {parms.faction.Name}.");
                }
            }
            else if (((Def)parms.raidStrategy).defName == "StageThenAttack") {
                if (((Def)parms.faction.def).defName == "HoraxCult") {
                    Log.Message("[TrueRandomRaids] HoraxCult cannot stage then attack. Rerolling faction...");
                    for (int index = 0; ((Def)parms.faction.def).defName == "HoraxCult" && index < 100; ++index)
                        parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
                    if (((Def)parms.faction.def).defName == "HoraxCult")
                        Log.Warning("[TrueRandomRaids] Failed to find a valid faction after 100 attempts! Keeping HoraxCult.");
                    else
                        Log.Message("[TrueRandomRaids] Selected faction: " + parms.faction.Name);
                }
            }
            else if (((Def)parms.raidStrategy).defName == "EmergeFromWater") {
                if (((Def)parms.faction.def).defName != "Mechanoid")
                    parms.faction = Faction.OfMechanoids;
            }
            else if (((Def)parms.raidStrategy).defName == "ImmediateAttackBreaching" || ((Def)parms.raidStrategy).defName == "ImmediateAttackBreachingSmart") {
                Faction faction = parms.faction;
                if (((Def)parms.faction.def).defName == "Yttakin" || ((Def)parms.faction.def).defName == "HoraxCult" || ((Def)parms.faction.def).defName == "Entities") {
                    Log.Message($"[TrueRandomRaids] {((Def)parms.faction.def).defName} cannot breach. Rerolling faction...");
                    for (int index = 0; (((Def)parms.faction.def).defName == "Yttakin" || ((Def)parms.faction.def).defName == "HoraxCult" || ((Def)parms.faction.def).defName == "Entities") && index < 100; ++index)
                        parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
                    if (((Def)parms.faction.def).defName == "Yttakin" || ((Def)parms.faction.def).defName == "HoraxCult" || ((Def)parms.faction.def).defName == "Entities") {
                        parms.faction = faction;
                        Log.Message($"[TrueRandomRaids] Failed to find a valid faction after 100 attempts! Keeping {parms.faction.Name} with ImmediateAttack.");
                        parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("ImmediateAttack", true);
                    }
                    else
                        Log.Message($"[TrueRandomRaids] Selected faction: {parms.faction.Name}.");
                }
            }
            else if (((Def)parms.raidStrategy).defName == "ImmediateAttackSappers") {
                if (((Def)parms.faction.def).defName == "Yttakin" || ((Def)parms.faction.def).defName == "HoraxCult") {
                    Log.Message("[TrueRandomRaids] Furries and cult cannot be sappers. Rerolling faction...");
                    for (int index = 0; (((Def)parms.faction.def).defName == "Yttakin" || ((Def)parms.faction.def).defName == "HoraxCult" || ((Def)parms.faction.def).defName == "Entities") && index < 100; ++index)
                        parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
                    if (((Def)parms.faction.def).defName == "Yttakin" || ((Def)parms.faction.def).defName == "HoraxCult" || ((Def)parms.faction.def).defName == "Entities")
                        Log.Warning($"[TrueRandomRaids] Failed to find a valid faction after 100 attempts! Keeping {parms.faction.Name}.");
                    else
                        Log.Message($"[TrueRandomRaids] Selected faction: {parms.faction.Name}.");
                }
            }
            else if (((Def)parms.raidStrategy).defName == "Siege") {
                if (parms.faction.def.techLevel <= TechLevel.Neolithic) {
                    Log.Message("[TrueRandomRaids] Tribals cannot siege. Rerolling faction...");
                    for (int index = 0; parms.faction.def.techLevel <= TechLevel.Neolithic && index < 100; ++index)
                        parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
                    if (parms.faction.def.techLevel <= TechLevel.Neolithic)
                        Log.Warning($"[TrueRandomRaids] Failed to find a valid faction after 100 attempts! Keeping {parms.faction.Name}.");
                    else
                        Log.Message($"[TrueRandomRaids] Selected faction: {parms.faction.Name}.");
                }
                if (((Def)parms.faction.def).defName == "Mechanoid") {
                    Faction faction = parms.faction;
                    Log.Message("[TrueRandomRaids] Mechanoids cannoy siege (in raid incidents). Rerolling faction...");
                    for (int index = 0; ((Def)parms.faction.def).defName == "Mechanoid" && index < 100; ++index)
                        parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
                    if (((Def)parms.faction.def).defName == "Mechanoid") {
                        parms.faction = faction;
                        Log.Warning($"[TrueRandomRaids] Failed to find other after 100 attempts! Keeping {parms.faction.Name} with StageThenAttack.");
                        parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("StageThenAttack", true);
                    }
                    else
                        Log.Message($"[TrueRandomRaids] Selected faction: {parms.faction.Name}.");
                }
            }
            else if (((Def)parms.raidStrategy).defName == "PsychicRitualSiege") {
                Log.Message("[TrueRandomRaids] Somehow game rolled \"PsychicRitualSiege\".");
                if (((Def)parms.faction.def).defName != "HoraxCult") {
                    Log.Message("[TrueRandomRaids] Only HoraxCult can perform \"PsychicRitualSiege\". Rerolling faction...");
                    for (int index = 0; ((Def)parms.faction.def).defName != "HoraxCult" && index < 100; ++index)
                        parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
                    if (((Def)parms.faction.def).defName != "HoraxCult")
                        Log.Warning($"[TrueRandomRaids] Failed to find a valid faction after 100 attempts! Keeping {parms.faction.Name}.");
                    else
                        Log.Message($"[TrueRandomRaids] Selected faction: {parms.faction.Name}.");
                }
            }
            else if (((Def)parms.raidStrategy).defName == "ShamblerAssault") {
                Log.Message("[TrueRandomRaids] Somehow game rolled \"ShamblerAssault\".");
                if (((Def)parms.faction.def).defName != "Entities") {
                    Log.Message("[TrueRandomRaids] Only Entities can perform \"ShamblerAssault\". Rerolling faction...");
                    for (int index = 0; ((Def)parms.faction.def).defName != "Entities" && index < 100; ++index)
                        parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, true, (TechLevel)0);
                    if (((Def)parms.faction.def).defName != "Entities")
                        Log.Warning($"[TrueRandomRaids] Failed to find a valid faction after 100 attempts! Keeping {parms.faction.Name}.");
                    else
                        Log.Message($"[TrueRandomRaids] Selected faction: {parms.faction.Name}.");
                }
            }
            if (parms.raidStrategy == null) {
                parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("StageThenAttack", true);
                Log.Message("[TrueRandomRaids] Raid strategy was null. Assigned: " + ((Def)parms.raidStrategy).defName);
            }
            // declare with named tuple elements
            List<(PawnsArrivalModeDef arrivalMode, float weight)> source2 = new List<(PawnsArrivalModeDef arrivalMode, float weight)>();

            if (settings.EdgeDrop_Chance > 0.0)
                source2.Add((TRI.arrivalModeCache["EdgeDrop"], settings.EdgeDrop_Chance));

            if (settings.EdgeWalkIn_Chance > 0.0)
                source2.Add((TRI.arrivalModeCache["EdgeWalkIn"], settings.EdgeWalkIn_Chance));

            if (settings.CenterDrop_Chance > 0.0)
                source2.Add((TRI.arrivalModeCache["CenterDrop"], settings.CenterDrop_Chance));

            if (settings.RandomDrop_Chance > 0.0)
                source2.Add((TRI.arrivalModeCache["RandomDrop"], settings.RandomDrop_Chance));

            if (settings.EdgeDropGroups_Chance > 0.0)
                source2.Add((TRI.arrivalModeCache["EdgeDropGroups"], settings.EdgeDropGroups_Chance));

            if (settings.EdgeWalkInGroups_Chance > 0.0)
                source2.Add((TRI.arrivalModeCache["EdgeWalkInGroups"], settings.EdgeWalkInGroups_Chance));

            if (settings.EmergeFromWaterMode_Chance > 0.0)
                source2.Add((TRI.arrivalModeCache["EmergeFromWater"], settings.EmergeFromWaterMode_Chance));

            Faction faction1 = parms.faction;
            if (faction1 != null && faction1.def.techLevel <= TechLevel.Neolithic) {
                Log.Message($"[TrueRandomRaids] {parms.faction.Name} is Neolithic or lower, restricting arrival modes to EdgeWalkIn and EdgeWalkInGroups.");

                // now works because tuple element is named "arrivalMode"
                source2 = source2
                    .Where(a => a.arrivalMode == TRI.arrivalModeCache["EdgeWalkIn"]
                             || a.arrivalMode == TRI.arrivalModeCache["EdgeWalkInGroups"])
                    .ToList();

                if (source2.Count == 0) {
                    Log.Message($"[TrueRandomRaids] No valid arrival modes left for {parms.faction.Name}, forcing EdgeWalkIn.");
                    source2.Add((TRI.arrivalModeCache["EdgeWalkIn"], 1f));
                }
            }
            if (parms.raidStrategy.defName == "ImmediateAttack" || parms.raidStrategy.defName == "ImmediateAttackSmart") {
                Log.Message("[TrueRandomRaids] Restricting arrival modes based on the selected strategy: " + parms.raidStrategy.defName);
                source2 = source2
                    .Where(a => a.arrivalMode == TRI.arrivalModeCache["EdgeDrop"]
                             || a.arrivalMode == TRI.arrivalModeCache["EdgeWalkIn"]
                             || a.arrivalMode == TRI.arrivalModeCache["CenterDrop"]
                             || a.arrivalMode == TRI.arrivalModeCache["RandomDrop"]
                             || a.arrivalMode == TRI.arrivalModeCache["EdgeWalkInGroups"]
                             || a.arrivalMode == TRI.arrivalModeCache["EdgeDropGroups"])
                    .ToList();

                if (source2.Count == 0) {
                    Log.Message($"[TrueRandomRaids] No valid arrival modes left for {parms.raidStrategy.defName}, forcing EdgeWalkIn.");
                    source2.Add((TRI.arrivalModeCache["EdgeWalkIn"], 1f));
                }
            }

            if (parms.raidStrategy.defName == "StageThenAttack" || parms.raidStrategy.defName == "ImmediateAttackSappers") {
                Log.Message("[TrueRandomRaids] Restricting arrival modes based on the selected strategy: " + parms.raidStrategy.defName);
                source2 = source2
                    .Where(a => a.arrivalMode == TRI.arrivalModeCache["EdgeWalkIn"]
                             || a.arrivalMode == TRI.arrivalModeCache["EdgeDrop"]
                             || a.arrivalMode == TRI.arrivalModeCache["EdgeWalkInGroups"]
                             || a.arrivalMode == TRI.arrivalModeCache["EdgeDropGroups"])
                    .ToList();

                if (source2.Count == 0) {
                    Log.Message($"[TrueRandomRaids] No valid arrival modes left for {parms.raidStrategy.defName}, forcing EdgeWalkIn.");
                    source2.Add((TRI.arrivalModeCache["EdgeWalkIn"], 1f));
                }
            }

            if (parms.raidStrategy.defName == "ImmediateAttackBreaching") {
                Log.Message("[TrueRandomRaids] Restricting arrival modes based on the selected strategy: " + parms.raidStrategy.defName);
                source2 = source2
                    .Where(a => a.arrivalMode == TRI.arrivalModeCache["EdgeWalkIn"])
                    .ToList();

                if (source2.Count == 0) {
                    Log.Message($"[TrueRandomRaids] No valid arrival modes left for {parms.raidStrategy.defName}, forcing EdgeWalkIn.");
                    source2.Add((TRI.arrivalModeCache["EdgeWalkIn"], 1f));
                }
            }

            if (parms.raidStrategy.defName == "ImmediateAttackBreachingSmart") {
                Log.Message("[TrueRandomRaids] Restricting arrival modes based on the selected strategy: " + parms.raidStrategy.defName);
                source2 = source2
                    .Where(a => a.arrivalMode == TRI.arrivalModeCache["EdgeWalkIn"])
                    .ToList();

                if (source2.Count == 0) {
                    Log.Message($"[TrueRandomRaids] No valid arrival modes left for {parms.raidStrategy.defName}, forcing EdgeWalkIn.");
                    source2.Add((TRI.arrivalModeCache["EdgeWalkIn"], 1f));
                }
            }

            if (parms.raidStrategy.defName == "Siege") {
                Log.Message("[TrueRandomRaids] Restricting arrival modes based on the selected strategy: " + parms.raidStrategy.defName);
                source2 = source2
                    .Where(a => a.arrivalMode == TRI.arrivalModeCache["EdgeWalkIn"]
                             || a.arrivalMode == TRI.arrivalModeCache["EdgeDrop"])
                    .ToList();

                if (source2.Count == 0) {
                    Log.Message($"[TrueRandomRaids] No valid arrival modes left for {parms.raidStrategy.defName}, forcing EdgeWalkIn.");
                    source2.Add((TRI.arrivalModeCache["EdgeWalkIn"], 1f));
                }
            }

            if (parms.raidStrategy.defName == "EmergeFromWater") {
                Log.Message("[TrueRandomRaids] Restricting arrival modes based on the selected strategy: " + parms.raidStrategy.defName);
                source2 = source2
                    .Where(a => a.arrivalMode == TRI.arrivalModeCache["EmergeFromWater"])
                    .ToList();

                if (source2.Count == 0) {
                    Log.Message($"[TrueRandomRaids] No valid arrival modes left for {parms.raidStrategy.defName}, forcing default arrival mode.");
                    source2.Add((TRI.arrivalModeCache["EmergeFromWater"], 1f));

                    if (parms.faction.def.defName != "Mechanoid")
                        source2.Add((TRI.arrivalModeCache["EdgeWalkIn"], 1f));
                    else
                        source2.Add((TRI.arrivalModeCache["EdgeWalkIn"], 1f)); // identical both branches? maybe redundant
                }
            }
            int num6 = 0;
            foreach ((PawnsArrivalModeDef _, float num7) in source2)
                num6 += Mathf.RoundToInt(num7 * 100f);
            Log.Message($"[TrueRandomRaids] Max value based on selected arrival modes: {num6}");
            int num8 = Random.Range(0, num6);
            int num9 = 0;
            PawnsArrivalModeDef pawnsArrivalModeDef1 = (PawnsArrivalModeDef)null;
            foreach ((PawnsArrivalModeDef pawnsArrivalModeDef2, float num10) in source2) {
                num9 += Mathf.RoundToInt(num10 * 100f);
                Log.Message($"[TrueRandomRaids] Checking arrival mode: {((Def)pawnsArrivalModeDef2).defName} with cumulative chance: {num9}");
                if (num8 < num9) {
                    pawnsArrivalModeDef1 = pawnsArrivalModeDef2;
                    Log.Message($"[TrueRandomRaids] Selected arrival mode: {((Def)pawnsArrivalModeDef1).defName} (random value: {num8}, threshold: {num9})");
                    break;
                }
            }
            if (pawnsArrivalModeDef1 != null) {
                parms.raidArrivalMode = pawnsArrivalModeDef1;
            }
            else {
                parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                Log.Message("[TrueRandomRaids] No arrival mode selected. Defaulted to Edge Walk-In.");
            }
        }

        private static void ApplyThreatPointsHelperMethod(ref IncidentParms parms) {
            TrueRandomRaidsSettings settings = LoadedModManager.GetMod<TrueRandomRaidsMod>().GetSettings<TrueRandomRaidsSettings>();
            if (settings.EnableRandomRaidPoints) {
                int num1 = Mathf.RoundToInt(settings.MinRaidPoints);
                int num2 = Mathf.RoundToInt(settings.MaxRaidPoints);
                settings.DynamicRaidPoints = (float)Random.Range(num1, num2);
                parms.points = settings.DynamicRaidPoints;
                Log.Message($"[TrueRandomRaids] True Dynamic Threat Points: {settings.DynamicRaidPoints}, Min Threat Points: {num1}, Max Raid Points: {num2}");
            }
            else {
                if (!settings.EnableVanillaRandomRaidPoints)
                    return;
                float num3 = Mathf.Max(StorytellerUtility.DefaultThreatPointsNow(parms.target), 100f);
                int num4 = Mathf.RoundToInt(settings.MinVanillaRaidPoints);
                int num5 = Mathf.RoundToInt(num3);
                settings.DynamicRaidPoints = (float)Random.Range(num4, num5);
                parms.points = settings.DynamicRaidPoints;
                Log.Message($"[TrueRandomRaids] Vanilla Dynamic Threat Points: {settings.DynamicRaidPoints}, Min Vanilla Threat Points: {num4}, Max Vanilla Raid Points: {num5}");
            }
        }
    }
}