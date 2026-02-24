using System.Collections.Generic;
using UnityEngine;

public class TriggerShowUntilZero : MonoBehaviour
{
    [Header("Licznik (x) – startowa wartoœæ")]
    public int x = 3;

    [Header("Obiekty do w³¹czenia/wy³¹czenia")]
    public List<GameObject> targets = new List<GameObject>();

    [Header("Tag gracza (wejdzie w trigger)")]
    public string playerTag = "Player";

    [Header("Aktywacje przy niszczeniu")]
    [Tooltip("przeciwnik fala 1")]
    public List<GameObject> wrog_1 = new List<GameObject>();
    [Tooltip("przeciwnik fala 2")]
    public List<GameObject> wrog_2 = new List<GameObject>();
    [Tooltip("przeciwnik fala 3")]
    public List<GameObject> wrog_3 = new List<GameObject>();
    public AudioClip walka, po_walce;
    private int raz = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {

            if (x > 0) { SetAll(true);
                if (raz == 0) 
                {
                    GameObject okna = GameObject.FindGameObjectWithTag("Player");
                    MusicManager menagerOkienek = okna.GetComponent<MusicManager>();
                    menagerOkienek.PlaySpecificMusic(walka);
                    raz = 1; 
                }
            }
            SetAll_wrogowie(true, 3);
            
        }
    }

    private void Update()
    {
        // NEW: przed sprawdzaniem liczników usuñ puste (zniszczone) wpisy z list:
        CleanNulls(wrog_3);   // usuñ null-e z fali 3
        CleanNulls(wrog_2);   // usuñ null-e z fali 2
        CleanNulls(wrog_1);   // usuñ null-e z fali 1  (to by³o Twoje wymaganie)

        if (x == 3)
        {
            if (wrog_3 == null || wrog_3.Count == 0)
            {
                x = 2;
                SetAll_wrogowie(true, 2);
            }
        }
        if (x == 2)
        {
            if (wrog_2 == null || wrog_2.Count == 0)
            {
                x = 1;
                SetAll_wrogowie(true, 1);
            }
        }
        if (x == 1)
        {
            if (wrog_1 == null || wrog_1.Count == 0)
            {
                x = 0;
                GameObject okna = GameObject.FindGameObjectWithTag("Player");
                MusicManager menagerOkienek = okna.GetComponent<MusicManager>();
                menagerOkienek.PlaySpecificMusic(po_walce);// gdy x osi¹gnie 0 -> wy³¹cz wszystko
            }
        }
        if (x == 0)
        {

            SetAll(false);
            
        }
    }

    // NEW: jedna funkcja do czyszczenia listy z pustych (zniszczonych) elementów
    private static bool CleanNulls(List<GameObject> list)
    {
        if (list == null) return false;
        // RemoveAll zwraca liczbê usuniêtych elementów; w Unity e == null wykrywa te¿ Destroy()
        return list.RemoveAll(e => e == null) > 0;
    }

    private void SetAll(bool state)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                targets[i].SetActive(state);
            }
        }
    }

    private void SetAll_wrogowie(bool state, int x)
    {
        if (x == 3)
        {
            for (int i = 0; i < wrog_3.Count; i++)
            {
                if (wrog_3[i] != null)
                {
                    wrog_3[i].SetActive(state);
                }
                else {  }
            }
        }
        if (x == 2)
        {
            for (int i = 0; i < wrog_2.Count; i++)
            {
                if (wrog_2[i] != null)
                {
                    wrog_2[i].SetActive(state);
                }
                else { }
            }
        }
        if (x == 1)
        {
            for (int i = 0; i < wrog_1.Count; i++)
            {
                if (wrog_1[i] != null)
                {
                    wrog_1[i].SetActive(state);
                }
            }
        }
    }
}
