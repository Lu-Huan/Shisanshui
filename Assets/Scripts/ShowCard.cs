using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;
using UnityEngine.UI;
public enum DunTpye
{
    Top,
    Middle,
    Buttom
}
public class ShowCard : MonoBehaviour
{
    public DunTpye dunTpye;
    private GameObject show;
    private bool beuse = false;
    private bool canPut;
    public int count = 5;
    public  List<Vector3> listpos = new List<Vector3>();
    static Dictionary<DeckTypeEnum, string> TypeName = new Dictionary<DeckTypeEnum, string>()
    {
        { DeckTypeEnum.Bomb,"炸弹"},
        { DeckTypeEnum.Double,"一对"},
        { DeckTypeEnum.ContinuousTwoDouble,"连对"},
        { DeckTypeEnum.Gourd,"葫芦"},
        { DeckTypeEnum.ShunZi,"顺子"},
        { DeckTypeEnum.Single,"乌龙"},
        { DeckTypeEnum.Three,"三张"},
        { DeckTypeEnum.TongHua,"同花"},
        { DeckTypeEnum.TongHuaShun,"同花顺"},
        { DeckTypeEnum.TwoDouble,"两对"},
    };
    private Text ShowType;
    // Start is called before the first frame update
    void Awake()
    {
        
        ShowType = transform.GetChild(0).GetComponent<Text>();
        Transform[] tras = transform.GetChild(1).GetComponentsInChildren<Transform>();
        //Debug.Log(ListPos.Length);
        for (int i = 1; i < tras.Length; i++)
        {           
            listpos.Add(tras[i].position);
            int index = (2-(int)dunTpye) * 5 + i - 1;
            Pokemgr.Instance.UIPositonList[index] = tras[i].position;
        }
        show = transform.GetChild(2).gameObject;
        show.SetActive(false);
        Pokemgr.Instance.UIshow += Show;
        Pokemgr.Instance.AISet += AIShow;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (canPut && !beuse)
            {
                beuse = true;
                Pokemgr.Instance.CardUP(listpos, dunTpye);
            }
        });
        Pokemgr.Instance.CardAllBack += (DunTpye type) =>
        {
            if (type == dunTpye)
            {
                beuse = false;
            }
        };
    }

    private void AIShow(DunTpye dunTpye, DeckTypeEnum type)
    {
        if (this.dunTpye == dunTpye)
        {
            ShowType.text = TypeName[type];
            beuse = true;
        }         
    }

    public void Show(int r)
    {
        canPut = r == count;
        show.SetActive(canPut&&!beuse);
    }
}
