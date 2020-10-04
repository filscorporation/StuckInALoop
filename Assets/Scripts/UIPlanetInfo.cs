using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPlanetInfo : MonoBehaviour
{
    [SerializeField] private Text titanText;
    [SerializeField] private Text crystalsText;
    [SerializeField] private Text dangerText;
    [SerializeField] private Image dangerIcon;
    [SerializeField] private Sprite homeIcon;
    [SerializeField] private Text travelCostText;

    private RectTransform rectTransform;
    
    public void Initialize(Planet planet)
    {
        rectTransform = GetComponent<RectTransform>();
        transform.SetParent(FindObjectOfType<Canvas>().transform);
        int side = planet.transform.position.x > GameManager.Instance.Player.transform.position.x ? -1 : 1;
        if (GameManager.Instance.Player.GetCurrentPlanet() == planet)
            side = -side;
        rectTransform.pivot = new Vector2(side == 1 ? 0 : 1, 0.5f);
        transform.position = Camera.main.WorldToScreenPoint(planet.transform.position + new Vector3(side, 0));
        
        titanText.text = Mathf.CeilToInt(planet.Titan).ToString();
        crystalsText.text = Mathf.CeilToInt(planet.Crystals).ToString();
        if (planet.IsHomePlanet)
        {
            dangerText.text = String.Empty;
            dangerIcon.sprite = homeIcon;
        }
        else
        {
            dangerText.text = planet.DangerLevel.ToString();
        }

        travelCostText.text = Mathf.CeilToInt(GameManager.Instance.Player.EnergyToMoveToPlanet(planet)).ToString();
    }

    public void Refresh()
    {
        StopAllCoroutines();
    }

    public void UpdateValues(Planet planet)
    {
        int side = planet.transform.position.x > GameManager.Instance.Player.transform.position.x ? -1 : 1;
        if (GameManager.Instance.Player.GetCurrentPlanet() == planet)
            side = -side;
        rectTransform.pivot = new Vector2(side == 1 ? 0 : 1, 0.5f);
        transform.position = Camera.main.WorldToScreenPoint(planet.transform.position + new Vector3(side, 0));
        
        titanText.text = Mathf.RoundToInt(planet.Titan).ToString();
        crystalsText.text = Mathf.RoundToInt(planet.Crystals).ToString();
    }

    public void Hide()
    {
        StartCoroutine(HideCoroutine());
    }

    private IEnumerator HideCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}