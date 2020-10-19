using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIUpgradeMenu : MonoBehaviour
{
    private List<UIUpgradeLine> lines = new List<UIUpgradeLine>();
    private const float LINE_HEIGHT = 150f;
    private const float LINE_SPACING = 20f;
    [SerializeField] private GameObject uiUpgradeLinePrefab;
    [SerializeField] private RectTransform scrollContentRect;
    [SerializeField] private Toggle showInstalledToggle;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && gameObject.activeSelf)
        {
            RectTransform rt = GetComponent<RectTransform>();
            Vector2 localMousePosition = rt.InverseTransformPoint(Input.mousePosition);
            if (!rt.rect.Contains(localMousePosition))
            {
                ShowHide();
            }
        }
    }

    public void Rebuild()
    {
        Clear();
        
        float y = 0;
        bool showInstalled = showInstalledToggle.isOn;
        foreach (var pair in Upgrade.UpgradeDictionary.OrderBy(p => p.Value.Level))
        {
            if (!showInstalled && pair.Value.Installed)
                continue;
            lines.Add(CreateUpgradeLine(pair.Value, -y));
            y += (LINE_HEIGHT + LINE_SPACING);
        }

        y -= LINE_SPACING;
        scrollContentRect.sizeDelta = new Vector2(scrollContentRect.sizeDelta.x, y);
    }

    public void ShowHide()
    {
        if (gameObject.activeSelf)
        {
            Clear();
            gameObject.SetActive(false);
            
            return;
        }
        
        gameObject.SetActive(true);
        
        Rebuild();
    }

    private UIUpgradeLine CreateUpgradeLine(Upgrade upgrade, float y)
    {
        GameObject go = Instantiate(uiUpgradeLinePrefab);
        UIUpgradeLine line = go.GetComponent<UIUpgradeLine>();
        line.transform.SetParent(scrollContentRect);
        line.transform.localScale = Vector3.one;
        RectTransform rect = line.GetComponent<RectTransform>();
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = new Vector2(0, LINE_HEIGHT);
        rect.anchoredPosition = new Vector2(0, y);
        line.Initialize(this, upgrade);

        return line;
    }

    private void Clear()
    {
        foreach (UIUpgradeLine line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();
    }
}