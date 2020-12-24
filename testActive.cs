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
    public class Esense : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Holy Incense";
            string resourceName = "TheBulwark/Resources/holyincense.png";
            GameObject obj = new GameObject(itemName);
            Esense testActive = obj.AddComponent<Esense>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "CHAAARGE!";
            string longDesc = "A blessed relic that fills up with empowering essences. Those who drink from it are cleansed and temporarily percieve time differently.";
            testActive.SetupItem(shortDesc, longDesc, "blw");
            testActive.SetCooldownType(ItemBuilder.CooldownType.Timed, 2f);
            testActive.consumable = false;
            testActive.quality = PickupObject.ItemQuality.EXCLUDED;
            Esense.spriteIDs = new int[Esense.spritePaths.Length];
            Esense.spriteIDs[0] = SpriteBuilder1.AddSpriteToCollection(Esense.spritePaths[0], testActive.sprite.Collection);
            Esense.spriteIDs[1] = SpriteBuilder1.AddSpriteToCollection(Esense.spritePaths[1], testActive.sprite.Collection);
            Esense.spriteIDs[2] = SpriteBuilder1.AddSpriteToCollection(Esense.spritePaths[2], testActive.sprite.Collection);
            Esense.spriteIDs[3] = SpriteBuilder1.AddSpriteToCollection(Esense.spritePaths[3], testActive.sprite.Collection);
            Esense.spriteIDs[4] = SpriteBuilder1.AddSpriteToCollection(Esense.spritePaths[4], testActive.sprite.Collection);
            Esense.spriteIDs[5] = SpriteBuilder1.AddSpriteToCollection(Esense.spritePaths[5], testActive.sprite.Collection);
            Esense.spriteIDs[6] = SpriteBuilder1.AddSpriteToCollection(Esense.spritePaths[6], testActive.sprite.Collection);
        }
        public override void Pickup(PlayerController player)
        {
            this.CanBeDropped = false;
            base.Pickup(player);
        }
        public override void Update()
        {
            PlayerController player = this.LastOwner;
            bool a = player;
            bool flag2 = a;
            bool flag3 = flag2;
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
                    this.id = Esense.spriteIDs[charges];
                }
                base.sprite.SetSprite(this.id);
            }
            base.Update();
        }
        private void OnEnemyDamaged(float damage, bool fatal, HealthHaver enemy)
        {
            this.damagecharge += damage;
            bool flag = this.damagecharge > 250f;
            bool flag2 = flag;
            if (flag2)
            {
                if (charges != 4)
                {
                    AkSoundEngine.PostEvent("Play_BOSS_doormimic_appear_01", base.gameObject);
                    this.damagecharge = 0f;
                    this.charges += 1;
                }

            }
        }
        private void GrantCharge()
        {
            this.damagecharge = 0;
            if (charges != 4)
            {
                AkSoundEngine.PostEvent("Play_BOSS_doormimic_appear_01", base.gameObject);
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
            bool ners = target.HasPickupID(190);
            if (ners)
            {
                SpeculativeRigidbody specRigidbody = target.specRigidbody;
                specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision));

            }
            StartCoroutine(HandleShield(target));
            AkSoundEngine.PostEvent("Play_ENM_highpriest_dash_01", base.gameObject);
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
            bool fuckyoueatshit = user.HasPickupID(665);
            if (!fuckyoueatshit)
            {
                user.IsGunLocked = true;
            }
            m_invactiveDuration = this.duration;
            SpeculativeRigidbody specRigidbody = user.specRigidbody;
            this.activateSlow(user);
            user.healthHaver.IsVulnerable = false;
            float elapsed = 0f;
            while (elapsed < this.duration)
            {
                bool fuckyoueatshitagain = user.HasPickupID(193);
                if (fuckyoueatshitagain)
                {
                    AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
                    GoopDefinition goopDef = assetBundle.LoadAsset<GoopDefinition>("assets/data/goops/poison goop.asset");
                    DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDef).TimedAddGoopCircle(user.sprite.WorldBottomCenter, 2f, 1f, false);
                }
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
                //SpeculativeRigidbody specRigidbody = target.specRigidbody;
                specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision));
                user.IsGunLocked = false;
                AkSoundEngine.PostEvent("Play_WPN_Life_Orb_Fade_01", base.gameObject);
            }
            PlayerController player = base.LastOwner as PlayerController;
            yield break;
        }
        private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            PlayerController player = (GameManager.Instance.PrimaryPlayer);
            Projectile component = otherRigidbody.GetComponent<Projectile>();
            if (component != null && !(component.Owner is PlayerController))
            {
                PassiveReflectItem.ReflectBullet(component, true, player.specRigidbody.gameActor, 10f, 1f, 1f, 0f);
                PhysicsEngine.SkipCollision = true;
            }
        }
        protected void activateSlow(PlayerController user)
        {
            new RadialSlowInterface
            {
                DoesSepia = true,
                RadialSlowHoldTime = 1.5f,
                RadialSlowTimeModifier = 0.166f
            }.DoRadialSlow(user.specRigidbody.UnitCenter, user.CurrentRoom);

        }
        public override bool CanBeUsed(PlayerController user)
        {
            if (charges == 0)
            {
                return false;
            }
            else
            {
                return true;
            }     
        }
        float m_invactiveDuration = 1.5f;
        float duration = 1.5f;
        private static int[] spriteIDs;
        private static readonly string[] spritePaths = new string[]
        {
            "TheBulwark/Resources/holyincense.png",
            "TheBulwark/Resources/holyincense1.png",
            "TheBulwark/Resources/holyincense2.png",
            "TheBulwark/Resources/holyincense3.png",
            "TheBulwark/Resources/holyincense4.png",
            "TheBulwark/Resources/holyincense5.png",
            "TheBulwark/Resources/holyincense6.png"
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

        public int NumProjectilesToFire = 5;

        // Token: 0x04007545 RID: 30021
        public float ProjectileArcAngle = 45f;

        // Token: 0x04007546 RID: 30022
        public float FireCooldown = 2f;

        // Token: 0x04007547 RID: 30023

        // Token: 0x04007548 RID: 30024

        // Token: 0x04007549 RID: 30025
        [Header("Synergues")]
        public ExplosionData BlastBootsExplosion;

        // Token: 0x0400754A RID: 30026
    }
}