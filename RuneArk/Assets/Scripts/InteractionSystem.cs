using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
public class InteractionSystem : MonoBehaviour
{
    public Transform detectionPoint;
    private const float detectionRadius = 0.2f;
    public LayerMask detectionLayer;
    public GameObject detectedObject;
    public List<GameObject> pickedItems = new List<GameObject>();
    public Image examineItemIcon;
    public Text examineItemText;
    public GameObject examineItemWin;
    public GameObject prefabWin;
    public bool isEximing;
    void Update()
    {
        if (DetectObject())
        {
            if (InteractInput())
            {
                detectedObject.GetComponent<Item>().Interact();
            }
        }
    }

    bool InteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    bool DetectObject()
    {
        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);
        if(obj == null)
        {
            detectedObject = null;
            return false;
        }
        else
        {
            detectedObject = obj.gameObject;
            return true;
        }
    }

    public void ExamineItem(Item item)
    {
        if (isEximing)
        {
            examineItemWin.SetActive(false);
            prefabWin.SetActive(false);
            isEximing = false;
            item.OnDestroy();
        }
        else
        {
            examineItemIcon.sprite = item.GetComponent<SpriteRenderer>().sprite;
            examineItemText.text = item.name + "\n" + "<color=#A9A9A9>" + item.destcriptionText + "</color>";
            examineItemWin.SetActive(true);
            prefabWin.SetActive(true);
            isEximing = true;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);
    }
}