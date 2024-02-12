using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Movement : MonoBehaviour
{
    [SerializeField] float numFuerza = 5f;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigRb = hit.collider.attachedRigidbody;
        if (rigRb == null || rigRb.isKinematic)
        {
            return;
        }
        if (rigRb.gameObject.tag == "Box")
        {
            Vector3 ve3Direccion = new(hit.moveDirection.x, 0f, hit.moveDirection.z);
            rigRb.velocity = ve3Direccion * numFuerza / rigRb.mass;
        }
    }
}