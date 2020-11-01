using System;
using DataManagement;
using UIManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MapManagement
{
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
        public int PrefabIndex = -1;
        public int Index;
    
        [SerializeField] private GameObject uiPlanetInfoPrefab;
        private UIPlanetInfo uiInfo;

        private void Start()
        {
            if (!WasLoaded)
            {
                Titan = titanMax;
                Crystals = crystalsMax;
            }

            if (PrefabIndex < 0)
                PrefabIndex = PlanetGenerator.Instance.GetPlanetPrefabIndex(this);
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
    
        public override IData ToData()
        {
            return new PlanetData
            {
                Index = Index,
                PrefabIndex = PrefabIndex,
                PositionX = transform.position.x,
                PositionY = transform.position.y,
                TitanLeft = Titan,
                CrystalsLeft = Crystals,
            };
        }

        [Serializable]
        public class PlanetData : IData
        {
            public int Priority => 0;

            public int Index;
            public int PrefabIndex;
            public float PositionX;
            public float PositionY;
            public float TitanLeft;
            public float CrystalsLeft;
        
            public DataObject ToObject()
            {
                Planet planet = PlanetGenerator.Instance.SpawnPlanet(PrefabIndex, new Vector3(PositionX, PositionY));
                planet.Index = Index;
                planet.PrefabIndex = PrefabIndex;
                planet.Titan = TitanLeft;
                planet.Crystals = CrystalsLeft;

                planet.WasLoaded = true;
            
                return planet;
            }
        }
    
        #endregion
    }
}