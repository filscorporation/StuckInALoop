using System;
using System.Collections.Generic;
using DataManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DangerLevel
{
    None,
    Low,
    High,
}

public class Planet : DataObject
{
    public float Titan;
    [SerializeField] private int titanMax;
    public float Crystals;
    [SerializeField] private int crystalsMax;
    [SerializeField] public int Mass;
    [SerializeField] public bool IsHomePlanet = false;
    [SerializeField] public DangerLevel DangerLevel;

    [SerializeField] private GameObject uiPlanetInfoPrefab;
    private UIPlanetInfo uiInfo;

    private void Start()
    {
        Titan = titanMax;
        Crystals = crystalsMax;

        index = planets.Count;
        planets.Add(this);
    }

    private void Update()
    {
        if (uiInfo != null)
        {
            uiInfo.UpdateValues(this);
        }
    }

    public float GetOrbit()
    {
        return 2;
    }

    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        if (uiInfo != null)
        {
            uiInfo.Refresh();
        }
        else
        {
            uiInfo = Instantiate(uiPlanetInfoPrefab).GetComponent<UIPlanetInfo>();
            uiInfo.Initialize(this);
        }
    }

    private void OnMouseExit()
    {
        if (uiInfo != null)
        {
            uiInfo.Hide();
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        GameManager.Instance.Player.MoveToPlanet(this);
    }
    
    #region Data

    private int index;
    private static List<Planet> planets = new List<Planet>();
    
    public override IData ToData()
    {
        return new PlanetData
        {
            Index = index,
            PositionX = transform.position.x,
            PositionY = transform.position.y,
            TitanLeft = Titan,
            CrystalsLeft = Crystals,
        };
    }

    [Serializable]
    public class PlanetData : IData
    {
        public int Index; // TODO: instantiate
        public float PositionX;
        public float PositionY;
        public float TitanLeft;
        public float CrystalsLeft;
        
        public DataObject ToObject()
        {
            Planet planet = planets[Index];
            planet.transform.position = new Vector3(PositionX, PositionY);
            planet.Titan = TitanLeft;
            planet.Crystals = CrystalsLeft;
            
            return planet;
        }
    }
    
    #endregion
}