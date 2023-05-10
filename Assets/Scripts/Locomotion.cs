using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Locomotion : MonoBehaviour
{
    public bool is_detailed = true;
    public float force;
    public float speed;
    private SoftBody softBody;
    public RemoteLocomote RL;
    public float influence = 0.05f;
    public float acc = 2;
    private Vector3 ideal_vel = Vector3.zero;
    private Vector3 ideal_acc = Vector3.zero;
    public Vector3 cur_vel = Vector3.zero;
    private Vector3 cur_acc = Vector3.zero;
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
        MoveRot();
        if (is_detailed) {
            MoveDetailed(new Vector3(x, 0, z));
            ToShapeAcc();
        } else {
            Move(new Vector3(x, 0, z));
        }
        //ResetRotation();
    }
    void MoveRot()
    {
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
        //RL.targetPos = softBody.centerOfMass;
    }

    void Move(Vector3 dir)
    {
        Vector3 dir_center = softBody.centerOfMass - softBody.ctrl_root.transform.position;
        dir_center.y = 0;
        softBody.ctrl_root.transform.position += dir_center * Time.deltaTime * speed*0.7f;
        softBody.ctrl_up.transform.position += dir * Time.deltaTime * speed;
        RL.targetPos = softBody.centerOfMass;
        RL.speed = speed;
    }


    void MoveDetailed(Vector3 dir) {
        ideal_vel = dir.normalized * speed;
        if ((ideal_vel - cur_vel).magnitude < speed * 0.05f) {
            ideal_acc = Vector3.zero;
            cur_vel = ideal_vel;
        } else {
            ideal_acc = (ideal_vel - cur_vel).normalized * acc;
        }

        if (Vector3.Dot(ideal_acc, cur_vel) < 0) {
            ideal_acc *= 1.2f;
            cur_acc = Vector3.Lerp(cur_acc, ideal_acc, Time.deltaTime * 2);
        } else {
            cur_acc = Vector3.Lerp(cur_acc, ideal_acc, Time.deltaTime);
        }
        cur_vel += cur_acc * Time.deltaTime;

        softBody.ctrl_up.transform.position += cur_vel * Time.deltaTime*0.7f;
        //softBody.ctrl_root.transform.position += cur_vel * Time.deltaTime * speed;
    }
    
    void ToShapeAcc() {
        Vector3 relativePos = softBody.centerOfMass - cur_acc * influence;
        //Vector3 relativePos = softBody.ctrl_up.transform.position - cur_acc * influence;
        relativePos.y = softBody.ctrl_root.transform.position.y;
        Vector3 dir = relativePos - softBody.ctrl_root.transform.position;
        if (dir.magnitude >= 1f) dir = dir.normalized;
        softBody.ctrl_root.transform.position += dir * speed * Time.deltaTime;
        RL.targetPos = relativePos;
        RL.speed = speed;
    }

    /*
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(softBody.ctrl_root.transform.position, 1);
        Gizmos.DrawSphere(softBody.ctrl_up.transform.position, 1);
    }*/
}
