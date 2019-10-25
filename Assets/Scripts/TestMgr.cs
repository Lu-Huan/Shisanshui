using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMgr : MonoBehaviour
{
    public bool IsTest;
    // Start is called before the first frame update
    void Start()
    {
        if (IsTest)
        {
            NetMgr.TokenDate = "fd1157a6 - 90d2 - 44b8 - b702 - 6904a1f76a82";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
