using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Pokemgr : Singleton<Pokemgr>
{
    public GameObject CardPanal;
    public GameObject ButtonPanal;
    public Action<int> UIshow;
    public Action<DunTpye> CardAllBack;
    public static int pokeNum = 13;
    //private Card[] Cards;
    public Transform pointstart;

    public Sprite[] pokes;
    public float interval = 0.77f;
    bool SeleUP = false;
    Dictionary<string, int> Num=new Dictionary<string, int>();
    Dictionary<char, int> CardType=new Dictionary<char, int>();
    private List<List<Card>> cards = new List<List<Card>>();

    List<Card> DownCardList = new List<Card>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 2; i <=10; i++)
        {
            Num[i.ToString()] = i;
        }
        Num["J"] = 11; Num["Q"] = 12; Num["K"] = 13; Num["A"] = 14;
        CardType['$'] = 0; CardType['&'] = 1; CardType['%'] = 2; CardType['#'] = 3;
        // Down.Sort();
        cards.Add(new List<Card>());
        cards.Add(new List<Card>());
        cards.Add(new List<Card>());
        Card[] Cards = GetComponentsInChildren<Card>();
        foreach (Card item in Cards)
        {
            DownCardList.Add(item);
        }
        //Debug.Log(Cards.Length);
        gameObject.SetActive(false);
    }
    public void Init()
    {
        string strCards = "*K #2 $4 *3 &5 &10 *6 *J *7 *9 #Q &8 $A";
        strCards=strCards.Replace("*", "%");
        //strCards = strCards.Trim();
        //Regex.Replace(strCards, "*", "t");
        string[] splitcard= Regex.Split(strCards, " ");
        //Debug.Log(splitcard);
        CardPanal.SetActive(true);
        ButtonPanal.SetActive(true);
        for (int i = 0; i < DownCardList.Count; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("PokeCard/" + splitcard[i]);
            int cardType = CardType[splitcard[i][0]];
            int cardNum = Num[splitcard[i].Remove(0,1)];
           
            //初始化
            DownCardList[i].InitCard(cardType, cardNum ,sprite);
        }
        //排序，先按数字顺序排序
        DownCardList.Sort((x, y) => x.CardNum.CompareTo(y.CardNum) * 2 + x.CardType.CompareTo(y.CardType) );
        for (int i = 0; i < DownCardList.Count; i++)
        {
            Vector3 posi = pointstart.position + new Vector3(i * interval, 0, 20 - i);
            DownCardList[i].InitNumIndex(posi, i);
        }
        //排序，按花色顺序排序
        DownCardList.Sort((x, y) => x.CardNum.CompareTo(y.CardNum)  + x.CardType.CompareTo(y.CardType) * 2);
        for (int i = 0; i < DownCardList.Count; i++)
        {
            DownCardList[i].InitColorIndex(i, pointstart.position + new Vector3(i * interval, 0, 20 - i));
        }
    }
    public void SortChange(bool IsColor)
    {
        if (IsColor)
        {
            DownCardList.Sort((x, y) => x.CardNum.CompareTo(y.CardNum) + x.CardType.CompareTo(y.CardType) * 2);
        }
        else
        {
            DownCardList.Sort((x, y) => x.CardNum.CompareTo(y.CardNum) * 2 + x.CardType.CompareTo(y.CardType));
        }
        foreach (Card item in DownCardList)
        {
            item.IsColorMode = IsColor;
        }
    }
    private bool isstart = true;
    private int start = 0;
    private int end = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Debug.Log("11");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit2D;

            if (Physics.Raycast(ray, out hit2D))
            {
                hit2D.collider.GetComponent<Card>().BeSeleted();
                if (SeleUP)
                {
                    Card card = hit2D.collider.GetComponent<Card>();
                    card.BeSeleted();
                }
                if (isstart)
                {
                    Card card = hit2D.collider.GetComponent<Card>();
                    card.BeSeleted();
                    start = card.Sortpos;
                    SeleUP = card.state == Postype.BeUsed;

                    isstart = false;
                }
                else
                {
                    if (!SeleUP)
                    {
                        end = hit2D.collider.GetComponent<Card>().Sortpos;
                        if (start == end)
                        {
                            return;
                        }
                        int i = start;
                        int j = end;
                        if (start > end)
                        {
                            i = end;
                            j = start;
                        }
                        for (int z = i; z <= j; z++)
                        {
                            if (DownCardList[z].state != Postype.BeUsed)
                            {
                                DownCardList[z].BeSeleted();
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }
    public void CardUP(List<Vector3> poslist,DunTpye le)
    {
        int i = 0;
        foreach (Card item in DownCardList)
        {
            if (item.state == Postype.Beseleted)
            {
                item.BeUsed(poslist[i++],le);
                cards[(int)le].Add(item);
            }
        }
    }
    public void CardBack(Card card,DunTpye dun)
    {
        cards[(int)dun].Remove(card);
        if (cards[(int)dun].Count==0)
        {
            CardAllBack(dun);
        }
    }
    public void Restart()
    {
        foreach (List<Card> cardslist in cards)
        {
            foreach (Card item in cardslist)
            {
                item.GoBack();
            }
        }
        cards[0].Clear();
        cards[1].Clear();
        cards[2].Clear();
        CardAllBack(DunTpye.Buttom);
        CardAllBack(DunTpye.Middle);
        CardAllBack(DunTpye.Top);
    }
    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            int t = 0;
            foreach (Card item in DownCardList)
            {
                if (item.state == Postype.Beseleted)
                {
                    t++;
                }
            }
            UIshow(t);
            start = 0;
            end = 0;
            isstart = true;
        }
    }
}
