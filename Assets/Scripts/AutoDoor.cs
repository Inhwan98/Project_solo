using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{

    [Header("Door Sensor")]
    [SerializeField] float detectionAngle = 45f;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] LayerMask CharacterLayer;
    [SerializeField] Color color;


    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    Vector3 StartPos_left;
    Vector3 StartPos_right;

    Vector3 tartget_left;
    Vector3 tartget_right;

    [SerializeField] private bool isOpen;

    void Start()
    {
        StartPos_left = leftDoor.transform.position;
        StartPos_right = rightDoor.transform.position;
        
        
        tartget_left = StartPos_left+ (new Vector3(4.0f, 0, 0));
        tartget_right = StartPos_right+ new Vector3(-4.0f, 0, 0);

        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, CharacterLayer);

        if(colliders.Length > 0)
        {
            isOpen = true;
            leftDoor.transform.position = Vector3.Lerp(StartPos_left, tartget_left, 1);
            rightDoor.transform.position = Vector3.Lerp(StartPos_right, tartget_right, 1);
        }
        else
        {
            isOpen = false;
            leftDoor.transform.position = Vector3.Lerp(tartget_left, StartPos_left, 1);
            rightDoor.transform.position = Vector3.Lerp(tartget_right, StartPos_right, 1);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
