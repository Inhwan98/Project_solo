using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    static public PlayerSensor instance = null;

    [Header("Player Sensor")]
    [SerializeField] float detectionAngle = 45f; 
    [SerializeField] float detectionRange = 5f;   
    [SerializeField] LayerMask targetLayer;
    [SerializeField] Color color;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    // void Update()
    // {
    //      Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, npcLayer);

    //     foreach (Collider collider in colliders)
    //     {
    //         Vector3 directionToNPC = collider.transform.position - transform.position;
    //         float angleToNPC = Vector3.Angle(transform.forward, directionToNPC);

    //         if (angleToNPC <= detectionAngle * 0.5f)
    //         {
    //             // NPC 감지됨
    //             // 추가 동작을 수행하거나 필요한 처리를 진행합니다.
    //         }
    //     }


    // }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-detectionAngle * 0.5f, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(detectionAngle * 0.5f, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(transform.position, leftRayDirection * detectionRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * detectionRange);
        Gizmos.DrawRay(transform.position, transform.forward * detectionRange);
        Gizmos.DrawRay(transform.position, -transform.forward * detectionRange);
        Gizmos.DrawRay(transform.position, transform.right * detectionRange);
        Gizmos.DrawRay(transform.position, -transform.right * detectionRange);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
