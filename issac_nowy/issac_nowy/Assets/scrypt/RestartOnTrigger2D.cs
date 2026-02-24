using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class RestartOnTrigger2D : MonoBehaviour
{
    [Header("Ustawienia")]
    public string playerTag = "Player";
    [Min(0f)] public float delay = 0f;   // opóŸnienie restartu (sekundy)

    bool _restarting;

    void Reset()
    {
        // Ten obiekt ma byæ TRIGGEREM
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_restarting) return;
        if (!other.CompareTag(playerTag)) return;

        _restarting = true;
        // na wszelki wypadek przywróæ czas
        Time.timeScale = 1f;

        if (delay > 0f) StartCoroutine(RestartAfter(delay));
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    System.Collections.IEnumerator RestartAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
