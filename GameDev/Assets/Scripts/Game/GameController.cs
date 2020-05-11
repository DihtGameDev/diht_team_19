using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static bool GameIsPaused = false;
    public GameObject PauseMenuUI;
    public GameObject SidePanelUI;
    public SidePanelBehaviour SidePanelController;
    
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

    public void Register(PredatorController predator)
    {
        foxes.Add(predator);
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

    void Start()
    {
        Debug.Log("The game has started!");
    }

    void Update()
    {
        CheckPressedButtons();
    }

    void CheckPressedButtons()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                PlayWithPause();
            }
            else
            {
                PauseWithMenu();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
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
    }

    private List<PredatorController> foxes = new List<PredatorController>();
    private List<FoodSourceBehaviour> foodSources = new List<FoodSourceBehaviour>();
}
