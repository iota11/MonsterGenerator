using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone 
{
    public GameObject root; //limb root
    public GameObject bone;
    public Vector3 positionOffset;
    public Quaternion origRotation;
    public float offsetRadius;

    public Bone(GameObject b, GameObject r, Vector3 rootPos, Quaternion origRot)
    {
        bone = b;
        root = r;
        positionOffset = bone.transform.position - rootPos;
        origRotation = origRot;
        //offsetRadius = Random.Range(0.01f, 0.25f);
        offsetRadius = 0.1f;
    }
}
