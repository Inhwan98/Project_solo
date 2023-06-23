using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float zOffset;
    [SerializeField] private float xOffset;
    [SerializeField] private float height;
    [SerializeField] private float damping = 0.2f;

    Vector3 velocity;
    PlayerController playerCtr;
    Transform playerTr;

    private bool isMove = false;
    void Start()
    {  
       playerCtr = PlayerController.instance; 
       playerTr = playerCtr.transform;
    }

    // Update is called once per frame

    void LateUpdate()
    {

        Vector3 pos = playerTr.position +
                      (-Vector3.forward * zOffset) +
                      (Vector3.right * xOffset) +
                       (Vector3.up * height);

        transform.position = Vector3.SmoothDamp(transform.position,
                                                pos,
                                                ref velocity,
                                                damping);
   

        

       
    }
}
