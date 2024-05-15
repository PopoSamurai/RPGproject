using UnityEngine;
public class CameraScr : MonoBehaviour
{
    GameObject player;
    public Vector3 offset;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        Follow();
    }
    void Follow()
    {
        Vector3 targetPos = player.transform.position + offset;
        transform.position = targetPos;
    }
}
