using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDCamera3D : MonoBehaviour
{
    [Header("Pan Settings")]
    public float dragSpeed = 0.5f;// toc do di chuyen camera

    [Header("Zoom Settings")]
    public float zoomSpeed = 500f;// toc do Zoom camera
    public float minY = 10f;// Gioi han
    public float maxY = 60f;// Gioi han
    public float minX = -10f;// Gioi han
    public float maxX = 10f;// Gioi han
    public float minZ = -10f;// Gioi han
    public float maxZ = 10f;// Gioi han

    private Vector3 dragOrigin; // tam goc di chuyen

    void Update()
    {
        HandleMouseDrag();
        HandleMouseZoom();
    }

    // di chuyen camera
    void HandleMouseDrag()
    {
        // Khi nhan chuot trai
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;// tam la vi tri chuot luc ban dau
        }

        // Khi giu chuot trai di chuyen
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);// vi tri khoang cach di chuyen
            Vector3 move = new Vector3(-difference.x * dragSpeed, 0, -difference.y * dragSpeed);

            transform.Translate(move, Space.World);

            // Clamp gioi han bien do di chuyen
            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
            clampedPos.z = Mathf.Clamp(clampedPos.z, minZ, maxZ);
            transform.position = clampedPos;

            dragOrigin = Input.mousePosition;
        }
    }

    void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");// chuot giua
        Vector3 pos = transform.position;
        pos.y -= scroll * zoomSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}

