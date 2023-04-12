using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone 
{
    public GameObject root; //limb root
    public GameObject bone;

    public Bone(GameObject b, GameObject r)
    {
        bone = b;
        root = r;
    }
}
