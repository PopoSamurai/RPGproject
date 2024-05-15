using UnityEngine;
public class bulletScr : MonoBehaviour
{
    Vector3 shootPoint; 
    public float bulletSpeed;
    public float maxDistance;
    GameObject point;
    void Start()
    {
        point = GameObject.FindGameObjectWithTag("point");
        shootPoint = point.transform.position;
    }
    void Update()
    {
        MoveProjectile();
    }
    public void MoveProjectile()
    {
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);

        if (Vector3.Distance(shootPoint, transform.position) > maxDistance)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}