using System;
using Harmony;
using UnityEngine;

namespace AudicaModding
{
    internal static class MapperNames
    {
        public static void FixMappers()
        {
            for (int i = 0; i < SongList.I.songs.Count; i++)
            {
                if (SongList.I.songs[i].songID == "destiny" ||
                    SongList.I.songs[i].songID == "adrenaline" ||
                    SongList.I.songs[i].songID == "collider" ||
                    SongList.I.songs[i].songID == "golddust" ||
                    SongList.I.songs[i].songID == "hr8938cephei" ||
                    SongList.I.songs[i].songID == "ifeellove" ||
                    SongList.I.songs[i].songID == "iwantu" ||
                    SongList.I.songs[i].songID == "lazerface" ||
                    SongList.I.songs[i].songID == "popstars" ||
                    SongList.I.songs[i].songID == "perfectexceeder" ||
                    SongList.I.songs[i].songID == "predator" ||
                    SongList.I.songs[i].songID == "resistance" ||
                    SongList.I.songs[i].songID == "smoke" ||
                    SongList.I.songs[i].songID == "splinter" ||
                    SongList.I.songs[i].songID == "synthesized" ||
                    SongList.I.songs[i].songID == "thespace" ||
                    SongList.I.songs[i].songID == "titanium_cazzette" ||
                    SongList.I.songs[i].songID == "reedsofmitatrush" ||
                    SongList.I.songs[i].songID == "destiny_full" ||
                    SongList.I.songs[i].songID == "popstars_full")
                {
                    SongList.I.songs[i].author = "HMXJeff";
                }
                else if (SongList.I.songs[i].songID == "addictedtoamemory" ||
                         SongList.I.songs[i].songID == "breakforme" ||
                         SongList.I.songs[i].songID == "channel42" ||
                         SongList.I.songs[i].songID == "everyday" ||
                         SongList.I.songs[i].songID == "gametime" ||
                         SongList.I.songs[i].songID == "highwaytooblivion_short" ||
                         SongList.I.songs[i].songID == "overtime" ||
                         SongList.I.songs[i].songID == "tothestars" ||
                         SongList.I.songs[i].songID == "addictedtoamemory_full" ||
                         SongList.I.songs[i].songID == "highwaytooblivion_full" ||
                         SongList.I.songs[i].songID == "avalanche" ||
                         SongList.I.songs[i].songID == "badguy" ||
                         SongList.I.songs[i].songID == "believer" ||
                         SongList.I.songs[i].songID == "betternow" ||
                         SongList.I.songs[i].songID == "cantfeelmyface" ||
                         SongList.I.songs[i].songID == "centuries" ||
                         SongList.I.songs[i].songID == "countingstars" ||
                         SongList.I.songs[i].songID == "dontletmedown" ||
                         SongList.I.songs[i].songID == "exitwounds" ||
                         SongList.I.songs[i].songID == "gdfr" ||
                         SongList.I.songs[i].songID == "girlsbedancing" ||
                         SongList.I.songs[i].songID == "intoyou" ||
                         SongList.I.songs[i].songID == "juice" ||
                         SongList.I.songs[i].songID == "longrun" ||
                         SongList.I.songs[i].songID == "methanebreather" ||
                         SongList.I.songs[i].songID == "moveslikejagger" ||
                         SongList.I.songs[i].songID == "newrules" ||
                         SongList.I.songs[i].songID == "sorryforpartyrocking" ||
                         SongList.I.songs[i].songID == "starships" ||
                         SongList.I.songs[i].songID == "stook" ||
                         SongList.I.songs[i].songID == "thegreatest" ||
                         SongList.I.songs[i].songID == "themiddle" ||
                         SongList.I.songs[i].songID == "themotherweshare" ||
                         SongList.I.songs[i].songID == "urprey" ||
                         SongList.I.songs[i].songID == "weallbecome" ||
                         SongList.I.songs[i].songID == "youngblood")
                {
                    SongList.I.songs[i].author = "HMXRick";
                }
                else if (SongList.I.songs[i].songID == "boomboom" ||
                         SongList.I.songs[i].songID == "raiseyourweapon_noisia" ||
                         SongList.I.songs[i].songID == "timeforcrime")
                {
                    SongList.I.songs[i].author = "HMXJeff & HMXRick";
                }
                else if (SongList.I.songs[i].songID == "eyeforaneye" ||
                         SongList.I.songs[i].songID == "goatpolyphia" ||
                         SongList.I.songs[i].songID == "illmerica" ||
                         SongList.I.songs[i].songID == "funkycomputer")
                {
                    SongList.I.songs[i].author = "Simon";
                }
                else if (SongList.I.songs[i].songID == "loyal")
                {
                    SongList.I.songs[i].author = "Simon & HMXRick";
                }
                else if (SongList.I.songs[i].songID == "highhopes" ||
                         SongList.I.songs[i].songID == "goodbyedearsorrows_ab42b2e6b0934471474875729b4f9934" ||
                         SongList.I.songs[i].songID == "shatterme_30eb4181110577459bc89b8650d3386a")
                {
                    SongList.I.songs[i].author = "aggrogahu";
                }
                else if (SongList.I.songs[i].songID == "bigppwoo_0966cf748cb5f637e3b0f00feeda9d9a")
                {
                    SongList.I.songs[i].author = "Sleepyhead";
                }
                else if (SongList.I.songs[i].songID == "children-of-a-miracle_bc34b4da4eea98a2a2e7c28d378738e6" ||
                         SongList.I.songs[i].songID == "get-jinxed_bd9d8a475804d6e2086fc3d2090ea9fb" ||
                         SongList.I.songs[i].songID == "no-worries_b48e9121c4f412e2da920212ff375a45")
                {
                    SongList.I.songs[i].author = "Fredrix";
                }
                else if (SongList.I.songs[i].songID == "LegendsNeverDie_96f3da6e3455fc7b74535f4ad3171955")
                {
                    SongList.I.songs[i].author = "CircuitLord";
                }
                else if (SongList.I.songs[i].songID == "Camellia_The_King_of_Lions_f39276e8867fbfd9c0d9c1e99dc03052")
                {
                    SongList.I.songs[i].author = "CriminalCannoli";
                }
                else if (SongList.I.songs[i].songID == "weaponizedcelldwellershark_a0508d763c057c198291149233c4d150")
                {
                    SongList.I.songs[i].author = "whattheshark";
                }
                else if (SongList.I.songs[i].songID == "ainideaikoiwatsudzukuoctober_d674ca136c43e57ef82a92cb8f80da87" ||
                         SongList.I.songs[i].songID == "sadmachine_a49b6978f4ab867057f8bb22bcf53580")
                {
                    SongList.I.songs[i].author = "october";
                }
                else if (SongList.I.songs[i].songID == "deviltrigger_728ff099c5d7d1f2ad0f724fb53b9b43" ||
                         SongList.I.songs[i].songID == "onceagain_ProtoPip_0e8bb6d431dd2fdabb62a4d988c263eb")
                {
                    SongList.I.songs[i].author = "ProtoPip";
                }
            }
        }

        [HarmonyPatch(typeof(SongList), "Start", new Type[0])]
        private static class SongListStartPatch
        {
            private static void Postfix(SongList __instance)
            {
                SongList.OnSongListLoaded.On(new Action(() => { FixMappers(); KataConfig.I.CreateDebugText("Songs Loaded", new Vector3(0f, -1f, 5f), 5f, null, false, 0.2f); }));
            }
        }
    }
}
