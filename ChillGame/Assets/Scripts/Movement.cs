using UnityEngine;
using UnityEngine.EventSystems;
public enum Actions { Empty, Fishing, Seeds, Water, Crop, Fight}
public class Movement : MonoBehaviour
{
    public Actions action;
    Rigidbody2D rb;
    Animator anim;
    public float speed;
    Vector3 change;
    public static bool move = true;
    public GameObject ActionUI;
    public bool actionButton = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    public void ActionFix()
    {
        move = true;
        actionButton = false;
    }
    public void ActionOn()
    {
        move = false;
    }

    void Update()
    {
        if (actionButton)
        {
            switch (action)
            {
                case Actions.Empty:
                    anim.SetTrigger("grab");
                    break;
                case Actions.Fishing:
                    anim.SetTrigger("fish");
                    break;
                case Actions.Seeds:
                    anim.SetTrigger("seed");
                    break;
                case Actions.Water:
                    anim.SetTrigger("water");
                    break;
                case Actions.Crop:
                    anim.SetTrigger("crop");
                    break;
                case Actions.Fight:
                    anim.SetTrigger("fight");
                    break;
            }
        }
        if (move)
        {
            if (Input.GetKey(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
            {
                actionButton = true;
                UseTool();
            }
            change = Vector3.zero;
            change.x = Input.GetAxisRaw("Horizontal");
            change.y = Input.GetAxisRaw("Vertical");
            updateAnimationAndMove();

            if (Input.GetKeyDown(KeyCode.Mouse2) && !actionButton)
            {
                ActionUI.SetActive(true);
            }

            //actions
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Empty();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Fishing();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Seeds();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Water();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Crop();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Attack();
            }
        }
    }
    private void UseTool()
    {
        //Vector2 position = rb.position + this.transform.
    }
    void updateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            anim.SetFloat("moveX", change.x);
            anim.SetFloat("moveY", change.y);
            anim.SetBool("move", true);
        }
        else
        {
            anim.SetBool("move", false);
        }
    }
    public void MoveCharacter()
    {
        rb.MovePosition(transform.position + change.normalized * speed * Time.fixedDeltaTime);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("NPC"))
        {
            other.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("NPC"))
        {
            other.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            other.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    //Akcje
    public void Empty()
    {
        action = Actions.Empty;
        Debug.Log("Nic");
        ActionUI.SetActive(false);
    }
    public void Attack()
    {
        action = Actions.Fight;
        Debug.Log("Walka");
        ActionUI.SetActive(false);
    }
    public void Seeds()
    {
        action = Actions.Seeds;
        Debug.Log("Nasiona");
        ActionUI.SetActive(false);
    }
    public void Crop()
    {
        action = Actions.Crop;
        Debug.Log("Motyka");
        ActionUI.SetActive(false);
    }
    public void Fishing()
    {
        action = Actions.Fishing;
        Debug.Log("�owienie");
        ActionUI.SetActive(false);
    }
    public void Water()
    {
        action = Actions.Water;
        Debug.Log("Konewka");
        ActionUI.SetActive(false);
    }
}
