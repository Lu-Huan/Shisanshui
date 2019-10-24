using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;
using System.Text.RegularExpressions;


public class GameManager : Singleton<GameManager>
{
    //全局方法
    public void LoadScene(int sceneindex)
    {
        //发布事件
        //SendEvent(ConstName.E_ExitScene, level);
        ExitScene(sceneindex);
        //---同步加载新场景
        SceneManager.LoadScene(sceneindex, LoadSceneMode.Single);
        SceneManager.sceneLoaded += LoadedEve;
    }
    void LoadedEve(Scene s, LoadSceneMode l)
    {
        if (l == LoadSceneMode.Single)
        {
            SceneManager.sceneLoaded -= LoadedEve;
            //事件参数
            int SceneIndex = s.buildIndex;

            //发布事件
            //SendEvent(ConstName.E_EnterScene, SceneIndex);
            EnterScene(SceneIndex);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        //确保Game对象一直存在
        DontDestroyOnLoad(gameObject);
        //启动游戏
        /*if (PlayerPrefs.GetInt("CurrentLevel", -1) == -1)
        {
            Debug.Log("第一次游戏");
            PlayerPrefs.SetInt("CurrentLevel", 1);
        }
        else
        {
            
        }*/

    }

    private void Start()
    {
    }
    private void Update()
    {

    }
    private void ExitScene(int data)
    {
        int index = (int)data;
        switch (index)
        {
            case 0:
                break;
            case 1:
                ///读出的数据导入到Consts
                // ReadUserData();

                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
    private void EnterScene(int data)
    {
        switch (data)
        {
            case 0:
                break;
            case 1:

                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
}


