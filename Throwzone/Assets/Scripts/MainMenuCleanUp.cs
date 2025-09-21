using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour {


    private void Awake() {
        if (NetworkManager.Singleton != null) {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (ThrowGameMultiplayer.Instance != null) {
            Destroy(ThrowGameMultiplayer.Instance.gameObject);
        }

        if (ThrowGameLobby.Instance != null) {
            Destroy(ThrowGameLobby.Instance.gameObject);
        }
    }

}