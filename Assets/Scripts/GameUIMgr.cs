using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

public class GameUIMgr : Singleton<GameUIMgr>
{
    public GameObject Mes;
    private Text MesText;
    bool IsColor = false;
    public Text ColorChange;
    // Start is called before the first frame update
    void Start()
    {
        MesText = Mes.transform.GetChild(0).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RestartCard()
    {
        Pokemgr.Instance.Restart();
    }
    //开始游戏
    public void StartGame()
    {
        StartCoroutine(StartAGame());
    }
    //加载完成
    private void CardIsDone()
    {
        StopCoroutine(StartAGame());
        Pokemgr.Instance.gameObject.SetActive(true);
        Pokemgr.Instance.Init();
    }
    //提交
    public void UploadCard()
    {
        string result = Pokemgr.Instance.PostResult();
        StartCoroutine(PostResult(result));
    }
    public void ChangeSort()
    {
        if (IsColor)
        {
            ColorChange.text = "花色";
        }
        else
        {
            ColorChange.text = "大小";
        }
        IsColor = !IsColor;
        Pokemgr.Instance.SortChange(IsColor);
    }
    public void AISort()
    {
        Pokemgr.Instance.GetAICard();
    }
    public void ExitGame()
    {

    }
    public void ShowMes(string Mess)
    {

        MesText.text = Mess;
        Mes.SetActive(true);
    }
    IEnumerator UserExit(string username, string password)
    {
        UnityWebRequest webRequest = new UnityWebRequest(NetMgr.GetUrl("Logout"), "POST");
        webRequest.SetRequestHeader("X-Auth-Token", NetMgr.TokenDate + ";charset=utf-8");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        if (webRequest.isDone)
        {
            string result = webRequest.downloadHandler.text;

            // Debug.Log(result);
            if (result == "")
            {
                ShowMes("网络连接失败");
            }
            else
            {
                JsonData results = JsonMapper.ToObject(result);
                string state = "";
                try
                {
                    state = results["status"].ToJson();
                }
                catch
                {
                    ShowMes("连接失败");
                }
                if (state == "0")
                {
                    //回到登录界面
                    NetMgr.TokenDate = "";
                    GameManager.Instance.LoadScene(0);
                }
                else
                {
                    ShowMes(NetMgr.GetState(state));
                }
            }
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowMes("网络连接失败");
            }
        }
    }



    IEnumerator StartAGame()
    {
        UnityWebRequest webRequest = new UnityWebRequest(NetMgr.GetUrl("StartGame"), "POST");
        webRequest.SetRequestHeader("X-Auth-Token", NetMgr.TokenDate);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        if (webRequest.isDone)
        {
            string result = webRequest.downloadHandler.text;
            // Debug.Log(result);
            if (result == "")
            {
                ShowMes("网络连接失败");
            }
            else
            {
                JsonData results = JsonMapper.ToObject(result);
                string state = "";
                try
                {
                    state = results["status"].ToJson();
                }
                catch
                {
                    ShowMes("连接失败");

                }
                if (state == "0")
                {
                    Debug.Log("成功获取牌");
                    NetMgr.GameID = (int)results["data"]["id"];
                    JsonData list = results["data"]["card"];
                    //NetMgr.CardList = list[0].ToString() + list[1].ToString() + list[2].ToString();
                    NetMgr.CardList = list.ToString();
                    Debug.Log(NetMgr.GameID + "牌:" + NetMgr.CardList);
                    CardIsDone();
                    //GameManager.Instance.LoadScene(1);
                }
                else
                {
                    ShowMes(NetMgr.GetState(state));
                }
            }
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowMes("网络连接失败");
            }
        }
    }


    IEnumerator PostResult(string jsondata)
    {

        byte[] postBytes = Encoding.UTF8.GetBytes(jsondata);

        UnityWebRequest webRequest = new UnityWebRequest(NetMgr.GetUrl("submit"), "POST")
        {
            uploadHandler = new UploadHandlerRaw(postBytes)
        };
        webRequest.SetRequestHeader("X-Auth-Token", NetMgr.TokenDate);
        webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        yield return webRequest.SendWebRequest();
        if (webRequest.isDone)
        {
            string result = webRequest.downloadHandler.text;
            // Debug.Log(result);
            if (result == "")
            {
                ShowMes("网络连接失败");
            }
            else
            {
                JsonData results = JsonMapper.ToObject(result);
                string state = "";
                try
                {
                    state = results["status"].ToJson();
                }
                catch
                {
                    ShowMes("连接失败");

                }
                if (state == "0")
                {

                    ShowMes("出牌成功");
                    //GameManager.Instance.LoadScene(1);
                }
                else
                {
                    ShowMes(NetMgr.GetState(state));
                }
            }
        }
        else
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowMes("网络连接失败");
            }
        }
    }
}
