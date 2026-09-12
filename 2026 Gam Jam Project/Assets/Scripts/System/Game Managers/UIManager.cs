using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMainGame()
    {
        SceneManager.LoadScene("Game");
    }
}
