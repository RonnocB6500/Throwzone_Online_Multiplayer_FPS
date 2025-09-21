using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer Cloak1;
    [SerializeField] private SkinnedMeshRenderer Cloak2;
    [SerializeField] private SkinnedMeshRenderer Body1;
    [SerializeField] private SkinnedMeshRenderer Body2;
    [SerializeField] private MeshRenderer Hair1;
    [SerializeField] private MeshRenderer Hair2;
    [SerializeField] private MeshRenderer Head1;
    [SerializeField] private MeshRenderer Head2;
    [SerializeField] private MeshRenderer Eyes1;
    [SerializeField] private MeshRenderer Eyes2;
    [SerializeField] private MeshRenderer Mouth1;
    [SerializeField] private MeshRenderer Mouth2;


    private Material cloakMat1;
    private Material cloakMat2;
    private Material bodyMat1;
    private Material bodyMat2;
    private Material hairMat1;
    private Material hairMat2;

    private void Awake()
    {
        cloakMat1 = new Material(Cloak1.material);
        Cloak1.material = cloakMat1;
        cloakMat2 = new Material(Cloak2.material);
        Cloak2.material = cloakMat2;

        bodyMat1 = new Material(Body1.material);
        Body1.material = bodyMat1;
        bodyMat2 = new Material(Body2.material);
        Body2.material = bodyMat2;

        hairMat1 = new Material(Hair1.material);
        Hair1.material = hairMat1;
        hairMat2 = new Material(Hair2.material);
        Hair2.material = hairMat2;
    }

    
    public void SetPlayerCloakColor(Color color)
    {
        if (Cloak1.gameObject.activeSelf)
        {
            cloakMat1.color = color;
        }
        else if (Cloak2.gameObject.activeSelf)
        {
            cloakMat2.color = color;
        }
    }
    public void SetPlayerBodyColor(Color color)
    {
        if (Body1.gameObject.activeSelf)
        {
            bodyMat1.color = color;
        }
        else if (Body2.gameObject.activeSelf)
        {
            bodyMat2.color = color;
        }
    }
    public void SetPlayerHairColor(Color color)
    {
        if (Hair1.gameObject.activeSelf)
        {
            hairMat1.color = color;
        }
        else if (Hair2.gameObject.activeSelf)
        {
            hairMat2.color = color;
        }
    }



    public void ActiveCloakOption(int optionId)
    {
        if (optionId == 0)
        {
            Cloak1.gameObject.SetActive(true);
            Cloak2.gameObject.SetActive(false);
        }
        else if (optionId == 1)
        {
            Cloak1.gameObject.SetActive(false);
            Cloak2.gameObject.SetActive(true);
        }
    }
    public void ActiveBodyOption(int optionId)
    {
        if (optionId == 0){
            Body1.gameObject.SetActive(true);
            Body2.gameObject.SetActive(false);
        }
        else if(optionId == 1){
            Body1.gameObject.SetActive(false);
            Body2.gameObject.SetActive(true);
        }
    }
    public void ActiveHairOption(int optionId)
    {
        if (optionId == 0){
            Hair1.gameObject.SetActive(true);
            Hair2.gameObject.SetActive(false);
        }
        else if(optionId == 1){
            Hair1.gameObject.SetActive(false);
            Hair2.gameObject.SetActive(true);
        }
    }
    public void ActiveHeadOption(int optionId)
    {
        if (optionId == 0){
            Head1.gameObject.SetActive(true);
            Head2.gameObject.SetActive(false);
        }
        else if(optionId == 1){
            Head1.gameObject.SetActive(false);
            Head2.gameObject.SetActive(true);
        }
    }
    public void ActiveEyesOption(int optionId)
    {
        if (optionId == 0){
            Eyes1.gameObject.SetActive(true);
            Eyes2.gameObject.SetActive(false);
        }
        else if(optionId == 1){
            Eyes1.gameObject.SetActive(false);
            Eyes2.gameObject.SetActive(true);
        }
    }
    public void ActiveMouthOption(int optionId)
    {
        if (optionId == 0){
            Mouth1.gameObject.SetActive(true);
        Mouth2.gameObject.SetActive(false);
        }
        else if(optionId == 1){
            Mouth1.gameObject.SetActive(false);
            Mouth2.gameObject.SetActive(true);
        }
    }



    /*
    public void ActiveCloakOption1()
    {
        Cloak1.gameObject.SetActive(true);
        Cloak2.gameObject.SetActive(false);
    }
    public void ActiveCloakOption2()
    {
        Cloak1.gameObject.SetActive(false);
        Cloak2.gameObject.SetActive(true);
    }
    public void ActiveBodyOption1()
    {
        Body1.gameObject.SetActive(true);
        Body2.gameObject.SetActive(false);
    }
    public void ActiveBodyOption2()
    {
        Body1.gameObject.SetActive(false);
        Body2.gameObject.SetActive(true);
    }
    public void ActiveHairOption1()
    {
        Hair1.gameObject.SetActive(true);
        Hair2.gameObject.SetActive(false);
    }
    public void ActiveHairOption2()
    {
        Hair1.gameObject.SetActive(false);
        Hair2.gameObject.SetActive(true);
    }
    public void ActiveHeadOption1()
    {
        Head1.gameObject.SetActive(true);
        Head2.gameObject.SetActive(false);
    }
    public void ActiveHeadOption2()
    {
        Head1.gameObject.SetActive(false);
        Head2.gameObject.SetActive(true);
    }
    public void ActiveEyesOption1()
    {
        Eyes1.gameObject.SetActive(true);
        Eyes2.gameObject.SetActive(false);
    }
    public void ActiveEyesOption2()
    {
        Eyes1.gameObject.SetActive(false);
        Eyes2.gameObject.SetActive(true);
    }
    public void ActiveMouthOption1()
    {
        Mouth1.gameObject.SetActive(true);
        Mouth2.gameObject.SetActive(false);
    }
    public void ActiveMouthOption2()
    {
        Mouth1.gameObject.SetActive(false);
        Mouth2.gameObject.SetActive(true);
    }*/

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}