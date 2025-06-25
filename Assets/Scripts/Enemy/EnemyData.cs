using Unity.Mathematics;

namespace Enemy
{
    public struct EnemyData
    {
        public float3 Position;
        public bool IsActive;
        public float Radius;
        public int Health;
        public int PrevCol;
        public int PrevRow;
        public bool HasValidPrevCell;
    }
}