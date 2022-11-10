using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public GameObject capsuleMesh;

    void OnDrawGizmos()
    {
        RaycastHit hit;
        float rayLength = 20.0f;

        
        Vector3 pos = this.transform.position;
        Vector3 cubePos = pos ;

        Vector3 boxSize = new Vector3(1, 1, 1);
        if (Physics.BoxCast(cubePos, boxSize / 2, Vector3.up, out hit, Quaternion.identity, rayLength))
            print("UP" + (hit.point - transform.position));
        if (Physics.BoxCast(cubePos, boxSize / 2, Vector3.down, out hit, Quaternion.identity, rayLength))
            print("Down" + (hit.point - transform.position));
        if (Physics.BoxCast(cubePos, boxSize / 2, Vector3.left, out hit, Quaternion.identity, rayLength))
            print("Left" + (hit.point - transform.position));
        if (Physics.BoxCast(cubePos, boxSize / 2, Vector3.right, out hit, Quaternion.identity, rayLength))
            print("Right" + (hit.point - transform.position));
    }
}
