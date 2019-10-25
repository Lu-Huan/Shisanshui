using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    /// <summary>
    /// 自动理牌
    /// </summary>
    public class CardModel
    {
        public List<TypeCard> typeCardList;
        public CardModel()
        {
            typeCardList = new List<TypeCard>();
        }
    }

    /// <summary>
    /// 牌型
    /// </summary>
    public enum DeckTypeEnum : byte
    {
        /// <summary>
        /// 未识别
        /// </summary>
        Error = 255,
        /// <summary>
        /// 过牌为空
        /// </summary>
        None = 0,
        /// <summary>
        /// 高牌-散牌-乌龙1
        /// </summary>
        Single,
        /// <summary>
        /// 对子2
        /// </summary>
        Double,
        /// <summary>
        /// 两对3
        /// </summary>
        TwoDouble,
        /// <summary>
        /// 三张4
        /// </summary>
        Three,
        /// <summary>
        /// 顺子5
        /// </summary>
        ShunZi,
        /// <summary>
        /// 同花6
        /// </summary>
        TongHua,
        /// <summary>
        /// 葫芦7
        /// </summary>
        Gourd,
        /// <summary>
        /// 炸弹8
        /// </summary>
        Bomb,
        /// <summary>
        /// 同花顺9
        /// </summary>
        TongHuaShun
    }

    public class TypeCard
    {
        public DeckTypeEnum cardType;//牌型
        public List<int> cardList;//手牌
    }
    class CaculalAI
    {

        /// <summary>
        /// 牌面从大到小排序
        /// </summary>
        public static void SortCard(List<int> cards)
        {
            //牌面从大到小排序
            cards.Sort((b, a) =>
            {
                int result = (a % 100) - (b % 100);
                // if (result == 0)
                // {
                //     result = (a / 100) - (b / 100);
                // }
                return result;
            });
        }

        /// <summary>
        /// 牌面从小到大排序
        /// </summary>
        public static void SortCardMinToMax(List<int> cards)
        {
            //面大小排序
            cards.Sort((a, b) =>
            {
                int result = (a % 100) - (b % 100);
                // if (result == 0)
                // {
                //     result = (b / 100) - (a / 100);
                // }
                return result;
            });
        }

        /// <summary>
        /// 计算一副牌里面的所有可能牌型
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        public static List<TypeCard> GetMaxCardType(List<int> cardList)
        {
            List<TypeCard> typeCardList = new List<TypeCard>();

            //从大到小排序
            SortCard(cardList);

            //复制一份从小到大的牌
            List<int> cardList2 = cardList.GetRange(0, cardList.Count);
            SortCardMinToMax(cardList2);

            List<int> FourNum = cardList.GroupBy(p => p % 100).Where(p => p.Count() >= 4).Select(p => p.Key).ToList();
            List<int> ThreeNum = cardList.GroupBy(p => p % 100).Where(p => p.Count() >= 3).Select(p => p.Key).ToList();
            List<int> TwoNum = cardList.GroupBy(p => p % 100).Where(p => p.Count() >= 2).Select(p => p.Key).ToList();
            #region 同花顺
            foreach (var item in cardList2)
            {
                if (item % 100 >= 11)
                    break;

                List<int> tempList = new List<int>();
                tempList.Add(item);

                foreach (var item2 in cardList2)
                {
                    if (item2 - 1 == tempList.Last())
                        tempList.Add(item2);

                    if (tempList.Count >= 5)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.TongHuaShun };
                        typeCardList.Add(typeCard);
                        break;
                    }
                }
            }
            #endregion

            #region 炸弹
            foreach (var FourItem in FourNum)
            {
                List<int> tempList = new List<int>();
                foreach (var item in cardList)
                {
                    if (item % 100 == FourItem)
                        tempList.Add(item);

                    if (tempList.Count >= 4)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Bomb };
                        typeCardList.Add(typeCard);
                        break;
                    }
                }

            }
            #endregion

            #region 葫芦
            foreach (var ThreeItem in ThreeNum)
            {
                List<int> tempList = new List<int>();
                foreach (var item in cardList)
                {
                    if (item % 100 == ThreeItem)
                        tempList.Add(item);

                    if (tempList.Count >= 3)
                    {
                        //找两对
                        foreach (var TwoItem in TwoNum)
                        {
                            if (ThreeItem != TwoItem)
                            {
                                foreach (var AllItem in cardList)
                                {
                                    if (AllItem % 100 != ThreeItem && AllItem % 100 == TwoItem)
                                        tempList.Add(AllItem);

                                    if (tempList.Count >= 5)
                                    {
                                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Gourd };
                                        typeCardList.Add(typeCard);
                                        tempList = typeCard.cardList.GetRange(0, 3);
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            #endregion

            #region 同花
            List<int> FiveColor = cardList.GroupBy(p => p / 100).Where(p => p.Count() >= 5).Select(p => p.Key).ToList();
            List<int> FourColor = cardList.GroupBy(p => p / 100).Where(p => p.Count() >= 4).Select(p => p.Key).ToList();
            List<int> ThreeColor = cardList.GroupBy(p => p / 100).Where(p => p.Count() >= 3).Select(p => p.Key).ToList();

            foreach (var Fiveitem in FiveColor)
            {
                List<int> tempList = new List<int>();
                foreach (var item in cardList)
                {
                    if (item / 100 == Fiveitem)
                    {
                        tempList.Add(item);
                    }
                }

                while (true)
                {
                    if (tempList.Count >= 5)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList.GetRange(0, 5), cardType = DeckTypeEnum.TongHua };
                        typeCardList.Add(typeCard);
                        tempList.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            #endregion

            #region 顺子

            foreach (var MaxItem in cardList)
            {

                if (MaxItem % 100 <= 5)
                {
                    break;//从大到小，后面的也不需要循环了
                }

                List<int> tempList = new List<int>();
                tempList.Add(MaxItem);

                while (tempList.Count < 5)
                {
                    bool flag = false;
                    foreach (var item in cardList)
                    {
                        if (item % 100 + 1 == tempList.Last() % 100)
                        {
                            tempList.Add(item);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                }

                if (tempList.Count >= 5)
                {
                    TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.ShunZi };
                    typeCardList.Add(typeCard);
                }
            }

            #endregion

            #region 三张
            foreach (var Threeitem in ThreeNum)
            {
                List<int> tempList = new List<int>();
                foreach (var item in cardList)
                {
                    if (item % 100 == Threeitem)
                        tempList.Add(item);

                    if (tempList.Count >= 3)
                    {
                        TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Three };
                        typeCardList.Add(typeCard);
                        break;
                    }
                }
            }

            #endregion

            #region 两对

            for (int i = 0; i < TwoNum.Count; i++)
            {
                List<int> tempList = new List<int>();

                foreach (var item in cardList)
                {
                    if (item % 100 == TwoNum[i])
                    {
                        tempList.Add(item);
                    }
                }

                while (tempList.Count >= 2)
                {

                    for (int j = i + 1; j < TwoNum.Count; j++)
                    {
                        List<int> tempList2 = new List<int>();
                        foreach (var item in cardList)
                        {
                            if (item % 100 == TwoNum[j])
                            {
                                tempList2.Add(item);
                            }
                        }

                        while ((tempList2.Count >= 2))
                        {
                            TypeCard typeCard = new TypeCard() { cardList = tempList.GetRange(0, 2), cardType = DeckTypeEnum.TwoDouble };
                            typeCard.cardList.AddRange(tempList2.GetRange(0, 2));
                            typeCardList.Add(typeCard);
                            tempList2.RemoveAt(0);
                        }

                    }

                    tempList.RemoveAt(0);

                }

            }

            #endregion

            #region 对子

            foreach (var Twoitem in TwoNum)
            {
                List<int> tempList = new List<int>();

                foreach (var item in cardList)
                {
                    if (item % 100 == Twoitem)
                    {
                        tempList.Add(item);
                    }
                }

                while (tempList.Count >= 2)
                {
                    TypeCard typeCard = new TypeCard() { cardList = tempList.GetRange(0, 2), cardType = DeckTypeEnum.Double };
                    typeCardList.Add(typeCard);
                    tempList.RemoveAt(0);
                }
            }

            #endregion

            //乌龙-只拿三张牌
            if (true)
            {
                if (cardList.Count < 3)
                {
                    Console.WriteLine("*****************************************************************最后剩牌不足三张");
                }
                else
                {
                    List<int> tempList = new List<int>();
                    tempList = cardList.GetRange(0, 3);
                    TypeCard typeCard = new TypeCard() { cardList = tempList, cardType = DeckTypeEnum.Single };
                    typeCardList.Add(typeCard);
                }
            }
            return typeCardList;
        }


        /// <summary>
        /// 删除元素
        /// </summary>
        /// <param name="list"></param>
        /// <param name="element"></param>
        public static void DeleteListElement(List<int> list, List<int> element)
        {
            foreach (var item in element)
            {
                list.Remove(item);
            }
        }


        /// <summary>
        /// 得到所有自动牌型
        /// </summary>
        /// <param name="cmlist"></param>
        /// <param name="CardList"></param>
        /// <returns></returns>
        public static List<CardModel> GetAllResult(List<CardModel> cmlist, List<int> CardList)
        {
            CardModel cm = new CardModel();
            List<TypeCard> typeCardList = new List<TypeCard>();

            //新建一个副本
            List<int> newCardlist = CardList.Select(p => p).ToList();

            //第一个
            if (cmlist.Count == 0)
            {
                while (true)
                {

                    typeCardList = GetMaxCardType(newCardlist);

                    //首道只能有三张牌
                    if (cm.typeCardList.Count == 2 && typeCardList[0].cardList.Count > 3)
                    {
                        typeCardList = GetMaxCardType(typeCardList[0].cardList.GetRange(0, 3));
                    }

                    cm.typeCardList.Add(typeCardList[0]);
                    DeleteListElement(newCardlist, typeCardList[0].cardList);

                    //补全少的牌
                    if (cm.typeCardList.Count >= 3)
                    {
                        for (int i = 0; i < cm.typeCardList.Count; i++)
                        {
                            if (i == 2)
                            {
                                while (cm.typeCardList[i].cardList.Count < 3)
                                {
                                    cm.typeCardList[i].cardList.Add(newCardlist[0]);
                                    newCardlist.RemoveAt(0);
                                }
                            }
                            else
                            {
                                while (cm.typeCardList[i].cardList.Count < 5)
                                {
                                    cm.typeCardList[i].cardList.Add(newCardlist[0]);
                                    newCardlist.RemoveAt(0);
                                }
                            }

                        }

                        break;
                    }
                }

                cmlist.Add(cm);

                GetAllResult(cmlist, CardList);
            }


            //更改该值，可获取更多的组牌方式
            if (cmlist.Count < 1)
            {
                int SingleCount = 0;//乌龙次数
                while (true)
                {
                    typeCardList = GetMaxCardType(newCardlist);
                    int index = cmlist.Count;//第几组自动
                    int typeCardListIndex = 0;

                    if (cm.typeCardList.Count == 3)
                    {

                    }
                    else if (cm.typeCardList.Count == 0)
                    {
                        if (typeCardList.Count < index)
                        {
                            return cmlist;
                        }

                        cm.typeCardList.Add(typeCardList[index]);
                        DeleteListElement(newCardlist, typeCardList[index].cardList);
                    }
                    else
                    {
                        //前道的牌不能大于后道
                        while (true)
                        {
                            if (typeCardList[typeCardListIndex].cardType > cm.typeCardList.Last().cardType)
                            {
                                typeCardListIndex++;
                                continue;
                            }
                            if (typeCardList[typeCardListIndex].cardType == cm.typeCardList.Last().cardType)
                            {
                                if (typeCardList[typeCardListIndex].cardList.Max() >= cm.typeCardList.Last().cardList.Max())
                                {
                                    return cmlist;
                                }
                            }

                            break;
                        }

                        //首道只能有三张牌
                        if (cm.typeCardList.Count == 2 && typeCardList[typeCardListIndex].cardList.Count > 3)
                        {
                            typeCardList = GetMaxCardType(typeCardList[typeCardListIndex].cardList.GetRange(0, 3));
                            cm.typeCardList.Add(typeCardList[0]);
                            DeleteListElement(newCardlist, typeCardList[0].cardList);
                        }
                        else
                        {
                            cm.typeCardList.Add(typeCardList[typeCardListIndex]);
                            DeleteListElement(newCardlist, typeCardList[typeCardListIndex].cardList);
                        }

                    }

                    //补全不够牌的
                    if (cm.typeCardList.Count >= 3)
                    {
                        for (int i = 0; i < cm.typeCardList.Count; i++)
                        {
                            if (i == 2)
                            {
                                while (cm.typeCardList[i].cardList.Count < 3)
                                {
                                    cm.typeCardList[i].cardList.Add(newCardlist[0]);
                                    newCardlist.RemoveAt(0);
                                }
                            }
                            else
                            {
                                while (cm.typeCardList[i].cardList.Count < 5)
                                {
                                    cm.typeCardList[i].cardList.Add(newCardlist[0]);
                                    newCardlist.RemoveAt(0);
                                }
                            }

                            if (cm.typeCardList[i].cardType == DeckTypeEnum.Single)
                            {
                                SingleCount++;
                            }

                        }

                        break;
                    }
                }

                cmlist.Add(cm);

                if (SingleCount >= 2)
                {
                    return cmlist;
                }

                GetAllResult(cmlist, CardList);

            }

            return cmlist;
        }
    }
}