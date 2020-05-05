using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEditor;
using System.Collections;

/// <summary>
/// 文本显示器
/// </summary>
public class ShowText : MonoBehaviour
{
    //UI组件模块改良
    [Header("UI组件")]public UIPanel dailogbox;
    //资源加载模块改良
    public TextLoad textload;

    //文本显示速度
    public float textspeed;
    //单个顺序字符完成打印的标志，确保不出现文本错误打印
    private bool textFinished;
    //跳过字符显示动画直接打印的标志
    private bool cancelTyping;

    /// <summary>
    ///  生命周期第一阶段
    ///  加载所需要的文本
    /// </summary>
    void Awake()
    {
        textload.GetTextFromFile();
    }

    void Start()
    {
        //textload.index = 0;
        textFinished = true;
        cancelTyping = false;
        StartCoroutine(SetText());
    }

    void Update()
    {
        //输入模块改良
        //开始界面显示
        if (Input.GetKeyDown(KeyCode.Space)&& textload.index == 0)
        {
            dailogbox.showCanvas(true);
        }

        //结束界面显示
        if (Input.GetKeyDown(KeyCode.Space)&& textload.index == textload.textList.Count)
        {
            dailogbox.showCanvas(false);
            textload.index = 0;
            return;
        }

       // if (Input.GetKeyDown(KeyCode.Space) &&　textFinished)
       // {
       //     //dailogbox.SetContentText(textload.textList[textload.index]);
       //     //textload.index++;
       //     StartCoroutine(SetText());
       // }

       if (Input.GetKeyDown(KeyCode.Space))
       {
           if (textFinished && !cancelTyping)
           {
               StartCoroutine(SetText());
           }
           else if(!textFinished)
           {
               cancelTyping = !cancelTyping;
           }
       }

    }


    /// <summary>
    /// 事件分发改良
    /// 协程函数，用于确定文本显示效果
    /// </summary>
    /// <returns></returns>
    IEnumerator SetText()
    {
        textFinished = false;
        //获取需要打印的字符串
        string templine = textload.textList[textload.index];
        //缓冲字符串
        string temptext = "";

        int letter = 0;

        while (!cancelTyping && letter < textload.textList[letter].Length)
        {
            if (templine[letter] == 'A'||templine[letter] == 'B')
            {
                break;
            }
            //逐字加入并更新
            temptext += templine[letter++];
            dailogbox.SetContentText(temptext); 
            //停止运行代码，直到等待时间结束
            yield return new WaitForSeconds(textspeed);  
        }
        dailogbox.SetContentText(templine);
        cancelTyping = false;
        textFinished = true;
        //转换到下一句内容
        textload.index++;
    }

}
