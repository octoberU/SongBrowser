using csvorbis;
using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal static class EitherHand
{
    static Color eitherHandColor = new Color(1f, 1f, 1f);
    [HarmonyPatch(typeof(TargetColorSetter), "Start", new Type[0])]
    private static class FixEitherHandColors
    {
        private static void Prefiz(TargetColorSetter __instance)
        {
            __instance.telegraphEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_Color1", eitherHandColor);

            __instance.telegraphSustainEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_Color1", eitherHandColor);

            __instance.telegraphSlotEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_Color1", eitherHandColor);

            __instance.telegraphRingEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_Color1", eitherHandColor);

            __instance.telegraphCenterEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_Color1", eitherHandColor);

            __instance.telegraphCenterSustainEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_Color1", eitherHandColor);

            __instance.telegraphCenterHorizontalEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_Color1", eitherHandColor);

            __instance.telegraphCenterVerticalEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_Color1", eitherHandColor);

            __instance.telegraphDartLineEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_Color1", eitherHandColor);

            __instance.telegraphGlowEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_Color1", eitherHandColor);

            //__instance.targetTrailEither.startColor = eitherHandColor;

            __instance.mStandardTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_Color1", eitherHandColor);
        }
    }

    [HarmonyPatch(typeof(TargetColorSetter), "GetTargetMaterial", new Type[] { typeof(Target.TargetBehavior), typeof(Target.TargetHandType) })]
    private static class FixEitherHandColors2
    {
        private static void Postfix(TargetColorSetter __instance, Target.TargetBehavior behavior, Target.TargetHandType hand, Material __result)
        {
            __result.SetColor("_EmissionColor", eitherHandColor);
        }
    }
}

