using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class UIUpgradeLine : MonoBehaviour
    {
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text costText;
        [SerializeField] private GameObject underConstructionIcon;
        [SerializeField] private Text timeLeftText;
        [SerializeField] private AudioClip onClickSound;

        private UIUpgradeMenu menu;
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
                    
                    menu.Rebuild();
                }
            }
        }

        public void Initialize(UIUpgradeMenu _menu, Upgrade _upgrade)
        {
            menu = _menu;
            upgrade = _upgrade;
        
            descriptionText.text = $"{upgrade.Name}: {upgrade.Description}";
            descriptionText.color = upgrade.Installed ? Color.gray : Color.white;
            costText.text = $"Cost: {(upgrade.Installed ? upgrade.Cost.ToString() : upgrade.Cost.RelativeToString(GameManager.Instance.Player.Resources))}";
            costText.color = upgrade.Installed ? Color.gray : Color.white;
            GetComponent<Button>().interactable = !_upgrade.Installed;
        }

        public void OnClick()
        {
            menu.AudioSource.PlayOneShot(onClickSound);
            
            if (GameManager.Instance.Player.TryPayResources(upgrade.Cost))
            {
                upgrade.Installed = true;
                GameManager.Instance.Player.ApplyUpgrade(upgrade);
                GetComponent<Button>().interactable = false;
                descriptionText.color = Color.gray;
                costText.text = $"Cost: {upgrade.Cost.ToString()}";
                costText.color = Color.gray;
                
                menu.Rebuild();
            }
        }
    }
}