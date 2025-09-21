using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCloakColorSelectSingleUI : MonoBehaviour {


    [SerializeField] private int cloakColorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;


    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            //Debug.Log("CloakButton");
            ThrowGameMultiplayer.Instance.ChangePlayerCloakColor(cloakColorId);
        });
    }

    private void Start() {
        ThrowGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        image.color = ThrowGameMultiplayer.Instance.GetPlayerCloakColor(cloakColorId);
        UpdateIsSelected();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        UpdateIsSelected();
    }

    private void UpdateIsSelected() {
        if (ThrowGameMultiplayer.Instance.GetPlayerData().cloakColorId == cloakColorId) {
            selectedGameObject.SetActive(true);
        } else {
            selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy() {
        ThrowGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}