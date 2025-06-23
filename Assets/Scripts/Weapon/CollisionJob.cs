using Enemy;
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
        public NativeArray<BulletData> bullets;
        [ReadOnly] public NativeArray<EnemyData> enemies;
        [ReadOnly] public NativeArray<int> spatialGrid;
        [ReadOnly] public NativeArray<int> gridCounts;

        public int gridWidth;
        public int gridHeight;
        public float cellSize;
        public NativeArray<CollisionResult> collisionResults;
        public float deltaTime;
        
        
        public void Execute(int index)
        {
            if (!bullets[index].isActive)
            {
                collisionResults[index] = new CollisionResult(){isHit = false, enemyIndex = -1};
                return;
            }
            
            BulletData bullet = bullets[index];

            bullet.position += bullet.velocity * deltaTime;
            bullets[index] = bullet;
            
            int gridX = (int)(bullet.position.x / cellSize) + gridWidth / 2;
            int gridY = (int)(bullet.position.z / cellSize) + gridHeight / 2;

            if (gridX < 0 || gridX >= gridWidth || gridY < 0 || gridY >= gridHeight)
            {
                collisionResults[index] = new CollisionResult()
                {
                    isHit = false,
                    enemyIndex = -1
                };
                return;
            }

            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    int checkX = gridX + offsetX;
                    int checkY = gridY + offsetY;

                    if (checkX < 0 || checkX >= gridWidth || checkY < 0 || checkY >= gridHeight)
                        continue;

                    int cellIdx = checkX * gridWidth + checkY;
                    int enemyCount = gridCounts[cellIdx];

                    // Duyệt qua các chỉ số của kẻ thù trong ô lưới
                    for (int i = 0; i < enemyCount; i++)
                    {
                        int enemyIdx = spatialGrid[cellIdx * 32 + i];
                        if(enemyIdx >= enemies.Length || !enemies[enemyIdx].isActive)
                            continue;
                        
                        EnemyData enemy = enemies[enemyIdx];
                        float distance = math.distance(bullet.position, enemy.position);
                        if (distance <= bullet.radius + enemy.radius)
                        {
                            collisionResults[index] = new CollisionResult()
                            {
                                isHit = true,
                                enemyIndex = enemyIdx
                            };
                            return;
                        }
                    }
                }
            }

            collisionResults[index] = new CollisionResult()
            {
                isHit = false,
                enemyIndex = -1
            };
        }
    }
}