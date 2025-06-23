using System;
using Enemy;
using Framework.ObjectPooling;
using Manager;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Weapon
{
    public class BulletCollision : MonoBehaviour
    {
        [Header("Bullet Collision Settings")]
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private float bulletRadius = 0.5f;
        [SerializeField] private float enemyRadius = 0.5f;
        // [SerializeField] private int maxBullets = 100;
        // [SerializeField] private int maxEnemies = 50;
        
        [Header("Spatial Grid Settings")]
        [SerializeField] private float cellSize = 2f;
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 20;
        
        [SerializeField] private Transform[] bulletTransforms;
        private NativeArray<BulletData> bulletData;
        
        [SerializeField] private Transform[] enemyTransforms;
        private NativeArray<EnemyData> enemyData;
        
        private NativeArray<int> spatialGrid;
        private NativeArray<int> gridCounts;
        
        private NativeArray<CollisionResult> collisionResults;
        
        private SpatialGridJob spatialGridJob;
        private CollisionJob collisionJob;
        private JobHandle collisionJobHandle;
        JobHandle spatialGridJobHandle;


        private void Start()
        {
            InitializeBullets();
            InitializeEnemies();
            InitializeSpatialGrid();
        }

        private void InitializeBullets()
        {
            bulletTransforms = new Transform[PoolingManager.MaxBullets];
            bulletData = new NativeArray<BulletData>(PoolingManager.MaxBullets, Allocator.Persistent);
            for (int i = 0; i < PoolingManager.MaxBullets; i++)
            {
                BulletController bulletController = ObjectPooler.GetFromPool<BulletController>(PoolingType.NormalBullet);
                bulletController.index = i;
                bulletTransforms[i] = bulletController.transform;
                bulletData[i] = new BulletData()
                {
                    position = float3.zero,
                    velocity = float3.zero,
                    radius = bulletRadius,
                    isActive = false
                };
            }

            for (int i = 0; i < PoolingManager.MaxBullets; i++)
            {
                ObjectPooler.ReturnToPool(PoolingType.NormalBullet, bulletTransforms[i].GetComponent<BulletController>());
            }
        }
        
        void InitializeEnemies()
        {
            enemyTransforms = new Transform[PoolingManager.MaxEnemies];
            enemyData = new NativeArray<EnemyData>(PoolingManager.MaxEnemies, Allocator.Persistent);
        
            for (int i = 0; i < PoolingManager.MaxEnemies; i++)
            {
                UnitEnemyController enemy = ObjectPooler.GetFromPool<UnitEnemyController>(PoolingType.EnemyUnit);
                Vector3 pos = new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0, UnityEngine.Random.Range(10f, 50f));
                enemy.selfTransform.position = pos;
                enemy.transform.position = pos;
                enemyTransforms[i] = enemy.transform;
                enemyData[i] = new EnemyData
                {
                    position = pos,
                    isActive = true,
                    radius = enemyRadius,
                    health = 100
                };
            }
        }
        
        void InitializeSpatialGrid()
        {
            int totalCells = gridWidth * gridHeight;
            spatialGrid = new NativeArray<int>(totalCells * 32, Allocator.Persistent);
            gridCounts = new NativeArray<int>(totalCells, Allocator.Persistent);
            collisionResults = new NativeArray<CollisionResult>(PoolingManager.MaxBullets, Allocator.Persistent);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                FireBullet();
            }
            
            UpdateEnemyData();
            ScheduleAllJobs();
        }

        private void LateUpdate()
        {
            spatialGridJobHandle.Complete();
            collisionJobHandle.Complete();

            string tmp = "";
            for (int i = 0; i < spatialGrid.Length; i++)
            {
                tmp += spatialGrid[i] + " ";
            }
            print(tmp);

            ApplyCollisionResults();
            UpdateBulletTransform();
        }

        private void ScheduleAllJobs()
        {
            spatialGridJob = new SpatialGridJob
            {
                enemies = enemyData,
                spatialGrid = spatialGrid,
                gridCounts = gridCounts,
                gridWidth = gridWidth,
                gridHeight = gridHeight,
                cellSize = cellSize
            };
            
            collisionJob = new CollisionJob()
            {
                bullets = bulletData,
                enemies = enemyData,
                spatialGrid = spatialGrid,
                gridCounts = gridCounts,
                gridWidth = gridWidth,
                gridHeight = gridHeight,
                cellSize = cellSize,
                collisionResults = collisionResults,
                deltaTime = Time.deltaTime
            };

            spatialGridJobHandle = spatialGridJob.Schedule(enemyData.Length, 64);
            collisionJobHandle = collisionJob.Schedule(bulletData.Length, 64, spatialGridJobHandle);
        }

        private void UpdateBulletTransform()
        {
            for (int i = 0; i < PoolingManager.MaxBullets; i++)
            {
                if (bulletData[i].isActive)
                {
                    bulletTransforms[i].position = bulletData[i].position;
                }
            }
        }

        private void UpdateEnemyData()
        {
            for (int i = 0; i < PoolingManager.MaxEnemies; i++)
            {
                if (enemyData[i].isActive)
                {
                    var enemy = enemyData[i];
                    enemy.position = enemyTransforms[i].position;
                    enemyData[i] = enemy;
                }
            }
        }
        
        void FireBullet()
        {
            BulletController bulletController = ObjectPooler.GetFromPool<BulletController>(PoolingType.NormalBullet);
            bulletController.transform.position = transform.position;

            BulletData bullet = bulletData[bulletController.index];
            bullet.position = transform.position;
            bullet.velocity = new float3(0, 0, 1) * bulletSpeed;
            bullet.isActive = true;
            bulletData[bulletController.index] = bullet;
        }

        private void ApplyCollisionResults()
        {
            for (int i = 0; i < PoolingManager.MaxBullets; i++)
            {
                CollisionResult collisionResult = collisionResults[i];

                if (collisionResult.isHit)
                {
                    BulletData bullet = bulletData[i];
                    bullet.isActive = false;
                    bulletData[i] = bullet;
                    ObjectPooler.ReturnToPool(PoolingType.NormalBullet, bulletTransforms[i].GetComponent<BulletController>());
                    
                    if(collisionResult.enemyIndex >= 0 && collisionResult.enemyIndex < PoolingManager.MaxEnemies)
                    {
                        EnemyData enemy = enemyData[collisionResult.enemyIndex];
                        enemy.health -= 100; 
                        
                        if (enemy.health <= 0)
                        {
                            enemy.isActive = false;
                            ObjectPooler.ReturnToPool(PoolingType.EnemyUnit, enemyTransforms[collisionResult.enemyIndex].GetComponent<UnitEnemyController>());
                        }
                        
                        enemyData[collisionResult.enemyIndex] = enemy;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            collisionJobHandle.Complete();
            spatialGridJobHandle.Complete();
            
            if (bulletData.IsCreated)
            {
                bulletData.Dispose();
            }
            if (enemyData.IsCreated)
            {
                enemyData.Dispose();
            }
            if (spatialGrid.IsCreated)
            {
                spatialGrid.Dispose();
            }
            if (gridCounts.IsCreated)
            {
                gridCounts.Dispose();
            }

            if (collisionResults.IsCreated)
            {
                collisionResults.Dispose();
            }
        }
    }
}