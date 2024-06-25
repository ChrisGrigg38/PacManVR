using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Boolean IsAttackPickup;

    private float YBob;
    private float YBobSpeed = 5;
    private float YBobHeight = 0.1f;
    private float RotateSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(0, (float)Math.Cos(Time.time * YBobSpeed) * YBobHeight, 0);
        transform.localRotation = Quaternion.Euler(-90, (float)(Math.Sin(Time.time) + 1f) * 180f, transform.localRotation.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("coin collided");
        if(other.tag == "Player")
        {
            other.SendMessage("RecieveItem");
            GameObject.Destroy(transform.parent.gameObject);
        }
    }
}
