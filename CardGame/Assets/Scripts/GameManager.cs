using UnityEngine;
public class GameManager : MonoBehaviour
{
    public GameObject[] scenes;
    public int sceneIndex;
    public int floorNumber = 1;
    public int goldAmount;
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
    public void SelectChar()
    {
        sceneIndex = 1;
    }
    public void GameOn()
    {
        sceneIndex = 2;
    }
    public void RetryGame()
    {
        sceneIndex = 0;
    }
    public void ExitApp()
    {
        Application.Quit();
    }
    public void RestScreen(string sceneName)
    {
        sceneIndex = 3;
    }
    public void ChestScreen(string sceneName)
    {
        sceneIndex = 4;
    }
    public void ShopScene(string sceneName)
    {
        sceneIndex = 5;
    }
    public void UrpageWinOpen()
    {
        sceneIndex = 6;
    }
}