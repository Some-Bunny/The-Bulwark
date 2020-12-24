using ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using System.Reflection;
using MonoMod.RuntimeDetour;
using Dungeonator;
using CustomCharacters;

namespace TheBulwark
{
    public class BulwarkModule : ETGModule
    {
        public static readonly string MOD_NAME = "The Bulwark";
        public static readonly string VERSION = "1.0.0";
        public static readonly string TEXT_COLOR = "#ffbc00";

        public override void Start()
        {
            FakePrefabHooks.Init();
            Hooks.Init();
            ItemBuilder.Init();
            //HolyIncense.Init();
            HolyProtection.Init();
            Esense.Init();
            RailBlade.Add();
            Log($"{MOD_NAME} v{VERSION} started successfully.", TEXT_COLOR);
        }
        public static string GetTalkingPlayerNickHook(Func<string> orig)
        {
            PlayerController talkingPlayer = GetTalkingPlayer();
            if (talkingPlayer.IsThief)
            {
                return "#THIEF_NAME";
            }
            if (talkingPlayer.GetComponent<CustomCharacter>() != null)
            {
                if (talkingPlayer.GetComponent<CustomCharacter>().data != null)
                {
                    return "#PLAYER_NICK_" + talkingPlayer.GetComponent<CustomCharacter>().data.nameShort.ToUpper();
                }
            }
            return orig();
        }

        public static string GetValueHook(Func<dfLanguageManager, string, string> orig, dfLanguageManager self, string key)
        {
            if (characterDeathNames.Contains(key))
            {
                if (GameManager.Instance.PrimaryPlayer != null && GameManager.Instance.PrimaryPlayer.GetComponent<CustomCharacter>() != null && GameManager.Instance.PrimaryPlayer.GetComponent<CustomCharacter>().data != null)
                {
                    return GameManager.Instance.PrimaryPlayer.GetComponent<CustomCharacter>().data.name;
                }
            }
            return orig(self, key);
        }

        public static string GetTalkingPlayerNameHook(Func<string> orig)
        {
            PlayerController talkingPlayer = GetTalkingPlayer();
            if (talkingPlayer.IsThief)
            {
                return "#THIEF_NAME";
            }
            if (talkingPlayer.GetComponent<CustomCharacter>() != null)
            {
                if (talkingPlayer.GetComponent<CustomCharacter>().data != null)
                {
                    return "#PLAYER_NAME_" + talkingPlayer.GetComponent<CustomCharacter>().data.nameShort.ToUpper();
                }
            }
            return orig();
        }
        private static PlayerController GetTalkingPlayer()
        {
            List<TalkDoerLite> allNpcs = StaticReferenceManager.AllNpcs;
            for (int i = 0; i < allNpcs.Count; i++)
            {
                if (allNpcs[i])
                {
                    if (!allNpcs[i].IsTalking || !allNpcs[i].TalkingPlayer || GameManager.Instance.HasPlayer(allNpcs[i].TalkingPlayer))
                    {
                        if (allNpcs[i].IsTalking && allNpcs[i].TalkingPlayer)
                        {
                            return allNpcs[i].TalkingPlayer;
                        }
                    }
                }
            }
            return GameManager.Instance.PrimaryPlayer;
        }
        //GameManager.Instance.RewardManager.GunMimicMimicGunChance
        public static void Mimicgunpog(Action<Gun> orig,Gun spawnedGun)
        {
            //Gun spawnedGun = Gun;
            spawnedGun.gameObject.SetActive(true);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ITEMSPECIFIC_HAS_BEEN_PEDESTAL_MIMICKED) && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.NONE && UnityEngine.Random.value < 1)
            {
                spawnedGun.gameObject.AddComponent<MimicGunMimicModifier>();
            }
        }
        public static void Log(string text, string color= "#ffbc00")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }
        public static List<string> characterDeathNames = new List<string>
        {
            "#CHAR_ROGUE_SHORT",
            "#CHAR_CONVICT_SHORT",
            "#CHAR_ROBOT_SHORT",
            "#CHAR_MARINE_SHORT",
            "#CHAR_GUIDE_SHORT",
            "#CHAR_CULTIST_SHORT",
            "#CHAR_BULLET_SHORT",
            "#CHAR_PARADOX_SHORT",
            "#CHAR_GUNSLINGER_SHORT"
        };
        public override void Exit() { }
        public override void Init() { }
    }
}


namespace TheBulwark
{
	// Token: 0x02000018 RID: 24
	public static class Hooks
	{
		// Token: 0x060000B7 RID: 183 RVA: 0x00008CE4 File Offset: 0x00006EE4
		public static void Init()
		{
			try
			{

				//Hook hook = new Hook(typeof(LootEngine).GetMethod("PostprocessGunSpawn", BindingFlags.Static | BindingFlags.NonPublic), typeof(Module).GetMethod("Mimicgunpog"));
                Hook hook1 = new Hook(typeof(PlayerController).GetMethod("HandleStartDodgeRoll", BindingFlags.Instance | BindingFlags.NonPublic), typeof(HolyProtection).GetMethod("rollingbad"));
                //Hook hook1 = new Hook(typeof(PlayerController).GetMethod("CheckDodgeRollDepth", BindingFlags.Instance | BindingFlags.NonPublic), typeof(HolyProtection).GetMethod("rollingbad"));
               
                Hook getNicknamehook = new Hook(typeof(StringTableManager).GetMethod("GetTalkingPlayerNick", BindingFlags.NonPublic | BindingFlags.Static),typeof(BulwarkModule).GetMethod("GetTalkingPlayerNickHook"));

                Hook getNamehook = new Hook(typeof(StringTableManager).GetMethod("GetTalkingPlayerName", BindingFlags.NonPublic | BindingFlags.Static),typeof(BulwarkModule).GetMethod("GetTalkingPlayerNameHook"));

                Hook getValueHook = new Hook(typeof(dfLanguageManager).GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance),typeof(BulwarkModule).GetMethod("GetValueHook"));
            }
            catch (Exception e)
			{
				Tools.PrintException(e, "FF0000");
			}
		}

	}
}