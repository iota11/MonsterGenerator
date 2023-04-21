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
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Move(Vector3.left);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Move(Vector3.right);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Move(Vector3.up);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Move(Vector3.down);
        }
    }

    void Move(Vector3 dir)
    {
        foreach (Transform limb in transform)
        {
            limb.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
    }
}
