using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoteLocomote : MonoBehaviour
{
    // Start is called before the first frame update
    private ProceduralWalk pw;
    public float x;
    public float z;
    public float speed = 3f;
    public Vector3 targetPos;
    public float targetRotation;
    private Transform m_transform;

    public float rotate;
    void Start()
    {
        m_transform = this.GetComponent<Transform>();   
        pw = GetComponent<ProceduralWalk>();
        pw.directControl = false;
    }

    // Update is called once per frame
    void Update()
    {
        ScheduleLocomote();
    }

    void ScheduleLocomote() {
        Vector3 gap = targetPos - transform.position;
        z = Vector3.Dot(gap, m_transform.forward) ;
        x = Vector3.Dot(gap, m_transform.right);

        z = Mathf.Clamp(z,-1, 1);
        x = Mathf.Clamp(x,-1, 1);
        /*
        if (Mathf.Abs((targetRotation%180 + 360) - m_transform.rotation.eulerAngles.y) > 30f) {
            if (targetRotation > m_transform.rotation.eulerAngles.y) {
                rotate = 1;
            } else {
                rotate = -1;
            }
        } else {
            rotate = 0;
        }*/
    }
}
