using System;
using System.Collections.Generic;
using Enemy;
using Framework;
using TMPro;
using Unity.Collections;
using UnityEngine;

#if UNITY_EDITOR
namespace Weapon
{
    public class SpatialGridDebugger : MonoBehaviour
    {
        [Header("Grid Visualization")]
        [SerializeField] private bool showGrid = true;
        [SerializeField] private bool showCounts = true;
        [SerializeField] private bool showEnemyIndices = true;
        
        [Header("Display Options")]
        [SerializeField] private float cellHeightOffset = 0.1f;
        [SerializeField] private Color emptyColor = Color.white;
        [SerializeField] private Color occupiedColor = Color.yellow;
        [SerializeField] private Color highlightColor = Color.red;
        
        // References to actual grid data
        private NativeArray<int> enemyIndicesInCell;
        private NativeArray<int> enemyCounterInCell;
        private NativeArray<EnemyData> enemies;
        
        private BulletCollision bulletCollisionSystem;

        private void Start()
        {
            bulletCollisionSystem = GetComponent<BulletCollision>();
        }

        public void SetDebugData(NativeArray<int> enemyIndicesInCell, NativeArray<int> enemyCounterInCell, NativeArray<EnemyData> enemies)
        {
            this.enemyIndicesInCell = enemyIndicesInCell;
            this.enemyCounterInCell = enemyCounterInCell;
            this.enemies = enemies;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !showGrid) return;
            
            // Get references if needed (works in editor during play)
            if (!enemyCounterInCell.IsCreated && bulletCollisionSystem != null)
            {
                // Use reflection to get private fields if needed
                var counterField = bulletCollisionSystem.GetType().GetField("EnemyCounterInCell", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (counterField != null)
                    enemyCounterInCell = (NativeArray<int>)counterField.GetValue(bulletCollisionSystem);
                
                var indicesField = bulletCollisionSystem.GetType().GetField("EnemyIndicesInCell", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (indicesField != null)
                    enemyIndicesInCell = (NativeArray<int>)indicesField.GetValue(bulletCollisionSystem);
                
                var enemiesField = bulletCollisionSystem.GetType().GetField("enemyDataNativeArray", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
                if (enemiesField != null)
                    enemies = (NativeArray<EnemyData>)enemiesField.GetValue(bulletCollisionSystem);
            }

            if (!enemyCounterInCell.IsCreated) return;

            // Calculate grid offset
            float offsetX = -(KeySave.GridWidth * KeySave.CellSize) / 2;
            float offsetZ = -(KeySave.GridHeight * KeySave.CellSize) / 2;

            // Draw each grid cell
            for (int row = 0; row < KeySave.GridHeight; row++)
            {
                for (int col = 0; col < KeySave.GridWidth; col++)
                {
                    int cellIdx = row * KeySave.GridWidth + col;
                    int count = enemyCounterInCell[cellIdx];
                    
                    Vector3 cellCenter = new Vector3(
                        offsetX + (col * KeySave.CellSize) + (KeySave.CellSize/2),
                        cellHeightOffset,
                        offsetZ + (row * KeySave.CellSize) + (KeySave.CellSize/2)
                    );

                    // Set cell color based on occupancy
                    Gizmos.color = count > 0 ? (count >= KeySave.MaxEnemyPerCell * 0.8f ? highlightColor : occupiedColor) : emptyColor;
                    
                    // Draw cell border
                    Gizmos.DrawWireCube(cellCenter, new Vector3(KeySave.CellSize, 0.1f, KeySave.CellSize));

                    // Show enemy count
                    if (showCounts && count > 0)
                    {
                        UnityEditor.Handles.color = Color.black;
                        UnityEditor.Handles.Label(cellCenter, count.ToString());
                    }
                    
                    // Show enemy indices in cell
                    if (showEnemyIndices && count > 0 && enemyIndicesInCell.IsCreated)
                    {
                        string indices = "";
                        for (int i = 0; i < count; i++)
                        {
                            int enemyIdx = enemyIndicesInCell[cellIdx * KeySave.MaxEnemyPerCell + i];
                            indices += enemyIdx + ",";
                            
                            // Show lines connecting to actual enemies
                            if (enemies.IsCreated && enemyIdx < enemies.Length && enemies[enemyIdx].IsActive)
                            {
                                Gizmos.color = Color.cyan;
                                Gizmos.DrawLine(cellCenter, enemies[enemyIdx].Position);
                            }
                        }
                        
                        if (indices.Length > 0)
                        {
                            UnityEditor.Handles.Label(cellCenter + new Vector3(0, 0.3f, 0), indices);
                        }
                    }
                }
            }
        }

        // Add this function to your BulletCollision class to expose the grid data to the debugger
        public void ConnectDebugger(SpatialGridDebugger debugger)
        {
            debugger.SetDebugData(enemyIndicesInCell, enemyCounterInCell, enemies);
        }
    }
}

#endif