using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDarkLoop : MonoBehaviour
{
    public GameObject ghost;
    public GameObject sign;
    public GameObject wall;

    //每次开始，检查isDarkLoop，若是则启用，否则禁用（由主脚本管理）
    private void Start()
    {
        InActiveAll();
    }
    public void ActiveAll()
    {
        if(ghost.active == true) ghost.transform.position = ghost.GetComponent<GhostMove>().ghostBornPlace;
        ghost.SetActive(true);
        sign.SetActive(true);
        wall.SetActive(true);
        ghost.GetComponent<GhostMove>().Summon();
        transform.GetComponent<LightManager>().TurnOffAllLights();
        Debug.Log("执行ActiveAll");
    }
    public void InActiveAll()
    {
        ghost.transform.position = ghost.GetComponent<GhostMove>().ghostBornPlace;
        ghost.SetActive(false);
        sign.SetActive(false);
        wall.SetActive(false);
        transform.GetComponent<LightManager>().TurnOnAllLights();
        Debug.Log("执行InactiveAll");
    }
}
