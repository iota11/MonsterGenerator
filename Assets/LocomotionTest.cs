using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class LocomotionTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CG;
    public GameObject root;
    public float influence = 0f;
    public float speed;
    public float acc = 2;
    public Vector3 ideal_vel = Vector3.zero;
    public Vector3 ideal_acc = Vector3.zero;
    public Vector3 cur_vel = Vector3.zero;
    public Vector3 cur_acc = Vector3.zero;

    // Update is called once per frame
    void FixedUpdate() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Move(new Vector3(x, 0, z));
        ToShapeAcc();
    }


    void Move(Vector3 dir) {
        ideal_vel = dir.normalized * speed;
        if ((ideal_vel - cur_vel).magnitude < speed * 0.05f) {
            ideal_acc = Vector3.zero;
            cur_vel = ideal_vel;
        } else {
            ideal_acc = (ideal_vel - cur_vel).normalized * acc;
        }
        
        if (Vector3.Dot(ideal_acc, cur_vel) < 0) {
            ideal_acc *= 1.2f;
            cur_acc = Vector3.Lerp(cur_acc, ideal_acc, Time.deltaTime*2);
        } else {
            cur_acc = Vector3.Lerp(cur_acc, ideal_acc, Time.deltaTime);
        }
        //cur_acc = ideal_acc;
        //cur_acc = (CG.transform.position - root.transform.position) / influence;
        //cur_acc.y = 0;
        cur_vel += cur_acc * Time.deltaTime;

        CG.transform.position += cur_vel * Time.deltaTime ;
        //root.transform.position += dir * Time.deltaTime * speed;

    }
    void ToShapeAcc() {
        Vector3 relativePos = CG.transform.position - cur_acc* influence;
        relativePos.y = root.transform.position.y;
        Vector3 dir = relativePos - root.transform.position;
        if (dir.magnitude >= 1f) dir = dir.normalized;
        root.transform.position += dir * speed * Time.deltaTime*1.5f ;
    }

    private void OnDrawGizmos() {
        Vector3 center = root.transform.position;
        Gizmos.color = Color.red;
        drawLine(center, center+cur_acc/3f);

        Gizmos.color = Color.cyan;
        center += new Vector3(0, 0.2f, 0);
        drawLine(center, center+ideal_acc/3f);


    }
    void drawLine(Vector3 start, Vector3 end, float width = 0.3f) {
        Vector3 tmpS;
        Vector3 tmpE;
        Vector3 dir = (end - start).normalized;
        Vector3 right = Vector3.Cross(new Vector3(0, 1, 0), dir).normalized;
        for(int i= 0; i < 20; i++) {
            tmpS = start + right * width / 2 - right * width * i/20f;
            tmpE = end;
            Gizmos.DrawLine(tmpE, tmpS);
        }
    }
}
