using UnityEngine;
public class Movement : MonoBehaviour
{
    public enum WeamponType
    {
        pistol,
        shotgun
    }
    public WeamponType weampon;
    Rigidbody rb;
    public float speed = 5f;
    public bool move;
    public int posPlayer = 2;
    [Header("Interact")]
    public GameObject interact;
    public GameObject collectInteract;
    public GameObject collectTxt;
    public GameObject LoadBar;
    public bool interactClick;
    public bool dialogOn = false;
    //
    public bool interactOn = false;
    GameObject gameM;
    public bool attack = false;
    public Vector3 direction;
    public Animator anim;
    public Animator pistolAnim;
    public GameObject bulletPrefab;
    public Transform shootPos;
    public GameObject[] weamponList;
    public int weamponNumber = 0;
    float pistolShoot = 0.41f;
    float shotgunShoot = 0.61f;
    public string animationName;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameM = GameObject.FindGameObjectWithTag("GameManager");
    }
    void Update()
    {
        switch(weamponNumber)
        {
            case 0:
                for (int i = 0; i < weamponList.Length; i++)
                {
                    weamponList[i].SetActive(false);
                }
                weampon = WeamponType.pistol;
                weamponList[0].SetActive(true);
                animationName = "shoot";
                break;
            case 1:
                for (int i = 0; i < weamponList.Length; i++)
                {
                    weamponList[i].SetActive(false);
                }
                weampon = WeamponType.shotgun;
                weamponList[1].SetActive(true);
                animationName = "shootShotgun";
                break;
        }
        if (move == true)
        {
            direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            direction = direction.normalized;
            //walk anim
            if(direction != Vector3.zero)
            {
                anim.SetBool("move", true);
                if(attack == true)
                {
                    anim.SetBool(animationName, true);
                    pistolAnim.SetBool("shootWalk", true);
                }
            }
            else
            {
                anim.SetBool("move", false);
                if (attack == true)
                {
                    pistolAnim.SetBool("shoot", true);
                    anim.SetBool(animationName, true);
                }
            }

            transform.Translate(direction * speed * Time.deltaTime);

            if (Input.GetMouseButton(0) && attack == false)
            {
                Instantiate(bulletPrefab, shootPos.position, shootPos.rotation);
                attack = true;
                if(weamponNumber == 0)
                    Invoke("AttackCo", pistolShoot);
                if (weamponNumber == 1)
                    Invoke("AttackCo", shotgunShoot);
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.F) && interact.activeSelf == true)
        {
            interactOn = true;
            interact.SetActive(false);
            move = false;
        }
        else
        {
            interactOn = false;
        }

        if (collectInteract.activeSelf == true || dialogOn == true)
        {
            move = false;
        }
        else
        {
            move = true;
        }
    }
    void AttackCo()
    {
        pistolAnim.SetBool("shootWalk", false);
        pistolAnim.SetBool("shoot", false);
        anim.SetBool("shoot", false);
        anim.SetBool("shootShotgun", false);
        attack = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "NPC" || collision.transform.tag == "Enemy")
            collision.rigidbody.isKinematic = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "NPC" || collision.transform.tag == "Enemy")
            collision.rigidbody.isKinematic = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "NPC")
        {
            interact.SetActive(true);
        }
        if (other.tag == "interactive")
        {
            interact.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        interact.SetActive(false);
    }
    public void MoveOn()
    {
        move = true;
    }
    public void MoveOff()
    {
        move = false;
    }
}