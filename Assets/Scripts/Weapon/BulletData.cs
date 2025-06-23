using Unity.Jobs;
using Unity.Mathematics;

namespace Weapon
{
    public struct BulletData
    {
        public float3 position;
        public float3 velocity;
        public bool isActive;
        public float radius;
    }
}