using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Planet : MonoBehaviour
{
    public float Titan;
    [SerializeField] private int titanMax;
    public float Crystals;
    [SerializeField] private int crystalsMax;
    [SerializeField] public int Mass;
    [SerializeField] private bool isHomePlanet = false;

    private void Start()
    {
        Titan = titanMax;
        Crystals = crystalsMax;
    }
    
    public float GetOrbit()
    {
        return Mass;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        GameManager.Instance.Player.MoveToPlanet(this);
    }
}