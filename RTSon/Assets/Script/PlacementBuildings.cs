using UnityEngine;
using UnityEngine.EventSystems;
public class PlacementBuildings : MonoBehaviour
{
    public Vector3 place;
    public GameObject build;
    public bool placeNow;
    private Camera cam;
    public LayerMask ground;
    public GameObject groundMarker;
    GameObject selection;
    public bool isOverUI;
    private void Start()
    {
        cam = Camera.main;
        selection = GameObject.FindGameObjectWithTag("selector");
    }
    void Update()
    {
        isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (placeNow == true)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                groundMarker.SetActive(true);
                groundMarker.transform.localScale = build.transform.localScale;
                groundMarker.transform.position = new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z);

                if (Input.GetMouseButtonDown(0) && groundMarker.GetComponent<Mark>().isGood == true && !isOverUI)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                    {
                        selection.GetComponent<UnitSelectionBox>().isPlacing = false;
                        place = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        Instantiate(build, place, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        placeNow = false;
                        DeselectAll();
                    }
                }
            }
        }
        if(Input.GetKey(KeyCode.Escape))
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
        selection.GetComponent<UnitSelectionBox>().isPlacing = false;
        groundMarker.SetActive(false);
        placeNow = false;
    }
}