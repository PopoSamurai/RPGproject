using UnityEngine;
public class CameraMOve : MonoBehaviour
{
    GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        Follow();
    }
    void Follow()
    {
        Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.position = targetPos;
    }    
}