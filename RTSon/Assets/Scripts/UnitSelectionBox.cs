using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class UnitSelectionBox : MonoBehaviour
{
    Camera cam;
    [SerializeField] RectTransform boxVisual;
    Rect selectionBox;
    Vector2 startPosition;
    Vector2 endPosition;
    public bool isPlacing;
    bool isOverUI;
    void Start()
    {
        cam = Camera.main;
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        DrawVisual();
    }
    void Update()
    {
        if (isPlacing == false && !isOverUI)
        {
            //click
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = Input.mousePosition;
                selectionBox = new Rect();
            }
            //drag
            if (Input.GetMouseButton(0))
            {
                if (boxVisual.rect.width > 0 || boxVisual.rect.height > 0)
                {
                    GameManager.instance.DeselectAll();
                    SelectUnits();
                }

                endPosition = Input.mousePosition;
                DrawVisual();
                DrawSelection();
            }
            //relasing
            if (Input.GetMouseButtonUp(0))
            {
                SelectUnits();
                startPosition = Vector2.zero;
                endPosition = Vector2.zero;
                DrawVisual();
            }
        }
    }
    private void DrawVisual()
    {
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;
        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        boxVisual.position = boxCenter;
        Vector2 boxSize = new Vector2(Math.Abs(boxStart.x - boxEnd.x), Math.Abs(boxStart.y - boxEnd.y));
        boxVisual.sizeDelta = boxSize;
    }
    private void DrawSelection()
    {
        if(Input.mousePosition.x < startPosition.x)
        {
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Input.mousePosition.x;
        }
        if(Input.mousePosition.y < startPosition.y)
        {
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }
    private void SelectUnits()
    {
        foreach(var unit in GameManager.instance.allUnitsList)
        {
            if(selectionBox.Contains(cam.WorldToScreenPoint(unit.transform.position)))
            {
                GameManager.instance.DragSelect(unit);
            }
        }
    }
}
