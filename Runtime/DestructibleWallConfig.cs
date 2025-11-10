using System.Collections.Generic;
using UnityEngine;

namespace Ravel.DestructibleWalls
{
    public enum DestructionBehavior
    {
        UnbreakableObject,
        DestroyWallObject,
        DisableWallObject,
    }

    public enum ParticleLocation
    {
        Top,
        Middle,
        Bottom,
    }

    [CreateAssetMenu(fileName = "DestructibleWallConfig", menuName = "Ravel/Destructible Wall Config")]
    public class DestructibleWallConfig : ScriptableObject
    {
        [Header("Impact Settings")]
        [Tooltip("Minimum speed required to break the wall")]
        [SerializeField] private float speedThreshold = 10f;

        [Tooltip("Percentage of player momentum retained after breaking through wall")]
        [Range(0f, 100f)]
        [SerializeField] private float momentumRetentionPercent = 100f;

        [Tooltip("Tag of the object that can break walls")]
        [SerializeField] private string breakableByTag = "Player";

        [Header("Destruction Settings")]
        [Tooltip("The behavior of the wall when hit")]
        [SerializeField] private DestructionBehavior destructionBehavior = DestructionBehavior.DestroyWallObject;

        [Header("Optional Settings")]
        [Tooltip("(OPTIONAL) A prefab containing a particle effect instantiated when a wall is destroyed")]
        [SerializeField] private GameObject destructionParticleEffectPrefab;
        
        [Tooltip("(OPTIONAL) The location where the origin of the particle effect will be instantiated")]
        [SerializeField] private ParticleLocation particleLocation;

        [Tooltip("(OPTIONAL) A prefab to replace the wall with when broken")]
        [SerializeField] private GameObject brokenWallPrefab;

        [Tooltip("(OPTIONAL) A list of audio sound effects that can play when wall is destroyed")]
        [SerializeField] private List<AudioClip> destructionSounds;

        // public accessors
        public float SpeedThreshold => speedThreshold;
        public float MomentumRetentionPercent => momentumRetentionPercent;
        public string BreakableByTag => breakableByTag;
        public DestructionBehavior WallDestructionBehavior => destructionBehavior;
        public GameObject DestructionParticleEffectPrefab => destructionParticleEffectPrefab;
        public ParticleLocation DestructionParticleLocation => particleLocation;
        public GameObject BrokenWallPrefab => brokenWallPrefab;
        public List<AudioClip> DestructionSounds => destructionSounds;

        void OnValidate()
        {
            speedThreshold = Mathf.Max(0f, speedThreshold);

            if(string.IsNullOrEmpty(breakableByTag))
            {
                breakableByTag = "Player";
            }
        }
    }
}
