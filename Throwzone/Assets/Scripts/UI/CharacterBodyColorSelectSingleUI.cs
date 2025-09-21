using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBodyColorSelectSingleUI : MonoBehaviour {


    [SerializeField] private int bodyColorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;


    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            //Debug.Log("BodyButton");
            ThrowGameMultiplayer.Instance.ChangePlayerBodyColor(bodyColorId);
        });
    }

    private void Start() {
        ThrowGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        image.color = ThrowGameMultiplayer.Instance.GetPlayerBodyColor(bodyColorId);
        UpdateIsSelected();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        UpdateIsSelected();
    }

    private void UpdateIsSelected() {
        if (ThrowGameMultiplayer.Instance.GetPlayerData().bodyColorId == bodyColorId) {
            selectedGameObject.SetActive(true);
        } else {
            selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy() {
        ThrowGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}