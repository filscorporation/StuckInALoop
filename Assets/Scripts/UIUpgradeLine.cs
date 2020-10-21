using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeLine : MonoBehaviour
{
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text costText;
    [SerializeField] private GameObject underConstructionIcon;
    [SerializeField] private Text timeLeftText;

    private Upgrade upgrade;

    private void Update()
    {
        if (upgrade.TimeLeft > 0)
        {
            if (!timeLeftText.gameObject.activeSelf)
            {
                timeLeftText.gameObject.SetActive(true);
                underConstructionIcon.SetActive(true);
            }
            timeLeftText.text = upgrade.TimeLeft.ToString("F1");
        }
        else
        {
            if (timeLeftText.gameObject.activeSelf)
            {
                timeLeftText.gameObject.SetActive(false);
                underConstructionIcon.SetActive(false);
            }
        }
    }

    public void Initialize(UIUpgradeMenu menu, Upgrade _upgrade)
    {
        upgrade = _upgrade;
        
        descriptionText.text = $"{upgrade.Name}: {upgrade.Description}";
        descriptionText.color = upgrade.Installed ? Color.gray : Color.white;
        costText.text = $"Cost: {(upgrade.Installed ? upgrade.Cost.ToString() : upgrade.Cost.RelativeToString(GameManager.Instance.Player.Resources))}";
        costText.color = upgrade.Installed ? Color.gray : Color.white;
        GetComponent<Button>().interactable = !_upgrade.Installed;
    }

    public void OnClick()
    {
        if (GameManager.Instance.Player.TryPayResources(upgrade.Cost))
        {
            upgrade.Installed = true;
            GameManager.Instance.Player.ApplyUpgrade(upgrade);
            GetComponent<Button>().interactable = false;
            descriptionText.color = Color.gray;
            costText.text = $"Cost: {upgrade.Cost.ToString()}";
            costText.color = Color.gray;
        }
    }
}