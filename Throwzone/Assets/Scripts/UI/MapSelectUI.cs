using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectUI : MonoBehaviour
{
    [SerializeField] private Button forestMapButton;
    [SerializeField] private Button townMapButton;
    [SerializeField] private Button kitchenMapButton;
    


    private void Awake() {
        gameObject.SetActive(true);
        
        forestMapButton.onClick.AddListener(() =>
        {
            Loader.LoadNetwork(Loader.Scene.FantasyForest);
        });
        townMapButton.onClick.AddListener(() =>
        {
            Loader.LoadNetwork(Loader.Scene.TinyTown);
        });
        /*kitchenMapButton.onClick.AddListener(() =>
        {
            Loader.LoadNetwork(Loader.Scene.MainMenuScene);
        });*/
    }
}
