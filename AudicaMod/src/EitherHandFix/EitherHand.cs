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
    //static Color eitherHandColor = Color.green;
    //static Color eitherHandColor = new Color(0.9f, 0.72f, 0.13f);
    [HarmonyPatch(typeof(TargetColorSetter), "SetColors", new Type[] {typeof(Color), typeof(Color), typeof(bool) })]
    private static class FixEitherHandColors
    {
        private static void Postfix(TargetColorSetter __instance, Color colorLeft, Color colorRight, bool simpleChange = false)
        {
            __instance.telegraphEitherMat.color = eitherHandColor;
            __instance.telegraphEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.telegraphSustainEitherMat.color = eitherHandColor;
            __instance.telegraphSustainEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphSustainEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.telegraphSlotEitherMat.color = eitherHandColor;
            __instance.telegraphSlotEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphSlotEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.telegraphRingEitherMat.color = eitherHandColor;
            __instance.telegraphRingEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphRingEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.telegraphCenterEitherMat.color = eitherHandColor;
            __instance.telegraphCenterEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphCenterEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.telegraphCenterSustainEitherMat.color = eitherHandColor;
            __instance.telegraphCenterSustainEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphCenterSustainEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.telegraphCenterHorizontalEitherMat.color = eitherHandColor;
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphCenterHorizontalEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.telegraphCenterVerticalEitherMat.color = eitherHandColor;
            __instance.telegraphCenterVerticalEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphCenterVerticalEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.telegraphDartLineEitherMat.color = eitherHandColor;
            __instance.telegraphDartLineEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphDartLineEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.telegraphGlowEitherMat.color = eitherHandColor;
            __instance.telegraphGlowEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_Color", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.telegraphGlowEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.mChainTargetEitherMat.color = eitherHandColor;
            __instance.mChainTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_Color", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.mChainTargetEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.mChainStartTargetEitherMat.color = eitherHandColor;
            __instance.mChainStartTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_Color", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.mChainStartTargetEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.mHoldTargetEitherMat.color = eitherHandColor;
            __instance.mHoldTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_Color", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.mHoldTargetEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.mHorizontalTargetEitherMat.color = eitherHandColor;
            __instance.mHorizontalTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_Color", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.mHorizontalTargetEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.mVerticalTargetEitherMat.color = eitherHandColor;
            __instance.mVerticalTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_Color", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.mVerticalTargetEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            __instance.mStandardTargetEitherMat.color = eitherHandColor;
            __instance.mStandardTargetEitherMat.SetColor("_Color0", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_Color1", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_Color", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_EmissionColor", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_EmisColor", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_Emission", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_GlowColor", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_OutlineColor", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_RimColor", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_SpecColor", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_center_highlight_color", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_TintColor", eitherHandColor);
            __instance.mStandardTargetEitherMat.SetColor("_swirl_alpha", eitherHandColor);

            //__instance.targetTrailEither.startColor = eitherHandColor;
        }
    }



}

