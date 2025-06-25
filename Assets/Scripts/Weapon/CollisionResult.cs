namespace Weapon
{
    public struct CollisionResult
    {
        public bool IsHit;
        public int EnemyIndex;
        
        public CollisionResult(bool isHit, int enemyIndex)
        {
            IsHit = isHit;
            EnemyIndex = enemyIndex;
        }
    }
}