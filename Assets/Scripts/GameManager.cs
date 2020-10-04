using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Spaceship Player;

    [SerializeField] private GameObject winnigScreen;
    [SerializeField] public UIScanner Scanner;

    [SerializeField] public bool FreeEverything = false;

    private void Awake()
    {
        Instance = this;
        Player = FindObjectOfType<Spaceship>();
        Upgrade.LoadUpgradeDictionary();
    }

    public void Win()
    {
        winnigScreen.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Continue()
    {
        winnigScreen.SetActive(false);
    }
}