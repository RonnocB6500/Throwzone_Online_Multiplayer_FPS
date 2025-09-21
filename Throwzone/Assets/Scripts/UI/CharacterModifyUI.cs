using UnityEngine;
using UnityEngine.UI;

public class CharacterModifyUI : MonoBehaviour
{
    [Header("Cloak")]
    [SerializeField] private Button CloakMenuButton;
    [SerializeField] private GameObject CloakMenu;
    [SerializeField] private Button CloakOption1;
    [SerializeField] private Button CloakOption2;
    [SerializeField] private GameObject CloakColorMenu;



    [Header("Body")]
    [SerializeField] private Button BodyMenuButton;
    [SerializeField] private GameObject BodyMenu;
    [SerializeField] private Button BodyOption1;
    [SerializeField] private Button BodyOption2;
    [SerializeField] private GameObject BodyColorMenu;



    [Header("Hair")]
    [SerializeField] private Button HairMenuButton;
    [SerializeField] private GameObject HairMenu;
    [SerializeField] private Button HairOption1;
    [SerializeField] private Button HairOption2;
    [SerializeField] private GameObject HairColorMenu;



    [Header("Head")]
    [SerializeField] private Button HeadMenuButton;
    [SerializeField] private GameObject HeadMenu;
    [SerializeField] private Button HeadOption1;
    [SerializeField] private Button HeadOption2;



    [Header("Eyes")]
    [SerializeField] private Button EyesMenuButton;
    [SerializeField] private GameObject EyesMenu;
    [SerializeField] private Button EyesOption1;
    [SerializeField] private Button EyesOption2;



    [Header("Mouth")]
    [SerializeField] private Button MouthMenuButton;
    [SerializeField] private GameObject MouthMenu;
    [SerializeField] private Button MouthOption1;
    [SerializeField] private Button MouthOption2;


    [Header("Player")]
    [SerializeField] private PlayerVisual playerVisual;

    private void Awake()
    {
        CloakMenuButton.onClick.AddListener(() =>
        {
            SetAllUnactiveExceptInput(CloakMenu);
            CloakColorMenu.SetActive(true);
            BodyColorMenu.SetActive(false);
            HairColorMenu.SetActive(false);
        });
        BodyMenuButton.onClick.AddListener(() =>
        {
            SetAllUnactiveExceptInput(BodyMenu);
            CloakColorMenu.SetActive(false);
            BodyColorMenu.SetActive(true);
            HairColorMenu.SetActive(false);
        });
        HairMenuButton.onClick.AddListener(() =>
        {
            SetAllUnactiveExceptInput(HairMenu);
            CloakColorMenu.SetActive(false);
            BodyColorMenu.SetActive(false);
            HairColorMenu.SetActive(true);
        });
        HeadMenuButton.onClick.AddListener(() =>
        {
            SetAllUnactiveExceptInput(HeadMenu);
            CloakColorMenu.SetActive(false);
            BodyColorMenu.SetActive(false);
            HairColorMenu.SetActive(false);
        });
        EyesMenuButton.onClick.AddListener(() =>
        {
            SetAllUnactiveExceptInput(EyesMenu);
            CloakColorMenu.SetActive(false);
            BodyColorMenu.SetActive(false);
            HairColorMenu.SetActive(false);
        });
        MouthMenuButton.onClick.AddListener(() =>
        {
            SetAllUnactiveExceptInput(MouthMenu);
            CloakColorMenu.SetActive(false);
            BodyColorMenu.SetActive(false);
            HairColorMenu.SetActive(false);
        });


        CloakOption1.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerCloak(0);
        });
        CloakOption2.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerCloak(1);
        });
        BodyOption1.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerBody(0);
        });
        BodyOption2.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerBody(1);
        });
        HairOption1.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerHair(0);
        });
        HairOption2.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerHair(1);
        });
        HeadOption1.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerHead(0);
        });
        HeadOption2.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerHead(1);
        });
        EyesOption1.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerEyes(0);
        });
        EyesOption2.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerEyes(1);
        });
        MouthOption1.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerMouth(0);
        });
        MouthOption2.onClick.AddListener(() =>
        {
            ThrowGameMultiplayer.Instance.ChangePlayerMouth(1);
        });


    }

    private void SetAllUnactiveExceptInput(GameObject obj)
    {
        CloakMenu.SetActive(false);
        BodyMenu.SetActive(false);
        HairMenu.SetActive(false);
        HeadMenu.SetActive(false);
        EyesMenu.SetActive(false);
        MouthMenu.SetActive(false);

        obj.SetActive(true);
    }

}
