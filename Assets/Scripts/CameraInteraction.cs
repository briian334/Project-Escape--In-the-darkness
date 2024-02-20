using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private new Transform camera;
    public float rayDistance;

    // Start is called before the first frame update
    void Start()
    {
        camera = transform.Find("PlayerCamera");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(camera.position, camera.forward * rayDistance, Color.red);


        if (Input.GetButtonDown("Interactable"))
        {
            RaycastHit hit;//Información del objeto al que estamos mirando

            if (Physics.Raycast(camera.position, camera.forward, out hit, rayDistance, LayerMask.GetMask("Interactable")))
            {
                //Debug.Log(hit.transform.name);
                hit.transform.GetComponent<Interactable>().Interact();

            }
        }

    }
}
