using Enemy;
using Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Weapon
{
    [BurstCompile]
    public struct CollisionJob : IJobParallelFor
    {
        public NativeArray<BulletData> Bullets;
        [ReadOnly] public NativeArray<EnemyData> Enemies;
        [ReadOnly] public NativeArray<int> EnemyIndicesInCell;
        [ReadOnly] public NativeArray<int> EnemyCounterInCell;
        [ReadOnly] public float DeltaTime;

        [WriteOnly] public NativeArray<CollisionResult> BulletCollisionResults;
        
        
        public void Execute(int bulletIdx)
        {
            if (!Bullets[bulletIdx].isActive)
            {
                BulletCollisionResults[bulletIdx] = new CollisionResult(false, -1);
                return;
            }
            
            BulletData bullet = Bullets[bulletIdx];

            bullet.position += bullet.velocity * DeltaTime;
            Bullets[bulletIdx] = bullet;

            int col = (int)math.floor(bullet.position.x / KeySave.CellSize + KeySave.HalfGridWidth);
            int row = (int)math.floor(bullet.position.z / KeySave.CellSize + KeySave.HalfGridHeight);

            if (!MatrixHelper.IsValidIndex(row, col, KeySave.GridWidth, KeySave.GridHeight))
            {
                BulletCollisionResults[bulletIdx] = new CollisionResult()
                {
                    IsHit = false,
                    EnemyIndex = -1
                };
                return;
            }

            int cellIdx = row * KeySave.GridWidth + col;
            int enemyCount = EnemyCounterInCell[cellIdx];

            // Duyệt qua các chỉ số của kẻ thù trong ô lưới
            for (int i = 0; i < enemyCount; i++)
            {
                int enemyIdx = EnemyIndicesInCell[cellIdx * KeySave.MaxEnemyPerCell + i];
                if(enemyIdx 
                   >= Enemies.Length || !Enemies[enemyIdx].IsActive)
                    continue;
                        
                EnemyData enemy = Enemies[enemyIdx];
                float distance = math.distance(bullet.position, enemy.Position);
                if (distance <= bullet.radius + enemy.Radius)
                {
                    BulletCollisionResults[bulletIdx] = new CollisionResult()
                    {
                        IsHit = true,
                        EnemyIndex = enemyIdx
                    };
                    return;
                }
            }

            BulletCollisionResults[bulletIdx] = new CollisionResult()
            {
                IsHit = false,
                EnemyIndex = -1
            };
        }
    }
}