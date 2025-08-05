using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject howto;
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void EnableHowTo()
    {
        howto.SetActive(true);
    }
    public void BackHome()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
