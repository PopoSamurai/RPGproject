using UnityEngine;
public class Mark : MonoBehaviour
{
    Renderer render;
    public Material green, red;
    public bool isGood = false;
    private void Start()
    {
        render = GetComponent<Renderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" || other.transform.tag == "Finish")
        {
            isGood = false;
            render.material = red;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player" || other.transform.tag == "Finish")
        {
            isGood = false;
            render.material = red;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isGood = true;
        render.material = green;
    }
}