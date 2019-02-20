﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class Planet : MonoBehaviour {

    //Sprites
    public Sprite InhabitedPlanetSprite;
    public Sprite InhabitedPlanetSelectedSprite;
    public Sprite UninhabitedPlanetSprite;
    public Sprite UninhabitedPlanetSelectedSprite;

    //GUI elements
    Canvas DetailsCanvas;
    GameObject PlanetDetailsPanelObj;


    //whether or not this planet appears in planet details
    bool IsSelected=false;
    
    //set this to true when population is high enough
    //dictates whether inhabited or uninhabited sprite is used
    bool IsTerraformed=false;

    //Planet Data that varies by planet basis 
    public string planetName;

    private int _planetID;
    public int planetID
    {
        get { return _planetID; }
        set { _planetID = value; }
    }

    private float _Theta; 
    public float Theta
    {
      get { return _Theta; }
        set { _Theta = value; }
    }
    private float _R = 1; 
    public float R
    {
      get { return _R; }
        set { _R = value; }
    }

    private double _population;
    public double population
    {
        get { return _population; }
        set { _population = value; }
    }

    // text like "+100" that will fly above planet when clicked
    public GameObject planetClickAnimation;
    // how much to scale planet when hovered
    private float scalePlanetHover = 1.4f;
    private float scalePlanetClick = 1.2f;

    public bool newPlanetSpawned = false;
    public double newPlanetPopThreshold;
    public double productivity;
    public double popCapacity;
    public double popIncreaseCost;
    public double popGrowthRate;
    public double colonizeMoneyCost;
    public double fixedPopGrowth;
    public double autoGrowthCost;

    // Use this for initialization
    void Start () {
        newPlanetPopThreshold = planetData.newPlanetPopThreshold;
        popGrowthRate = planetData.popGrowthRate;
        colonizeMoneyCost = planetData.colonizeMoneyCost;
        autoGrowthCost = planetData.popAutoGrowthCostBase;

        //Initialize GUI elements
        DetailsCanvas = GameObject.Find("DetailsCanvas").GetComponent<Canvas>();
        PlanetDetailsPanelObj = DetailsCanvas.transform.Find("PlanetDetailsPanel").gameObject;
        //Theta
        Theta = UnityEngine.Random.value * 2*Mathf.PI;
        fixedPopGrowth = 0;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 position = new Vector2(R*Mathf.Cos(Theta), R*Mathf.Sin(Theta));
        transform.parent.position = position;
	}

  private void FixedUpdate()
    {
        //simplified astrophysics
        Theta += Time.fixedDeltaTime / (10 * R * R);
        Theta = Theta % (2 * Mathf.PI);

        //Sigmoid approximation of population growth //Could try to refine this
        population += popGrowthRate * population *
                      (Math.Log10(Math.Max(1, popCapacity)) - Math.Log10(Math.Max(1, population))) *
                      Time.fixedDeltaTime +
                      fixedPopGrowth * Time.deltaTime;
    }

    private void OnMouseEnter()
    {
        gameObject.transform.localScale = new Vector3(scalePlanetHover, scalePlanetHover, scalePlanetHover);
    }

    private void OnMouseExit()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnMouseUpAsButton()
    {
        gameObject.transform.localScale = new Vector3(scalePlanetHover, scalePlanetHover, scalePlanetHover);
    }

    private void OnMouseDown()
    {
        PlanetDetailsPanelObj.SetActive(true);
        GameObject.Find("DetailsCanvas").SendMessage("SetActivePlanetID", planetID);
        
        // Logic and Animation for each Planet Click
        GameObject.Find("DetailsCanvas").SendMessage("PlanetClicked", planetID);
        Instantiate(planetClickAnimation).transform.parent = transform.parent;
        gameObject.transform.localScale = new Vector3(scalePlanetClick, scalePlanetClick, scalePlanetClick);
    }
    
    public void AssignID(int id)
    {
        planetID = id;
        R = (1.5f + id)*0.6f;
        gameObject.name = "planet" + planetID.ToString();
    }

    //Call either SetStarting or SetNonStarting on spawn
    public void SetStarting()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = InhabitedPlanetSprite;
        IsTerraformed = true;
        IsSelected = false;
    }

    public void SetNonStarting() 
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = UninhabitedPlanetSprite;
        IsTerraformed = false;
        IsSelected = false;
    }

    public void AddPopulation()
    {
        Details gameDetails = GameObject.Find("DetailsCanvas").GetComponent<Details>();
        int ActivePlanetId = gameDetails.ActivePlanetId;
        Planet activePlanet = GameObject.Find("planet" + ActivePlanetId.ToString()).GetComponent<Planet>();
        if (population >= planetData.popIncreaseThreshold && gameDetails.money > activePlanet.popIncreaseCost)
        {
            activePlanet.population += planetData.popIncreaseModifier;
            gameDetails.money -= popIncreaseCost; 
            popIncreaseCost *= planetData.popIncreaseCostScale;
        }
        else if (activePlanet.population < planetData.popIncreaseThreshold && gameDetails.money > planetData.colonizeMoneyCost)
        {
            for (int i = 0; i < generalData.numPlanets; i++)
            {
                Debug.Log("1");
                Planet selectedPlanet = GameObject.Find("planet" + i.ToString()).GetComponent<Planet>();
                selectedPlanet.population -= planetData.colonizePopCost;
                if (i != ActivePlanetId 
                    && selectedPlanet.population >= planetData.popIncreaseThreshold 
                    && (selectedPlanet.population - planetData.popIncreaseThreshold) >= planetData.colonizePopCost)
                {
                    Debug.Log("2");
                    selectedPlanet.population -= planetData.colonizePopCost;
                    gameDetails.money -= planetData.colonizeMoneyCost;
                    activePlanet.population += planetData.colonizePopCost;
                    colonizeMoneyCost *= planetData.colonizeMoneyCostScale;
                    break;
                }
            }
        }
    }

    public void AddPopulationGrowth()
    {
        fixedPopGrowth += 1;
        autoGrowthCost *= planetData.popAutoGrowthCostScale;
    }

    //The planet is selected in the details component
    public void SetSelected(int id)
    {
        if (id == planetID)
        {
            IsSelected = true;
            if (IsTerraformed)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = InhabitedPlanetSelectedSprite;
            } else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = UninhabitedPlanetSelectedSprite;
            }
        } else
        {
            IsSelected = false;
            if (IsTerraformed)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = InhabitedPlanetSprite;
            } else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = UninhabitedPlanetSprite;
            }
        }
    }
}
