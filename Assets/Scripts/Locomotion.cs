using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Locomotion : MonoBehaviour
{
    public float force;
    public float speed;
    private SoftBody softBody;
    public RemoteLocomote RL;
    // Start is called before the first frame update
    void Start()
    {
        softBody = GetComponent<SoftBody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        MoveRot(new Vector3(z, 0, -x));
        Move(new Vector3(x, 0, z));
        //ResetRotation();
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
        }
        transform.Rotate(0, 0, -1 * Input.GetAxis("Horizontal") * Time.deltaTime * force);
        transform.Rotate(Input.GetAxis("Vertical") * Time.deltaTime * force, 0, 0);
        RL.targetPos = softBody.centerOfMass;*/

        //control rotation

        float dr = 0;
        if (Input.GetKey(KeyCode.E)) {
            dr = 1;
        }
        if (Input.GetKey(KeyCode.Q)) {
            dr = -1;
        }
        softBody.ctrl_up.GetComponent<Transform>().Rotate(transform.up, Time.deltaTime * 30 * 2 * dr);
        softBody.ctrl_root.GetComponent<Transform>().Rotate(transform.up, Time.deltaTime * 30 * 2 * dr);
        RL.targetRotation += Time.deltaTime * 30 * 2 * dr;
        RL.targetPos = softBody.centerOfMass;
    }

    void Move(Vector3 dir)
    {/*
        foreach (Transform limb in transform)
        {
       
                if (limb.position.y > 5)
                {
                    var force = dir * (limb.childCount >= 1 ? 0.25f : 2f)*0.1f;
                    limb.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                }
            
        }*/

        softBody.ctrl_forward.transform.position += dir * Time.deltaTime*speed;
        Vector3 dir_center = softBody.centerOfMass - softBody.ctrl_root.transform.position;
        dir_center.y = 0;
        softBody.ctrl_root.transform.position += dir_center * Time.deltaTime * speed;
        softBody.ctrl_up.transform.position += dir * Time.deltaTime * speed;
    }

}
