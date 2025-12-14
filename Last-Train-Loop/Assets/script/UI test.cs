using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UItest : MonoBehaviour
{
    GameObject canvas;
    GameObject back, guide_obj,panel;
    private void Awake()
    {
        canvas = GameObject.Find("Canvas_start");
        guide_obj = GameObject.Find("Guide Scroll view");
        back = GameObject.Find("Back Button");
        panel = GameObject.Find("panel_main");
    }
    private void Start()
    {
        guide_obj.SetActive(false);
        back.transform.parent = canvas.transform;
        RectTransform rect = back.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1,1);
        rect.anchorMax = new Vector2(1,1);
        rect.sizeDelta = new Vector2(100, 30);
        back.SetActive(false);
        
    }
    public void OnclickGuideButton()
    {
        panel.SetActive(false);
        guide_obj.SetActive(true);
        back.SetActive(true);
    }
    public void OnclickCancelButton()
    {
        back.SetActive(false);
        guide_obj.SetActive(false);
        panel.SetActive(true);
    }
    public void OnclickStartButton()
    {
        canvas.SetActive(false);
        //ÇÐ³¡¾°
    }
    
}
