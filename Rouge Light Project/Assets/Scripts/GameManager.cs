using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public void playGame()
    {
        SceneManager.LoadScene("SampleScene");
        //SceneManager.LoadScene.(1);
    }

    public void backMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        //SceneManager.LoadScene.(0);
    }

    public void exitGame()
    {
        Debug.Log("Игра закрылась");
        Application.Quit();
    }
}


