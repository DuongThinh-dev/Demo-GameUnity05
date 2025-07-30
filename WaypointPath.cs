using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class WaypointPath : MonoBehaviour
{
    public Transform[] waypoints;

    void OnValidate()
    {
        int childCount = transform.childCount;
        waypoints = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                // to mau diem waypoint
                if (i == 0)
                    Gizmos.color = Color.green; // diem bat dau
                else if (i == waypoints.Length - 1)
                    Gizmos.color = Color.blue;  // diem ket thuc
                else
                    Gizmos.color = Color.red;

                Gizmos.DrawSphere(waypoints[i].position, 0.1f);

#if UNITY_EDITOR
                Handles.Label(waypoints[i].position + Vector3.up * 0.3f, $"WP {i}");// hien thi nhan
#endif

                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);// to mau duong waypointpath
                }
            }
        }
    }
}


