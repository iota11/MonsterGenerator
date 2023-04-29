using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    public float force;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Move(new Vector3(x, 0, z));
    }

    void Move(Vector3 dir)
    {
        foreach (Transform limb in transform)
        {
            foreach (Transform bone in limb.GetComponentsInChildren<Transform>())
            {
                if (bone.childCount == 0 && Random.value >= 0.5)
                {
                    bone.gameObject.GetComponent<Rigidbody>().AddForce(dir * force * Random.Range(0.25f, 0.75f));
                }
            }
        }
    }
}
