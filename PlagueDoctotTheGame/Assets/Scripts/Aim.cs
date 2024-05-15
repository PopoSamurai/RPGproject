using UnityEngine;
public class Aim : MonoBehaviour
{
    Vector3 pos;
    public float posX, posY;
    private void Start()
    {
        Cursor.visible = false;
    }
    private void Update()
    {
        pos.x = Input.mousePosition.x - posX;
        pos.y = Input.mousePosition.y + posY;
        this.transform.position = pos;
    }
}