using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection instance;
    public CharacterScriptableObj characteData;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }
    }
    public static CharacterScriptableObj GetData()
    {
        return instance.characteData;
    }
    public void SelectCharacter(CharacterScriptableObj character)
    {
        characteData = character;
    }
    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
