using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {


    private Player player;
    private float footstepTimer;
    private float footstepTimerMax = .1f;


    private void Awake() {
        player = GetComponent<Player>();
    }

    private void Update() {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f) {
            footstepTimer = footstepTimerMax;

            if (player.IsWalkingFWD() || player.IsWalkingBWD() || player.IsWalkingLFT() || player.IsWalkingRGT() || player.IsRunning()) {
                float volume = 1f;
                SoundManager.Instance.PlayFootstepsSound(player.transform.position, volume);
            }
        }
    }
}