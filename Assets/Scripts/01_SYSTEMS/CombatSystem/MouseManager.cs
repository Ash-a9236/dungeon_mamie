using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Transform reticle;
    // Start is called before the first frame update

    public static RaycastHit GetRaycastHit()
    {
        RaycastHit hit;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out hit);

        return hit;
    }

    public static Vector3 GetScreenSide()
    {
        RaycastHit hit = GetRaycastHit();

        Vector3 point = Camera.main.WorldToViewportPoint(hit.point);

        return point;
    }
}
