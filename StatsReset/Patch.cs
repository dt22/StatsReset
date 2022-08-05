using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Geoscape.View.ViewModules;
using PhoenixPoint.Geoscape.Entities;
using PhoenixPoint.Common.View.ViewControllers;
using UnityEngine.UI;
using PhoenixPoint.Geoscape.View.ViewControllers;
using HarmonyLib;

namespace StatsReset
{
	[HarmonyPatch(typeof(UIModuleCharacterProgression), "ChangeCharacterStat")]
	internal static class UIModuleCharacterProgression_ChangeCharacterStat
	{

		private static bool Prefix(ref int __result, GeoCharacter ____character, ref int ____currentMutagens, bool ____hasPandoranProgression, ref int ____currentSkillPoints, int ____startingFactionPoints, ref int ____currentFactionPoints, CharacterBaseAttribute baseStat, int currentStatValue, int startingStatValue, bool increase, ref int __instance)
		{

			if (increase)
			{
				if (____character.Progression.CanModifyBaseStat(baseStat, currentStatValue + 1))
				{
					int baseStatCost = ____character.Progression.GetBaseStatCost(baseStat, currentStatValue + 1);
					if (____hasPandoranProgression)
					{
						if (____currentMutagens - baseStatCost >= 0)
						{
							____currentMutagens -= baseStatCost;
							__result = 1;
						}
					}
					else
					{
						if (____currentSkillPoints - baseStatCost >= 0)
						{
							____currentSkillPoints -= baseStatCost;
							__result = 1;
						}
						if (____currentSkillPoints + ____currentFactionPoints - baseStatCost >= 0)
						{
							int num = ____currentSkillPoints - baseStatCost;
							____currentSkillPoints = 0;
							____currentFactionPoints += num;
							__result = 1;
						}
					}
				}
			}
			else
			{
				int baseStatCost2 = ____character.Progression.GetBaseStatCost(baseStat, currentStatValue);
				if (____hasPandoranProgression)
				{
					____currentMutagens += baseStatCost2;
					__result = -1;
				}
				if (____currentFactionPoints + baseStatCost2 <= ____startingFactionPoints)
				{
					____currentFactionPoints += baseStatCost2;
					__result = -1;
				}
				if (____currentFactionPoints == ____startingFactionPoints)
				{
					____currentSkillPoints += baseStatCost2;
					__result = -1;
				}
				if (____currentFactionPoints + baseStatCost2 > ____startingFactionPoints)
				{
					int num2 = ____currentFactionPoints + baseStatCost2 - ____startingFactionPoints;
					____currentFactionPoints = ____startingFactionPoints;
					____currentSkillPoints += num2;
					__result = -1;
				}
			}
			__instance = 0;
			return false;

		}
	}
	[HarmonyPatch(typeof(UIModuleCharacterProgression), "SetStatButtonInteractabilty")]
	internal static class UIModuleCharacterProgression_SetStatButtonInteractabilty
	{

		private static bool Prefix(UIModuleCharacterProgression __instance, PhoenixGeneralButton statButton, CharacterBaseAttribute stat, bool incrementButton, int ____currentStrengthStat, int ____currentWillStat, int ____currentSpeedStat, GeoCharacter ____character, ref int ____currentMutagens, bool ____hasPandoranProgression, ref int ____currentSkillPoints, ref int ____currentFactionPoints, int ____startingStrengthStat, int ____startingSpeedStat, int ____startingWillStat)
		{

			bool interactable = false;
			if (incrementButton)
			{
				int num = 0;
				switch (stat)
				{
					case CharacterBaseAttribute.Strength:
						num = ____currentStrengthStat;
						break;
					case CharacterBaseAttribute.Will:
						num = ____currentWillStat;
						break;
					case CharacterBaseAttribute.Speed:
						num = ____currentSpeedStat;
						break;
				}
				int baseStatCost = ____character.Progression.GetBaseStatCost(stat, num + 1);
				interactable = (____hasPandoranProgression ? (____currentMutagens >= baseStatCost) : (____currentSkillPoints + ____currentFactionPoints >= baseStatCost));
			}
			else
			{
				switch (stat)
				{
					case CharacterBaseAttribute.Strength:
						interactable = (____currentStrengthStat > ____startingStrengthStat || ____currentStrengthStat <= ____startingStrengthStat);
						break;
					case CharacterBaseAttribute.Will:
						interactable = (____currentWillStat > ____startingWillStat || ____currentWillStat <= ____startingWillStat);
						break;
					case CharacterBaseAttribute.Speed:
						interactable = (____currentSpeedStat > ____startingSpeedStat || ____currentSpeedStat <= ____startingSpeedStat);
						break;
				}
			}
			statButton.SetInteractable(interactable);
			return false;
		}
	}

	[HarmonyPatch(typeof(CharacterProgression), "CanModifyBaseStat")]
	internal static class CharacterProgression_CanModifyBaseStat
	{

		private static bool Prefix(CharacterBaseAttribute stat, int toValue, ref bool __result, CharacterProgression __instance)
		{
			int maxAttribute = __instance.BaseStatSheet.GetMaxAttribute(stat);
			__result = ((toValue >= 0 && toValue <= maxAttribute) || (toValue <= 0 && toValue <= maxAttribute));
			return false;
		}
	}
}
