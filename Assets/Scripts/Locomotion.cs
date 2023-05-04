using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Locomotion : MonoBehaviour
{
    public float force;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        MoveRot(new Vector3(z, 0, -x));
        Move(new Vector3(x, 0, z));
        ResetRotation();
    }

    void ResetRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * force * 0.05f);
    }

    void MoveTorque(Vector3 dir)
    {
        foreach (Transform limb in transform)
        {
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            rb.AddTorque(dir * force);
        }
    }

    void MoveRot(Vector3 dir)
    {
        /*foreach (Transform limb in transform)
        {
            Rigidbody rb = limb.GetComponent<Rigidbody>();
            rb.AddTorque(dir * force);
        }*/
        transform.Rotate(0, 0, -1 * Input.GetAxis("Horizontal") * Time.deltaTime * force);
        transform.Rotate(Input.GetAxis("Vertical") * Time.deltaTime * force, 0, 0);
    }

    void Move(Vector3 dir)
    {
        foreach (Transform limb in transform)
        {
            foreach (Transform bone in limb.GetComponentsInChildren<Transform>())
            {
                if (bone.childCount < 2 && Random.value >= 0.5)
                {
                    var force = dir * (bone.childCount >= 1 ? 0.25f : 2f);
                    bone.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                }
            }
        }
    }
}
