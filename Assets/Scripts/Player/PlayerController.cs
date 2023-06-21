using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private bool isAttack;
    Quaternion attackRotation;
    //// Attack End

    private float time = 0;

    ////

    public static PlayerController instance = null;

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

    // Update is called once per frame
     private void FixedUpdate()
    {
        PlayerInput();
        PlayerMove();
        PlayerTurn();
    }
    void Update()
    {
        
        
        
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
        _moveVector = new Vector3(_horizontalAxis, 0, _verticalAxis).normalized * _moveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + _moveVector);
        anim.SetFloat(hashRun, _isRun ? 1f : 0f);
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

}
