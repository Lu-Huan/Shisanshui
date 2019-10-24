using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class StartUIMgr : MonoBehaviour
{
    public InputField userName;
    public InputField userPassWord;


    public InputField reName;
    public InputField rePassWord;
    public InputField bindNum;
    public InputField bindPassWord;
    public GameObject Mes;
    private Text MesText;
    //public Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        MesText = Mes.transform.GetChild(0).GetComponent<Text>();
        //toggle.onValueChanged.AddListener(ShowPassword);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartLog()
    {
        string Name = userName.text;
        string PassWord = userPassWord.text;
        if (PassWord == "" || Name == "")
        {
            ShowMes("用户名/密码不能为空");
        }
        // MesText.text = Name + "\n" + PassWord;

        StartCoroutine(UserRigister(Name, PassWord));
       
    }
    public void Enter()
    {
        GameManager.Instance.LoadScene(1);
    }
    public void StartBind()
    {
        string reN = reName.text;
        string rePw = rePassWord.text;
        string bindN = bindNum.text;
        string bindPW = bindPassWord.text;

        if (reN == "" || reN == "" || bindN == "" || bindPW == "")
        {
            ShowMes("输入不能为空");
        }
        else
        {
            StartCoroutine(Bind(reN, rePw, bindN, bindPW));
        }
        // MesText.text = Name + "\n" + PassWord;

        // StartCoroutine(Bind());
    }
    private void ShowMes(string Mess)
    {
        MesText.text = Mess;
        Mes.SetActive(true);
    }
    IEnumerator Bind(string username, string password, string student_num, string student_password)
    {
        JsonData data = new JsonData
        {
            ["username"] = username,
            ["password"] = password,
            ["student_number"] = student_num,
            ["student_password"] = student_password
        };
        string js1 = data.ToJson();
        byte[] postBytes = Encoding.UTF8.GetBytes(js1);
        Debug.Log(js1);
        UnityWebRequest webRequest = new UnityWebRequest(NetMgr.GetUrl("Binding"), "POST")
        {
            uploadHandler = new UploadHandlerRaw(postBytes)
        };
        Debug.Log(NetMgr.GetUrl("Binding"));
        webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();
        if (webRequest.isDone)
        {
            string result = webRequest.downloadHandler.text;

            Debug.Log(result);
            if (result == "")
            {
                ShowMes("网络连接失败1");
            }
            else
            {
                JsonData results = JsonMapper.ToObject(result);
                string state="";
                try
                {
                    state = results["status"].ToJson();
                }
                catch
                {
                    ShowMes("连接失败");
                }
                if (state=="0")
                {
                    ShowMes("绑定成功");
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
                ShowMes("网络连接失败2");
            }
        }
    }

    IEnumerator UserRigister(string username, string password)
    {
        JsonData data = new JsonData
        {
            ["username"] = username,
            ["password"] = password
        };
        string js1 = data.ToJson();
        byte[] postBytes = Encoding.UTF8.GetBytes(js1);

        UnityWebRequest webRequest = new UnityWebRequest(NetMgr.GetUrl("Rigister"), "POST")
        {
            uploadHandler = new UploadHandlerRaw(postBytes)
        };
        webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
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
                    Saver.WriteJsonString(result, Saver.MapDataPath);
                    string token = results["data"]["token"].ToString();
                    Debug.Log(token);
                    NetMgr.TokenDate=token;
                    NetMgr.UserID = results["data"]["user_id"].ToString();
                    GameManager.Instance.LoadScene(1);
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
