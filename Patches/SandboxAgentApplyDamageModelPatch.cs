using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using HarmonyLib;

namespace AttributesReloaded
{
	[HarmonyPatch(typeof(SandboxAgentApplyDamageModel))]
	[HarmonyPatch("CalculateDamage")]
	class SandboxAgentApplyDamageModelPatch
	{
		public static int Postfix(int __result, BasicCharacterObject affectorBasicCharacter, BasicCharacterObject affectedBasicCharacter, MissionWeapon offHandItem, bool isHeadShot, bool isAffectedAgentMount, bool isAffectedAgentHuman, bool hasAffectorAgentMount, bool isAffectedAgentNull, bool isAffectorAgentHuman, AttackCollisionData collisionData, WeaponComponentData weapon)
		{
			CharacterObject atacker = affectorBasicCharacter as CharacterObject;
			CharacterObject victim = affectedBasicCharacter as CharacterObject;
			if (isAffectedAgentNull || atacker == null || !isAffectorAgentHuman || !isAffectedAgentHuman)
			{
				return __result;
			}
			if (!collisionData.IsFallDamage && atacker != null && victim != null && !collisionData.IsAlternativeAttack)
			{
				var bonuses = new CharacterAttributeBonuses(atacker);
				var isMelee = weapon != null && !weapon.IsRangedWeapon;
				float damageMultiplier = isMelee
					? bonuses.MeleeDamageMultiplier
					: bonuses.RangeDamageMultiplier;
				var bonusDamage = (int)(__result * damageMultiplier);
				if (atacker.IsPlayerCharacter && Config.Instance.enable_messages)
				{
					InformationManager.DisplayMessage(new InformationMessage("Bonus " + bonusDamage + " damage from " + (isMelee ? "VIG" : "CON"), Colors.Red));
				}
				__result += bonusDamage;
			}
			return __result;
		}
	}
}
