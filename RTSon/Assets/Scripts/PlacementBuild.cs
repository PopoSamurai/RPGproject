using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
public enum buildSize
{
    church,
    house
}
public class PlacementBuild : MonoBehaviour
{
    public buildSize buildSize;
    public Vector3 place;
    public GameObject build;
    public bool placeNow;
    private Camera cam;
    public LayerMask ground;
    public GameObject groundMarker;
    GameObject selection;
    public bool isOverUI;
    Vector3 mark;
    private void Start()
    {
        cam = Camera.main;
        selection = GameObject.FindGameObjectWithTag("selector");
    }
    void Update()
    {
        switch(buildSize)
        {
            case buildSize.church:
                mark = new Vector3(1.2375f, 0.85f, 1.2375f);
                break;
            case buildSize.house:
                mark = new Vector3(1.2375f, 0.85f, 1.2375f);
                break;
        }
        isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (placeNow == true)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                groundMarker.SetActive(true);
                groundMarker.transform.localScale = mark;
                groundMarker.transform.position = new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z);

                if (Input.GetMouseButtonDown(0) && groundMarker.GetComponent<Mark>().isGood == true && !isOverUI)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                    {
                        place = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        Instantiate(build, place, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        placeNow = false;
                        DeselectAll();
                    }
                }
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            DeselectAll();
        }
    }
    public void PlaceBuild()
    {
        selection.GetComponent<UnitSelectionBox>().isPlacing = true;
        placeNow = true;
    }
    public void DeselectAll()
    {
        groundMarker.SetActive(false);
        placeNow = false;
        StartCoroutine(czekaj());
    }
    public IEnumerator czekaj()
    {
        yield return new WaitForSeconds(1f);
        selection.GetComponent<UnitSelectionBox>().isPlacing = false;
    }
}