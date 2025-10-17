using System.ComponentModel;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Health : MonoBehaviour
{
    // Life Points
    public int health = 200;
    public int maxHealth = 0;

    // Health Slider and Text
    public Slider healthBar;
    public TMP_Text healthText;
    

    private void Start()
    {

        maxHealth = health;
       
        
    }

   
    void Update()
    {
        healthText.text = health + " / " + maxHealth;
        healthBar.value = (float)health / (float)maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            health = health - 25;

            if(health <= 0)
            {
                EnemyDeath();
                PlayerDeath(0);
            }
        }
    }

   
    public void EnemyDeath()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }

    }
    
    public void PlayerDeath(int sceneindex)
    {
        if (gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(0);

            Time.timeScale = 1f; 

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

       

    
}
