using Verse;

namespace TrueRandomRaids
{
    public class TrueRandomRaidsSettings : ModSettings {

        public bool isThreat = false;

        public bool isRaid = false;

        public bool EnableRandomRaids = true;

        public bool EnableRandomIncidentPoints = true;

        public bool EnableVanillaRandomRaidPoints = true;

        public bool EnableRandomRaidPoints = false;

        public float MinRaidPoints = 100f;

        public float MaxRaidPoints = 10000f;

        public float MinVanillaRaidPoints = 100f;

        public float DynamicRaidPoints = 0f;

        public float ImmediateAttack_Chance = 1f;

        public float ImmediateAttackSmart_Chance = 1f;

        public float StageThenAttack_Chance = 1f;

        public float EmergeFromWater_Chance = 1f;

        public float ImmediateAttackBreaching_Chance = 1f;

        public float ImmediateAttackBreachingSmart_Chance = 1f;

        public float ImmediateAttackSappers_Chance = 1f;

        public float Siege_Chance = 1f;

        public float EdgeDrop_Chance = 1f;

        public float EdgeWalkIn_Chance = 1f;

        public float CenterDrop_Chance = 1f;

        public float RandomDrop_Chance = 1f;

        public float EdgeDropGroups_Chance = 1f;

        public float EdgeWalkInGroups_Chance = 1f;

        public float EmergeFromWaterMode_Chance = 1f;

        public override void ExposeData() {
            Scribe_Values.Look(ref EnableRandomRaids, "EnableRandomRaids", true);
            Scribe_Values.Look(ref EnableRandomIncidentPoints, "EnableRandomIncidentPoints", true);
            Scribe_Values.Look(ref EnableRandomRaidPoints, "EnableRandomRaidPoints", false);
            Scribe_Values.Look(ref EnableVanillaRandomRaidPoints, "EnableVanillaRandomRaidPoints", true);
            Scribe_Values.Look(ref MinRaidPoints, "MinRaidPoints", 100f);
            Scribe_Values.Look(ref MaxRaidPoints, "MaxRaidPoints", 10000f);
            Scribe_Values.Look(ref MinVanillaRaidPoints, "MinVanillaRaidPoints", 100f);
            Scribe_Values.Look(ref DynamicRaidPoints, "DynamicRaidPoints", 0f);
            Scribe_Values.Look(ref ImmediateAttack_Chance, "ImmediateAttack_Chance", 1f);
            Scribe_Values.Look(ref ImmediateAttackSmart_Chance, "ImmediateAttackSmart_Chance", 1f);
            Scribe_Values.Look(ref StageThenAttack_Chance, "StageThenAttack_Chance", 1f);
            Scribe_Values.Look(ref EmergeFromWater_Chance, "EmergeFromWater_Chance", 1f);
            Scribe_Values.Look(ref ImmediateAttackBreaching_Chance, "ImmediateAttackBreaching_Chance", 1f);
            Scribe_Values.Look(ref ImmediateAttackBreachingSmart_Chance, "ImmediateAttackBreachingSmart_Chance", 1f);
            Scribe_Values.Look(ref ImmediateAttackSappers_Chance, "ImmediateAttackSappers_Chance", 1f);
            Scribe_Values.Look(ref Siege_Chance, "Siege_Chance", 1f);
            Scribe_Values.Look(ref EdgeDrop_Chance, "EdgeDrop_Chance", 1f);
            Scribe_Values.Look(ref EdgeWalkIn_Chance, "EdgeWalkIn_Chance", 1f);
            Scribe_Values.Look(ref CenterDrop_Chance, "CenterDrop_Chance", 1f);
            Scribe_Values.Look(ref RandomDrop_Chance, "RandomDrop_Chance", 1f);
            Scribe_Values.Look(ref EdgeDropGroups_Chance, "EdgeDropGroups_Chance", 1f);
            Scribe_Values.Look(ref EdgeWalkInGroups_Chance, "EdgeWalkInGroups_Chance", 1f);
            Scribe_Values.Look(ref EmergeFromWaterMode_Chance, "EmergeFromWaterMode_Chance", 1f);
        }
    }
}
