using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picktable : Interactable // Herada de Interactable
{
    public override void Interact() //Sobreescribir Interact, llama al padre
    {
        base.Interact();
        Destroy(gameObject);
        Debug.Log("Conseguiste la " + transform.name);
    }
}
