using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivorMainMenu : MonoBehaviour
{
    public string gameplaySceneName = "GameScene";

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        
        Debug.Log("Quit requested.");
    }
}