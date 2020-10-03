using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Spaceship Player;

    private void Awake()
    {
        Instance = this;
        Player = FindObjectOfType<Spaceship>();
        Upgrade.LoadUpgradeDictionary();
    }
}