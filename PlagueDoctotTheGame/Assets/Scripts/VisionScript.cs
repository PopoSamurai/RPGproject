using System.Collections;
using UnityEngine;

public class VisionScript : MonoBehaviour
{
    public float radius;
    [Range(0,360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public Enemy enemyScript;
    public bool seePlayer;
    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }
    void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    enemyScript.firstlook = true;
                    seePlayer = true;
                }
                else
                {
                    seePlayer = false;
                    enemyScript.ResetFollow();
                }
            }
            else
            {
                seePlayer = false;
                enemyScript.ResetFollow();
            }
        }
        else if (seePlayer)
        {
            seePlayer = false;
            enemyScript.ResetFollow();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "StopMap")
        {
            Debug.Log("connector");
            radius = 0f;
            angle = 0;
            enemyScript.follow = false;
            enemyScript.ResetFollow();
        }
    }
}