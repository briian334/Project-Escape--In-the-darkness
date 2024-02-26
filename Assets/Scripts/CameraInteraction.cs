using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraInteraction : MonoBehaviour
{
    LayerMask mask;
    float distancia = 1.5f;

    //public float rayDistance;
    GameObject ultimoReconocido = null;
    public Texture2D puntero;
    public GameObject TextDetect;
    public GameObject TextDetectDoor; // Canvas para puertas


    // Start is called before the first frame update
    void Start()
    {
        //camera = transform.Find("Camera");
        TextDetect.SetActive(false);
        TextDetectDoor.SetActive(false);
        mask = LayerMask.GetMask("Interactable");
    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia, mask))
        {
            Deselect();
            SelectedObject(hit.transform);
            if (hit.collider.tag == "ObjetoInteractivo")
            {

                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.transform.GetComponent<ObjetoInteractivo>().ActivarObjeto();

                }
                TextDetect.SetActive(true); // Activa el canvas genérico
                TextDetectDoor.SetActive(false); // Asegúrate de desactivar el canvas de puertas
            }


            if (hit.collider.tag == "Locker")
            {

                if (Input.GetMouseButtonDown(0))
                {
                    hit.collider.transform.GetComponent<Mov_HideLockers>();

                }

                TextDetect.SetActive(false); // Desactiva el canvas genérico
                TextDetectDoor.SetActive(true); // Activa el canvas específico para puertas
            }


            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * distancia, Color.red);
        }


        else
        {
            Deselect();
        }


    }



    //Cambiar color de objecto
    void SelectedObject(Transform trasform)
    {
        trasform.GetComponent<MeshRenderer>().material.color = Color.red;
        ultimoReconocido = trasform.gameObject;
    }

    //Volver a color default
    private void Deselect()
    {
        if (ultimoReconocido)
        {
            ultimoReconocido.GetComponent<Renderer>().material.color = Color.white;
            ultimoReconocido = null;
            TextDetect.SetActive(false); // Desactiva el canvas genérico
            TextDetectDoor.SetActive(false); // Desactiva el canvas de puertas
        }
    }

    //Puntero
    private void OnGUI()
    {
        Rect rect = new Rect(Screen.width / 2 - puntero.width / 2, Screen.height / 2 - puntero.height / 2, puntero.width, puntero.height);
        GUI.DrawTexture(rect, puntero);

        // Si hay un objeto reconocido, determinar qué texto mostrar
        if (ultimoReconocido)
        {
            // Si el objeto reconocido es una puerta, activar el texto de la puerta
            if (ultimoReconocido.CompareTag("Locker"))
            {
                TextDetect.SetActive(false);
                TextDetectDoor.SetActive(true);
            }
            // Si no es una puerta, asumir que es un objeto interactuable genérico
            else
            {
                TextDetect.SetActive(true);
                TextDetectDoor.SetActive(false);
            }
        }
        else // Si no hay objeto reconocido, asegurarse de que ambos textos estén desactivados
        {
            TextDetect.SetActive(false);
            TextDetectDoor.SetActive(false);
        }
    }
}
