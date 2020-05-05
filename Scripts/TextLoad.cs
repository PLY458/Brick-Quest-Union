using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;


public class TextLoad : MonoBehaviour
{


    [Header("文本文件")]
    public TextAsset textFile;
    //文本行的索引
    public int index;

    //等待载入的字符串数组
    public List<string> textList = new List<string>();

    public void GetTextFromFile()
    {
        textList.Clear();
        index = 0;

        var lineData = textFile.text.Split('\n');

        foreach (var text in lineData)
        {
            textList.Add(text);
        }
    }


}
