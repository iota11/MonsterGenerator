using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoftBody : MonoBehaviour
{
    private List<GameObject> rig = new List<GameObject>();
    public float mass;
    public float drag;
    public float angularDrag;
    public float spring;

    private void Awake()
    {
        foreach (Transform bone in transform.GetComponentsInChildren<Transform>())
        {
            if (UnityEngine.Random.value >= 0.5)
            {
                continue;
            }
            GameObject newBone = bone.gameObject;
            rig.Add(newBone);


        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject bone in rig)
        {
            Rigidbody rb = bone.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.constraints = RigidbodyConstraints.FreezeRotationY;

            SphereCollider sc = bone.AddComponent<SphereCollider>();
            sc.radius = 0.001f;
        }

        foreach (GameObject b1 in rig)
        {
            foreach (GameObject b2 in rig)
            {
                if (b1 != b2)
                {
                    SpringJoint sj = b1.AddComponent<SpringJoint>();
                    sj.spring = spring;
                    sj.connectedBody = b2.GetComponent<Rigidbody>();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
