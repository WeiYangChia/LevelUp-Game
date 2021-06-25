using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ZombieCharacterControl : MonoBehaviour
{

    [SerializeField] private float m_moveSpeed = 5;
    [SerializeField] private float m_turnSpeed = 500;

    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidBody = null;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;

    private Vector3 m_currentDirection = Vector3.zero;

    public GameObject player = null;

    public bool alive;
    public bool justAte;

    Vector3 startPoint;

    private void Awake()
    {
        if (!m_animator) { gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }

        startPoint = transform.position;
        alive = true;
    }

    private void FixedUpdate()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null && !player.GetComponent<PlayerController>().respawning){
            DirectUpdate();
        }

        if (transform.position.y < startPoint.y - 3){
            zombieDie();
        }
    }

    private void DirectUpdate()
    {
        Vector3 direction = (player.transform.position - transform.position);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction.normalized);
            transform.position += direction.normalized * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }
    }

    private void zombieDie(){
        alive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Conditions to ensure the right player is activating the block
        if (other.gameObject.tag == "Player" && player.GetComponent<PlayerController>().moveable && !justAte)
        {
            player.GetComponent<PlayerController>().getEaten();
            justAte = true;
            StartCoroutine("eatCooldown",5);
            
        }
    }

    IEnumerator eatCooldown(int counter)
    {
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);

            counter--;

            if (counter == 0){
                justAte = false;
            }
        }
    }


}
