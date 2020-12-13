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
                switch (SongList.I.songs[i].songID)
                {
                    case "destiny":
                    case "adrenaline":
                    case "collider":
                    case "golddust":
                    case "hr8938cephei":
                    case "ifeellove":
                    case "iwantu":
                    case "lazerface":
                    case "popstars":
                    case "perfectexceeder":
                    case "predator":
                    case "resistance":
                    case "smoke":
                    case "splinter":
                    case "synthesized":
                    case "thespace":
                    case "titanium_cazzette":
                    case "reedsofmitatrush":
                    case "destiny_full":
                    case "popstars_full":
                        SongList.I.songs[i].author = "HMXJeff";
                        break;
                    case "addictedtoamemory":
                    case "breakforme":
                    case "channel42":
                    case "everyday":
                    case "gametime":
                    case "highwaytooblivion_short":
                    case "overtime":
                    case "tothestars":
                    case "addictedtoamemory_full":
                    case "highwaytooblivion_full":
                    case "avalanche":
                    case "badguy":
                    case "believer":
                    case "betternow":
                    case "cantfeelmyface":
                    case "centuries":
                    case "countingstars":
                    case "dontletmedown":
                    case "exitwounds":
                    case "gdfr":
                    case "girlsbedancing":
                    case "intoyou":
                    case "juice":
                    case "longrun":
                    case "methanebreather":
                    case "moveslikejagger":
                    case "newrules":
                    case "sorryforpartyrocking":
                    case "starships":
                    case "stook":
                    case "thegreatest":
                    case "themiddle":
                    case "themotherweshare":
                    case "urprey":
                    case "weallbecome":
                    case "youngblood":
                        SongList.I.songs[i].author = "HMXRick";
                        break;
                    case "boomboom":
                    case "raiseyourweapon_noisia":
                    case "timeforcrime":
                        SongList.I.songs[i].author = "HMXJeff & HMXRick";
                        break;
                    case "eyeforaneye":
                    case "goatpolyphia":
                    case "illmerica":
                    case "funkycomputer":
                        SongList.I.songs[i].author = "Simon";
                        break;
                    case "loyal":
                        SongList.I.songs[i].author = "Simon & HMXRick";
                        break;
                    case "highhopes":
                    case "goodbyedearsorrows_ab42b2e6b0934471474875729b4f9934":
                    case "shatterme_30eb4181110577459bc89b8650d3386a":
                        SongList.I.songs[i].author = "aggrogahu";
                        break;
                    case "bigppwoo_0966cf748cb5f637e3b0f00feeda9d9a":
                        SongList.I.songs[i].author = "Sleepyhead";
                        break;
                    case "children-of-a-miracle_bc34b4da4eea98a2a2e7c28d378738e6":
                    case "get-jinxed_bd9d8a475804d6e2086fc3d2090ea9fb":
                    case "no-worries_b48e9121c4f412e2da920212ff375a45":
                        SongList.I.songs[i].author = "Fredrix";
                        break;
                    case "LegendsNeverDie_96f3da6e3455fc7b74535f4ad3171955":
                        SongList.I.songs[i].author = "CircuitLord";
                        break;
                    case "Camellia_The_King_of_Lions_f39276e8867fbfd9c0d9c1e99dc03052":
                        SongList.I.songs[i].author = "CriminalCannoli";
                        break;
                    case "weaponizedcelldwellershark_a0508d763c057c198291149233c4d150":
                        SongList.I.songs[i].author = "whattheshark";
                        break;
                    case "ainideaikoiwatsudzukuoctober_d674ca136c43e57ef82a92cb8f80da87":
                    case "sadmachine_a49b6978f4ab867057f8bb22bcf53580":
                        SongList.I.songs[i].author = "october";
                        break;
                    case "deviltrigger_728ff099c5d7d1f2ad0f724fb53b9b43":
                    case "onceagain_ProtoPip_0e8bb6d431dd2fdabb62a4d988c263eb":
                        SongList.I.songs[i].author = "ProtoPip";
                        break;
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
