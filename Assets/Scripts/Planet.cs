using UnityEngine;
using UnityEngine.EventSystems;

public enum DangerLevel
{
    None,
    Low,
    High,
}

public class Planet : MonoBehaviour
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
}