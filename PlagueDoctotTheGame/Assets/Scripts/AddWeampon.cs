using UnityEngine;

public class AddWeampon : MonoBehaviour
{
    GameObject player;
    float roateSpeed = 20;
    public int numberW = 0;
    //
    public float speed;
    public int startingpoint;
    public Transform[] points;
    private int i;
    public GameObject gun;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.position = points[startingpoint].position;
    }

    void Update()
    {
        gun.transform.Rotate(new Vector3(0, roateSpeed, 2) * Time.deltaTime);

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
        if(other.CompareTag("Player"))
        {
            player.GetComponent<Movement>().weamponNumber = numberW;
            Destroy(transform.parent.gameObject);
        }
    }
}
