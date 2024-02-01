using UnityEngine;

public class Dice : MonoBehaviour
{
    Rigidbody rb;
    public static Vector3 direction;
    public static bool start = true;
    //
    Vector3 point;
    public bool move = false;
    public static bool end = false;
    public static bool stop = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        point = transform.position;
    }

    void Update()
    {
        if (move && rb.IsSleeping() && Mathf.Approximately(rb.angularVelocity.sqrMagnitude, 0.0f)
            && Mathf.Approximately(rb.velocity.sqrMagnitude, 0.0f) && start)
        {
            GameM.addBlock = false;
            end = true;
            Invoke("BackToStart", 0.5f);
        }
    }
    public void BackToStart()
    {
        PickUp.buttonOn = false;
        PickUp.power = 0f;
        move = false;
        end = false;
        transform.position = point;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        PickUp.thorwOn = false;
        start = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("block"))
            BackToStart();
    }
}