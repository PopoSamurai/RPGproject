using UnityEngine;
public class Platform : MonoBehaviour
{
    public float speed;
    public int startingpoint;
    public Transform[] points;
    private int i;
    void Update()
    {
        if (Vector3.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}