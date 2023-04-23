using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public enum LegState
{
    move,
    stall
}
public class ProceduralLeg: MonoBehaviour
{
    public Transform m_transform;
    public ProceduralWalk body;
    private Transform body_transform;
    public Transform end_transform;
    public float strideRatio = 1.0f;
    public float legLength = 1f;
    public float legStride = 0.8f;
    public LegState m_state = LegState.stall;
    public float lerp = 0.0f;
    public Vector3 curPos;
    public Vector3 oldPos;
    public Vector3 newPos;
    public float heightScale = 1.0f;
    public float moveTime;
    private Quaternion initial_rotation;
    private Vector3 pos_offset;
    public float selfTimer = 0.0f;
    public bool istriggered = false;
    private Vector3 bodyToRootOffset;

    public float VirtualHeight = 0.0f; //to messure the strength
    public float virtualHeightOffset = 0.0f;
    public float virtualHeightScale = 1.0f;
    //constructor
    public ProceduralLeg(Transform legTransform, float footHeightScale, float newlegLength)
    {
        m_transform = legTransform;
        legLength = newlegLength;
        curPos = m_transform.position;
        oldPos = m_transform.position;
        newPos = m_transform.position;
        heightScale = footHeightScale;
        m_state = LegState.stall;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPos, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(oldPos, 0.05f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(curPos, 0.02f);
    }

    private void Start()
    {
        body_transform = body.m_transform;
        m_transform = this.GetComponent<Transform>();
        curPos = m_transform.position;
        oldPos = m_transform.position;
        initial_rotation = m_transform.localRotation;
        pos_offset = m_transform.position - body_transform.position;
        legStride =2* strideRatio * Mathf.Sqrt(Mathf.Pow(legLength, 2) - Mathf.Pow(end_transform.position.y, 2)); // may use ray cast insead
    }

    public void Update()
    {
        //m_transform.position = body_transform.position +body_transform.rotation*pos_offset;
        m_transform.localRotation = body_transform.localRotation * initial_rotation;
        if (istriggered)
        {
            selfTimer += Time.deltaTime;
            VirtualHeight = Mathf.Sin((2*Mathf.PI)/(body.period)* selfTimer) * virtualHeightScale;
        }
    }

   public void CheckTimer( float period)
    {
        if(selfTimer > period)
        {
            istriggered = false;
            selfTimer = 0.0f;
        }
    }

    public Vector3 IdealLocation(Vector3 bodyPredict)
    {
        Vector3 idealPlace = bodyPredict  + body_transform.rotation * pos_offset;
        Vector3 test = curPos;
        Vector3 rayStartpoint =idealPlace + new Vector3(0f, 10f, 0f);
        int layerMask = 1 << 6;
        RaycastHit hit;
        if (Physics.Raycast(rayStartpoint, new Vector3(0f, -1f, 0f), out hit, 20f, layerMask))
        {
            return hit.point;
        }
        else {
            return idealPlace;
        }
    }

    public bool isStall()
    {
        return m_state == LegState.stall;
    }

    public void MoveLeg(float deltaTime)
    {
        if ((lerp < 1) && m_state == LegState.move)
        {
            Vector3 footpos = Vector3.Lerp(oldPos, newPos, lerp);
            footpos.y += footHeight(lerp) * heightScale;
            curPos = footpos;
            m_transform.position = curPos;
            lerp += deltaTime/moveTime;
        }
        if (lerp >= 1.0f) //reset trigger
        {
            oldPos = newPos;
            m_state = LegState.stall;
        }
    }

    public void updateNewPos(Vector3 bodyPredict)
    {
        newPos = bodyPredict + body_transform.rotation * pos_offset;
        int layerMask = 1 << 6;
        RaycastHit hit;
        Vector3 rayStartpoint = newPos + new Vector3(0f, 10f, 0f);
        if (Physics.Raycast(rayStartpoint, new Vector3(0f, -1f, 0f), out hit, 20f, layerMask))
        {
            newPos = hit.point;
        }
    }

    public void setDestination(Vector3 newDes)
    {
        oldPos = curPos;
        newPos = newDes;
        m_state = LegState.move;
        lerp = 0.0f;
        float distance = Vector3.Distance(curPos, newDes);
        istriggered = true;
        selfTimer = 0.0f;
    }

    private float footHeight(float x)
    {
        x = Mathf.Clamp(x, 0f, 1f);
        return Mathf.Pow((1 - x), 3) * x;
    }

    private float footHeight2(float x)
    {
        x = Mathf.Clamp(x, 0f, 1f);
        return Mathf.Sin(x * Mathf.PI);
    }

    private float footHeightCircle(float x)
    {
        return Mathf.Sin(lerp * Mathf.PI) * 0.3f;
    }

}