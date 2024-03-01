using UnityEngine;
using UnityEngine.UI;
public class Aim : MonoBehaviour
{
    public Sprite nevIco;
    public Sprite oldIco;
    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.y = mousePosition.y - 0.2f;
        mousePosition.x = mousePosition.x + 0.2f;
        mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        transform.position = mousePosition;
        if(Input.GetMouseButtonDown(0))
        {
            GetComponent<Image>().sprite = nevIco;
        }
        if (Input.GetMouseButtonUp(0))
        {
            GetComponent<Image>().sprite = oldIco;
        }
    }
}