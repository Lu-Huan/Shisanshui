using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using AI;
using System.Text;
using LitJson;

public class AICard
{
    public List<DeckTypeEnum> deckTypes = new List<DeckTypeEnum>();
    //数字对应的位置
    public Dictionary<int, int> Num_Point = new Dictionary<int, int>();
}
public class Pokemgr : Singleton<Pokemgr>
{
    public GameObject CardPanal;
    public GameObject ButtonPanal;
    public Action<int> UIshow;

    public Action<DunTpye, DeckTypeEnum> AISet;

    public Action<DunTpye> CardAllBack;
    public static int pokeNum = 13;
    //private Card[] Cards;
    public Transform pointstart;

    List<CardModel> cardModels = new List<CardModel>();
    public Sprite[] pokes;
    public float interval = 0.77f;
    bool SeleUP = false;
    Dictionary<string, int> Num = new Dictionary<string, int>();
    Dictionary<char, int> CardType = new Dictionary<char, int>();

    //摆牌的结果
    private List<List<Card>> cardsResult = new List<List<Card>>();
    public Vector3[] UIPositonList = new Vector3[13];

    List<AICard> aICards = new List<AICard>();
    private int AICardIndex = 0;

    List<Card> AllCardList = new List<Card>();

    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 2; i <= 10; i++)
        {
            Num[i.ToString()] = i;
        }
        Num["J"] = 11; Num["Q"] = 12; Num["K"] = 13; Num["A"] = 14;
        CardType['$'] = 1; CardType['&'] = 2; CardType['%'] = 3; CardType['#'] = 4;
        // Down.Sort();
        cardsResult.Add(new List<Card>());
        cardsResult.Add(new List<Card>());
        cardsResult.Add(new List<Card>());
        Card[] Cards = GetComponentsInChildren<Card>();
        foreach (Card item in Cards)
        {
            AllCardList.Add(item);
        }
        //Debug.Log(Cards.Length);
        gameObject.SetActive(false);
    }
    public void Clean()
    {
        aICards.Clear();
        AICardIndex = 0;
        foreach (var item in cardsResult)
        {
            item.Clear();
        }
        cardModels.Clear();
    }
    public void Init(string strCards)
    {
        //Clean();
         //strCards = NetMgr.CardList;
       
        strCards = strCards.Replace("*", "%");
        //strCards = strCards.Trim();
        //Regex.Replace(strCards, "*", "t");
        string[] splitcard = Regex.Split(strCards, " ");
        //Debug.Log(splitcard);
        CardPanal.SetActive(true);
        ButtonPanal.SetActive(true);

        List<int> CardListToAI = new List<int>();
        for (int i = 0; i < AllCardList.Count; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("PokeCard/" + splitcard[i]);
            int cardType = CardType[splitcard[i][0]];
            int cardNum = Num[splitcard[i].Remove(0, 1)];
            CardListToAI.Add(cardType * 100 + cardNum);
            //初始化
            AllCardList[i].InitCard(cardType, cardNum, sprite);
        }
        //AI计算
        AICards(CardListToAI);



        //排序，按花色顺序排序
        AllCardList.Sort((x, y) => x.CardNum.CompareTo(y.CardNum) + x.CardType.CompareTo(y.CardType) * 2);
        for (int i = 0; i < AllCardList.Count; i++)
        {
            AllCardList[i].InitColorIndex(i, pointstart.position + new Vector3(i * interval, 0, 20 - i));
        }

        //排序，按数字顺序排序
        AllCardList.Sort((x, y) => x.CardNum.CompareTo(y.CardNum) * 2 + x.CardType.CompareTo(y.CardType));
        for (int i = 0; i < AllCardList.Count; i++)
        {
            Vector3 posi = pointstart.position + new Vector3(i * interval, 0, 20 - i);
            AllCardList[i].InitNumIndex(posi, i);
        }
        if (GameUIMgr.Instance.AIScore)
        {
            GameUIMgr.Instance.AIButton.onClick.Invoke();
            GameUIMgr.Instance.PushButton.onClick.Invoke();
        }
    }
    //AI组牌可以形成多种方式
    private void AICards(List<int> CardList)
    {
        cardModels = CaculalAI.GetAllResult(cardModels, CardList);
        int i = 0;
        foreach (CardModel CardModel in cardModels)
        {
            AICard cardPos = new AICard();
            foreach (TypeCard TypeCard in CardModel.typeCardList)
            {
                cardPos.deckTypes.Add(TypeCard.cardType);
                foreach (int Num in TypeCard.cardList)
                {
                    cardPos.Num_Point[Num] = i++;
                }
            }
            aICards.Add(cardPos);
        }
    }


    public void GetAICard()
    {
        AICard AListCard = aICards[AICardIndex];
        foreach (Card card in AllCardList)
        {
            int keyPoint = card.CardType * 100 + card.CardNum;
            int index = AListCard.Num_Point[keyPoint];//1~13在哪个位置
            //将牌分堆
            int dui = 2 - (index / 5);
            cardsResult[dui].Add(card);


            card.AISort(UIPositonList[index]);
        }
        AISet(DunTpye.Top, AListCard.deckTypes[2]);
        AISet(DunTpye.Middle, AListCard.deckTypes[1]);
        AISet(DunTpye.Buttom, AListCard.deckTypes[0]);


        AICardIndex++;
        AICardIndex %= aICards.Count;
    }
    public string PostResult()
    {
        string[] result = new string[3];
        if (cardsResult[0].Count == 3 && cardsResult[1].Count == 5 && cardsResult[2].Count == 5)
        {
            for (int i = 0; i < cardsResult.Count; i++)
            {
                for (int j = 0; j < cardsResult[i].Count; j++)
                {
                    Card card = cardsResult[i][j];
                    foreach (char key in CardType.Keys)
                    {
                        if (CardType[key].Equals(card.CardType))
                        {
                            result[i] += key;
                        }
                    }
                    foreach (string key in Num.Keys)
                    {
                        if (Num[key].Equals(card.CardNum))
                        {
                            result[i] += key;
                        }
                    }
                    if (j != cardsResult[i].Count - 1)
                    {
                        result[i] += " ";
                    }
                }
            }
        }
        else
        {
            GameUIMgr.Instance.ShowMes("未完成分堆!!");
            return "";
        }
        JsonData list = new JsonData
        {
              result[0],
              result[1],
              result[2]
        };
        JsonData data = new JsonData
        {
            ["id"] = NetMgr.GameID,
            ["card"] = list
        };
        string jsondata = data.ToJson();
        jsondata = jsondata.Replace("%", "*");
        Debug.Log("提交json结果:"+jsondata);
        return jsondata;
    }
    public void SortChange(bool IsColor)
    {
        if (IsColor)
        {
            AllCardList.Sort((x, y) => x.CardNum.CompareTo(y.CardNum) + x.CardType.CompareTo(y.CardType) * 2);
        }
        else
        {
            AllCardList.Sort((x, y) => x.CardNum.CompareTo(y.CardNum) * 2 + x.CardType.CompareTo(y.CardType));
        }
        foreach (Card item in AllCardList)
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
                            if (AllCardList[z].state != Postype.BeUsed)
                            {
                                AllCardList[z].BeSeleted();
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
    public void CardUP(List<Vector3> poslist, DunTpye dunType)
    {
        int i = 0;
        foreach (Card item in AllCardList)
        {
            if (item.state == Postype.Beseleted)
            {
                item.BeUsed(poslist[i++], dunType);
                cardsResult[(int)dunType].Add(item);
            }
        }
    }
    public void CardBack(Card card, DunTpye dun)
    {
        cardsResult[(int)dun].Remove(card);
        if (cardsResult[(int)dun].Count == 0)
        {
            CardAllBack(dun);
        }
    }
    public void Restart()
    {
        foreach (List<Card> cardslist in cardsResult)
        {
            foreach (Card item in cardslist)
            {
                item.GoBack();
            }
        }
        cardsResult[0].Clear();
        cardsResult[1].Clear();
        cardsResult[2].Clear();
        CardAllBack(DunTpye.Buttom);
        CardAllBack(DunTpye.Middle);
        CardAllBack(DunTpye.Top);
    }
    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            int t = 0;
            foreach (Card item in AllCardList)
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
