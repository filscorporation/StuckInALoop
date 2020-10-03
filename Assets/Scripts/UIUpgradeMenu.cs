using System.Collections.Generic;
using UnityEngine;

public class UIUpgradeMenu : MonoBehaviour
{
    private List<UIUpgradeLine> lines = new List<UIUpgradeLine>();
    private const float LINE_HEIGHT = 150f;
    private const float LINE_SPACING = 20f;
    [SerializeField] private GameObject uiUpgradeLinePrefab;
    [SerializeField] private RectTransform scrollContentRect;
    
    public void ShowHide()
    {
        if (gameObject.activeSelf)
        {
            Hide();
            return;
        }
        
        gameObject.SetActive(true);
        
        float y = 0;
        foreach (Upgrade upgrade in Upgrade.UpgradeDictionary.Values)
        {
            lines.Add(CreateUpgradeLine(upgrade, -y));
            y += (LINE_HEIGHT + LINE_SPACING);
        }

        y -= LINE_SPACING;
        scrollContentRect.anchoredPosition = Vector2.zero;
        scrollContentRect.sizeDelta = new Vector2(scrollContentRect.sizeDelta.x, y);
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

    private void Hide()
    {
        foreach (UIUpgradeLine line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();
        gameObject.SetActive(false);
    }
}