using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMessages : MonoBehaviour
{
    public GameObject Text;
    public GameObject contentPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void prefixedMessageOne()
    {
        whatMessage("Hi");
    }

    public void prefixedMessageTwo()
    {
        whatMessage("GG");
    }

    public void prefixedMessageThree()
    {
        whatMessage("What");
    }

    private void whatMessage(string a)
    {
        GameObject temp = Instantiate(Text, contentPanel.transform);
        temp.transform.SetParent(contentPanel.transform);
        temp.GetComponent<Text>().text = a;
    }
}
