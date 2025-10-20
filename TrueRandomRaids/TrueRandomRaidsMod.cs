using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TrueRandomRaids {
    public class TrueRandomRaidsMod : Mod {
        private TrueRandomRaidsSettings settings;
        private int selectedTab = 0;

        public TrueRandomRaidsMod(ModContentPack content) : base(content) {
            this.settings = GetSettings<TrueRandomRaidsSettings>();
            var harmony = new Harmony("ISee.TrueRandomSettings");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect) {
            base.DoSettingsWindowContents(inRect);

            List<TabRecord> tabs = new List<TabRecord>()
            {
                new TabRecord("General", () => selectedTab = 0, selectedTab == 0),
                new TabRecord("Raid Strategies", () => selectedTab = 1, selectedTab == 1),
                new TabRecord("Raid Arrival Modes", () => selectedTab = 2, selectedTab == 2)
            };

            Rect tabRect = new Rect(inRect.x + inRect.width / 4f, inRect.y + 30f, inRect.width / 2f, 40f);
            TabDrawer.DrawTabs(tabRect, tabs, 200f);

            Rect contentRect = inRect;
            contentRect.yMin += 90f;

            DrawContent(contentRect, settings);
        }

        private void DrawContent(Rect inRect, TrueRandomRaidsSettings settings) {
            Listing_Standard listing = new Listing_Standard();

            Rect contentRect = inRect;
            contentRect.width = inRect.width / 2f;
            contentRect.x = inRect.x + inRect.width / 4f;

            listing.Begin(contentRect);

            switch (selectedTab) {
                case 0:
                    DrawThreatPointsSettings(listing, settings);
                    break;
                case 1:
                    DrawRaidStrategiesSettings(listing, settings);
                    break;
                case 2:
                    DrawRaidArrivalModesSettings(listing, settings);
                    break;
            }

            listing.End();
        }

        private void DrawThreatPointsSettings(Listing_Standard listing, TrueRandomRaidsSettings settings) {
            listing.Gap(12f);
            Color oldColor = GUI.color;

            GUI.color = Color.yellow;
            listing.CheckboxLabeled("Random Raids", ref settings.EnableRandomRaids,
                "If enabled, raid incidents will use settings from \"Raid Strategy\" and \"Raid Arrival Modes\" tabs.");

            listing.Gap(12f);
            listing.CheckboxLabeled("Incidents Random Threat Points", ref settings.EnableRandomIncidentPoints,
                "If enabled, threat points for all incidents will be generated based on the selected method.");

            listing.Gap(12f);
            listing.CheckboxLabeled("Vanilla Random Threat Points", ref settings.EnableVanillaRandomRaidPoints,
                "If enabled, threat points for each raid will be randomly generated based on min, difficulty, wealth, and colonist count.");

            GUI.color = oldColor;

            if (settings.EnableVanillaRandomRaidPoints) {
                settings.EnableRandomRaidPoints = false;
                listing.Label($"Min Threat Points: {settings.MinVanillaRaidPoints:F0}");
                settings.MinVanillaRaidPoints = listing.Slider(settings.MinVanillaRaidPoints, 100f, 10000f);
                listing.Label("Max: Player Wealth + Game Difficulty + Colonist Count");
            }

            GUI.color = Color.yellow;
            listing.CheckboxLabeled("True Random Threat Points", ref settings.EnableRandomRaidPoints,
                "If enabled, threat points for each raid will be randomly generated based on min and max values.");

            GUI.color = oldColor;

            if (settings.EnableRandomRaidPoints) {
                settings.EnableVanillaRandomRaidPoints = false;
                listing.Label($"Min Threat Points: {settings.MinRaidPoints:F0}");
                settings.MinRaidPoints = listing.Slider(settings.MinRaidPoints, 100f, 10000f);
                listing.Label($"Max Threat Points: {settings.MaxRaidPoints:F0}");
                settings.MaxRaidPoints = listing.Slider(settings.MaxRaidPoints, 100f, 10000f);

                if (settings.MaxRaidPoints < settings.MinRaidPoints)
                    settings.MaxRaidPoints = settings.MinRaidPoints;
            }
        }

        private void DrawRaidStrategiesSettings(
          Listing_Standard listing,
          TrueRandomRaidsSettings settings) {
            ((Listing)listing).Gap(12f);
            ((Listing)listing).Gap(12f);
            this.DrawSettingsColumn(listing, settings, ("Immediate Attack", (Func<float>)(() => settings.ImmediateAttack_Chance), (Action<float>)(v => settings.ImmediateAttack_Chance = v), "Raiders will attack immediately without any preparation. Supports all arrival modes except \"Emerge From Water\"."), ("Immediate Attack Smart", (Func<float>)(() => settings.ImmediateAttackSmart_Chance), (Action<float>)(v => settings.ImmediateAttackSmart_Chance = v), "Raiders will attack immediately and try to avoid turrets and some traps. Supports all arrival modes except \"Emerge From Water\"."), ("Stage Then Attack", (Func<float>)(() => settings.StageThenAttack_Chance), (Action<float>)(v => settings.StageThenAttack_Chance = v), "Raiders will gather at the edge of the map before attacking. Doesn't support \"Center Drop\", \"Random Drop\", and \"Emerge From Water\"."), ("Emerge From Water", (Func<float>)(() => settings.EmergeFromWater_Chance), (Action<float>)(v => settings.EmergeFromWater_Chance = v), "Mechs will emerge from nearby rivers or oceans (lakes don't count), if conditions for mechs aren't met, a default faction and strategy will chosen. Supports only the \"Emerge From Water\" arrival mode."), ("Immediate Attack Breaching", (Func<float>)(() => settings.ImmediateAttackBreaching_Chance), (Action<float>)(v => settings.ImmediateAttackBreaching_Chance = v), "Raiders will breach walls instead of choosing the easiest path. Yttakin don't use this strategy regardless of percentage. Supports only the \"Edge Walk In\" arrival mode."), ("Immediate Attack Breaching Smart", (Func<float>)(() => settings.ImmediateAttackBreachingSmart_Chance), (Action<float>)(v => settings.ImmediateAttackBreachingSmart_Chance = v), "Raiders will breach walls instead of choosing the easiest path and try to avoid turrets and some traps. Yttakin don't use this strategy regardless of percentage. Supports only the \"Edge Walk In\" arrival mode.\""), ("Immediate Attack Sappers", (Func<float>)(() => settings.ImmediateAttackSappers_Chance), (Action<float>)(v => settings.ImmediateAttackSappers_Chance = v), "Raiders will dig through almost anything to get to your beds. Yttakin don't use this strategy regardless of percentage. Doesn't support \"Center Drop\", \"Random Drop\", and \"Emerge From Water\"."), ("Siege", (Func<float>)(() => settings.Siege_Chance), (Action<float>)(v => settings.Siege_Chance = v), "Raiders will set up a siege camp and bombard your base. Tribes cannot use the siege strategy regardless of percentage but surprisingly, Yttakin can. Supports only the \"Edge Walk In\" arrival mode.\""));
        }

        private void DrawRaidArrivalModesSettings(
          Listing_Standard listing,
          TrueRandomRaidsSettings settings) {
            ((Listing)listing).Gap(12f);
            ((Listing)listing).Gap(12f);
            this.DrawSettingsColumn(listing, settings, ("Edge Drop", (Func<float>)(() => settings.EdgeDrop_Chance), (Action<float>)(v => settings.EdgeDrop_Chance = v), "Raiders will drop in at the edge of the map."), ("Edge Walk In", (Func<float>)(() => settings.EdgeWalkIn_Chance), (Action<float>)(v => settings.EdgeWalkIn_Chance = v), "Raiders will walk in from the edge of the map."), ("Center Drop", (Func<float>)(() => settings.CenterDrop_Chance), (Action<float>)(v => settings.CenterDrop_Chance = v), "Raiders will drop in at your pawns, most likely the center of your settlement."), ("Random Drop", (Func<float>)(() => settings.RandomDrop_Chance), (Action<float>)(v => settings.RandomDrop_Chance = v), "Raiders will drop in at random locations on the map."), ("Edge Drop Groups", (Func<float>)(() => settings.EdgeDropGroups_Chance), (Action<float>)(v => settings.EdgeDropGroups_Chance = v), "Raiders will drop in groups at the edge of the map."), ("Edge Walk In Groups", (Func<float>)(() => settings.EdgeWalkInGroups_Chance), (Action<float>)(v => settings.EdgeWalkInGroups_Chance = v), "Raiders will walk in groups from the edge of the map."), ("Emerge From Water", (Func<float>)(() => settings.EmergeFromWaterMode_Chance), (Action<float>)(v => settings.EmergeFromWaterMode_Chance = v), "If \"Emerge From Water\" strategy is enabled, mechs may use this arrival mode, if \"Emerge From Water\" strategy is disabled, this arrival mode will be ignored regardless of its percentage. If there are no bodies of water on the map, raid will be skipped."));
        }


        private void DrawSettingsColumn(Listing_Standard listing, TrueRandomRaidsSettings settings,
            params (string label, Func<float> getter, Action<float> setter, string tooltip)[] sliders) {
            foreach (var (label, getter, setter, tooltip) in sliders) {
                Text.Anchor = TextAnchor.MiddleLeft;
                float currentValue = getter();

                // Draw label
                Rect rect = listing.GetRect(Text.LineHeight);
                Widgets.Label(rect, $"{label}: {(currentValue * 100f):F0}%");

                // Add tooltip
                if (!string.IsNullOrEmpty(tooltip))
                    TooltipHandler.TipRegion(rect, tooltip);

                Text.Anchor = TextAnchor.UpperLeft;

                // Draw slider
                setter(listing.Slider(currentValue, 0f, 1f));

                listing.Gap(4f); // optional spacing between sliders
            }
        }

        public override string SettingsCategory() => "True Random Raids";
    }
}