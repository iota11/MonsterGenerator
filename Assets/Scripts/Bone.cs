using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone 
{
    public GameObject root; //limb root
    public GameObject bone;
    public Vector3 positionOffset;
    public float offsetRadius;

    public Bone(GameObject b, GameObject r, Vector3 rootPos)
    {
        bone = b;
        root = r;
        positionOffset = bone.transform.position - rootPos;
        offsetRadius = Random.Range(0.01f, 0.25f);
    }
}
