using System.Runtime.CompilerServices;
using UnityEngine;

public class camera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float camera_speed;
    public float camera_scroll_speed;
    public float first_camera_margin;

    public float camera_max_scroll_limit;
    public float camera_min_scroll_limit;

    public float camera_object_max_height;
    public float camera_object_min_height;

    public float camera_rotation_amount;

    public LayerMask camera_layers_to_hit;

    private float camera_GD_support = 1f;
    private float camera_boost_speed;
    private float camera_default_speed;

    private float raycast_length;
    private float raycast_length_second;

    public Transform camera_object;

    private float map_x_border_limit = 1000f;
    private float map_z_border_limit = 1000f;

    void Start()
    {
        camera_boost_speed = 2 * camera_speed;
        camera_default_speed = camera_speed;
        check_camera_distance();
    }

    void Update()
    {
        Ground_Dependence();
        Vector3 position = transform.position;
        Vector3 camera_object_position = camera_object.transform.localPosition;

        if (Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y >= Screen.height - first_camera_margin)
        {
            position += new Vector3(camera_object.forward.x, 0, camera_object.forward.z) * camera_speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y <= first_camera_margin)
        {
            position += new Vector3(-camera_object.forward.x, 0, -camera_object.forward.z) * camera_speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x <= first_camera_margin)
        {
            position += new Vector3(-camera_object.right.x, 0, -camera_object.right.z) * camera_speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x >= Screen.width - first_camera_margin)
        {
            position += new Vector3(camera_object.right.x, 0, camera_object.right.z) * camera_speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Camera_Rotation_Left();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Camera_Rotation_Right();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            camera_speed = camera_boost_speed;
        }
        else
        {
            camera_speed = camera_default_speed;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if ((scroll < 0 && camera_object_position.y < camera_max_scroll_limit) || (scroll > 0 && camera_object_position.y > camera_min_scroll_limit))
        {
            camera_object_position.y -= scroll * camera_scroll_speed * 100 * Time.deltaTime;
            camera_object_position.z += scroll * camera_scroll_speed * 100 * Time.deltaTime;
            camera_object_position.x += scroll * camera_scroll_speed * 100 * Time.deltaTime;
            
            camera_object_position.y = Mathf.Clamp(camera_object_position.y, camera_min_scroll_limit, camera_max_scroll_limit);
            camera_object_position.x = Mathf.Clamp(camera_object_position.x, -camera_max_scroll_limit, -camera_min_scroll_limit);
            camera_object_position.z = Mathf.Clamp(camera_object_position.z, -camera_max_scroll_limit, -camera_min_scroll_limit);

            camera_object.transform.localPosition = camera_object_position;
            check_camera_distance();
        }
        
        position.y = Mathf.Clamp(position.y, camera_object_min_height, camera_object_max_height);
        position.x = Mathf.Clamp(position.x, 0, map_x_border_limit);
        position.z = Mathf.Clamp(position.z, 0, map_z_border_limit);

        transform.position = position;
    }


    void Ground_Dependence()
    {
        Vector3 position = transform.position;

        //Raycast Checking ground
        bool ground_raycast_forward = Physics.Raycast(camera_object.transform.position, camera_object.transform.forward, raycast_length, camera_layers_to_hit);
        bool ground_raycast_backward = Physics.Raycast(camera_object.transform.position, -camera_object.transform.up, raycast_length, camera_layers_to_hit);

        //Raycast Checking ground touching
        bool checking_raycast_forward = Physics.Raycast(camera_object.transform.position, camera_object.transform.forward, raycast_length_second, camera_layers_to_hit);
        bool checking_racyast_backward = Physics.Raycast(camera_object.transform.position, -camera_object.transform.up, raycast_length_second, camera_layers_to_hit);
    
        if (ground_raycast_forward || ground_raycast_backward)
        {
            position.y += camera_GD_support * Time.deltaTime * 100f;
        }

        if (!checking_raycast_forward && !checking_racyast_backward)
        {
            position.y -= camera_GD_support * Time.deltaTime * 100f;
        }


        

        transform.position = position;

    }

    void check_camera_distance()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera_object.transform.position, camera_object.transform.forward, out hit, Mathf.Infinity))
        {
            raycast_length = Vector3.Distance(camera_object.transform.position, hit.point)-1;
            raycast_length_second = raycast_length + 1;
        }
    }
    void Camera_Rotation_Right()
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y += 45.0f;
        transform.eulerAngles = currentRotation;
    }

    void Camera_Rotation_Left()
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y -= 45.0f;
        transform.eulerAngles = currentRotation;
    }

   
}


