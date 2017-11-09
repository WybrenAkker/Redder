using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class CreatePools : MonoBehaviour
{
    Stopwatch sw = new Stopwatch();

    [Serializable]
    public struct Airships
    {
        public bool createPool;

        public GameObject shipPrefab;
        public int shipAmount;

        public GameObject bulletPrefab;
        public int bulletAmount;
    }

    [Serializable]
    public struct Planes
    {
        public bool createPool;

        public GameObject planePrefab;
        public int planeAmount;

        public GameObject bulletPrefab;
        public int bulletAmount;
    }

    [Serializable]
    public struct Rockets
    {
        public bool createPool;

        public GameObject prefab;
        public int amount;
    }

    public Airships airShips;
    public Planes planes;
    public Rockets rockets;

    private void Awake()
    {
        sw.Start();
        if (airShips.createPool)
        {
            PoolManager.instance.CreatePool(airShips.bulletPrefab, airShips.bulletAmount);
        }

        if (planes.createPool)
        {
            PoolManager.instance.CreatePool(planes.bulletPrefab, planes.bulletAmount);
            PoolManager.instance.CreatePool(planes.planePrefab, planes.planeAmount);
        }

        if (rockets.createPool)
        {
            PoolManager.instance.CreatePool(rockets.prefab, rockets.amount);
        }
        sw.Stop();
        UnityEngine.Debug.Log(sw.Elapsed);
    }
}
