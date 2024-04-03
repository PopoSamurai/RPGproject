using System.Collections;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public GameObject reaction;
    public bool firstlook = false;
    public bool follow = false;
    [Header("move")]
    float speed = 4f;
    float minDist = 1.5f;
    float minDist2 = 0f;
    public VisionScript vision;
    public Vector3 startPos;
    public bool off = false;
    public Rigidbody rb;
    public bool move = true;
    float knockbackPower = 20;
    GameObject player;
    float distance;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        startPos = this.transform.position;
    }
    private void Update()
    {
        if (firstlook == true && follow == false) //!
        {
            reaction.SetActive(true);
            Invoke("waitToEffect", 0.5f);
        }
        else if(follow == true) //follow
        {
            FollowPlayer();
        }
        else //no see player
        {
            ResetFollow();
        }
    }
    public void FollowPlayer()
    {
        distance = Vector3.Distance(transform.position, vision.playerRef.transform.position);

        if (distance > minDist && move == true)
        {
            transform.LookAt(vision.playerRef.transform);
            transform.position += transform.forward * speed * Time.deltaTime;
            off = false;
        }
    }
    public void ResetFollow()
    {
        reaction.SetActive(false);
        firstlook = false;
        follow = false;

        distance = Vector3.Distance(transform.position, startPos);

        if (distance > minDist2)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, startPos, speed * Time.deltaTime);
        }
        else
        {
            off = true;
            vision.radius = 7f;
            vision.angle = 360;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            Vector3 targetHeadingAway = (transform.position - player.transform.position).normalized;
            rb.AddForce(targetHeadingAway * knockbackPower, ForceMode.Impulse);
            StartCoroutine(attackCo());
        }
    }
    IEnumerator attackCo()
    {
        yield return new WaitForSeconds(0.3f);
        rb.isKinematic = true;
        yield return new WaitForSeconds(0.2f);
        rb.isKinematic = false;
    }
    void waitToEffect()
    {
        reaction.SetActive(false);
        firstlook = false;
        follow = true;
    }
}