using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : NetworkBehaviour {


    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private Button playAgainButton;


    private void Awake() {
        playAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start() {
        ThrowGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (ThrowGameManager.Instance.IsGameOver()) {
            recipesDeliveredText.text = "Player 1 Kills:" + ThrowGameMultiplayer.Instance.playerKills[0].ToString() + "\nPlayer 2 Kills: " + ThrowGameMultiplayer.Instance.playerKills[1].ToString();
            Show();

        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
        playAgainButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}