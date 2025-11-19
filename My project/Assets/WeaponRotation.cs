using UnityEngine;
public class WeaponController : MonoBehaviour
{
    public Transform socketR;
    public Transform socketL;
    public Transform socketUp;
    public Transform socketD;

    public SpriteRenderer weaponSR;
    public SpriteRenderer bodySR;

    public int orderFront = 2;
    public int orderBack = 0;

    public float angleOffsetDeg = 0f;
    public float positionLerpSpeed = 25f;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (!weaponSR) weaponSR = GetComponentInChildren<SpriteRenderer>(true);
    }

    void LateUpdate()
    {
        if (!socketR || !socketL || !socketUp || !socketD || !weaponSR) return;

        Vector3 origin = (bodySR ? bodySR.transform.position : transform.position);
        Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouse - origin;
        if (dir.sqrMagnitude < 1e-6f) return;

        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffsetDeg;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        bool left = dir.x < 0f;
        weaponSR.flipY = left;
        weaponSR.sortingOrder = (dir.y > 0f) ? orderBack : orderFront;

        Transform a = (dir.x >= 0f) ? socketR : socketL;
        Transform b = (dir.y >= 0f) ? socketUp : socketD;

        float wx = Mathf.Abs(dir.x);
        float wy = Mathf.Abs(dir.y);
        float sum = wx + wy; if (sum < 1e-6f) sum = 1f;
        wx /= sum; wy /= sum;

        Vector3 targetPos = a.position * wx + b.position * wy;
        float t = 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPos, t);
    }
}