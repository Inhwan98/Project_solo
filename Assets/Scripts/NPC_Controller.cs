using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Inan.NPC
{
    public class NPC_Controller : MonoBehaviour
    {
        private readonly int hashWalk = Animator.StringToHash("IsWalk");
        private readonly int hashThink = Animator.StringToHash("IsThink");
        private readonly int hashHit = Animator.StringToHash("IsHit");
        private readonly int hashShouting = Animator.StringToHash("IsShout");

        [Header("NPC State")]
        [SerializeField] NPCState npcState = NPCState.IDLE;
        private NavMeshAgent agent;


        private Animator anim;
        private Transform destTr; // agent의 목적지. 처음 목적지는 카트
        private Transform tempDestTr; // agent의 목적지. 처음 목적지는 카트
        private Transform cartTr;
        private Transform boxTr;
        [SerializeField] private GameObject boxPrefab;

        private float time; //layer 바꿔주는 time
        private bool isCatchCart = false;
        GameManager gmr;
        PlayerController playerController;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
        }
        void Start()
        {
            gmr = GameManager.instance;
            playerController = PlayerController.instance;

            //agent.isStopped = true;

            StartCoroutine(CheckState());
            StartCoroutine(CheckAnimState());
        }

        // Update is called once per frame
        void Update()
        {
            
            //Cart 잡기
            ActionCatchCart();

        }

        void ActionCatchCart()
        {

            if (isCatchCart)
            {
                if (time <= 0.75)
                {
                    time += Time.deltaTime * 3f;
                }
                anim.SetLayerWeight(1, time);
            }
            else
            {
                if (time > 0)
                {
                    time -= Time.deltaTime * 2f;
                }
                anim.SetLayerWeight(1, time);
            }
        }

        IEnumerator CheckState()
        {
            while (true)
            {
                switch (npcState)
                {
                    case NPCState.IDLE:
                        Debug.Log("NPC : IDLE");
                        break;
                    //카트에 근접하면 카트 잡는 레이어 활성화
                    case NPCState.CATCHCART:
                       
                        if (Vector3.Distance(transform.position, destTr.position + (-destTr.right * 4.0f)) <= 1f)
                        {
                            
                            Debug.Log("Catch");
                            transform.LookAt(destTr.position + (Vector3.right));

                            agent.isStopped = true;
                            isCatchCart = true;
                            yield return new WaitForSeconds(0.3f);
                            destTr.parent = transform;
                            cartTr = destTr;
                            

                            //Rigidbody cartRigid = cartTr.gameObject.GetComponent<Rigidbody>();
                            //cartRigid.isKinematic = true; // 마법사 NPC

                            //catch후 첫 입장
                            destTr = gmr.GetRandTr();
                            agent.SetDestination(destTr.position);
                            agent.isStopped = false;

                            npcState = NPCState.MARTIN;
                        }
                        break;

                    case NPCState.MARTIN:
                        if (agent.remainingDistance <= 0.1f)
                        {
                            agent.isStopped = true;
                            yield return new WaitForSeconds(0.3f);
                            gmr.SetRandTr(destTr);
                            destTr = gmr.GetPayTr();

                            agent.isStopped = false;
                            agent.SetDestination(destTr.position);
                            npcState = NPCState.PAY;
                            //cartTr.SetParent(null);

                            //Debug.Log("자식 분리");
                            //anim.SetBool(hashThink, true);
                            //isCatchCart = false;
                        }
                        break;
                    case NPCState.PAY:
                        if (agent.remainingDistance <= 0.1f)
                        {
                            GameObject boxObj = Instantiate(boxPrefab, cartTr.position + (Vector3.up * 2.2f), Quaternion.identity);
                            boxObj.transform.parent = cartTr;
                            boxTr = boxObj.transform;
                            destTr = gmr.GetExitTr();
                            agent.SetDestination(destTr.position);
                            gameObject.layer = LayerMask.NameToLayer("NPC");
                            npcState = NPCState.EXIT;
                        }
                        break;

                    case NPCState.EXIT:
                        if (agent.remainingDistance <= 0.1f)
                        {
                            boxTr.SetParent(this.transform);
                            boxTr.localPosition = (transform.forward * 1.0f) + (transform.up * 4f);
                            cartTr.SetParent(null);
                            cartTr.gameObject.layer = LayerMask.NameToLayer("PicDownCart");

                            //Rigidbody cartRigid = cartTr.gameObject.GetComponent<Rigidbody>();
                            //cartRigid.isKinematic = false;
                            destTr = gmr.GetFinalTr();
                            agent.SetDestination(destTr.position);

                            npcState = NPCState.FINAL;
                        }
                        break;

                    case NPCState.FINAL:
                        if (agent.remainingDistance <= 0.1f)
                        {
                            Destroy(this.gameObject);
                        }
                        break;

                    case NPCState.HITCART:
                        anim.enabled = true;
                        isCatchCart = false;
                        anim.SetBool(hashHit, true);
                        
                        yield return new WaitForSeconds(1.5f);
                        
                        anim.SetBool(hashHit, false);
                        anim.SetBool(hashShouting, true);
                                                
                        yield return new WaitForSeconds(2.0f);
                        agent.SetDestination(destTr.position);
                        anim.SetBool(hashShouting, false);
                        agent.isStopped = false;
                        
                        destTr = tempDestTr;
                        if (cartTr != null && boxTr != null)
                        {
                            boxTr.SetParent(null);
                            Destroy(boxTr.gameObject);
                            //cartTr.SetParent(transform);
                        }
                        npcState = NPCState.FINAL;
                        break;

                    case NPCState.ANGRY:
                        if (Vector3.Distance(transform.position, destTr.position) <= 1f)
                        {
                            Debug.Log("Angry");
                            anim.SetBool(hashShouting, true);

                        }
                        break;


                }
                yield return new WaitForSeconds(0.3f);
            }
        }

        IEnumerator CheckAnimState()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.3f);
                if (agent.velocity.magnitude > 0.1f)
                {
                    anim.SetBool("IsWalk", true);
                }
                else
                {
                    anim.SetBool(hashWalk, false);
                }
            }
        }

        public void SetNPCState(NPCState _npcState)
        {
            npcState = _npcState;
        }

        public void SetCartDestination(Transform _destTr)
        {
            destTr = _destTr;
            agent.isStopped = false;
            agent.SetDestination(_destTr.position + (-_destTr.right * 4.0f));
            npcState = NPCState.CATCHCART;
        }

        public void SetDestination(Transform _destTr)
        {
            destTr = _destTr;
            agent.isStopped = false;
            agent.SetDestination(_destTr.position);
        }

        private void OnTriggerEnter(Collider coll)
        {
            {
                if (coll.gameObject.layer == LayerMask.NameToLayer("PlayerCart"))
                {
                    if (npcState == NPCState.EXIT || npcState == NPCState.FINAL)
                    {
                        this.gameObject.layer = LayerMask.NameToLayer("Player");

                        isCatchCart = false;
                        agent.isStopped = true;

                        //Box 설정
                        var boxRigid = boxTr.GetComponent<Rigidbody>();
                        boxRigid.isKinematic = false;
                        boxRigid.AddExplosionForce(300.0f, coll.transform.position, 10.0f, 300.0f);

                        var boxColl = boxTr.GetComponent<Collider>();
                        boxColl.isTrigger = false;

                        if (cartTr != null && boxTr != null)
                        {
                            cartTr.SetParent(null);
                            boxTr.SetParent(null);
                            cartTr.gameObject.layer = LayerMask.NameToLayer("PicDownCart");
                        }

                        npcState = NPCState.HITCART;
                    }

                }
            }
        }

    }
}


