using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace GUnJammer
{
    [HarmonyPatch]
    internal class AbilityPatch
    {
        private static bool buffered = false;
        private static readonly MethodInfo fireCard = typeof(MechController).GetMethod("FireCard", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int)], null);

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MechController), "UpdateCards")]
        private static void SetBuffer(ref bool ____isWeaponReloading)
        {
            GameInput controller = Singleton<GameInput>.Instance;
            if (controller.GetButtonDown(GameInput.GameActions.FireCard, GameInput.InputType.Game) && ____isWeaponReloading && RM.GetAcceptInput())
            {
                buffered = true;
                Debug.Log("Buffer Loaded");
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechController), "FireCard", [typeof(int)])]
        private static void ClearBuffer(ref bool __result)
        {
            if (__result)
                buffered = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechController), "UpdateCards")]
        private static void ModifyBuffer(ref MechController __instance, ref bool ____waitToStopFiring, ref bool ____isWeaponReloading, ref float ___shotTimer, ref PlayerCardDeck ___deck, ref bool ____shotOnDown)
        {
            if (buffered && !____isWeaponReloading && ___shotTimer < 0f && ___deck.GetCardInHand(0).currentAmmo > 0 && RM.GetAcceptInput())
            {
                fireCard.Invoke(__instance, [0]);
                ____waitToStopFiring = false;
                ____shotOnDown = true;
                Debug.Log("Buffer Shot");
            }
        }
    }
}
