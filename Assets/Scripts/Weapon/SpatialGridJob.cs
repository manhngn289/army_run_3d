using Enemy;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Weapon
{
    [BurstCompile]
    public struct SpatialGridJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<EnemyData> enemies;
        [NativeDisableParallelForRestriction] public NativeArray<int> spatialGrid;
        [NativeDisableParallelForRestriction] public NativeArray<int> gridCounts;
        public int gridWidth;
        public int gridHeight;
        public float cellSize;
        
        
        public void Execute(int index)
        {
            if(!enemies[index].isActive) return;
            
            float3 position = enemies[index].position;
            int gridX = (int)(position.x / cellSize) + gridWidth / 2;
            int gridY = (int)(position.z / cellSize) + gridHeight / 2;

            if (gridX >= 0 && gridX < gridWidth && gridY >= 0 && gridY < gridHeight)
            {
                int cellIndex = gridY * gridWidth + gridX;
                int currentCount = gridCounts[cellIndex];

                if (currentCount < 32)
                {
                    spatialGrid[cellIndex * 32 + currentCount] = index;
                    gridCounts[cellIndex] = currentCount + 1;
                }
            }
        }
    }
}