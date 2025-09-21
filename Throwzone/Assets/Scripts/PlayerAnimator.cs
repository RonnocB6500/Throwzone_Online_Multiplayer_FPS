using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{


    private const string IS_WALKING_FWD = "IsWalkingFWD";
    private const string IS_WALKING_BWD = "IsWalkingBWD";
    private const string IS_WALKING_LFT = "IsWalkingLFT";
    private const string IS_WALKING_RGT = "IsWalkingRGT";
    private const string IS_RUNNING = "IsRunning";
    private const string IS_JUMPING = "IsJumping";
    private const string THROW = "Throw";
    private const string DEATH = "Death";


    [SerializeField] private Player player;


    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        if (IsOwner)
        {
            ThrowObject.Instance.throwObject += ThrowObject_throwObject;
            //PlayerHealth.Instance.playerDeath += PlayerHealth_playerDeath; 
        }
    }

    private void ThrowObject_throwObject(object sender, EventArgs e)
    {
        if (IsOwner)
        {
            animator.SetTrigger(THROW);
        }
    }
    private void PlayerHealth_playerDeath(object sender, EventArgs e)
    {
        if (IsOwner)
        {
            animator.SetTrigger(DEATH);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        animator.SetBool(IS_WALKING_FWD, player.IsWalkingFWD());
        animator.SetBool(IS_WALKING_BWD, player.IsWalkingBWD());
        animator.SetBool(IS_WALKING_LFT, player.IsWalkingLFT());
        animator.SetBool(IS_WALKING_RGT, player.IsWalkingRGT());
        animator.SetBool(IS_RUNNING, player.IsRunning());
        animator.SetBool(IS_JUMPING, player.IsJumping());
        animator.SetBool(DEATH, player.gameObject.GetComponent<PlayerHealth>().dead);
    }
  

}