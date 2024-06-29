using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Ghost : MonoBehaviour
{
    public GameObject normalGhost;
    public GameObject scaredGhost;
    private GameObject VRRig;
    public float minUpdateTime = 10;
    public float maxUpdateTime = 40;
    public float damageAmt = 5000;
    public AudioSource dieSound;
    public float attackPlayerDistance = 30;
    public float playAfterGameTimeStart = 8;

    //globals
    private NavMeshAgent navMeshAgent;
    private float updateTime;
    private GameModeManager globals;
    private float destroyTimer = -1;
    private GameObject[] poi;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        updateTime = UnityEngine.Random.Range(minUpdateTime, maxUpdateTime);
        VRRig = GameObject.FindGameObjectWithTag("Player");
        poi = GameObject.FindGameObjectsWithTag("Point");
    }

    void RunGhostDeadLogic()
    {
        if (destroyTimer > 0)
        {
            destroyTimer -= Time.deltaTime;

            if (scaredGhost.transform.localRotation.y < 0)
            {
                float newY = scaredGhost.transform.localRotation.x + (Time.deltaTime * 90);

                if (newY > 0) newY = 0;

                scaredGhost.transform.localRotation = Quaternion.Euler(newY, scaredGhost.transform.localRotation.y, scaredGhost.transform.localRotation.z);
                normalGhost.transform.localRotation = Quaternion.Euler(newY, normalGhost.transform.localRotation.y, normalGhost.transform.localRotation.z);
            }

            if (destroyTimer <= 0)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    void LazyFetchGlobals()
    {
        if (globals == null && VRRig != null)
        {
            globals = VRRig.GetComponent<GameModeManager>();
        }
    }

    void RunGhostVisibleLogic()
    {
        if (globals != null)
        {
            if (globals.ghostAttackTimer > 0)
            {
                scaredGhost.SetActive(true);
                normalGhost.SetActive(false);
            }
            else
            {
                scaredGhost.SetActive(false);
                normalGhost.SetActive(true);
            }
        }
    }

    void RunGhostMovementLogic()
    {
        updateTime -= Time.deltaTime;
        playAfterGameTimeStart -= Time.deltaTime;

        if (globals != null && globals.itemsLeft == 0)
        {
            navMeshAgent.isStopped = true;
            float rotationX = (float)(-30 - (Math.Sin((((Time.time / 50) % 1) * 180)) * 60));
            
            scaredGhost.transform.localRotation = Quaternion.Euler(rotationX, scaredGhost.transform.localRotation.y, scaredGhost.transform.localRotation.z);
            normalGhost.transform.localRotation = Quaternion.Euler(rotationX, normalGhost.transform.localRotation.y, normalGhost.transform.localRotation.z);
        }
        else if (playAfterGameTimeStart <= 0 && (updateTime <= 0 || !navMeshAgent.hasPath))
        {
            if (VRRig != null)
            {
                if (destroyTimer == -1)
                {
                    if (Vector3.Distance(VRRig.transform.position, transform.position) < attackPlayerDistance || globals.itemsLeft < 50)
                    {
                        navMeshAgent.SetDestination(VRRig.transform.position);
                        updateTime = 2;
                    }
                    else
                    {
                        int index = (int)UnityEngine.Random.Range(0, 1.0f) * poi.Count();
                        navMeshAgent.SetDestination(poi[index].transform.position);
                        updateTime = UnityEngine.Random.Range(minUpdateTime, maxUpdateTime);
                    }
                }

            }
            else
            {
                VRRig = GameObject.FindGameObjectWithTag("Player");
            }

           
        }
    }

    // Update is called once per frame
    void Update()
    {
        LazyFetchGlobals();
        RunGhostMovementLogic();
        RunGhostVisibleLogic();
        RunGhostDeadLogic();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(scaredGhost.activeSelf && destroyTimer == -1)
            {
                destroyTimer = 3;
                navMeshAgent.isStopped = true;
                dieSound.Play();
            }
            else if(globals!= null && globals.ghostAttackTimer == 0 && globals.itemsLeft > 0)
            {
                VRFPSHealthCharacter healthComponent = other.gameObject.GetComponent<VRFPSHealthCharacter>();

                if (healthComponent != null)
                {
                    VRFPSDamageInfo dmgInfo = new VRFPSDamageInfo();
                    dmgInfo.type = VRFPSDamageType.DAMAGE_TYPE_AREA;
                    dmgInfo.amt = damageAmt;

                    healthComponent.SendMessage("ApplyDamage", dmgInfo);
                }
            }


            
        }
    }
}
