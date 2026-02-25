using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TargetDetector : MonoBehaviour
{
    public static TargetDetector Instance;
    public BoardTarget CurrentTarget;

    void Awake()
    {
        Instance = this;
    }
    public BoardTarget Detect()
    {
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        foreach (var r in results)
        {
            var target = r.gameObject.GetComponentInParent<BoardTarget>();
            if (target != null)
            {
                CurrentTarget = target;
                return target;
            }
        }

        CurrentTarget = null;
        return null;
    }
}