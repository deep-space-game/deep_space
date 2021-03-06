﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlanetSpawner : MonoBehaviour {

    public GameObject PlanetParent;
    public Ship ShipPrefab;
    public GameObject system;
    public int numPlanetsSpawned = 0;
    
    // Use this for initialization
    void Start ()
    {
        CreateNewPlanet(true);
        system = GameObject.Find("System");
    }

    public void CreateNewPlanet(bool starting = false)
    {
        GameObject planetParent = Instantiate(PlanetParent);
        system = GameObject.Find("System");
        planetParent.transform.parent = system.transform;
        Planet newPlanet = planetParent.gameObject.GetComponentsInChildren<Planet>()[0];
        InitializePlanetDetails(newPlanet, numPlanetsSpawned, starting);
        numPlanetsSpawned++;

        if (numPlanetsSpawned >= generalData.numPlanetsMax)
        {
            GameObject.Find("DetailsCanvas").GetComponent<Details>().RemoveSpawnPlanetUpgrade();
        }
    }
    
    public void InitializePlanetDetails(Planet newPlanet, int planetID, bool is_first)
    {
     
        if (is_first)
        {
            newPlanet.SetStarting();
            newPlanet.planetName = "Gaia";

            newPlanet.population = firstPlanetData.startingPopulation;
            newPlanet.productivity = firstPlanetData.productivity;
            newPlanet.popCapacity = firstPlanetData.populationCapacity;
            newPlanet.popIncreaseCost = firstPlanetData.popIncreaseCostBase;

        }
        else
        {
            newPlanet.SetNonStarting();
            newPlanet.planetName = GeneratePlanetName();

            newPlanet.population = planetData.startingPopulation;
            newPlanet.productivity = planetData.productivity;
            newPlanet.popCapacity = planetData.populationCapacity;
            newPlanet.popIncreaseCost = planetData.popIncreaseCostBase;
        }

        newPlanet.AssignID(planetID);
        
        /*if (!is_first)
        {
            Ship newShip = Instantiate<Ship>(ShipPrefab);
            newShip.transform.parent = system.transform;
            newShip.InitializeShip(0, planetID, 10);
        }*/

    }

    public string GeneratePlanetName()
    {
        string[] possibleStartingConsonants = {"B", "C", "D", "F","G","H","J","K","L","M","N",
            "P","Q","R","S","T","V","X","Z", "Ch", "St", "Tl","Ts","Sh","Dh","Ph","Kr",
            "Pl","Pr","Th","Zh","'" };
        string[] possibleVowels = { "a", "i", "o", "u", "e","a","i","o","u","e","a","i","o","u","e",
            "ai", "ou", "uo", "oi", "ae", "ao", "ei", "ua", "oe", "ui", "ue", "ia", "io", "iu", "ie" };
        string[] possibleMiddleConsonants = { "b", "c", "d", "f", "g", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "z" };
        return possibleStartingConsonants[(int)Math.Floor((double)UnityEngine.Random.Range(0, possibleStartingConsonants.Length - 0.1f))] +
            possibleVowels[(int)Math.Floor((double)UnityEngine.Random.Range(0, possibleVowels.Length - 0.1f))] +
            possibleMiddleConsonants[(int)Math.Floor((double)UnityEngine.Random.Range(0, possibleMiddleConsonants.Length - 0.1f))] +
            possibleVowels[(int)Math.Floor((double)UnityEngine.Random.Range(0, possibleVowels.Length - 0.1f))];
    }
   
}
