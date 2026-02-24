using UnityEngine;

/// <summary>
/// Prosty mened¿er muzyki, który mo¿na podpi¹æ bezpoœrednio do kamery.
/// Umo¿liwia podmianê muzyki w trakcie gry, start/stop i pauzê.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    // AudioSource z klipem, który chcemy zapêtliæ
    [SerializeField] private AudioSource audioSource;

    /// <summary>
    /// Prosta metoda do odtwarzania domyœlnego, przypisanego do AudioSource, klipu w pêtli.
    /// </summary>
    public void PlayMusic()
    {
        if (audioSource != null)
        {
            audioSource.loop = true;  // W³¹czamy zapêtlanie
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Brak przypisanego AudioSource!");
        }
    }

    /// <summary>
    /// Metoda pozwala podmieniæ aktualny klip audio i uruchomiæ go w pêtli.
    /// Wywo³aj j¹ np. z przycisku w Unity (public) lub z innego skryptu.
    /// </summary>
    /// <param name="clip">Nowy klip AudioClip do odtworzenia.</param>
    public void PlaySpecificMusic(AudioClip clip)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("Brak przypisanego AudioSource!");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("Przekazano pusty clip!");
            return;
        }

        audioSource.clip = clip;   // Podmiana klipu
        audioSource.loop = true;   // W³¹czamy zapêtlanie
        audioSource.Play();
    }

    /// <summary>
    /// Metoda zatrzymuje ca³kowicie odtwarzanie (zatrzymuje i wraca do momentu 0).
    /// </summary>
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        else
        {
            Debug.LogWarning("Brak przypisanego AudioSource!");
        }
    }

    /// <summary>
    /// Metoda pauzuje odtwarzanie muzyki (mo¿na je wznowiæ metod¹ PlayMusic albo PlaySpecificMusic).
    /// </summary>
    public void PauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
        else
        {
            Debug.LogWarning("Brak przypisanego AudioSource!");
        }
    }
}
