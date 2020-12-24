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



namespace TheBulwark
{
    public class HolyIncense : PlayerItem
    {
        public static void Init()
        {           
            string itemName = "Holy Incense";
            string resourceName = "TheBulwark/Resources/holyincense";
            GameObject obj = new GameObject(itemName);
            HolyIncense dioheart = obj.AddComponent<HolyIncense>();
            ItemBuilder.AddPassiveStatModifier(dioheart, PlayerStats.StatType.AdditionalItemCapacity, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "aa";
            string longDesc = "AAAAA.";
            dioheart.SetupItem(shortDesc, longDesc, "blw");
            dioheart.SetCooldownType(ItemBuilder.CooldownType.Timed, 2f);
            dioheart.consumable = false;
            dioheart.quality = PickupObject.ItemQuality.EXCLUDED;
            //HolyIncense.poggersheart = dioheart.PickupObjectId;
            HolyIncense.spriteIDs = new int[HolyIncense.spritePaths.Length];
            HolyIncense.spriteIDs[0] = ItemAPI.SpriteBuilder1.AddSpriteToCollection(HolyIncense.spritePaths[0], dioheart.sprite.Collection);
            HolyIncense.spriteIDs[1] = SpriteBuilder1.AddSpriteToCollection(HolyIncense.spritePaths[1], dioheart.sprite.Collection);
            HolyIncense.spriteIDs[2] = SpriteBuilder1.AddSpriteToCollection(HolyIncense.spritePaths[2], dioheart.sprite.Collection);
            HolyIncense.spriteIDs[3] = SpriteBuilder1.AddSpriteToCollection(HolyIncense.spritePaths[3], dioheart.sprite.Collection);
            HolyIncense.spriteIDs[4] = SpriteBuilder1.AddSpriteToCollection(HolyIncense.spritePaths[4], dioheart.sprite.Collection);
            HolyIncense.spriteIDs[5] = SpriteBuilder1.AddSpriteToCollection(HolyIncense.spritePaths[5], dioheart.sprite.Collection);
            HolyIncense.spriteIDs[6] = SpriteBuilder1.AddSpriteToCollection(HolyIncense.spritePaths[6], dioheart.sprite.Collection);
        }

        public override void Pickup(PlayerController player)
        {
            this.CanBeDropped = false;
            base.Pickup(player);
        }
        public override void Update()
        {
            PlayerController player = this.LastOwner;
            bool flag3 = player.HasPickupID(Game.Items["blw:holy_incense"].PickupObjectId);
            if (flag3)
            {
                bool flag = this.Countdown == 0;
                if (flag)
                {
                    player.OnAnyEnemyReceivedDamage = (Action<float, bool, HealthHaver>)Delegate.Combine(player.OnAnyEnemyReceivedDamage, new Action<float, bool, HealthHaver>(this.OnEnemyDamaged));
                    player.OnEnteredCombat += this.GrantCharge;
                    this.Countdown += 1f;
                }
                {
                    this.id = HolyIncense.spriteIDs[charges];
                }
                base.sprite.SetSprite(this.id);
            }
            //base.Update();C
        }
        private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemy)
        {
            this.damagecharge += damage;
            bool flag = this.damagecharge > 200f;
            bool flag2 = flag;
            if (flag2)
            {
                if (charges <= 6)
                {
                    AkSoundEngine.PostEvent("Play_ENM_book_blast_01", base.gameObject);
                    this.damagecharge = 0f;
                    this.charges += 1;
                }
            }
        }   
        private void GrantCharge()
        {
            if (this.charges <= 6)
            {
                AkSoundEngine.PostEvent("Play_ENM_book_blast_01", base.gameObject);
                this.charges += 1;
            }
        }
        protected override void DoEffect(PlayerController user)
        {
            this.charges -= 1;
            user.StartCoroutine(this.HandleDash(user));
        }
        public IEnumerator HandleDash(PlayerController target)
        {
            StartCoroutine(HandleShield(target));
            AkSoundEngine.PostEvent("Play_ENM_bulletking_charge_01", base.gameObject);
            target.IsOnFire = false;
            this.CurrentFireMeterValue = 0f;
            this.CurrentPoisonMeterValue = 0f;
            this.CurrentCurseMeterValue = 0f;
            this.CurrentDrainMeterValue = 0f;
            float duration = 0f;
            float elapsed = -BraveTime.DeltaTime;
            float angle = base.LastOwner.CurrentGun.CurrentAngle;
            float adjSpeed = 0f;
            target.ReceivesTouchDamage = false;
            target.SetIsFlying(true, "DEUS VULT", true, false);
            duration = 0.1f;
            adjSpeed = 40f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                base.LastOwner.specRigidbody.Velocity = BraveMathCollege.DegreesToVector(angle, 1f).normalized * adjSpeed;
                yield return null;
            }
            target.ReceivesTouchDamage = true;
            target.SetIsFlying(false, "DEUS VULT", true, false);
            yield break;
        }
        private IEnumerator HandleShield(PlayerController user)
        {
            user.IsGunLocked = true;
            m_invactiveDuration = this.duration;
            SpeculativeRigidbody specRigidbody = user.specRigidbody;
            this.activateSlow(user);
            user.healthHaver.IsVulnerable = false;
            float elapsed = 0f;
            while (elapsed < this.duration)
            {
                user.IsOnFire = false;
                elapsed += BraveTime.DeltaTime;
                user.healthHaver.IsVulnerable = false;
                yield return null;
            }
            if (user)
            {
                user.healthHaver.IsVulnerable = true;
                user.ClearOverrideShader();
                SpeculativeRigidbody specRigidbody2 = user.specRigidbody;
            }
            if (this)
            {
                user.IsGunLocked = false;
                AkSoundEngine.PostEvent("Play_OBJ_metalskin_end_01", base.gameObject);
            }
            PlayerController player = base.LastOwner as PlayerController;
            yield break;
        }
        protected void activateSlow(PlayerController user)
        {
            new RadialSlowInterface
            {
                DoesSepia = true,
                RadialSlowHoldTime = 1f,
                RadialSlowTimeModifier = 0f
            }.DoRadialSlow(user.specRigidbody.UnitCenter, user.CurrentRoom);

        }
        float m_invactiveDuration = 1f;
        float duration = 1f;
        private static int[] spriteIDs;
        private static readonly string[] spritePaths = new string[]
        {
            "TheBulwark/Resources/holyincense",
            "TheBulwark/Resources/holyincense1",
            "TheBulwark/Resources/holyincense2",
            "TheBulwark/Resources/holyincense3",
            "TheBulwark/Resources/holyincense4",
            "TheBulwark/Resources/holyincense5",
            "TheBulwark/Resources/holyincense6"
        };
        private int charges = 0;
        private float damagecharge;
        private int id;
        public float CurrentFireMeterValue;
        public bool eatshit;
        public float CurrentPoisonMeterValue;
        public float CurrentDrainMeterValue;
        public float CurrentCurseMeterValue;

        private float Countdown = 0;

    }
}