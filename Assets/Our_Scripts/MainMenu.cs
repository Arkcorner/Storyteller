using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);

    }
    public void PlayGame2()
    {
        SceneManager.LoadSceneAsync(2);
    }
    public void QuitGame()
     {
        Application.Quit();

     }
}
