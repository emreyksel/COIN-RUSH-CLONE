using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject[] obstacles;

    public GameObject winPanel;
    public GameObject failPanel;

    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isGameWin = false;
    [HideInInspector] public bool isGameFinish = false;
    [HideInInspector] public bool isGameStart = false;

    [SerializeField] private float delay = 1.5f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
    }

    private void Update()
    {
        if (isGameOver)
        {
            Invoke(nameof(FailPanelActive), delay);
        }
        else if (isGameWin)
        {
            Invoke(nameof(WinPanelActive), delay);
        }
    }

    public void WinPanelActive()
    {
        winPanel.SetActive(true);
    }

    public void FailPanelActive()
    {
        failPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
