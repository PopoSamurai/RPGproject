using UnityEngine;
public class Mark : MonoBehaviour
{
    Renderer render;
    public Material green, red;
    public bool isGood;
    public bool goWork = false;
    private void Start()
    {
        render = GetComponent<Renderer>();
        render.material = green;
        isGood = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" || other.transform.tag == "Build")
        {
            isGood = false;
            render.material = red;
        }
        if (other.transform.tag == "Build")
        {
            goWork = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player" || other.transform.tag == "Build")
        {
            isGood = false;
            render.material = red;
        }
        if (other.transform.tag == "Build")
        {
            render.material = green;
            goWork = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        isGood = true;
        render.material = green;

        if (other.transform.tag == "Build")
        {
            render.material = green;
            goWork = false;
        }
    }
}