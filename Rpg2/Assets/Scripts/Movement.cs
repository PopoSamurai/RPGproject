using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
public class Movement : MonoBehaviour
{
    private Vector2 target;
    public NavMeshAgent agent;
    public GameObject clickMark;
    [SerializeField] private Transform prefabParent;
    public LineRenderer myLineRender;
    private GameManager gameM;
    //
    public GameObject imagCollect;
    public GameObject collectInteract;
    public Text numberTxt;
    int number;
    public bool startNum = false;
    float counDown;
    public bool interact = false;
    public Character main;
    void Start()
    {
        target = this.transform.position;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        gameM = FindObjectOfType<GameManager>();

        myLineRender.startWidth = 0.15f;
        myLineRender.endWidth = 0.15f;
        myLineRender.positionCount = 0;

        counDown = 4f;
    }
    public void ResetLine()
    {
        myLineRender.startWidth = 0.15f;
        myLineRender.endWidth = 0.15f;
        myLineRender.positionCount = 0;
    }
    private void Update()
    {
        if (interact == false)
        {
            SetTargetPosition();
            SetAgentPosition();
        }

        if (counDown <= 1.0f)
        {
            startNum = false;
            endItem();
            Debug.Log("add item");
        }
        if (collectInteract.activeSelf == true)
        {
            if (startNum == true)
            {
                counDown -= Time.deltaTime;
                number = Mathf.FloorToInt(counDown);
                collectInteract.SetActive(true);
                imagCollect.GetComponent<Image>().fillAmount -= 1.0f / counDown * Time.deltaTime;
                numberTxt.text = number.ToString();
            }
        }
    }
    public void SetTargetPosition()
    {
        if(Input.GetMouseButtonDown(0) && gameM.hit == null)
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickMark.transform.SetParent(prefabParent);
            clickMark.SetActive(true);
            agent.SetDestination(target);
            clickMark.transform.position = agent.destination;
            gameM.DeSelect();
        }
        if (Vector2.Distance(agent.destination, transform.position) <= agent.stoppingDistance)
        {
            clickMark.SetActive(false);
        }
        else if(agent.hasPath)
        {
            DrawPath();
        }
    }
    public void endItem()
    {
        interact = false;
        counDown = 4f;
        collectInteract.SetActive(false);
        imagCollect.GetComponent<Image>().fillAmount = 1;
        gameM.hit.GetComponent<Interact>().active = true;
    }
    public void MovePlayer()
    {
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        clickMark.transform.SetParent(prefabParent);
        clickMark.SetActive(true);
        agent.SetDestination(target);
        clickMark.transform.position = agent.destination;
    }
    void SetAgentPosition()
    {
        agent.SetDestination(new Vector2(target.x, target.y));
    }
    void DrawPath()
    {
        myLineRender.positionCount = agent.path.corners.Length;
        myLineRender.SetPosition(0, transform.position);
        if(agent.path.corners.Length < 2)
        {
            return;
        }    
        for(int i = 1; i < agent.path.corners.Length; i++)
        {
            Vector2 pointPosition = new Vector2(agent.path.corners[i].x, agent.path.corners[i].y);
            myLineRender.SetPosition(i, pointPosition);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && gameM.hit != null)
        {
            interact = true;
            gameM.battlePanel.SetActive(true);
            gameM.battlePanel.GetComponent<BattleSystem>().Start();
            transform.position = other.transform.position;
        }
        if (other.gameObject.CompareTag("Cave") && gameM.hit != null)
        {
            interact = true;
            startNum = true;
            //transform.position = other.transform.position;
            collectInteract.SetActive(true);
            ResetLine();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            ResetLine();
            interact = false;
        }
    }
}