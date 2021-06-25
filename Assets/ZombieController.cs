using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public List<ZombieCharacterControl> zombieList = new List<ZombieCharacterControl>();
    GameObject zombie;

    private int maxZombie = 4;
    public Transform spawnPoint;

    // Update is called once per frame
    void Update()
    {
        foreach(var zombie in zombieList)
        {
            if (!zombie.alive){
                zombieList.Remove(zombie);
            }
        }
    }

    public void summonZombie(){
        if (zombieList.Count < maxZombie){
            print("create Zombie");
            var zombie_prefab = Resources.Load("Zombie");
            zombie = (GameObject)Instantiate(zombie_prefab, spawnPoint.transform.position, Quaternion.identity);
            zombie.tag = "Zombie";

            zombieList.Add(zombie.GetComponent<ZombieCharacterControl>());
        }
        else{
            print("Max zombies reached");
        }
    }
}
