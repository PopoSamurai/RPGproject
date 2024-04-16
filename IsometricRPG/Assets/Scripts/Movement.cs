using UnityEngine;
public class Movement : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 5f;
    public Vector3 direction;
    public float speedMouse;
    public bool move;
    public bool attack = false;
    public Animator anim;
    public SpriteRenderer playerSprite;
    public GameObject arms;
    public bool attackOn = false;
    GameObject aim;
    private void Start()
    {
        aim = GameObject.FindGameObjectWithTag("Aim");
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        direction = direction.normalized;
        transform.Translate(direction * speed * Time.deltaTime);

        if(attackOn == true)
        {
            anim.SetBool("AttackMode", true);
        }
        else
            anim.SetBool("AttackMode", false);

        if (direction != Vector3.zero)
        {
            anim.SetBool("walk", true);
        }
        else
            anim.SetBool("walk", false);

        //if (Input.GetMouseButtonDown(0) && attack == false)
        //{
        //    Debug.Log("Attack");
        //}
    }

    public void FlipX()
    {
        playerSprite.flipX = true;
        arms.transform.localScale = new Vector3(-1, 1, 1);
    }
    public void FlipY()
    {
        playerSprite.flipX = false;
        arms.transform.localScale = new Vector3(1, 1, 1);
    }
}