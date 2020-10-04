using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Spaceship Player;

    [SerializeField] private GameObject winnigScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject helpScreen;
    [SerializeField] public UIScanner Scanner;
    [SerializeField] public bool Cheat = false;

    [SerializeField] private GameObject damageEffect;
    private float damageEffectTimer = 0;
    private const float DAMAGE_EFFECT_LENGTH = 0.5f;

    private void Awake()
    {
        Instance = this;
        Player = FindObjectOfType<Spaceship>();
        Upgrade.LoadUpgradeDictionary();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Exit();
        
        if (damageEffectTimer > 0)
        {
            damageEffectTimer -= Time.deltaTime;
            if (damageEffectTimer <= 0)
                damageEffect.SetActive(false);
        }
    }

    public void ShowDamageEffect()
    {
        damageEffect.SetActive(true);
        damageEffectTimer = DAMAGE_EFFECT_LENGTH;
    }

    public void Win()
    {
        if (Player.IsDead)
            return;
        
        Player.Won();
        winnigScreen.SetActive(true);
    }

    public void Lose()
    {
        helpScreen.SetActive(false);
        loseScreen.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Continue()
    {
        winnigScreen.SetActive(false);
    }

    public void ShowHideHelp()
    {
        helpScreen.SetActive(!helpScreen.activeSelf);
    }
}