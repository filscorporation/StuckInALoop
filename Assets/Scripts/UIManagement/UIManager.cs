using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIManagement
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        
        [SerializeField] private GameObject winnigScreen;
        [SerializeField] private GameObject loseScreen;
        [SerializeField] private GameObject helpScreen;
        [SerializeField] private GameObject menuScreen;
        [SerializeField] public UIScanner Scanner;

        [SerializeField] private GameObject damageEffect;
        private float damageEffectTimer = 0;
        private const float DAMAGE_EFFECT_LENGTH = 0.5f;

        private void Awake()
        {
            Instance = this;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ShowHideMenu();
            
            if (damageEffectTimer > 0)
            {
                damageEffectTimer -= Time.deltaTime;
                if (damageEffectTimer <= 0)
                    damageEffect.SetActive(false);
            }
        }
    
        public void ShowHideMenu()
        {
            menuScreen.SetActive(!menuScreen.activeSelf);
    
            GameManager.Instance.Pause = menuScreen.activeSelf;
        }
    
        public void ShowDamageEffect()
        {
            damageEffect.SetActive(true);
            damageEffectTimer = DAMAGE_EFFECT_LENGTH;
        }
    
        public void Win()
        {
            winnigScreen.SetActive(true);
        }
    
        public void Lose()
        {
            helpScreen.SetActive(false);
            loseScreen.SetActive(true);
        }
    
        public void Exit()
        {
            GameManager.Instance.Exit();
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
}