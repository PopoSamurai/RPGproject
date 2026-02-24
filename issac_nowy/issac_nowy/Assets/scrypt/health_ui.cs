using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class health_ui : MonoBehaviour
{
    public RectTransform hp;
    public Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject druzyna = GameObject.FindGameObjectWithTag("HP_gracza");
        Health gracze = druzyna.GetComponent<Health>();

        slider.maxValue = gracze.maxHealth;
        slider.value = gracze.currentHealth;
    }
    public static void Restart()
    {
        Time.timeScale = 1f; // na wszelki wypadek
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
}
