using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCameraControl : MonoBehaviour
{
    private Transform myTransform = null;

    public float distance = 6.0f;
    public float height = 5.5f;
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    public Transform target = null; // player.

    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;

        float currentRotationAngle = myTransform.eulerAngles.y;
        float currentHeight = myTransform.position.y;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);

        myTransform.position = target.position;
        myTransform.position -= currentRotation * Vector3.forward * distance;

        myTransform.position = new Vector3(myTransform.position.x, currentHeight, myTransform.position.z);
        myTransform.LookAt(target);

    }
}
