using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Boolean IsAttackPickup;
    [SerializeField] AudioSource audioClipPickup;
    [SerializeField] GameObject coinModel;
    [SerializeField] GameObject radarModel;

    private float YBob;
    private float YBobSpeed = 5;
    private float YBobHeight = 0.1f;
    private float RotateSpeed = 1;
    private bool pickedUp = false;
    private float pickedUpRemoveCounter = 5;
    private float ThrowUpSpeed = 1;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(pickedUp)
        {
            if (pickedUpRemoveCounter > 0)
            {
                pickedUpRemoveCounter -= Time.deltaTime;

                coinModel.transform.localPosition = new Vector3(0, coinModel.transform.localPosition.y + (Time.deltaTime * ThrowUpSpeed), 0);
                coinModel.transform.localRotation = Quaternion.Euler(-90, coinModel.transform.localRotation.y + (Time.deltaTime * 20), transform.localRotation.z);
            }
            else
            {
                coinModel.SetActive(false);
                radarModel.SetActive(false);
            }
        }
        else 
        {
            coinModel.transform.localPosition = new Vector3(0, (float)Math.Cos(Time.time * YBobSpeed) * YBobHeight, 0);
            coinModel.transform.localRotation = Quaternion.Euler(-90, (float)(Math.Sin(Time.time) + 1f) * 180f, transform.localRotation.z);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !pickedUp)
        {
            pickedUp = true;
            if (IsAttackPickup)
            {        
                other.SendMessage("RecieveBonusItem");
            }
            
           other.SendMessage("RecieveItem");
           audioClipPickup.Play();   
        }
    }
}
