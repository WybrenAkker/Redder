using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    public int currentWave = 0;
    [Range(0, 100)]
    public float difficultyIncreasePerWave;

    

    #region structs
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

    [Serializable]
    public struct GunTurrets
    {
        public bool createPool;

        public GameObject gunTurretPrefab;

        public GameObject bulletPrefab;
        public int bulletAmount;
    }

    [Serializable]
    public struct PlayerSettings
    {
        public bool createPool;
        public float playerHealth;
        public GameObject bulletPrefab;
        public int bulletAmount;
    }
    #endregion

    [Header("Prefabs and first wave Data")]
    public List<Transform> airshipSpawnPositions = new List<Transform>();
    public List<Transform> gunTurretSpawnPositions = new List<Transform>();

    public Airships airShips;
    public Planes planes;
    public Rockets rockets;
    public GunTurrets gunTurrets;
    public PlayerSettings playerSettings;
    public int maxGunTurretEvents;

    [Header("Current Wave Data")]
    #region waveData
    public int activeAirships;
    public int activePlanes;
    public int activeGunTurrets;
    public int gunTurretEvents;
    #endregion

    [Header("Gun Turret Event Data")]
    public int usedSpots;
    public float minTimeBetweenGTEvent, maxTimeBetweenGTEvent;
    
    float timer;

    public GameObject waveScreen;

    private void Awake()
    {
        if (airShips.createPool) //Check if the creator wants to create an airships pool. Create one if true.
        {
            PoolManager.instance.CreatePool(airShips.bulletPrefab, airShips.bulletAmount);
        }

        if (planes.createPool) //Check if the creator wants to create a planes pool. Create one if true.
        {
            PoolManager.instance.CreatePool(planes.bulletPrefab, planes.bulletAmount);
            PoolManager.instance.CreatePool(planes.planePrefab, planes.planeAmount);
        }

        if (rockets.createPool) //Check if the creator wants to create a rockets pool. Create one if true.
        {
            PoolManager.instance.CreatePool(rockets.prefab, rockets.amount);
        }

        if(gunTurrets.createPool) //Check if the creator wants to create a gunTurrets pool. Create one if true.
        {
            PoolManager.instance.CreatePool(gunTurrets.bulletPrefab, gunTurrets.bulletAmount);
        }

        if(playerSettings.createPool) //Check if the creator wants to create a pool for player associated objects (bullets). Create one if true.
        {
            PoolManager.instance.CreatePool(playerSettings.bulletPrefab, playerSettings.bulletAmount);
        }

        timer = UnityEngine.Random.Range(minTimeBetweenGTEvent, maxTimeBetweenGTEvent); //Set a random timer between 0 and maxTimeBetweenGTEvent. The resulting time will be used as delay for the first spawn of a gunTurret.

        int amountOfPlanesPerAirship = Mathf.FloorToInt(planes.planeAmount / airShips.shipAmount); //Get the amount of planes per airship. 

        SpawnWave(amountOfPlanesPerAirship); //Spawn the first wave.
    }

    private void Update()
    {
        CheckForEndOFWave();

        GTEvents();
    }

    void GTEvents()
    {
        if (usedSpots < gunTurretSpawnPositions.Count) //Check if there is more room for gunTurrets.
        {
            if (maxGunTurretEvents > gunTurretEvents) //Check if there are any more allowed gunTurret events.
            {
                timer -= Time.deltaTime;
                if (timer <= 0) //Check if the set required time between events has been reduced to 0.
                {
                    StartNewGTEvent(); //Start new GT event.
                    activeGunTurrets += 1; //Increase activeGunTurrets by one.
                    gunTurretEvents += 1; //Increase gunTurretEvents by one.
                    usedSpots += 1; //Increase used spots by one.
                    timer = UnityEngine.Random.Range(minTimeBetweenGTEvent, maxTimeBetweenGTEvent); //Assign a new required time between this event and the next.
                }
            }
        }
    }

    void CheckForEndOFWave()
    {
        if(activeAirships == 0 && activePlanes == 0 && activeGunTurrets == 0) //Check if every enemy has been destroyed.
        {
            OpenWaveScreen(); //Open the "Wave over" screen.
        }
    }

    void OpenWaveScreen()
    {
        waveScreen.SetActive(true); //Set the waveScreen to active.
        Time.timeScale = 0; //Freeze time.
    }

    public void CloseWaveScreen()
    {
        waveScreen.SetActive(false); //Close the wave screen.
        Time.timeScale = 1; //Resume time.
        CreateNextWave(); //Create the next wave.
    }

    public void CreateNextWave()
    {
        currentWave += 1; 

        
        int newShipAmount = Mathf.CeilToInt(airShips.shipAmount * ((difficultyIncreasePerWave / 100) * currentWave)); //Calculate the new amount of airships for this wave.

        if (newShipAmount > 4) //Check if the result of newShipAmount isn't above 4. Due to limited space we can't fit more than 4.
        {
            newShipAmount = 4; //Reset newShipAmount to the maximum of 4.
        }

        if(newShipAmount < airShips.shipAmount) //Check if the last wave had more airships than the result of this calculation.
        {
            newShipAmount = airShips.shipAmount; //Reset the result to the previous wave's amount of airships.
        }

        airShips.bulletAmount = Mathf.CeilToInt(airShips.bulletAmount * (newShipAmount / airShips.shipAmount));  //Calculate the required amount of airship bullets for this wave.

        int newPlaneAmount = Mathf.CeilToInt(planes.planeAmount + (planes.planeAmount * ((difficultyIncreasePerWave / 100) * currentWave))); //Calculate the required amount of planes for this wave.
        int amountOfPlanesPerAirship = Mathf.CeilToInt(newPlaneAmount / airShips.shipAmount); //Calculate the amount of planes per airship.
        planes.bulletAmount = Mathf.CeilToInt(planes.bulletAmount * (newPlaneAmount / planes.planeAmount)); //Calculate the required amount of plane bullets for this wave.

        int newMaxGunTurretEvents = Mathf.CeilToInt(maxGunTurretEvents * ((difficultyIncreasePerWave / 100) * currentWave)); //Calculate the new maximum amount of gunTurret Events for this wave.

        if(newMaxGunTurretEvents < maxGunTurretEvents) //Check if the new result is lower than the previous one.
        {
            newMaxGunTurretEvents = maxGunTurretEvents; //Reset the new result to the previous result.
        }

        gunTurrets.bulletAmount = Mathf.CeilToInt(gunTurrets.bulletAmount * (maxGunTurretEvents / newMaxGunTurretEvents)); //Calculate the required amount of gunTurret bullets for this wave.

        //Apply the results.
        airShips.shipAmount = newShipAmount;
        planes.planeAmount = newPlaneAmount;
        maxGunTurretEvents = newMaxGunTurretEvents;

        
        ExpandPools(airShips.bulletAmount, planes.planeAmount, planes.bulletAmount); //Expand the pools
        SpawnWave(amountOfPlanesPerAirship); //Spawn the wave.
    }

    void ExpandPools(int amountOfAirshipBullets, int amountOfPlanes, int amountOfPlaneBullets)
    {
        if (airShips.createPool) //Checks if the creator wanted a pool for airships at the start of the game. If so expand the corresponding pool.
        {
            PoolManager.instance.ExpandPool(airShips.bulletPrefab, amountOfAirshipBullets);
        }

        if (planes.createPool) //Checks if the creator wanted a pool for planes at the start of the game. If so expand the corresponding pool.
        {
            PoolManager.instance.ExpandPool(planes.bulletPrefab, amountOfPlaneBullets);
            PoolManager.instance.ExpandPool(planes.planePrefab, amountOfPlanes);
        }

        if (gunTurrets.createPool) //Checks if the creator wanted a pool for gunTurrets at the start of the game. If so expand the corresponding pool.
        {
            PoolManager.instance.CreatePool(gunTurrets.bulletPrefab, gunTurrets.bulletAmount);
        }
    }

    void SpawnWave(int amountOfPlanesPerAirship)
    {
        GameObject.Find("Player").GetComponent<Playercontroller>().ResetHealth(playerSettings.playerHealth); //Reset the player's health.

        gunTurretEvents = 0; //Reset the gunTurret events.

        for(int i = 0; i < airShips.shipAmount; i++) //Cycle through each required airship.
        {
            Transform spawnPos = airshipSpawnPositions[i]; //Get a spawnPosition for the airship.
            GameObject newShip = Instantiate(airShips.shipPrefab, spawnPos.position, spawnPos.rotation); //Instantiate an airShip.
            newShip.GetComponent<AirshipAI>().hangar.maxActivePlanes = amountOfPlanesPerAirship; //Assign the maximum planes the airship can spawn.
            activeAirships += 1;
        }
    }

    void StartNewGTEvent()
    {
        for(int i = 0; i < gunTurretSpawnPositions.Count; i++) //Cycle through all available spawnPositions for gunTurrets.
        {
            GTSpawnPosition spawnPos = gunTurretSpawnPositions[i].gameObject.GetComponent<GTSpawnPosition>(); //Get the script from this position.

            if (spawnPos.isUsed != true) //Check if the position isn't already in use.
            {
                spawnPos.isUsed = true; //Set the spawnPosition to used.
                GameObject newGT = Instantiate(gunTurrets.gunTurretPrefab, spawnPos.dropPoint.position, spawnPos.dropPoint.rotation); //Spawn a new gunTurret.
                newGT.GetComponent<GunTurretAI>().groundTarget = spawnPos.groundPoint; //Assign the gunTurret's groundTarget.
                newGT.GetComponent<GunTurretAI>().gunTurretSpotIndex = i; //Assign the index of the gunTurret's spot.
                break;
            }
        }
    }
}
