using UnityEngine;

public class PickUp : MonoBehaviour
{
    public bool interact = false;
    private Vector3 startPos;
    //
    public Dice dice;
    public static float power = 0f;
    public float maxPower = 50;
    public static bool thorwOn = false;
    Rigidbody rb;
    //
    public static bool buttonOn = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (interact)
        {
            if (Input.GetMouseButtonUp(0))
            {
                rb.useGravity = true;
                Dicethrow();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && !dice.move)
            {
                goThrow();
            }
        }
        if (interact && !buttonOn)
        {
            moveDice();
        }
        if (power >= maxPower)
        {
            power = maxPower;
        }
        if (power <= maxPower / 3 && thorwOn)
        {
            dice.BackToStart();
        }
    }

    void moveDice()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        transform.position = Vector3.Lerp(transform.position, mousePos, Time.deltaTime * 10f);
        //
        float MOuseSpeed = (Input.mousePosition - startPos).magnitude / Time.deltaTime;
        power = Mathf.Lerp(power, MOuseSpeed, Time.deltaTime * 0.01f);
        startPos = Input.mousePosition;
    }

    public void Dicethrow()
    {
        rb.AddForce(transform.forward * power, ForceMode.Impulse);
        thorwOn = true;
        interact = false;
        dice.move = true;
    }

    public void Autothrow()
    {
        if (!dice.move)
        {
            rb.useGravity = true;
            buttonOn = true;
            power = 30;
            rb.AddForce(transform.up * 30, ForceMode.Impulse);
            rb.AddForce(transform.forward * power, ForceMode.Impulse);
            thorwOn = true;
            interact = false;
            dice.move = true;
        }
    }

    void goThrow()
    {
        rb.useGravity = false;
        interact = true;
        startPos = Input.mousePosition;
    }
}
