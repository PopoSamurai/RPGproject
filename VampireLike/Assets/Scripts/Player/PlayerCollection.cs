using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.TryGetComponent(out Collect collect))
        {
            collect.Collected();
        }
    }
}