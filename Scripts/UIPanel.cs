using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    public RawImage charaAImg;
    public RawImage charaBImg;
    public Image dailogBox;
    public Text contentText;
    public GameObject choiceA;
    public GameObject choiceB;

    public Canvas canvas;

    public void showButtons(bool value)
    {
        choiceA.SetActive(value);
        choiceB.SetActive(value);
    }

    public void SetButtonsName(string nameA, string nameB)
    {
        choiceA.name = nameA;
        choiceB.name = nameB;
    }

    public void SetButtonTexts(string txtA, string txtB)
    {
        Text tempText = choiceA.GetComponentInChildren<Text>();
        tempText.text = txtA;
        tempText = choiceB.GetComponentInChildren<Text>();
        tempText.text = txtB;
    }

    public void showCanvas(bool value)
    {
        canvas.enabled = value;
    }

    public void ShowCharaA(bool value)
    {
        charaAImg.enabled = value;
    }

    public void ShowCharaB(bool value)
    {
        charaBImg.enabled = value;
    }

    public void ShowDailogBox(bool value)
    {
        dailogBox.enabled = value;
    }

    public void ShowContextText(bool value)
    {
        contentText.enabled = value;
    }

    public void SetContentText(string value)
    {
        contentText.text = value;
    }

    public void ChangeCharaBTex(Texture tex)
    {
        charaBImg.texture = tex;
    }
    public void ChangeCharaATex(Texture tex)
    {
        charaAImg.texture = tex;
    }

}
