using System.Collections;
using System.Collections.Generic;
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
    private List<Vector3> listpos = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        Transform[] tras = transform.GetChild(1).GetComponentsInChildren<Transform>();
        //Debug.Log(ListPos.Length);
        for (int i = 1; i < tras.Length; i++)
        {
            listpos.Add(tras[i].position);
        }
        show = transform.GetChild(2).gameObject;
        show.SetActive(false);
        Pokemgr.Instance.UIshow += Show;
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

    public void Show(int r)
    {
        canPut = r == count;
        show.SetActive(canPut&&!beuse);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
