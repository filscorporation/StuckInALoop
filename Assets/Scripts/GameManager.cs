using System.Collections;
using DataManagement;
using MapManagement;
using UIManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Spaceship Player { get; private set; }
    
    [SerializeField] public bool Cheat = false;

    private bool isPaused = false;
    private float timeScale = 1f;
    private bool isGameEnded = false;
    
    private const int LOAD_SCENE_INDEX = 0;
    private const int STARTING_SCENE_INDEX = 1;
    
    public bool Pause
    {
        get => isPaused;
        set
        {
            if (isPaused == value)
                return;

            isPaused = value;

            if (isPaused)
            {
                timeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = timeScale;
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        Player = FindObjectOfType<Spaceship>();
        Upgrade.InitializeUpgradeDictionary();
        StartCoroutine(LoadGameIfExists());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            DataManager.Save();
            
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(LOAD_SCENE_INDEX);
        }
    }

    private void OnApplicationQuit()
    {
        if (!isGameEnded)
            DataManager.Save();
    }

    private IEnumerator LoadGameIfExists()
    {
        if (SceneManager.GetActiveScene().buildIndex == STARTING_SCENE_INDEX)
            yield break;
        
        if (!DataManager.HasSaveFile())
        {
            //WorldGenerator.Instance.Generate(new WorldParams(WorldLevel.Universe, 3, 1));
            SceneManager.LoadScene(STARTING_SCENE_INDEX);
            yield break;
        }
        
        Pause = true;
        
        yield return null;
        
        DataManager.Load();
        Pause = false;
        FindObjectOfType<CameraController>().FollowInstant();
    }
        
    public void Win()
    {
        if (Player.IsDead)
            return;
                
        Player.Won();
        UIManager.Instance.Win();
    }
        
    public void Lose()
    {
        isGameEnded = true;
        UIManager.Instance.Lose();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Pause = false;
        DataManager.ClearSave();
        SceneManager.LoadScene(STARTING_SCENE_INDEX);
    }
}