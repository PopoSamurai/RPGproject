using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    public Tilemap map;
    MouseInput mouseInput;
    private Vector3 destination;
    [SerializeField] private float moveSpeed;
    private void Awake()
    {
        mouseInput = new MouseInput();
    }
    private void OnEnable()
    {
        mouseInput.Enable();
    }
    private void OnDisable()
    {
        mouseInput.Disable();
    }
    private void Start()
    {
        destination = transform.position;
        mouseInput.Mouse.MOuseClick.performed += _ => MouseClick();
    }

    private void MouseClick()
    {
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //sprawdza czy klikasz w mapê
        Vector3Int gridPos = map.WorldToCell(mousePosition);
        if(map.HasTile(gridPos))
        {
            destination = mousePosition;
        }
    }
    private void Update()
    {
        if(Vector3.Distance(transform.position, destination) > 0.1f)
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }
}
