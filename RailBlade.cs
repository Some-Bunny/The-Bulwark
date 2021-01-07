using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Reflection;
using Random = System.Random;
using FullSerializer;
using System.Collections;
using Gungeon;
using MonoMod.RuntimeDetour;
using MonoMod;
using System.Collections.ObjectModel;

using UnityEngine.Serialization;

namespace TheBulwark
{
	// Token: 0x0200002D RID: 45
	public class RailBlade : GunBehaviour
	{
		// Token: 0x0600013F RID: 319 RVA: 0x0000D468 File Offset: 0x0000B668
		public static void Add()
		{
			Gun gun = ETGMod.Databases.Items.NewGun("RailBlade", "railblade");
			Game.Items.Rename("outdated_gun_mods:railblade", "blw:railblade");
			var behav = gun.gameObject.AddComponent<RailBlade>();
			//behav.preventNormalReloadAudio = true;
			//behav.overrideNormalReloadAudio = "Play_BOSS_doormimic_appear_01";
			GunExt.SetShortDescription(gun, "Heirloom");
			GunExt.SetLongDescription(gun, "A blade pillaged from an infamous Swordtress, retro-fitted with ranged capabilities.\n\nIn an emergency, the sharp blade can cleave through bullets.");
			GunExt.SetupSprite(gun, null, "railblade_idle_001", 8);
			GunExt.SetAnimationFPS(gun, gun.shootAnimation, 24);
			GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 24);
			GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 5);
			GunExt.SetAnimationFPS(gun, gun.idleAnimation, 1);
			GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(576) as Gun, true, false);
			//gun.gunSwitchGroup = (PickupObjectDatabase.GetById(543) as Gun).gunSwitchGroup;
			gun.DefaultModule.ammoCost = 1;
			gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
			gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
			gun.reloadTime = 0.8f;
			gun.DefaultModule.cooldownTime = 0.18f;
			gun.DefaultModule.numberOfShotsInClip = 8;
			gun.SetBaseMaxAmmo(90);
			gun.InfiniteAmmo = true;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_WPN_dl45heavylaser_shot_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].eventAudio = "Play_WPN_blasphemy_shot_01";
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[0].triggerEvent = true;
			gun.barrelOffset.transform.localPosition = new Vector3(2.0f, 0.4375f, 0f);
			gun.carryPixelOffset += new IntVector2((int)4f, (int)0f);
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
			gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;
			gun.encounterTrackable.EncounterGuid = "PEW PEW PEW PEW";
			Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(576) as Gun).DefaultModule.projectiles[0]);
			projectile.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile);
			gun.DefaultModule.projectiles[0] = projectile;
			projectile.baseData.damage = 7f;
			projectile.baseData.speed *= 0.7f;
			projectile.baseData.force *= 1f;
			projectile.baseData.range *= 1f;
			projectile.AdditionalScaleMultiplier = 0.8f;
			projectile.transform.parent = gun.barrelOffset;
			projectile.AdditionalScaleMultiplier *= 0.5f;
			projectile.HasDefaultTint = true;
			projectile.DefaultTintColor = new Color32(255, 255, 0, 255);
			Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(576) as Gun).DefaultModule.projectiles[0]);
			projectile2.gameObject.SetActive(false);
			FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
			UnityEngine.Object.DontDestroyOnLoad(projectile2);
			gun.DefaultModule.projectiles[0] = projectile2;
			//gun.gunHandedness = GunHandedness.HiddenOneHanded;
			projectile2.baseData.damage = 13f;
			projectile2.baseData.speed *= 0.8f;
			projectile2.baseData.force *= 1f;
			projectile2.baseData.range = 100f;
			projectile2.AdditionalScaleMultiplier *= 1.33f;
			projectile2.transform.parent = gun.barrelOffset;
			projectile2.HasDefaultTint = true;
			projectile2.DefaultTintColor = new Color32(255, 255, 0, 255);
			//projectile2.PenetratesInternalWalls = true;
			HomingModifier homing = projectile2.gameObject.AddComponent<HomingModifier>();
			homing.HomingRadius = 250f;
			homing.AngularVelocity = 250f;
			ProjectileModule.ChargeProjectile item = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile,
				ChargeTime = 0f
			};
			ProjectileModule.ChargeProjectile item2 = new ProjectileModule.ChargeProjectile
			{
				Projectile = projectile2,
				ChargeTime = 0.9f
			};
			gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
			{
				item,
				item2
			};
			gun.quality = PickupObject.ItemQuality.EXCLUDED;
			ETGMod.Databases.Items.Add(gun, null, "ANY");
		}

		private bool HasReloaded;

		public Vector3 projectilePos;
		public override void OnPostFired(PlayerController player, Gun flakcannon)
		{
			gun.PreventNormalFireAudio = true;
			//AkSoundEngine.PostEvent("Play_WPN_dl45heavylaser_shot_01", gameObject);
		}
		public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
		{
			if (gun.IsReloading && this.HasReloaded)
			{
				HasReloaded = false;
				AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
				base.OnReloadPressed(player, gun, bSOMETHING);
				AkSoundEngine.PostEvent("Play_WPN_blasphemy_shot_01", base.gameObject);
				Vector2 unitCenter = player.specRigidbody.GetUnitCenter(ColliderType.HitBox);
				SilencerInstance.DestroyBulletsInRange(unitCenter, 4.5f, true, false, null, false, null, false, null);
			}
			/*
			float num = (this.gun.CurrentOwner.sprite.WorldCenter).magnitude * 1.25f;
			float num2 = 30f;
			float num3 = num * num;
			ReadOnlyCollection<Projectile> allProjectiles2 = StaticReferenceManager.AllProjectiles;
			for (int j = allProjectiles2.Count - 1; j >= 0; j--)
			{
				Projectile projectile2 = allProjectiles2[j];
				if (projectile2 && (!(projectile2.Owner is PlayerController) || projectile2.ForcePlayerBlankable))
				{
					if (!(projectile2.Owner is AIActor) || (projectile2.Owner as AIActor).IsNormalEnemy)
					{
						Vector2 worldCenter2 = this.gun.CurrentOwner.sprite.WorldCenter;
						float num5 = Vector2.SqrMagnitude(worldCenter2);
						if (num5 < num3)
						{
							float target2 = BraveMathCollege.Atan2Degrees(worldCenter2);
							if (Mathf.DeltaAngle(this.gun.CurrentAngle, target2) < num2)
							{
								projectile2.DieInAir(false, true, true, true);
							}
						}
					}
				}
			}
			*/
		}
		public float blankReloadRadius = 1f;




		protected void Update()
		{
			if (gun.CurrentOwner)
			{

				if (!gun.PreventNormalFireAudio)
				{
					this.gun.PreventNormalFireAudio = true;
				}
				if (!gun.IsReloading && !HasReloaded)
				{
					this.HasReloaded = true;
				}
			}
		}
	}
}



