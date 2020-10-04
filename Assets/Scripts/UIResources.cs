using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIResources : MonoBehaviour
{
    [SerializeField] private Text energyText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text titanText;
    [SerializeField] private Text crystalsText;

    public void SetEnegry(float value, float max)
    {
        energyText.text = $"{Mathf.FloorToInt(value)}/{Mathf.FloorToInt(max)}";
    }
    
    public void SetHealth(float value, float max)
    {
        healthText.text = $"{Mathf.FloorToInt(value)}/{Mathf.FloorToInt(max)}";
    }
    
    public void SetTitan(float value, float max)
    {
        titanText.text = $"{Mathf.FloorToInt(value)}/{Mathf.FloorToInt(max)}";
    }
    
    public void SetCrystals(float value, float max)
    {
        crystalsText.text = $"{Mathf.FloorToInt(value)}/{Mathf.FloorToInt(max)}";
    }
    
    public void NotEnoughEnergy()
    {
        StopAllCoroutines();
        StartCoroutine(NotEnoughEnergyCoroutine());
    }

    private IEnumerator NotEnoughEnergyCoroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            energyText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            energyText.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }
}