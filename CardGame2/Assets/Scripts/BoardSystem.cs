using UnityEngine;
public class BoardSystem : MonoBehaviour
{
    public static BoardSystem Instance;
    public LayerMask boardMask;

    void Awake()
    {
        Instance = this;
    }
    public GameObject GetPointUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, boardMask))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
}