using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ravel.DestructibleWalls
{
    [RequireComponent(typeof(Collider), typeof(AudioSource), typeof(MeshRenderer))]
    public class DestructibleWall : MonoBehaviour, IDestructibleWall
    {
        #region Events

        public static event Action<DestructibleWall, Vector3> OnAnyWallBroken;
        public event Action<DestructibleWall, Vector3> OnThisWallBroken;
        public event Action<float, Vector3> OnWallImpacted;

        #endregion

        #region Inspector Settings

        [Header("Configuration")]
        [Tooltip("Use a preset config asset for settings")]
        [SerializeField] private DestructibleWallConfig config;

        #endregion

        #region Private Variables

        private bool isBroken = false;
        private MeshRenderer mRenderer;
        private Collider wallCollider;
        private AudioSource audioPlayer;
        private Rigidbody playerRigidbody;
        private Vector3 impactVelocityBeforeCollision;

        #endregion

        #region Properties

        public bool IsBroken => isBroken;

        private float SpeedThreshold => config.SpeedThreshold;
        private float MomentumRetentionPercent => config.MomentumRetentionPercent;
        private string BreakableByTag => config.BreakableByTag;
        private DestructionBehavior WallDestructionBehavior => config.WallDestructionBehavior;
        private GameObject DestructionParticleEffectPrefab => config.DestructionParticleEffectPrefab;
        private ParticleLocation DestructionParticleLocation => config.DestructionParticleLocation;
        private GameObject BrokenWallPrefab => config.BrokenWallPrefab;
        private List<AudioClip> DestructionSounds => config.DestructionSounds;

        #endregion

        #region Logic

        void Awake()
        {
            // get required components
            mRenderer = GetComponent<MeshRenderer>();
            wallCollider = GetComponent<Collider>();
            audioPlayer = GetComponent<AudioSource>();

            // ensure collider is not a trigger to prevent no clipping
            if (wallCollider.isTrigger)
            {
                Debug.LogWarning($"[DESTRUCTIBLE WALL WARNING] - {gameObject.name}'s collider is set as trigger. Please uncheck this box to prevent unwanted behavior.", this);
            }
        }

        void OnCollisionEnter(Collision other)
        {
            // if already broken, return
            if (isBroken) return;

            // if collision is not by player, return
            if (!other.gameObject.CompareTag(BreakableByTag)) return;

            // get rigidbody to check velocity
            Rigidbody rb = other.rigidbody;
            if (rb == null)
            {
                Debug.LogWarning($"[DESTRUCTIBLE WALL WARNING] - {other.gameObject.name} has no Rigidbody. Cannot detect impact speed.", this);
                return;
            }

            // calculate impact force
            float impactForce = other.relativeVelocity.magnitude;
            Vector3 impactPoint = other.contacts.Length > 0 ? other.contacts[0].point : transform.position;
            Vector3 impactVelocity = other.relativeVelocity;

            // notify about impact
            OnImpact(impactForce, impactPoint, impactVelocity);

            // check if wall can be broken
            if (CanBreak(impactForce))
            {
                playerRigidbody = rb;
                impactVelocityBeforeCollision = other.relativeVelocity;
                Break();
            }
        }

        public bool CanBreak(float impactForce)
        {
            return !isBroken && impactForce >= SpeedThreshold;
        }

        public void OnImpact(float impactForce, Vector3 impactPoint, Vector3 impactVelocity)
        {
            OnWallImpacted?.Invoke(impactForce, impactPoint);

            // TODO: Add particles and sounds
        }

        public void Break()
        {
            if (isBroken) return;

            isBroken = true;

            Vector3 breakPos = transform.position;

            // broadcast events for those subscribed
            OnThisWallBroken?.Invoke(this, breakPos);
            OnAnyWallBroken?.Invoke(this, breakPos);

            // spawn broken wall prefab if provided
            if (BrokenWallPrefab != null)
            {
                // spawn broken wall
                GameObject brokenWall = Instantiate(BrokenWallPrefab, transform.position, transform.rotation);
            }

            // play destruction sound when wall breaks
            if (DestructionSounds.Count > 0)
            {
                PlayDestructionNoise();
            }

            // spawn particles BEFORE disabling collider so bounds are correct
            if (DestructionParticleEffectPrefab != null)
            {
                GameObject particles = Instantiate(DestructionParticleEffectPrefab, transform);

                float halfHeight = wallCollider.bounds.extents.y;

                switch (DestructionParticleLocation)
                {
                    case ParticleLocation.Top:
                        particles.transform.position = transform.position + new Vector3(0, halfHeight, 0);
                        break;

                    case ParticleLocation.Middle:
                        particles.transform.position = transform.position;
                        break;

                    case ParticleLocation.Bottom:
                        particles.transform.position = transform.position + new Vector3(0, -halfHeight, 0);
                        break;
                }
            }

            wallCollider.enabled = false;

            if (playerRigidbody != null && impactVelocityBeforeCollision != Vector3.zero)
            {
                StartCoroutine(RestorePlayerVelocity(playerRigidbody, impactVelocityBeforeCollision));
            }

            HandleRemoveWall();
        }

        private IEnumerator RestorePlayerVelocity(Rigidbody playerRb, Vector3 velocity)
        {
            yield return new WaitForFixedUpdate();

            if (playerRb != null)
            {
                playerRb.linearVelocity = velocity * (MomentumRetentionPercent / 100f);
            }
        }

        private void HandleRemoveWall()
        {
            // hide visual so sound effect can play before destruction
            mRenderer.enabled = false;
            wallCollider.enabled = false;

            StartCoroutine(WaitThenRemove());
        }

        private IEnumerator WaitThenRemove()
        {
            yield return new WaitForSeconds(3f);

            switch (WallDestructionBehavior)
            {
                case DestructionBehavior.UnbreakableObject:
                    break;

                case DestructionBehavior.DestroyWallObject:
                    Destroy(gameObject);
                    break;

                case DestructionBehavior.DisableWallObject:
                    gameObject.SetActive(false);
                    break;
            }
        }

        private void PlayDestructionNoise()
        {
            audioPlayer.PlayOneShot(DestructionSounds[UnityEngine.Random.Range(0, DestructionSounds.Count)]);
        }

        #endregion

        #region Utility Methods

        public void ForceBreak()
        {
            Break();
        }

        public void Reset()
        {
            isBroken = false;

            gameObject.SetActive(true);
            wallCollider.enabled = true;
        }

        #endregion
    }
}
