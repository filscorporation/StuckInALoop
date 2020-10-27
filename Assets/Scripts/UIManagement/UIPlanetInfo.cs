using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class UIPlanetInfo : MonoBehaviour
    {
        [SerializeField] private Text titanText;
        [SerializeField] private Text crystalsText;
        [SerializeField] private Text dangerText;
        [SerializeField] private Image dangerIcon;
        [SerializeField] private Sprite homeIcon;
        [SerializeField] private Text travelCostText;
        [SerializeField] private Text massText;

        public void Initialize(Planet planet)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            transform.SetParent(FindObjectOfType<Canvas>().transform);
            rectTransform.anchoredPosition = Vector2.zero;
            transform.localScale = Vector3.one;

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
            massText.text = $"Mass: {planet.Mass}";
        }

        public void Refresh()
        {
            StopAllCoroutines();
        }

        public void UpdateValues(Planet planet)
        {
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
}