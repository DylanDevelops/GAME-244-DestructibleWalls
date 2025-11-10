using UnityEngine;

namespace Ravel.DestructibleWalls
{
    public interface IDestructibleWall
    {
        bool IsBroken { get; }

        bool CanBreak(float impactForce);
        void OnImpact(float impactForce, Vector3 impactPoint, Vector3 impactVelocity);
        void Break();
    }
}
