using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
public class BuildScr : MonoBehaviour
{
    MeshRenderer mesh;
    public bool start = false;
    public GameObject obj1;
    public GameObject obj2;
    public int stage = 1;
    public float buildTime;
    float time;
    //
    public Sprite icon;
    public string buildName;
    public Sprite skill1Icon;
    public NavMeshObstacle block;
    void Start()
    {
        block = GetComponent<NavMeshObstacle>();
        time = buildTime;
        mesh = GetComponent<MeshRenderer>();
        obj1.SetActive(true);
        mesh.enabled = false;
        block.enabled = false;
    }
    void Update()
    {
        if (GameManager.instance.active == false)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        if (start == true && stage == 1)
        {
            block.enabled = false;
            buildTime -= Time.deltaTime;
            obj1.SetActive(true);
            mesh.enabled = false;

            if (buildTime <= 0.0f)
            {
                buildTime = time;
                stage = 2;
            }
        }
        else if (start == true && stage == 2)
        {
            block.enabled = false;
            buildTime -= Time.deltaTime;
            obj1.SetActive(false);
            obj2.SetActive(true);
            mesh.enabled = false;

            if (buildTime <= 0.0f)
            {
                buildTime = time;
                stage = 3;
            }
        }
        else if (start == true && stage == 3)
        {
            block.enabled = true;
            buildTime = 0f;
            start = false;
            obj1.SetActive(false);
            obj2.SetActive(false);
            mesh.enabled = true;
        }
    }
    public void Skill1()
    {
        Debug.Log("Spawn unit");
    }
}