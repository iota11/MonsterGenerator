using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class SoftBody : MonoBehaviour
{
    private List<GameObject> rig = new List<GameObject>();
    private List<Bone> bones = new List<Bone>();
    public float mass;
    public float drag;
    public float angularDrag;

    public float damper;
    public float spring;

    public float uprightTorque;
    public float uprightForce;

    public Vector3 centerOfMass;

    private float bodyRadius = 1.0f;

    private float massRadius = 0.0075f;
    private float nborRadius = 1.0f;
    private float bendingRadius = 2.0f;

    private void Awake()
    {
        foreach (Transform limb in transform)
        {
            IndexLimb(limb.gameObject);
        }

    }

    void IndexLimb(GameObject limb)
    {
        var rootPosition = transform.position;
        foreach (Transform bone in limb.GetComponentsInChildren<Transform>())
        {
            var parentPos = bone.parent.position;
            Bone b = new Bone(bone.gameObject, limb, rootPosition);
            bones.Add(b);
        }
    }

    void ConnectBonesSimple()
    {
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

    void ConnectBones()
    {
        //neighboring springs
        foreach (Bone b1 in bones) {
            foreach (Bone b2 in bones)
            {
                var distance = Vector3.Distance(b1.bone.transform.position, b2.bone.transform.position);
                if (b1 != b2 && distance < nborRadius)
                {
                    SpringJoint sj = b1.bone.AddComponent<SpringJoint>();
                    sj.spring = spring;
                    sj.damper = damper;
                    sj.connectedBody = b2.bone.GetComponent<Rigidbody>();
                }
            }
        }

        //bending springs
        foreach (Bone b1 in bones)
        {
            foreach (Bone b2 in bones)
            {
                var distance = Vector3.Distance(b1.bone.transform.position, b2.bone.transform.position);
                if (b1 != b2 && distance > nborRadius && distance < bendingRadius)
                {
                    SpringJoint sj = b1.bone.AddComponent<SpringJoint>();
                    sj.spring = spring;
                    sj.damper = damper;
                    sj.connectedBody = b2.bone.GetComponent<Rigidbody>();
                }
            }
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
            //rb.constraints = RigidbodyConstraints.FreezePositionY;

            SphereCollider sc = bone.bone.AddComponent<SphereCollider>();
            sc.radius = massRadius;
        }

        ConnectBones();
    }

    void KeepUprightTorque()
    {
        foreach (Bone bone in bones)
        {
            var rb = bone.bone.GetComponent<Rigidbody>();
            var springTorque = uprightTorque * Vector3.Cross(rb.transform.up, Vector3.up);
            var dampTorque = -rb.angularVelocity;
            rb.AddTorque(springTorque + dampTorque, ForceMode.Acceleration);

            var desiredPos = transform.position + bone.positionOffset;
            var currentDistance = Vector3.Distance(bone.bone.transform.position, desiredPos);
            if (currentDistance > bone.offsetRadius)
            {
                var desiredDir = desiredPos - bone.bone.transform.position;
                rb.AddForce(desiredDir * uprightForce);
            }
        }
    }

    void KeepUprightFreeze()
    {
        foreach (Bone bone in bones)
        {
            bone.bone.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void KeepUpright()
    {
        var rootPos = transform.position;
        foreach (Bone bone in bones)
        {
            //var desiredPos = bone.bone.transform.parent.position + bone.positionOffset;
            var desiredPos = rootPos + bone.positionOffset;
            var currentDistance = Vector3.Distance(bone.bone.transform.position, desiredPos);
            if (currentDistance > bone.offsetRadius)
            {
                var desiredDir = desiredPos - bone.bone.transform.position;
                bone.bone.GetComponent<Rigidbody>().AddForce(desiredDir * uprightForce);
                //bone.bone.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

        }
    }

    Vector3 CalculateCenterOfMass()
    {
        Vector3 sum = Vector3.zero;
        var count = 0;
        foreach (Bone bone in bones)
        {
            var rb = bone.bone.GetComponent<Rigidbody>();
            sum += bone.bone.transform.position * rb.mass;
            count++;
        }
        return sum / count;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(centerOfMass, 1);
    }

    // Update is called once per frame
    void Update()
    {
        KeepUprightTorque();
        centerOfMass = CalculateCenterOfMass();
    }
}
