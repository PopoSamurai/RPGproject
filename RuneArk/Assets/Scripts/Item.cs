using Rune;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


[RequireComponent(typeof(BoxCollider2D))]
public class Item : MonoBehaviour
{
    public enum InteractionType { NONE, PickUp, Examine}
    public InteractionType type;
    public string destcriptionText;

    public UnityEvent customEvent;
    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
        gameObject.layer = 8;
    }

    public void Interact()
    {
        switch(type)
        {
            case InteractionType.PickUp:
                Destroy(gameObject);
                //dzwiek zniszczenia
                break;
            case InteractionType.Examine:
                FindObjectOfType<InteractionSystem>().ExamineItem(this);
                break;
            default:
                Debug.Log("NULL");
                break;
        }

        customEvent.Invoke();
    }

    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}
