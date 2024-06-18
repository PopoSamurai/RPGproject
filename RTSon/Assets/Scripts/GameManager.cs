using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; set; }
    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitSelected = new List<GameObject>();
    public List<GameObject> allBuildList = new List<GameObject>();
    public List<GameObject> buildSelected = new List<GameObject>();
    public LayerMask clickable;
    public LayerMask build;
    public LayerMask ground;
    public GameObject groundMarker;
    private Camera cam;
    public GameObject infoWin;
    public bool active;
    bool isOverUI;
    GameObject checkUnit;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        isOverUI = EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButtonDown(0) && !isOverUI && infoWin.activeSelf)
        {
            infoWin.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0) && !isOverUI)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            //unit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MultiSelct(hit.collider.gameObject);
                }
                else
                {
                    infoWin.GetComponent<InfoWin>().icon.sprite = hit.collider.GetComponent<Unit>().icon;
                    infoWin.GetComponent<InfoWin>().buildName.text = hit.collider.GetComponent<Unit>().nameV;
                    infoWin.GetComponent<InfoWin>().skill1Icon.sprite = hit.collider.GetComponent<Unit>().skill1Icon;
                    SelectByClicking(hit.collider.gameObject);
                    infoWin.GetComponent<InfoWin>().skills[0].onClick.RemoveAllListeners();
                    infoWin.GetComponent<InfoWin>().skills[0].onClick.AddListener(() => checkUnit.GetComponent<Unit>().Skill1());
                }
            }
            //build
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, build))
            {
                if (hit.collider.GetComponent<BuildScr>().stage == 3)
                {
                    infoWin.SetActive(true);
                    infoWin.GetComponent<InfoWin>().icon.sprite = hit.collider.GetComponent<BuildScr>().icon;
                    infoWin.GetComponent<InfoWin>().buildName.text = hit.collider.GetComponent<BuildScr>().buildName;
                    SelectByClickingBuild(hit.collider.gameObject);
                    infoWin.GetComponent<InfoWin>().skills[0].onClick.RemoveAllListeners();
                    infoWin.GetComponent<InfoWin>().skills[0].onClick.AddListener(() => hit.collider.GetComponent<BuildScr>().Skill1());
                    infoWin.GetComponent<InfoWin>().skill1Icon.sprite = hit.collider.GetComponent<BuildScr>().skill1Icon;
                }
                else
                    DeselectAll();
            }
            else if (Input.GetKey(KeyCode.LeftShift) == false)
            {
                DeselectAll();
            }
        }
        if (Input.GetMouseButtonDown(1) && unitSelected.Count > 0 && !isOverUI)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                groundMarker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                groundMarker.transform.position = new Vector3(hit.point.x, hit.point.y + 0.2f, hit.point.z);

                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
            }
        }
    }
    private void MultiSelct(GameObject unit)
    {
        if (unitSelected.Contains(unit) == false)
        {
            unitSelected.Add(unit);
            SelectUnit(unit, true);
            active = false;
        }
        else
        {
            SelectUnit(unit, false);
            unitSelected.Remove(unit);
            active = false;
        }
        infoWin.SetActive(false);
    }

    public void DeselectAll()
    {
        foreach (var unit in unitSelected)
        {
            SelectUnit(unit, false);
        }
        foreach (var build in buildSelected)
        {
            SelectBuild(build, false);
        }
        groundMarker.SetActive(false);
        unitSelected.Clear();
        buildSelected.Clear();
        active = false;
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();
        unitSelected.Add(unit);
        SelectUnit(unit, true);
        infoWin.SetActive(true);
        active = true;
        checkUnit = unit;
    }
    private void SelectByClickingBuild(GameObject build)
    {
        DeselectAll();
        buildSelected.Add(build);
        SelectBuild(build, true);
        infoWin.SetActive(true);
        active = true;
    }
    private void SelectUnit(GameObject unit, bool isSelected)
    {
        TriggerSelectionIndicator(unit, isSelected);
        EnableUnitMOvement(unit, isSelected);
    }
    private void SelectBuild(GameObject build, bool isSelected)
    {
        TriggerSelectionIndicator(build, isSelected);
    }
    private void EnableUnitMOvement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<Unit>().selectOn = shouldMove;
    }
    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        unit.transform.GetChild(0).gameObject.SetActive(isVisible);
    }
    internal void DragSelect(GameObject unit)
    {
        if (unitSelected.Contains(unit) == false)
        {
            unitSelected.Add(unit);
            SelectUnit(unit, true);
        }
    }
}