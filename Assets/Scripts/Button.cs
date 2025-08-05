using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public GameObject Timer;
    public GameObject setup;

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Home()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void TimerDown()
    {
        GameTimer gametimer=Timer.GetComponent<GameTimer>();
        gametimer.timeRemaining = 3f;
        Destroy(setup);
    }
}
