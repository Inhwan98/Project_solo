using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Inan.Obstalce;


public class PlayerController : MonoBehaviour
{
    private readonly int hashRun = Animator.StringToHash("IsRun");

    [Header("Player State")]

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

    ////

    public static PlayerController instance = null;

    // 카트 이동 구현
    List<Transform> cartTr = new List<Transform>();
    List<float> carDistanceList = new List<float>();
    float timeSum;
    [SerializeField]  float[] time = new float[28];

    [SerializeField] float[] destTime = new float[28];
    
    private float cartOffset = 3.5f;

    GameManager gmr;
    

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

    private void Start()
    {
        gmr = GameManager.instance;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
       
    }
    void Update()
    {
        PlayerInput();
        PlayerMove();
        PlayerTurn();
        if (cartTr.Count == 0) return;

        timeSum += Time.deltaTime;
        int cartCount = cartTr.Count;


        float offsetTime = 0.1f;

        
        for (int i = 0; i < cartCount; i++)
        {
            if (i < 5)
            {
                destTime[i] = offsetTime;
            }
            else if(i >= 5)
            {
                offsetTime += 0.1f;
                destTime[i] = offsetTime;
            }
            
        }

        for (int i = 0; i < cartTr.Count; i++)
        {
           
            if (timeSum < destTime[i])
            {
                time[i] = timeSum / destTime[i];

                if (cartTr[i].parent == this.transform) cartTr[i].SetParent(null);
                float distance = carDistanceList[i];

                cartTr[i].transform.position = Vector3.Slerp(cartTr[i].transform.position, this.transform.position + (transform.forward * distance), time[i]);
                Quaternion targetRot = Quaternion.Euler(0, 270f, 0);

                cartTr[i].localRotation = Quaternion.Slerp(cartTr[i].localRotation, targetRot * transform.rotation, time[i]);
            }
            else
            {
                time[i] = 0;
                timeSum = 0;
            }
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
        transform.position += _moveVector;
        //rigid.MovePosition(rigid.position + _moveVector);
        anim.SetFloat(hashRun, _isRun ? 1f : 0f);
    }

    // 플레이어 방향
    private void PlayerTurn()
    {
        if (_moveVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveVector);
            Quaternion moveQuat = Quaternion.Slerp(transform.rotation, targetRotation, 0.3f);
            transform.rotation = moveQuat;
            //rigid.MoveRotation(moveQuat);
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        GameObject otherObj = coll.gameObject;
        if(otherObj.layer == LayerMask.NameToLayer("Cart"))
        {
            NavMeshObstacle  navObstacle = otherObj.AddComponent<NavMeshObstacle>();
            navObstacle.carving = true;

            Rigidbody otherRigid = otherObj.GetComponent<Rigidbody>();
            otherObj.layer = this.gameObject.layer;
            otherObj.transform.SetParent(null);
            otherObj.transform.parent = this.transform;
            anim.SetLayerWeight(1, 0.75f);

            cartTr.Add(otherObj.transform);
            carDistanceList.Add(cartOffset);

            Destroy(otherRigid);

            cartOffset += 1.5f;
        }
        else if(otherObj.CompareTag("CartParking"))
        {
            if (cartTr == null) return;
            foreach(Transform _cartTr in cartTr)
            {
                GameObject cartObj = _cartTr.gameObject;
                NavMeshObstacle navObstacle = cartObj.GetComponent<NavMeshObstacle>();
                Rigidbody cartRigid = cartObj.AddComponent<Rigidbody>();
                Destroy(navObstacle);
            }

            anim.SetLayerWeight(1, 0);
            gmr.SetCartGroup(cartTr);
            cartTr.Clear();
        }

    }


}
