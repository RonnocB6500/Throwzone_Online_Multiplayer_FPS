using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {


    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;


    private void Awake() {
        kickButton.onClick.AddListener(() => {
            PlayerData playerData = ThrowGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            ThrowGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            ThrowGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start() {
        ThrowGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (ThrowGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = ThrowGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

            playerNameText.text = playerData.playerName.ToString();

            playerVisual.ActiveCloakOption(playerData.cloakOptionId);
            playerVisual.ActiveBodyOption(playerData.bodyOptionId);
            playerVisual.ActiveHairOption(playerData.hairOptionId);
            playerVisual.ActiveHeadOption(playerData.headOptionId);
            playerVisual.ActiveEyesOption(playerData.eyesOptionId);
            playerVisual.ActiveMouthOption(playerData.mouthOptionId);
            playerVisual.SetPlayerCloakColor(ThrowGameMultiplayer.Instance.GetPlayerCloakColor(playerData.cloakColorId));
            playerVisual.SetPlayerBodyColor(ThrowGameMultiplayer.Instance.GetPlayerBodyColor(playerData.bodyColorId));
            playerVisual.SetPlayerHairColor(ThrowGameMultiplayer.Instance.GetPlayerHairColor(playerData.hairColorId));
        }
        else
        {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        ThrowGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }


}