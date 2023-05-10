using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static Unity.VisualScripting.Member;

public class SoftBody : MonoBehaviour
{
    private List<GameObject> rig = new List<GameObject>();
    private List<Bone> bones = new List<Bone>();
    private List<Bone> ctrl_bones = new List<Bone>();
    public GameObject ctrl_up;
    public GameObject ctrl_forward;
    public GameObject ctrl_root;
    public bool showParticles = false;
    public float mass;
    public float drag;
    public float angularDrag;

    public float damper;
    public float spring;

    public float uprightTorque;
    public float uprightForce;

    public Vector3 centerOfMass;

    private float bodyRadius = 1.0f;

    private float massRadius = 0.01f;
    public float nborRadius = 2;
    private float bendingRadius = 2f;

    private void Awake()
    {
       // Rigidbody rb = transform.AddComponent<Rigidbody>();
       // rb.useGravity = true;
        //rb.constraints = RigidbodyConstraints.FreezeRotationY;

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
            Bone b = new Bone(bone.gameObject, limb, rootPosition, bone.rotation);
            bones.Add(b);  
            if(bone.gameObject.transform.position.y > 4 && (UnityEngine.Random.value > 0.7)) {
                ctrl_bones.Add(b);
            }
        }
    }
    /*
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
    */
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

            //rb.constraints = RigidbodyConstraints.FreezeRotation;

            SphereCollider sc = bone.bone.AddComponent<SphereCollider>();
            sc.radius = massRadius;
        }
         
        ConnectBones();
        ctrl_up.GetComponent<Rigidbody>().isKinematic = true;
        ctrl_root.GetComponent<Rigidbody>().isKinematic = true;
        //ctrl_forward.GetComponent<Rigidbody>().isKinematic = true;

    }

    void KeepUprightTorque()
    {
        foreach (Bone bone in bones)
        {
            //bone.bone.transform.rotation = Quaternion.RotateTowards(transform.rotation, bone.origRotation, uprightTorque);

            var rb = bone.bone.GetComponent<Rigidbody>();
            /*var springTorque = uprightTorque * Vector3.Cross(rb.transform.up, Vector3.up);
            var dampTorque = -rb.angularVelocity;
            rb.AddTorque(springTorque + dampTorque, ForceMode.Acceleration);*/

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
        if (showParticles) {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(centerOfMass + new Vector3(0, 0, -3), 1);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(ctrl_root.transform.position + new Vector3(0, 0, -3), 1);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       // keepUpright();
         centerOfMass = CalculateCenterOfMass();
    }
    void keepUpright() {
        foreach(Bone b in ctrl_bones) {
            if(b.bone.transform.position.y < 2f) {
                b.bone.GetComponent<Rigidbody>().AddForce(new Vector3(0, 10f, 0), ForceMode.Impulse);
            }
        }
    }
}
