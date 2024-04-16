using UnityEngine;
public class CursorAim : MonoBehaviour
{
    public Canvas parentCanvas;
    GameObject player;
    public GameObject point;
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Cursor.visible = false;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform, Input.mousePosition,
            parentCanvas.worldCamera,
            out pos);
    }

    public void Update()
    {
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
            out movePos);

        transform.position = parentCanvas.transform.TransformPoint(movePos);

        if (transform.position.x < point.transform.position.x)
        {
            player.GetComponent<Movement>().FlipX();
        }
        else if (transform.position.x > point.transform.position.x)
        {
            player.GetComponent<Movement>().FlipY();
        }
    }
}
