using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float defaultTargetDistance = 5.0f;
    [SerializeField] private float maxTargetDistance = 7.0f;
    [SerializeField] private float minTargetDistance = 3.0f;
    [SerializeField] private float targetRotateSpeed = 4.0f;

    public float rotSpeed = 1.5f;
    public float zoomSpeed = 10f;

    private float rotY;
    private float rotX;

    private float distance;
    private float targetDistance;
    private Vector3 targetRotation;
    private Vector3 targetCurRotation;

    // Start is called before the first frame update
    void Start()
    {
        rotY = transform.eulerAngles.y;
        rotX = transform.eulerAngles.x;
        targetDistance = defaultTargetDistance;
        targetCurRotation = target.transform.rotation.eulerAngles;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) 
        {
            rotY += Input.GetAxis("Mouse X") * rotSpeed * 3;
            rotX -= Input.GetAxis("Mouse Y") * rotSpeed * 2;

            rotX = Mathf.Clamp(rotX, 1f, 89.0f);
        } 

        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rotation;

        targetDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed; 
        targetDistance = Mathf.Clamp(targetDistance, minTargetDistance, maxTargetDistance);

        distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * 4f); 

        Vector3 targetPosition = target.position;
        transform.position = targetPosition - rotation * new Vector3(0, 0, distance);

        if (Input.GetMouseButton(1))
        {
            targetRotation = target.rotation.eulerAngles;
            targetRotation.y = transform.rotation.eulerAngles.y;
        }

        targetCurRotation = Quaternion.Lerp(Quaternion.Euler(targetCurRotation), Quaternion.Euler(targetRotation), Time.deltaTime * targetRotateSpeed).eulerAngles;
        target.transform.rotation = Quaternion.Euler(targetCurRotation);
    }
}
