using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static bool GameIsPaused = false;
    public GameObject PauseMenuUI;

    public void ExitToMainMenu()
    {
        Play();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Play()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Register(FoxController fox)
    {
        foxes.Add(fox);
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
                Play();
            }
            else
            {
                Pause();
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
                    Debug.Log("Found something:");
                    Debug.Log(hit.transform.gameObject.name);
                    Clickable cmp;
                    if (cmp = hit.transform.GetComponent<Clickable>())
                    {
                        cmp.OnClick.Invoke();
                    }
                }
            }
        }
    }

    private List<FoxController> foxes = new List<FoxController>();
    private List<FoodSourceBehaviour> foodSources = new List<FoodSourceBehaviour>();
}
