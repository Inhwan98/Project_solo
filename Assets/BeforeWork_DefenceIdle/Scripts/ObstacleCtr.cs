using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inan.Obstalce
{
    public class ObstacleCtr : MonoBehaviour
    {
        [SerializeField] private ObstacleState obstacleState = ObstacleState.NONE;
        [SerializeField] private ObstacleSizeState obstacleSizeState  = ObstacleSizeState.NONE;
        
        [SerializeField] private GameObject dropItem;

         private GameObject _dropItem;

        [SerializeField] private float dropHeightOffset;

        private Vector3 targetVec;

        private Vector3 P1;
        private Vector3 P2;
        private Vector3 P3;
        private Vector3 P4;

        private bool isDrop;

        private float time;
        private float t;

        private int obstacle_HP = 0;
        private bool isBroken = false;
        
        [SerializeField] private float destTime;

        // Start is called before the first frame update
        void Start()
        {
            isDrop = false;
            P1 = transform.position;
            P2 = P1 + (Vector3.up * dropHeightOffset);

            ObstacleSetHP();
        }

        // Update is called once per frame
        void Update()
        {
            StartMovement();

            if(isBroken)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 90.0f), Time.deltaTime);
            }
        }

        private void ObstacleSetHP()
        {
            switch(obstacleSizeState)
            {
                case ObstacleSizeState.SMALL:
                    obstacle_HP = 2;
                    break;
                case ObstacleSizeState.MIDIUM:
                    obstacle_HP = 3;
                    break;
                case ObstacleSizeState.BIG:
                    obstacle_HP = 4;
                    break;
            }
        }

        private void StartMovement()
        {
            if(isDrop)
            {
                if(time < destTime)
                {
                    time += Time.deltaTime;
                    _dropItem.transform.position = bazierCurve(time / destTime);
                }
                else
                {
                    time = 0;
                    isDrop = false;
                    _dropItem.transform.position = bazierCurve(1.0f);
                }
            }
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
            {
                if(isBroken) return;
                Debug.Log("플레이어의 무기와 충돌");
                _dropItem = Instantiate(dropItem, transform.position, Quaternion.Euler(-120f, 0f, 0f));
                Rigidbody itemRigid = _dropItem.GetComponent<Rigidbody>();
                

                //drop and rotaiton
                itemRigid.AddTorque(Vector3.up * 50.0f);
                
                //Random Pos
                Vector3 randPos = Vector3.zero;
                while(randPos == Vector3.zero)
                {
                    randPos.x += Random.Range(-1, 1);
                    randPos.z += Random.Range(-1, 1);
                }

                P4 = transform.position + randPos; //target Vec
                P3 = P4 + (Vector3.up * dropHeightOffset);

                isDrop = true;
                --obstacle_HP;
                Debug.Log($"Obstacle HP : {obstacle_HP}");

                if(obstacle_HP == 0)
                {
                    isDrop = false;
                    coll.gameObject.transform.root.GetComponent<PlayerCtr>().StopPickAction();
                    _dropItem = null;
                    isBroken = true;
                    this.gameObject.layer = 2; // Ignore RayCast;
                    foreach(Transform childTr in this.transform)
                    {
                        childTr.gameObject.layer = 2;
                    }
                    Destroy(this.gameObject, 3.0f);
                }
            }
        }

        private Vector3 bazierCurve(float value)
        {
            Vector3 A = Vector3.Lerp(P1, P2, value);
            Vector3 B = Vector3.Lerp(P2, P3, value);
            Vector3 C = Vector3.Lerp(P3, P4, value);

            Vector3 D = Vector3.Lerp(A, B, value);
            Vector3 E = Vector3.Lerp(B, C, value);

            return Vector3.Lerp(D, E, value);
        }

        public ObstacleState GetObstacleState()
        {
            return obstacleState;
        }
    }

}
