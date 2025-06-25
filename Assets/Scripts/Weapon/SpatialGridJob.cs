using Enemy;
using Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Weapon
{
    [BurstCompile]
    public struct SpatialGridJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<EnemyData> Enemies;
        [NativeDisableParallelForRestriction] public NativeArray<int> EnemyIndicesInCell;
        [NativeDisableParallelForRestriction] public NativeArray<int> EnemyCounterInCell;
        
        // Double buffer prevent read-write conflicts
        [WriteOnly] public NativeArray<EnemyData> UpdatedEnemies;
        
        
        public void Execute(int enemyIdx)
        {
            EnemyData updatedEnemy = Enemies[enemyIdx];
            
            if (!Enemies[enemyIdx].IsActive)
            {
                if (Enemies[enemyIdx].HasValidPrevCell)
                {
                    int prevCellIdx = Enemies[enemyIdx].PrevRow * KeySave.GridWidth + Enemies[enemyIdx].PrevCol;
                    RemoveEnemyFromPrevCell(prevCellIdx, enemyIdx);
                }
                
                updatedEnemy.HasValidPrevCell = false;
            }
            else
            {
                float3 position = updatedEnemy.Position;

                int col = (int)math.floor(position.x / KeySave.CellSize + KeySave.HalfGridWidth);
                int row = (int)math.floor(position.z / KeySave.CellSize + KeySave.HalfGridHeight);

                if (!MatrixHelper.IsValidIndex(row, col, KeySave.GridWidth, KeySave.GridHeight))
                    return;
            
                bool cellChanged = !updatedEnemy.HasValidPrevCell ||
                                   updatedEnemy.PrevCol != col ||
                                   updatedEnemy.PrevRow != row;

                if (cellChanged)
                {
                    if (updatedEnemy.HasValidPrevCell)
                    {
                        int prevCellIdx = updatedEnemy.PrevRow * KeySave.GridWidth + updatedEnemy.PrevCol;
                        RemoveEnemyFromPrevCell(prevCellIdx, enemyIdx);
                    }

                    int cellIndex = row * KeySave.GridWidth + col;
                    int currentCount = EnemyCounterInCell[cellIndex];

                    if (currentCount < KeySave.MaxEnemyPerCell)
                    {
                        EnemyIndicesInCell[cellIndex * KeySave.MaxEnemyPerCell + currentCount] = enemyIdx;
                        EnemyCounterInCell[cellIndex] = currentCount + 1;
                    }

                    updatedEnemy.HasValidPrevCell = true;
                    updatedEnemy.PrevCol = col;
                    updatedEnemy.PrevRow = row;
                }
            }
            
            UpdatedEnemies[enemyIdx] = updatedEnemy;
        }

        private void RemoveEnemyFromPrevCell(int cellIdx, int enemyIdx)
        {
            int count = EnemyCounterInCell[cellIdx];

            if (count <= 0) return;

            int enemyPosInCell = -1;
            for (int i = 0; i < count; i++)
            {
                if (EnemyIndicesInCell[cellIdx * KeySave.MaxEnemyPerCell + i] == enemyIdx)
                {
                    enemyPosInCell = i;
                    break;
                }
            }

            if (enemyPosInCell >= 0)
            {
                // Shift the last enemy to the position of the removed enemy
                if (enemyPosInCell < count - 1)
                {
                    EnemyIndicesInCell[cellIdx * KeySave.MaxEnemyPerCell + enemyPosInCell] = EnemyIndicesInCell[cellIdx * KeySave.MaxEnemyPerCell + count - 1];
                }
                
                EnemyIndicesInCell[cellIdx * KeySave.MaxEnemyPerCell + count - 1] = -1;
                EnemyCounterInCell[cellIdx] = count - 1;
            }
        }
    }
}