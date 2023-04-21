using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoftBody : MonoBehaviour
{
    private List<GameObject> rig = new List<GameObject>();
    private List<Bone> bones = new List<Bone>();
    public float mass;
    public float drag;
    public float angularDrag;

    public float damper;
    public float spring;

    private float bodyRadius = 0.5f;

    private void Awake()
    {
        foreach (Transform limb in transform)
        {
            IndexLimb(limb.gameObject);
        }

    }

    void IndexLimb(GameObject limb)
    {
        foreach (Transform bone in limb.GetComponentsInChildren<Transform>())
        {
            Bone b = new Bone(bone.gameObject, limb);
            bones.Add(b);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Bone bone in bones)
        {
            Rigidbody rb;
            var tmp = bone.bone.GetComponent<Rigidbody>();
            if (tmp != null)
            {
                rb = tmp;
            } else
            {
                rb = bone.bone.AddComponent<Rigidbody>();
            }
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.constraints = RigidbodyConstraints.FreezeRotationY;

            SphereCollider sc = bone.bone.AddComponent<SphereCollider>();
            sc.radius = 0.001f;
        }

        foreach (Bone b1 in bones)
        {
            foreach (Bone b2 in bones)
            {
                bool sameParent = b1.root == b2.root;
                bool inBody = (Vector3.Distance(b1.bone.transform.position,
                    transform.position) < bodyRadius) || (Vector3.Distance(b2.bone.transform.position,
                    transform.position) < bodyRadius);
                if (b1 != b2 && (sameParent || inBody))
                {
                    SpringJoint sj = b1.bone.AddComponent<SpringJoint>();
                    sj.spring = spring;
                    sj.damper = damper;
                    sj.connectedBody = b2.bone.GetComponent<Rigidbody>();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
