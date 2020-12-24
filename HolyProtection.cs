using System;
using UnityEngine;
using ItemAPI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoMod.RuntimeDetour;
using System.Reflection;
using MonoMod.Utils;
using Dungeonator;
using Brave.BulletScript;
using Random = System.Random;
using FullSerializer;
using Gungeon;

namespace TheBulwark
{
    public class HolyProtection : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Holy Protection";

            string resourceName = "TheBulwark/Resources/holyprotection.png";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<HolyProtection>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Kalibers Strength";
            string longDesc = "The Power of Kaliber is bestowed upon you! Grants the ability to traverse pits for a small period of time, weak immunity to fire, and slightly increases health drop rate.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "blw");
			item.quality = PickupObject.ItemQuality.EXCLUDED;
        }


		private bool HandleAboutToFall(bool partialPit)
		{
			PlayerController player = (GameManager.Instance.PrimaryPlayer);
			bool aa = player.HasPickupID(667);
			if (!aa)
            {
				Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(player.sprite);
				float num = 0.333f;
				if (!base.Owner.IsInCombat)
				{
					num += 0.667f;
				}
				bool fuckyoueatshit = player.HasPickupID(526);
				if (fuckyoueatshit)
				{
					num *= 2;
				}
				if (base.Owner && base.Owner.IsFlying)
				{
					return false;
				}
				this.m_frameWasPartialPit = partialPit;
				this.m_wasAboutToFallLastFrame = true;
				if (Time.frameCount <= this.m_lastFrameAboutToFall)
				{
					this.m_lastFrameAboutToFall = Time.frameCount - 1;
				}
				if (Time.frameCount != this.m_lastFrameAboutToFall + 1)
				{
					this.m_elapsedAboutToFall = 0f;
				}
				if (partialPit)
				{
					outlineMaterial.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
					this.m_elapsedAboutToFall = 0f;
				}
				this.m_lastFrameAboutToFall = Time.frameCount;
				this.m_elapsedAboutToFall += BraveTime.DeltaTime;
				if (this.m_elapsedAboutToFall < num)
				{
					if (!this.m_extantFloor)
					{
						if (!partialPit)
						{
							outlineMaterial.SetColor("_OverrideColor", new Color((num / num) / ((num / (num - m_elapsedAboutToFall))) * 120, (num / num) / ((num / (num - m_elapsedAboutToFall))) * 120, 0f));

						}
						/*
						GameObject gameObject = SpawnManager.SpawnVFX(this.FloorVFX, false);
						gameObject.transform.parent = base.Owner.transform;
						tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
						component.PlaceAtPositionByAnchor(base.Owner.SpriteBottomCenter, tk2dBaseSprite.Anchor.MiddleCenter);
						component.IsPerpendicular = false;
						component.HeightOffGround = -2.25f;
						component.UpdateZDepth();
						this.m_extantFloor = component;
						*/
					}
					if (this.m_elapsedAboutToFall > num - this.FlickerPortion)
					{
						bool enabled = Mathf.PingPong(this.m_elapsedAboutToFall - (num - this.FlickerPortion), this.FlickerFrequency * 2f) < this.FlickerFrequency;
						this.m_extantFloor.renderer.enabled = enabled;
					}
					else
					{
						outlineMaterial.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
						this.m_extantFloor.renderer.enabled = true;
					}
					return false;
				}
				if (this.m_extantFloor)
				{
					outlineMaterial.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
					SpawnManager.Despawn(this.m_extantFloor.gameObject);
					this.m_extantFloor = null;
				}
				outlineMaterial.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
				return true;
			}
			return true;
		}




		private void RoomCleared(PlayerController obj)
		{
			float value = UnityEngine.Random.value;
			if (obj.healthHaver.GetCurrentHealthPercentage() >= 1f)
			{
				return;
			}
			if (value < this.chanceToSpawn)
			{
				PickupObject byId = PickupObjectDatabase.GetById(73);
				LootEngine.SpawnItem(byId.gameObject, obj.specRigidbody.UnitCenter, Vector2.up, 1f, false, true, false);
			}
		}


        public override void Pickup(PlayerController player)
		{
			player.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OopsDamage);
			this.CanBeDropped = false;
			base.Pickup(player);
			player.OnRoomClearEvent += this.RoomCleared;
			player.OnAboutToFall = (Func<bool, bool>)Delegate.Combine(player.OnAboutToFall, new Func<bool, bool>(this.HandleAboutToFall));
		}
		private void ACTIVATETHESTUFF(PlayerController user)
		{
			LootEngine.TryGivePrefabToPlayer(ETGMod.Databases.Items["Holy Incense"].gameObject, base.Owner, true);

		}
		public override DebrisObject Drop(PlayerController player)
        {
			player.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OopsDamage);
			player.OnRoomClearEvent -= this.RoomCleared;
			player.OnAboutToFall = (Func<bool, bool>)Delegate.Remove(player.OnAboutToFall, new Func<bool, bool>(this.HandleAboutToFall));
			return base.Drop(player);
        }
		private void OopsDamage(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
		{
			PlayerController player = (GameManager.Instance.PrimaryPlayer);
			player.IsOnFire = false;
			this.CurrentFireMeterValue = 0f;
			this.CurrentPoisonMeterValue = 0f;
			this.CurrentCurseMeterValue = 0f;
			this.CurrentDrainMeterValue = 0f;
		}
		protected override void Update()
		{
			bool flag = base.Owner;
			if (flag)
			{
				/*
				if (!base.Owner.IsInCombat)
				{
					this.EnableVFX(base.Owner);
				}
				else
                {
					this.DisableVFX(base.Owner);
				}
			*/
			}
		}
		private void EnableVFX(PlayerController user)
		{
			Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(user.sprite);
			outlineMaterial.SetColor("_OverrideColor", new Color(15f, 15f, 1f));
		}
		private void DisableVFX(PlayerController user)
		{
			Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(user.sprite);
			outlineMaterial.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
		}

		public static bool rollingbad(Action<PlayerController, Vector2> orig, PlayerController self, Vector2 direction)
        {
			PlayerController player = (GameManager.Instance.PrimaryPlayer);
			bool fuvkyounorolling = player.HasPickupID(Game.Items["blw:holy_protection"].PickupObjectId) && !player.IsInMinecart;
			if (!fuvkyounorolling)
			{
				orig(self, direction);
				return true;
			}
			else
			{
				return false;
			}

		}


		public float HoverTime = 1.33f;

		// Token: 0x04007519 RID: 29977
		public float FlickerPortion = 1.0f;

		// Token: 0x0400751A RID: 29978
		public float FlickerFrequency = 0.33f;

		// Token: 0x0400751B RID: 29979
		public GameObject FloorVFX;

		// Token: 0x0400751C RID: 29980
		public tk2dSpriteAnimation RatAnimationLibrary;

		// Token: 0x0400751D RID: 29981
		private tk2dSprite m_extantFloor;

		// Token: 0x0400751E RID: 29982
		//private bool m_transformed;

		// Token: 0x0400751F RID: 29983
		//private PlayerController m_lastPlayer;

		// Token: 0x04007520 RID: 29984
		private bool m_frameWasPartialPit;

		// Token: 0x04007521 RID: 29985
		//private bool m_invulnerable;

		// Token: 0x04007522 RID: 29986
		public bool m_wasAboutToFallLastFrame;

		// Token: 0x04007523 RID: 29987
		private float m_elapsedAboutToFall;

		// Token: 0x04007524 RID: 29988
		private int m_lastFrameAboutToFall;


		public float chanceToSpawn = 0.066f;

		// Token: 0x04007A91 RID: 31377
		[PickupIdentifier]
		public int spawnItemId = 73;
		public float CurrentFireMeterValue;
		public float CurrentPoisonMeterValue;
		public float CurrentDrainMeterValue;
		public float CurrentCurseMeterValue;
	}
}