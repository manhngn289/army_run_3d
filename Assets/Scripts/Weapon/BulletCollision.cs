using System;
using Enemy;
using Framework;
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
        
        [SerializeField] private Transform[] bulletTransforms;
        private NativeArray<BulletData> bulletDataNativeArray;
        
        [SerializeField] private Transform[] enemyTransforms;
        private NativeArray<EnemyData> enemyDataNativeArray;
        private NativeArray<EnemyData> updatedEnemyDataNativeArray;
        
        
        private NativeArray<int> EnemyIndicesInCell;
        private NativeArray<int> EnemyCounterInCell;
        
        private NativeArray<CollisionResult> bulletCollisionResults;
        
        private SpatialGridJob spatialGridJob;
        private CollisionJob collisionJob;
        private JobHandle collisionJobHandle;
        JobHandle spatialGridJobHandle;

        private bool jobsInProgress = false;

        private bool AllJobsCompleted => collisionJobHandle.IsCompleted && spatialGridJobHandle.IsCompleted;


        private void Start()
        {
            InitializeBullets();
            InitializeEnemies();
            InitializeSpatialGrid();
            
            //GetComponent<SpatialGridDebugger>()?.SetDebugData(EnemyIndicesInCell, EnemyCounterInCell, enemyDataNativeArray);
        }

        private void InitializeBullets()
        {
            bulletTransforms = new Transform[PoolingManager.MaxBullets];
            bulletDataNativeArray = new NativeArray<BulletData>(bulletTransforms.Length, Allocator.Persistent);
            for (int i = 0; i < bulletTransforms.Length; i++)
            {
                BulletController bulletController = ObjectPooler.GetFromPool<BulletController>(PoolingType.NormalBullet);
                bulletController.index = i;
                bulletTransforms[i] = bulletController.transform;
                bulletDataNativeArray[i] = new BulletData()
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
            enemyDataNativeArray = new NativeArray<EnemyData>(enemyTransforms.Length, Allocator.Persistent);
            updatedEnemyDataNativeArray = new NativeArray<EnemyData>(enemyTransforms.Length, Allocator.Persistent);
        
            for (int i = 0; i < enemyTransforms.Length; i++)
            {
                UnitEnemyController enemy = ObjectPooler.GetFromPool<UnitEnemyController>(PoolingType.EnemyUnit);
                Vector3 pos = new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0, UnityEngine.Random.Range(10f, 50f));
                enemy.selfTransform.position = pos;
                enemy.id = i;
                enemyTransforms[i] = enemy.transform;
                enemyDataNativeArray[i] = new EnemyData
                {
                    Position = pos,
                    IsActive = true,
                    Radius = enemyRadius,
                    Health = 100,
                    HasValidPrevCell = false
                };
            }
        }
        
        void InitializeSpatialGrid()
        {
            int totalCells = KeySave.GridWidth * KeySave.GridHeight;
            EnemyIndicesInCell = new NativeArray<int>(totalCells * KeySave.MaxEnemyPerCell, Allocator.Persistent);
            EnemyCounterInCell = new NativeArray<int>(totalCells, Allocator.Persistent);
            bulletCollisionResults = new NativeArray<CollisionResult>(PoolingManager.MaxBullets, Allocator.Persistent);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                FireBullet();
            }
            
            if (!jobsInProgress)
            {
                ScheduleAllJobs();
                jobsInProgress = true;
            }
            
        }

        private void LateUpdate()
        {
            if (jobsInProgress && AllJobsCompleted)
            {
                spatialGridJobHandle.Complete();
                collisionJobHandle.Complete();
            
                for (int i = 0; i < enemyDataNativeArray.Length; i++)
                {
                    enemyDataNativeArray[i] = updatedEnemyDataNativeArray[i];
                }

                ApplyCollisionResults();
                UpdateBulletTransform();

                jobsInProgress = false;
            }
            
        }

        private void ScheduleAllJobs()
        {
            
            // for(int i = 0 ; i < enemyDataNativeArray.Length; i++)
            // {
            //     updatedEnemyData[i] = enemyDataNativeArray[i];
            // }

            NativeArray<EnemyData> tmp = enemyDataNativeArray;
            updatedEnemyDataNativeArray = enemyDataNativeArray;
            enemyDataNativeArray = tmp;
            
            spatialGridJob = new SpatialGridJob
            {
                Enemies = enemyDataNativeArray,
                EnemyIndicesInCell = EnemyIndicesInCell,
                EnemyCounterInCell = EnemyCounterInCell,
                UpdatedEnemies = updatedEnemyDataNativeArray
            };
            
            collisionJob = new CollisionJob()
            {
                Bullets = bulletDataNativeArray,
                Enemies = enemyDataNativeArray,
                EnemyIndicesInCell = EnemyIndicesInCell,
                EnemyCounterInCell = EnemyCounterInCell,
                BulletCollisionResults = bulletCollisionResults,
                DeltaTime = Time.deltaTime
            };

            spatialGridJobHandle = spatialGridJob.Schedule(enemyDataNativeArray.Length, 64);
            collisionJobHandle = collisionJob.Schedule(bulletDataNativeArray.Length, 64, spatialGridJobHandle);
        }

        private void UpdateBulletTransform()
        {
            for (int i = 0; i < bulletTransforms.Length; i++)
            {
                if (bulletDataNativeArray[i].isActive)
                {
                    bulletTransforms[i].position = bulletDataNativeArray[i].position;
                }
            }
        }

        private void UpdateEnemyData()
        {
            for (int i = 0; i < PoolingManager.MaxEnemies; i++)
            {
                if (enemyDataNativeArray[i].IsActive)
                {
                    var enemy = enemyDataNativeArray[i];
                    enemy.Position = enemyTransforms[i].position;
                    enemyDataNativeArray[i] = enemy;
                }
            }
        }
        
        void FireBullet()
        {
            BulletController bulletController = ObjectPooler.GetFromPool<BulletController>(PoolingType.NormalBullet);
            bulletController.transform.position = transform.position;

            BulletData bullet = bulletDataNativeArray[bulletController.index];
            bullet.position = transform.position;
            bullet.velocity = new float3(0, 0, 1) * bulletSpeed;
            bullet.isActive = true;
            bulletDataNativeArray[bulletController.index] = bullet;
        }

        private void ApplyCollisionResults()
        {
            for (int i = 0; i < bulletCollisionResults.Length; i++)
            {
                CollisionResult collisionResult = bulletCollisionResults[i];

                if (collisionResult.IsHit)
                {
                    BulletData bullet = bulletDataNativeArray[i];
                    bullet.isActive = false;
                    bulletDataNativeArray[i] = bullet;
                    ObjectPooler.ReturnToPool(PoolingType.NormalBullet, bulletTransforms[i].GetComponent<BulletController>());
                    
                    if(collisionResult.EnemyIndex >= 0 && collisionResult.EnemyIndex < PoolingManager.MaxEnemies)
                    {
                        EnemyData enemy = enemyDataNativeArray[collisionResult.EnemyIndex];
                        enemy.Health -= 100;
                        
                        if (enemy.Health <= 0)
                        {
                            enemy.IsActive = false;
                            ObjectPooler.ReturnToPool(PoolingType.EnemyUnit, enemyTransforms[collisionResult.EnemyIndex].GetComponent<UnitEnemyController>());
                        }
                        
                        enemyDataNativeArray[collisionResult.EnemyIndex] = enemy;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            collisionJobHandle.Complete();
            spatialGridJobHandle.Complete();
            
            if (bulletDataNativeArray.IsCreated)
            {
                bulletDataNativeArray.Dispose();
            }
            if (enemyDataNativeArray.IsCreated)
            {
                enemyDataNativeArray.Dispose();
            }
            if (EnemyIndicesInCell.IsCreated)
            {
                EnemyIndicesInCell.Dispose();
            }
            if (EnemyCounterInCell.IsCreated)
            {
                EnemyCounterInCell.Dispose();
            }
            if (updatedEnemyDataNativeArray.IsCreated)
            {
                updatedEnemyDataNativeArray.Dispose();
            }

            if (bulletCollisionResults.IsCreated)
            {
                bulletCollisionResults.Dispose();
            }
        }
    }
}