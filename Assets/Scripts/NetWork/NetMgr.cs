using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMgr : Singleton<NetMgr>
{
    static Dictionary<string, string> StateTable = new Dictionary<string, string>();
    static Dictionary<string, string> UrlTable = new Dictionary<string, string>();
    public static string TokenDate;
    public static string UserID;
    //游戏ID
    public static int GameID;
    //获取的牌
    public static string CardList;
    // Start is called before the first frame update
    void Start()
    {
        InitStateTable();
        InitUrlTable();
    }
    private void InitStateTable()
    {
        StateTable.Add("0", "成功");
        StateTable.Add("1001", "用户名已被使用");
        StateTable.Add("1002", "学号已绑定");
        StateTable.Add("1003", "教务处认证失败");
        StateTable.Add("1004", "Token过期");
        StateTable.Add("1005", "用户名或密码错误");
        StateTable.Add("2001", "未结束战局过多");
        StateTable.Add("2002", "出千！！！");
        StateTable.Add("2003", "不合法墩牌");
        StateTable.Add("2004", "格式错误");
        StateTable.Add("2005", "用户名或密码错误");
        StateTable.Add("2006", "超时");
        StateTable.Add("3001", "战局不存在或未结束");
        StateTable.Add("3002", "玩家不存在");
        StateTable.Add("5000", "未知错误");
    }

    private void InitUrlTable()
    {
        UrlTable.Add("Binding", "http://api.revth.com/auth/register2");
        UrlTable["Rigister"] = "http://api.revth.com/auth/login";
        UrlTable["Logout"] = "http://api.revth.com/auth/logout";
        UrlTable["StartGame"] = "http://api.revth.com/game/open";
        UrlTable["submit"] = "http://api.revth.com/game/submit";
        UrlTable["rank"] = "http://api.revth.com//rank";
        UrlTable["history"] = "http://api.revth.com/history";
    }
    // Update is called once per frame
    void Update()
    {

    }
    public static string GetState(string state)
    {
        if (StateTable.ContainsKey(state))
        {
            return StateTable[state];
        }
        return "";
    }
    public static string GetUrl(string str)
    {
        if (UrlTable.ContainsKey(str))
        {
            return UrlTable[str];
        }
        return "";
    }
}
