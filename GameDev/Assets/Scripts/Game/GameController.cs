using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController singleton;


    private static bool GameIsPaused;
    private PredatorController CurrentActive;
    private readonly List<FoodSourceBehaviour> foodSources = new List<FoodSourceBehaviour>();

    [SerializeField] public GameObject foxPrefab;

    public GameObject PauseMenuUI;

    private List<List<PredatorController>> predator_factions;
    public SidePanelBehaviour SidePanelController;
    public GameObject SidePanelUI;
    public GameObject Starter;

    [SerializeField] public GameObject wolfPrefab;
    [SerializeField] public GameInfoBehaviour gameInfo;
    
    public static GameController Get()
    {
        return singleton;
    }

    private static void init(GameController gc)
    {
        if (singleton == null) singleton = gc;
    }

    public void ShowSidePanel(PredatorController predator)
    {
        SidePanelUI.SetActive(true);
        SidePanelController.Bind(predator);
    }

    public void CloseSidePanel()
    {
        SidePanelController.Bind(null);
        SidePanelUI.SetActive(false);
    }

    public void SetActive(PredatorController predator)
    {
        CurrentActive = predator;
        ShowSidePanel(predator);
    }

    public void UnsetActive()
    {
        if (CurrentActive) CurrentActive.AfterClick();
        CurrentActive = null;
    }

    public void ExitToMainMenu()
    {
        PlayWithPause();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseWithMenu()
    {
        PauseMenuUI.SetActive(true);
        Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void PlayWithPause()
    {
        PauseMenuUI.SetActive(false);
        Play();
    }

    public void Play()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Register(PredatorController predator, int player_id)
    {
        predator_factions[player_id].Add(predator);
        if (player_id == 0)
        {
            gameInfo.UpdateAliveCount(predator_factions[0].Count);
        }
    }

    public void Unregister(PredatorController predator, int player_id)
    {
        predator_factions[player_id].Remove(predator);
        if (player_id == 0)
        {
            gameInfo.UpdateAliveCount(predator_factions[0].Count);
        }
    }

    public void Register(FoodSourceBehaviour source)
    {
        foodSources.Add(source);
    }

    public FoodSourceBehaviour GetClosestFoodSource(Vector3 position)
    {
        var min_distance = double.PositiveInfinity;
        FoodSourceBehaviour answer = null;

        foreach (var source in foodSources)
        {
            var cur_distance = Vector3.Distance(position, source.transform.position);
            if (cur_distance < min_distance)
            {
                answer = source;
                min_distance = cur_distance;
            }
        }

        return answer;
    }

    private void Start()
    {
        Pause();
        init(this);

        Debug.Log("The game has started!");
    }

    private void Update()
    {
        CheckPressedButtons();
    }

    private void CheckPressedButtons()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                PlayWithPause();
            else
                PauseWithMenu();
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
                if (hit.transform)
                {
                    Clickable cmp;
                    if (cmp = hit.transform.GetComponent<Clickable>())
                    {
                        cmp.OnClick.Invoke();
                    }
                    else
                    {
                        CloseSidePanel();
                        UnsetActive();
                    }
                }
        }
    }

    public void Choose(string choice)
    {
        GameSettings.chosen = choice;
        predator_factions = new List<List<PredatorController>>(GameSettings.number_of_players);
        for (var i = 0; i < GameSettings.number_of_players; ++i) predator_factions.Add(new List<PredatorController>());

        Spawn(5, choice, new Vector3(500, 0, 400), 0);
        Starter.SetActive(false);
        Play();
    }

    private void Spawn(int num, string type, Vector3 where, int player)
    {
        GameObject prefab;
        switch (type)
        {
            case "fox":
                prefab = foxPrefab;
                break;
            case "wolf":
                prefab = wolfPrefab;
                break;
            default:
                Debug.Log("Could not define animal to create");
                return;
        }

        for (var i = 0; i < num; ++i)
        {
            var clone = Instantiate(prefab, where, Quaternion.identity);
            var predator = clone.GetComponent<PredatorController>();
            if (predator)
            {
                predator.owner = player;
                Register(predator, player);
            }
        }
    }
}