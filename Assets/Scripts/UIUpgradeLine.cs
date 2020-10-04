using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeLine : MonoBehaviour
{
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text costText;

    private Upgrade upgrade;

    public void Initialize(UIUpgradeMenu menu, Upgrade _upgrade)
    {
        upgrade = _upgrade;
        
        descriptionText.text = $"{upgrade.Name}: {upgrade.Description}";
        descriptionText.color = upgrade.Installed ? Color.gray : Color.white;
        costText.text = $"Cost: {upgrade.Cost}";
        costText.color = upgrade.Installed ? Color.gray : Color.white;
        GetComponent<Button>().interactable = !_upgrade.Installed;
    }

    public void OnClick()
    {
        if (GameManager.Instance.Player.TryPayResources(upgrade.Cost))
        {
            upgrade.Installed = true;
            GameManager.Instance.StartCoroutine(ApplyUpgrade());
            GetComponent<Button>().interactable = false;
            descriptionText.color = Color.gray;
            costText.color = Color.gray;
        }
    }

    private IEnumerator ApplyUpgrade()
    {
        if (!GameManager.Instance.Cheat)
            yield return new WaitForSeconds(upgrade.Cost.Time);

        GameManager.Instance.Player.Upgrade();
        upgrade.Apply();
    }
}