using UnityEngine;
public class GameManager : MonoBehaviour
{
    public GameObject[] scenes;
    public int sceneIndex;
    void Start()
    {
        sceneIndex = 0;
    }
    void Update()
    {
        for (var i = 0; i < scenes.Length; i++)
        {
            if (i == sceneIndex)
            {
                scenes[i].SetActive(true);
            }
            else scenes[i].SetActive(false);
        }
    }
    public void StartGame()
    {
        sceneIndex = 1;
    }
    public void RetryGame()
    {
        sceneIndex = 0;
    }
    public void ExitApp()
    {
        Application.Quit();
    }
}