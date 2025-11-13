using UnityEngine;
using System.Collections;
public class ActivatingSprintZone : Detection
{

    [SerializeField] GameObject shieldbarrier;
    
    Animator movingWallLeft;
    Animator movingWallRight;
    Animator laserLeft;
    Animator laserRight;

    void Start()
    {
        movingWallLeft = GetComponent<Animator>();
        movingWallRight = GetComponent<Animator>();
        laserLeft = GetComponent<Animator>();
        laserRight = GetComponent<Animator>();
    }
    void ShieldOverride()
    {
        if (shieldbarrier != null)
        {
           shieldbarrier.SetActive(false); 
        }

    }

    void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        
        if (other.gameObject.CompareTag("Player"))
        {
            PlayMovingWallRightAnimation();
            PlayMovingWallRightAnimation();
            PlayLaserLeftAniamtion();
            PlayLaserRightAnimation();
        }

       
    }

    void PlayMovingWallLeftAnimation()
        {
            if(movingWallLeft != null)
            {
                movingWallLeft.Play("Moving Wall Left");
            }
        }
        
         void PlayMovingWallRightAnimation()
        {
            if(movingWallRight != null)
            {
                movingWallRight.Play("Moving Wall Right");
            }
        }

         void PlayLaserLeftAniamtion()
        {
            if(laserLeft != null)
            {
                laserLeft.Play("laser Left");
            }
        }

         void PlayLaserRightAnimation()
        {
            if(laserRight != null)
            {
                laserRight.Play("laser Right");
            }
        }




}
