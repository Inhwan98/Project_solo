using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBowCtr : MonoBehaviour
{
    [Header("Player Attack Info")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float sensorRadious = 10f;
    [SerializeField] private float sensorRange = 10f;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject arrowObj;

    private bool isAttack;
    private GameObject arrowTemp;
    private Quaternion attackRotation;

    private readonly int hashFire = Animator.StringToHash("Fire");
    private Animator anim;
    private Rigidbody rigid;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {
        anim.SetBool(hashFire, true);

        StartCoroutine(Targetting());
    }

    // Update is called once per frame
    void Update()
    {
        if(isAttack)
        {
            transform.rotation = Quaternion.Slerp(rigid.rotation, attackRotation, Time.deltaTime);
        }
    }
    IEnumerator Targetting()
    {
        while (true)
        {
            RaycastHit[] attackHits =
          Physics.SphereCastAll(transform.position,
                                sensorRadious,
                                transform.forward,
                                sensorRange,
                                enemyLayer);

            if (attackHits.Length > 0)
            {
                //Debug.Log("Cross Bow : Enemy 감지했습니다");
                Vector3 enemyPos = attackHits[0].transform.position;
                attackRotation = Quaternion.LookRotation(enemyPos - transform.position);
                //anim.SetBool(hassAttack, true);
                isAttack = true;     
            }
            else
            {
                attackRotation = Quaternion.identity;
                isAttack = false;
            }

            yield return null;
        }

    }

    private void CreateCrossArrow()
    {
        Transform arrowTr = arrowObj.transform;
        GameObject _arrowPrefab = Instantiate(arrowPrefab, arrowTr.position, arrowTr.rotation);
        arrowTemp = _arrowPrefab;
        arrowTemp.transform.parent = arrowTr.root;

         anim.SetBool(hashFire, false);
    }

    private void EndAttack()
    {
        arrowTemp.transform.SetParent(null);
        Rigidbody arrowRigid = arrowTemp.GetComponent<Rigidbody>();
        Vector3 dir = attackRotation * (transform.forward);
        //arrowTemp.transform.rotation = Quaternion.LookRotation(dir);
        arrowRigid.AddForce((arrowObj.transform.forward) * 600.0f);
        Destroy(arrowTemp, 5.0f);
        arrowTemp = null;

        anim.SetBool(hashFire, true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position + transform.forward * sensorRange);
        Gizmos.DrawWireSphere(transform.position + transform.forward * sensorRange, sensorRadious);
    }

}
