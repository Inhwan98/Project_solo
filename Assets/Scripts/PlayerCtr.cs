using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtr : MonoBehaviour
{
    //Animator cashing
    private readonly int hashRun = Animator.StringToHash("IsRun");

    //


    [Header("Player MoveSetting")]
    public float _moveSpeed = 5f;                //   이동 속도
    [SerializeField] private float _rotationSpeed = 5f;            //   이동 속도
    private float _horizontalAxis;                                  //  수평 입력 값
    private float _verticalAxis;                                    //  수직 입력 값
    private bool _isRun;                                            //  달리기 bool 값
    private Vector3 _moveVector;                                    //  moveVector 저장 
    private Animator anim;                               //  Animator 저장
    private Rigidbody rigid;                            //  Rigidbody 저장

    [Header("JoyStick Controller")]
    [SerializeField] private DynamicJoystick joy;

    ////Attack / Ray Cast
    [Header("Player Attack Info")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float sensorRadious = 10f;
    [SerializeField] private float sensorRange = 10f;

    [SerializeField] private GameObject arrowObj;
    [SerializeField] private GameObject arrowTemp;
    [SerializeField] private GameObject arrowPrefab;
    [Space(20)]
    [SerializeField] private Image chargeImage;
    [SerializeField] private float chageTime = 3.0f;
    private bool isAttack;
    Quaternion attackRotation;
    //// Attack End

    private float time = 0;

    




    ////

    public static PlayerCtr instance = null;




    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {
        StartCoroutine(Targetting());
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        PlayerMove();
        PlayerTurn();
        AttackLayerTime();
        ChargeAttack();
    }

    private void ChargeAttack()
    {
        if (chargeImage.fillAmount < 1.0f)
        {
            chargeImage.fillAmount += Time.deltaTime / chageTime;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            float chageAmount = chargeImage.fillAmount;
            if (chageAmount < 0.5f)
            {
                Debug.Log("소 공격");
            }
            else if (chageAmount >= 0.5f && chageAmount < 0.8f)
            {
                Debug.Log("중 공격");
            }
            else
            {
                Debug.Log("대 공격");
            }
            chargeImage.fillAmount = 0;
        }
    }

 

    IEnumerator Targetting()
    {
        while (true)
        {
            RaycastHit[] attackHits =
          Physics.SphereCastAll(transform.position,
                                sensorRadious,
                                Vector3.forward,
                                sensorRange,
                                enemyLayer);

            if (attackHits.Length > 0)
            {
                Debug.Log("Enemy 감지했습니다");
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


    // Input
    private void PlayerInput()
    {
        _horizontalAxis = joy.Horizontal;
        _verticalAxis = joy.Vertical;
        // 입력 값이 있는 경우에만 _isRun을 true로 설정
        _isRun = (_horizontalAxis != 0f || _verticalAxis != 0f);
    }
    // 플레이어 이동
    private void PlayerMove()
    {
        _moveVector = new Vector3(_horizontalAxis, 0, _verticalAxis).normalized * _moveSpeed * Time.deltaTime;
        rigid.MovePosition(rigid.position + _moveVector);
        anim.SetFloat(hashRun, _isRun ? 1f : 0f);
    }

    private void AttackLayerTime()
    {
        if(!isAttack)
        {
            if (time >= 0)
            {
                time -= Time.deltaTime * 2.0f;
            }
            else if(time <= 0.0f)
            {
                time = 0.0f;
            }
        }
        else
        {
            if (time <= 1)
            {
                time += Time.deltaTime * 3f;
            }
            else if(time >=1.0f)
            {
                time = 1.0f;
            }
        }

        anim.SetLayerWeight(1, time);
    }

    // 플레이어 방향
    private void PlayerTurn()
    {
        if (_moveVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveVector);
            Quaternion moveQuat = Quaternion.Slerp(rigid.rotation, targetRotation, 0.3f);
            rigid.MoveRotation(moveQuat);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position + Vector3.forward * sensorRange);
        Gizmos.DrawWireSphere(transform.position + Vector3.forward * sensorRange, sensorRadious);
    }


//Arrow Attack
    private void CreateArrow()
    {
        Transform arrowTr = arrowObj.transform;
        GameObject _arrowPrefab = Instantiate(arrowPrefab, arrowTr.position, arrowTr.rotation);
        arrowTemp = _arrowPrefab;
        arrowTemp.transform.parent = arrowTr.parent;
    }

    private void EndAttack()
    {
        arrowTemp.transform.SetParent(null);
        Rigidbody arrowRigid = arrowTemp.GetComponent<Rigidbody>();
        Vector3 dir = attackRotation * Vector3.forward;
        arrowTemp.transform.rotation = Quaternion.LookRotation(dir);
        arrowRigid.AddForce(arrowTemp.transform.forward * 500f);
        Destroy(arrowTemp, 5.0f);
        arrowTemp = null;
    }

    



}
