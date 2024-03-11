using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ene_Patrol : MonoBehaviour
{
    public Transform[] points;
    public float speed;
    public float rotationSpeed = 2f; // Velocidad de rotación
    private Animator animator; // Referencia al Animator

    private int current;

    void Start()
    {
        current = 0;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }        
    }

    void FixedUpdate()
    {
        if (transform.position != points[current].position)
        {
            Vector3 direction = points[current].position - transform.position;

            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, points[current].position, speed * Time.deltaTime);
           
            // Asigna los valores al Blend Tree (CORREGIR DESPUES, ACTUALMENTE SOLO VA HACIA DELANTE Y NO EJECUTA LAS DEMAS ANIMACIONES)
            animator.SetFloat("y", 1);
            animator.SetFloat("x", 0);

            animator.SetBool("isMoving", true); // Asume que tienes un parámetro de animación "isMoving"            
        }
        else
        {
            current = (current + 1) % points.Length;           
        }
    }    
}
