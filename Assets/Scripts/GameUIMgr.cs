using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System;

public class Rank
{
    public int player_id;
    public int score;
    public string name;
}
public class HitoryData
{
    public int id;
    public List<string> card = new List<string>();
    public int score;
    public int timestamp;
}
public class GameUIMgr : Singleton<GameUIMgr>
{
    public InputField His_ID;
    public int Limit;
    public int page;
    public Text[] LogText = new Text[4];



    public Text UserMes;
    public Text Score;
    public bool AIScore = false;
    public Button ExitButton;
    public Button BackButton;
    public Button StartButton;
    public Button AIButton;
    public Button PushButton;
    public string DebugstrCards;
    public bool IsDebug = false;
    private List<Rank> ranks = new List<Rank>();
    public GameObject Mes;
    private Text MesText;
    bool IsColor = false;
    public Text ColorChange;

    public Text[] Rank = new Text[3];
    // Start is called before the first frame update
    void Start()
    {
        AIScore = false;
        MesText = Mes.transform.GetChild(0).GetComponent<Text>();
        UserMes.text = "ID:" + NetMgr.UserID + "\n" + NetMgr.UserName;
        SetUserScore();
    }

    public void GetHistory()
    {
        StartCoroutine(Log(int.Parse(His_ID.text), Limit, page));
    }
    public void SetAIScore(bool ai)
    {
        AIScore = ai;
    }
    //提取分数
    public void SetUserScore()
    {
        StartCoroutine(PostRank(false));
    }
    //更新分数
    private void UpDateScore()
    {
        int i = 0;
        foreach (Rank item in ranks)
        {
            i++;
            if (item.player_id == NetMgr.UserID)
            {
                Score.text = "分数:" + item.score.ToString() + "\n排名:" + i;
                return;
            }
        }
    }
    public void RestartCard()
    {
        Pokemgr.Instance.Restart();
    }
    //开始游戏
    public void StartGame()
    {
        if (IsDebug)
        {
            NetMgr.CardList = DebugstrCards;
            CardIsDone();
        }
        else
        {
            StartCoroutine(StartAGame());
        }
        // CardIsDone();
    }
    //加载完成
    private void CardIsDone()
    {
        //StopCoroutine(StartAGame());
        Pokemgr.Instance.gameObject.SetActive(true);
        Pokemgr.Instance.Init(NetMgr.CardList);
    }
    //提交
    public void UploadCard()
    {
        string result = Pokemgr.Instance.PostResult();
        if (result == "")
        {
            return;
        }
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
    public void GetRank()
    {
        StartCoroutine(PostRank(true));
    }
    private void SetLog(List<JsonData> list)
    {
        ShowMes("可以接受数据，尚未开发UI");
       /* string na0 = "";
        string na1 = "";
        string na2 = "";
        string na3 = "";
        foreach (HitoryData item in list)
        {
            na0 += item.id + "\n";
            na1 += item.card[0] + "," + item.card[1] + "," + item.card[2] + "," + "\n";
            na2 += item.score.ToString() + "\n";
            na3 += item.timestamp.ToString() + "\n";
        }

        LogText[0].text = na0;
        LogText[1].text = na1;
        LogText[2].text = na2;
        LogText[3].text = na3;*/
    }

    private void SetRank()
    {
        string na0 = "";
        string na1 = "";
        string na2 = "";
        int i = 0;
        foreach (Rank rank in ranks)
        {
            i++;
            na0 += i.ToString() + "\n";
            na1 += rank.name + "\n";
            na2 += rank.score.ToString() + "\n";
        }

        Rank[0].text = na0;
        Rank[1].text = na1;
        Rank[2].text = na2;

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
                    NetMgr.CardList = list.ToString();
                    Debug.Log(NetMgr.GameID + "牌:" + NetMgr.CardList);
                    CardIsDone();
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
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        if (webRequest.isDone)
        {
            string result = webRequest.downloadHandler.text;
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
                    Pokemgr.Instance.Clean();
                    ExitButton.onClick.Invoke();
                    BackButton.onClick.Invoke();

                    if (AIScore)
                    {
                        StartButton.onClick.Invoke();
                    }
                    else
                    {
                        ShowMes("出牌成功");
                    }
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
    IEnumerator PostRank(bool SetUI)
    {
        UnityWebRequest webRequest = new UnityWebRequest(NetMgr.GetUrl("rank"), "GET")
        {
            downloadHandler = new DownloadHandlerBuffer()
        };
        yield return webRequest.SendWebRequest();
        if (webRequest.isDone)
        {
            string result = webRequest.downloadHandler.text;
            //Debug.Log(result);
            // Debug.Log(result);
            if (result == "")
            {
                ShowMes("网络连接失败");
            }
            else
            {
                ranks = JsonMapper.ToObject<List<Rank>>(result);
                if (SetUI)
                {
                    SetRank();
                }
                else
                {
                    UpDateScore();
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
    IEnumerator Log(int player_id, int limit, int page)
    {
        JsonData jsonTable = new JsonData
        {
            ["player_id"] = player_id,
            ["limit"] = limit,
            ["page"] = page
        };

        string jsondata = jsonTable.ToString();
        byte[] postBytes = Encoding.UTF8.GetBytes(jsondata);
        UnityWebRequest webRequest = new UnityWebRequest(NetMgr.GetUrl("history"), "GET")
        {
            uploadHandler = new UploadHandlerRaw(postBytes),
            downloadHandler = new DownloadHandlerBuffer()
        };
        webRequest.SetRequestHeader("X-Auth-Token", NetMgr.TokenDate);
        yield return webRequest.SendWebRequest();
        if (webRequest.isDone)
        {
            string result = webRequest.downloadHandler.text;
            Debug.Log(result);
            //Debug.Log(result);
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
                    JsonData LoadData = JsonMapper.ToObject(result);

                    List<JsonData> jsonDatas = JsonMapper.ToObject<List<JsonData>>(LoadData["data"].ToString());
                    Debug.Log(jsonDatas.Count);
                    SetLog(jsonDatas);
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
