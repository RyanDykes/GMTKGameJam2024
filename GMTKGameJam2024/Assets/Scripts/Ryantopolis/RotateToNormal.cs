using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToNormal : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        Debug.DrawRay(transform.position, -transform.up * 2f);
        if (Physics.Raycast(ray, out hit, 2f, groundLayer))
        {
            Debug.DrawRay(hit.point, hit.normal * 2f);
            Vector3 groundNormal = hit.normal;
            Quaternion toGroundRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            transform.rotation = toGroundRotation;
        }
    }
}
