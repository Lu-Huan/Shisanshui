using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Postype
{
    Init,
    Beseleted,
    BeUsed
}
public class ComparerCard : IComparer<Card>
{
    public int Compare(Card x, Card y)
    {
        throw new NotImplementedException();
    }
}
public class Card : MonoBehaviour
{

    public int CardType;
    public int CardNum;
    bool SetSele = false;
    public Postype state = Postype.Init;
    // Start is called before the first frame update
    private SpriteRenderer num;
    private GameObject mark;
    private Vector3 InitPos;
    private Vector3 TarPos;

    private Vector3 DownPos;
    //花色位置
    private Vector3 ColorPos;
    bool ToUp = false;
    bool ToDown = false;

    bool colorMode = false;
    public bool IsColorMode
    {
        set
        {
            colorMode = value;
            DownPos = value ? ColorPos : InitPos;
            ToDown = state != Postype.BeUsed;
            if (colorMode)
            {
                Sortpos = Index_Color;
            }
            else
            {
                Sortpos = Index_Num;
            }
        }
        get
        {
            return colorMode;
        }
    }

    DunTpye DunTpye;
    private int sortIndex = 0;
    public int Sortpos
    {
        set
        {
            sortIndex = value;
            GetComponent<SpriteRenderer>().sortingOrder = 3 * sortIndex + 1;
            num.sortingOrder = 3 * sortIndex + 2;
            mark.GetComponent<SpriteRenderer>().sortingOrder = 3 * sortIndex + 3;
        }
        get
        {
            return sortIndex;
        }
    }

    private int Index_Num;
    private int Index_Color;
    void Awake()
    {
        num = transform.GetChild(0).GetComponent<SpriteRenderer>();
        mark = transform.GetChild(1).gameObject;
        mark.SetActive(false);
    }
    public void InitCard(int cardType, int Number, Sprite sprite)
    {
        num.sprite = sprite;
        CardType = cardType;
        CardNum = Number;

    }
    public void InitNumIndex(Vector3 pos, int index)
    {
        InitPos = pos;
        Index_Num = index;
        IsColorMode = false;
    }
    public void InitColorIndex(int index, Vector3 colorPos)
    {
        Index_Color = index;
        ColorPos = colorPos;
       
    }
    public void BeSeleted()
    {
        if (SetSele)
        {
            return;
        }
        SetSele = true;
        mark.SetActive(true);
        /* switch (state)
         {
             case Postype.Init:
                 mark.SetActive(true);              
                 break;
             case Postype.Besele:
                 mark.SetActive(true);
                 break;
             case Postype.BeUse:
                 break;
             default:
                 break;
         }*/

    }
    public void AISort(Vector3 Po)
    {
        TarPos = Po;
        state = Postype.BeUsed;
        mark.SetActive(false);
        ToUp = true;
    }
    public void BeUsed(Vector3 pos, DunTpye dun)
    {
        DunTpye = dun;
        Vector3 scr = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);

        scr.z = 0;

        scr.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        TarPos = Camera.main.ScreenToWorldPoint(scr);
        ToUp = true;
        state = Postype.BeUsed;
        //transform.position = tar;
        //transform.DOBlendableLocalMoveBy(tar, 1f);
    }
    public void SeleEnd()
    {
        if (!SetSele)
        {
            return;
        }
        SetSele = false;
        switch (state)
        {
            case Postype.Init:
                transform.position += new Vector3(0, 0.5f, 0);
                state = Postype.Beseleted;
                mark.SetActive(false);
                break;
            case Postype.Beseleted:
                transform.position -= new Vector3(0, 0.5f, 0);
                state = Postype.Init;
                mark.SetActive(false);
                break;
            case Postype.BeUsed:
                mark.SetActive(false);
                ToDown = true;
                Pokemgr.Instance.CardBack(this, DunTpye);
                break;
            default:
                break;
        }
    }
    public void GoBack()
    {
        ToDown = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (SetSele && Input.GetMouseButtonUp(0))
        {
            SeleEnd();
        }
        if (ToUp)
        {
            float dis = Vector3.Magnitude(transform.position - TarPos);
            transform.position = Vector3.Lerp(transform.position, TarPos, Time.deltaTime * 15f / dis);
            if (dis < 0.1f)
            {
                transform.position = TarPos;
                ToUp = false;
            }
        }
        if (ToDown)
        {
            float dis = Vector3.Magnitude(transform.position - DownPos);
            transform.position = Vector3.Lerp(transform.position, DownPos, Time.deltaTime * 15f / dis);
            if (dis < 0.1f)
            {
                transform.position = DownPos;
                state = Postype.Init;
                ToDown = false;
            }
        }
    }
}
