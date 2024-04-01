using System.Collections;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public GameObject reaction;
    public bool firstlook = false;
    public bool follow = false;
    [Header("move")]
    float speed = 5f;
    float minDist = 1.5f;
    float minDist2 = 0f;
    public VisionScript vision;
    public Vector3 startPos;
    public bool off = false;
    public Rigidbody rb;
    public bool move = true;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = this.transform.position;
    }
    private void Update()
    {
        if (firstlook == true && follow == false) //!
        {
            StartCoroutine(waitToEffect());
        }
        else if(follow == true) //follow
        {
            FollowPlayer();
        }
        else //no see
        {
            ResetFollow();
        }
    }
    public void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, vision.playerRef.transform.position);

        if (distance > minDist && move == true)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, vision.playerRef.transform.position, speed * Time.deltaTime);
            off = false;
        }
    }
    public void ResetFollow()
    {
        if(follow == false)
        {
            reaction.SetActive(false);
            firstlook = false;
            follow = false;

            float distance = Vector3.Distance(transform.position, startPos);

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
    }
    IEnumerator waitToEffect()
    {
        reaction.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        reaction.SetActive(false);
        firstlook = false;
        follow = true;
    }
}