using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class timeReward : MonoBehaviour
{

    public int timeRewardLimit = 4;
    public GameObject counter;

    public Material yellow;
    public Material orange;
    public Material red;

    public ParticleSystem particle1;
    public ParticleSystem particle2;

    private void OnEnable()
    {
        particle1.Play();
        particle2.Play();
        GetComponent<Collider>().enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the tag of the colliding entity is a player
        if (other.gameObject.tag == "Player")
        {
            PlayerController PC = other.gameObject.GetComponent<PlayerController>();

            PC.ChangePoints(5);

            deactivate();
        } 
    }

    

    public void setTime(int time){
        counter.GetComponent<TextMeshPro>().text = time.ToString();

        if (time < 4){
            if (time == 3){
                counter.GetComponent<TextMeshPro>().color = yellow.color;
            }
            else if (time == 2){
                counter.GetComponent<TextMeshPro>().color = orange.color;
            }
            else{
                counter.GetComponent<TextMeshPro>().color = red.color;
            }
        }
        else{
            counter.GetComponent<TextMeshPro>().color = Color.green;
        }
        
    }

    public void deactivate(){
        StartCoroutine(TimeRewardDeactivate());
    }

    IEnumerator TimeRewardDeactivate()
    {
        counter.GetComponent<TextMeshPro>().text = "";

        particle1.Play();
        particle2.Play();

        yield return new WaitForSeconds(1f);

        gameObject.transform.parent = null;
        gameObject.transform.Translate(new Vector3(0, -50f, 0));

        this.enabled = false;
    }
}
