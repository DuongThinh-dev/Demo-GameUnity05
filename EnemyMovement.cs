using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public GameObject enemyObject;

    public WaypointPath path;

    private Enemy Enemy;
    private int currentIndex = 0;

    private void Start()
    {
        Enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        currentIndex = 0;
    }

    void Update()
    {
        if (path == null || path.waypoints.Length == 0) return;

        if (currentIndex >= path.waypoints.Length)
        {
            OnPathComplete();
            return;
        }

        Transform target = path.waypoints[currentIndex];
        Vector3 dir = target.position - transform.position;

        if (dir != Vector3.zero)
        {
            transform.Translate(dir.normalized * Enemy.speed * Time.deltaTime, Space.World);

            Vector3 lookDirection = new Vector3(dir.x, 0f, dir.z);

            Quaternion lookRot = Quaternion.LookRotation(lookDirection);

            enemyObject.transform.rotation = Quaternion.Slerp(
                enemyObject.transform.rotation,
                lookRot,
                Time.deltaTime * 10f
            );

            //enemyObject.transform.LookAt(target.position);
        }

        if (dir.magnitude < 0.1f)
        {
            currentIndex++;
        }


    }

    // den dich
    void OnPathComplete()
    {
        gameObject.SetActive(false);
    }

    public void ResetPath()
    {
        currentIndex = 0;// bat dau tu waypoint dau tien.
        if (path != null && path.waypoints.Length > 0)
        {
            transform.position = path.waypoints[0].position;// dat vi tri enemy ve vi tri xuat phat dau duong di
        }
    }

}

