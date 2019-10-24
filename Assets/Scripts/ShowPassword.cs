using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPassword : MonoBehaviour
{
    private InputField Input;

    // Start is called before the first frame update
    void Start()
    {
        Input = transform.parent.GetComponent<InputField>();
        GetComponent<Toggle>().onValueChanged.AddListener((bool isShow) =>
        {
            Input.contentType = isShow ? InputField.ContentType.Password : InputField.ContentType.Standard;
            Input.MoveTextEnd(true);
        });
    }
    // Update is called once per frame
    void Update()
    {

    }
}
