using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class UIRepairButton : MonoBehaviour
    {
        [SerializeField] private Text costText;
        private const float DEAULT_REPAIR_COST = 5f;
        private const float TIME_TO_REPAIR = 3f;
        private Button button;
        private bool isRepairing = false;

        private void Start()
        {
            costText.text = Mathf.RoundToInt(DEAULT_REPAIR_COST).ToString();
            button = GetComponent<Button>();
        }

        private void Update()
        {
            if (isRepairing)
                return;
            button.interactable = GameManager.Instance.Player.Health < GameManager.Instance.Player.HealthMax;
        }

        public void Repair()
        {
            if (GameManager.Instance.Player.TryPayResources(new Cost(titan: DEAULT_REPAIR_COST)))
            {
                button.interactable = false;
                isRepairing = true;
                StartCoroutine(RepairCoroutine());
            }
        }

        private IEnumerator RepairCoroutine()
        {
            StartCoroutine(GameManager.Instance.Player.Repair(TIME_TO_REPAIR));
            yield return new WaitForSeconds(TIME_TO_REPAIR);
            isRepairing = false;
        }
    }
}