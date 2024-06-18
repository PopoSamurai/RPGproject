using UnityEngine;
using UnityEngine.SceneManagement;
public class menuScr : MonoBehaviour
{
    public void StartGame()
    {
        Invoke("ChangeScene", 1f);
    }
    private void ChangeScene()
    {
        SceneManager.LoadScene("main");
    }
    public void ExitGame()
    {
        Invoke("Application.Quit()", 1f);
    }
}