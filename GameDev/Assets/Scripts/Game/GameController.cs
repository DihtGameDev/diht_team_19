using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    private static GameController singleton;
    public static Random rnd = new Random();
    private static bool GameIsPaused;
    private Displayable CurrentActive;
    private readonly List<FoodSourceBehaviour> foodSources = new List<FoodSourceBehaviour>();
    public GameObject PauseMenuUI;
    public List<Player> players;
    public SidePanelBehaviour SidePanelController;
    public GameObject SidePanelUI;
    public GameObject Starter;

    private const string food_source_id = "food_source";
    private const string initial_food_source_id = "initial_food_source";
    private const string fox_id = "fox";
    private const string wolf_id = "wolf";
    
    [SerializeField] public GameObject foxPrefab;
    [SerializeField] public GameObject wolfPrefab;
    [SerializeField] private GameObject initialFoodSourcePrefab;
    [SerializeField] private GameObject basicFoodSourcePrefab;
    [SerializeField] public GameInfoBehaviour gameInfo;

    private readonly Tuple<Vector3, Vector3> playAreaBorders = new Tuple<Vector3, Vector3>(
        new Vector3(400, 1, 400), 
        new Vector3(600, 1, 600)
        );

    private float GetPlayAreaWidth()
    {
        return playAreaBorders.Item2.x - playAreaBorders.Item1.x;
    }
    
    private float GetPlayAreaLength()
    {
        return playAreaBorders.Item2.z - playAreaBorders.Item1.z;
    }

    private Vector3 GetPlayareaCenter()
    {
        return playAreaBorders.Item1 + (playAreaBorders.Item2 - playAreaBorders.Item1) / 2;
    }
    
    public static GameController Get()
    {
        return singleton;
    }

    private static void init(GameController gc)
    {
        if (singleton == null) singleton = gc;
    }

    public void updatePointInfo()
    {
        gameInfo.UpdatePointsCount(players[0].points);
    }
    
    public void ShowSidePanel(Displayable obj)
    {
        SidePanelUI.SetActive(true);
        SidePanelController.Bind(obj);
    }

    public void CloseSidePanel()
    {
        SidePanelController.Bind(null);
        SidePanelUI.SetActive(false);
    }

    public void SetActive(Displayable obj)
    {
        CurrentActive = obj;
        ShowSidePanel(obj);
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
        players[player_id].predators.Add(predator);
        if (player_id == 0)
        {
            gameInfo.UpdateAliveCount(players[player_id].predators.Count);
        }
    }

    public void Unregister(PredatorController predator, int player_id)
    {
        players[player_id].predators.Remove(predator);
        if (player_id == 0)
        {
            gameInfo.UpdateAliveCount(players[player_id].predators.Count);
        }
    }

    public void Register(FoodSourceBehaviour source)
    {
        foodSources.Add(source);
    }

    public void Unregister(FoodSourceBehaviour source)
    {
        foodSources.Remove(source);
        if (foodSources.Count < players.Count * 1.5)
        {
            SpawnNear(1, food_source_id, GetPlayareaCenter(), 0.01f, GetPlayAreaWidth() / 11 * 4);
        }
    }

    public FoodSourceBehaviour GetClosestFoodSource(Vector3 position)
    {
        var min_distance = double.PositiveInfinity;
        FoodSourceBehaviour answer = null;

        foreach (var source in foodSources)
        {
            if (!source.gameObject.activeSelf) continue;
            
            var cur_distance = Vector3.Distance(position, source.transform.position);
            if (cur_distance < min_distance)
            {
                answer = source;
                min_distance = cur_distance;
            }
        }

        return answer;
    }

    public PredatorController GetClosestEnemy(Vector3 position, int player_id)
    {
        var min_distance = double.PositiveInfinity;
        PredatorController answer = null;

        for (var i = 0; i < players.Count; ++i)
        {
            if (i == player_id) continue;
            
            foreach (var enemy in players[i].predators)
            {
                if (!enemy.gameObject.activeSelf) continue;
            
                var cur_distance = Vector3.Distance(position, enemy.transform.position);
                if (cur_distance < min_distance)
                {
                    answer = enemy;
                    min_distance = cur_distance;
                }
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
            UnsetActive();
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
                    }
                }
        }
    }

    private string[] predator_types = { fox_id, wolf_id };

    private void InitUpgradeTree(Player player, string choice)
    {
        switch (choice)
        {
            case fox_id:
                player.tree.Open(Skill.foxUnique);
                break;
            case wolf_id:
                player.tree.Open(Skill.wolfUnique(1));
                break;
            default:
                break;
        }
    }

    private void InitSpawnArea(Player player)
    {
        var distance = (GetPlayAreaWidth() / 2 + GetPlayAreaLength() / 2) / 2;
        player.SpawnLocation = GetPlaceNear(GetPlayareaCenter(), distance * 0.8f, distance);
        Spawn(initial_food_source_id, player.SpawnLocation);
    }
    
    private void InitPlayer(Player player, string choice)
    {
        InitUpgradeTree(player, choice);
        InitSpawnArea(player);
    }
    public void Choose(string choice)
    {
        GameSettings.chosen = choice;
        players = new List<Player>(GameSettings.number_of_players);

        for (var i = 0; i < GameSettings.number_of_players; ++i)
        {
            var predator_type = choice;
            if (i > 0)
            {
                predator_type = predator_types[rnd.Next(predator_types.Length)];
            }
            var player = new Player();
            InitPlayer(player, choice);
            players.Add(player);
            SpawnNear(5, predator_type, player.SpawnLocation, 2, 15, i);
            SpawnNear(3, food_source_id, player.SpawnLocation, 0.3f, 30);
        }
        
        Starter.SetActive(false);

        updatePointInfo();
        Play();
    }

    GameObject prefabFromString(string type)
    {
        switch (type)
        {
            case fox_id:
                return foxPrefab;
            case wolf_id:
                return wolfPrefab;
            case food_source_id:
                return basicFoodSourcePrefab;
            case initial_food_source_id:
                return initialFoodSourcePrefab;
            default:
                Debug.Log("Could not define prefab to create");
                return null;
        }
    }

    public static Vector3 GetPlaceNear(Vector3 pivot, float minDistance, float maxDistance)
    {
        var angle = UnityEngine.Random.Range(0, (float) Math.PI * 2);
        var magnitude = UnityEngine.Random.Range(minDistance, maxDistance);
        Vector3 random_place = new Vector3(magnitude * (float) Math.Sin(angle), 0, magnitude * (float) Math.Cos(angle));
        return random_place + pivot;
    }
    private void SpawnNear(int num, string type, Vector3 where, float minDistance, float maxDistance, int player)
    {
        foreach (var _ in Enumerable.Repeat(0, num))
        {
            Spawn(type, GetPlaceNear(where, minDistance, maxDistance), player);
        }
    }
    
    private void SpawnNear(int num, string type, Vector3 where, float minDistance, float maxDistance)
    {
        foreach (var _ in Enumerable.Repeat(0, num))
        {
            Spawn(type, GetPlaceNear(where, minDistance, maxDistance));
        }
    }
    
    private void Spawn(string type, Vector3 where)
    {
        var prefab = prefabFromString(type);
        var clone = Instantiate(prefab, where, Quaternion.identity);
    }
    
    private void Spawn(string type, Vector3 where, int player)
    {
        var prefab = prefabFromString(type);
        var clone = Instantiate(prefab, where, Quaternion.identity);
        var predator = clone.GetComponent<PredatorController>();
        if (predator)
        {
            predator.owner = player;
            Register(predator, player);
        }
    }
}