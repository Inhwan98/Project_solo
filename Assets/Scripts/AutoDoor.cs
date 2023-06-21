using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    Vector3 StartPos_left;
    Vector3 StartPos_right;

    Vector3 tartget_left;
    Vector3 tartget_right;

    [SerializeField] float moveSpeed;

    private bool isOpen;
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
        if(isOpen)
        {
            if(Vector3.Distance(leftDoor.transform.position, tartget_left) <= 0.1f)
            {
                leftDoor.transform.position = tartget_left;
                rightDoor.transform.position = tartget_right;
            }
            else
            {
                leftDoor.transform.position = Vector3.Lerp(StartPos_left, tartget_left, Time.deltaTime * moveSpeed);
                rightDoor.transform.position = Vector3.Lerp(StartPos_right, tartget_right, Time.deltaTime * moveSpeed);
            }
        }
        else
        {
            if (Vector3.Distance(leftDoor.transform.position, StartPos_left) <= 0.1f)
            {
                leftDoor.transform.position = StartPos_left;
                rightDoor.transform.position = StartPos_right;
            }
            else
            {
                leftDoor.transform.position = Vector3.Lerp(tartget_left, StartPos_left, Time.deltaTime * moveSpeed);
                rightDoor.transform.position = Vector3.Lerp(tartget_right, StartPos_right, Time.deltaTime * moveSpeed);
            }

        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.CompareTag("CHARACTER"))
        {
            Debug.Log("문 열림");
            isOpen = true;
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if(coll.gameObject.CompareTag("CHARACTER"))
        {
            Debug.Log("문 열림");
            isOpen = false;
        }
    }
}
