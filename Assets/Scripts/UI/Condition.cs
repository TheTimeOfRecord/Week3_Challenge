using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue;//�ֱ������� ���ϴ� ��
    public Image uiBar;

    // Start is called before the first frame update
    void Start()
    {
        curValue = startValue;
    }

    // Update is called once per frame
    void Update()
    {
        //UI ������Ʈ
        uiBar.fillAmount = GetPercentage();
    }
    
    float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = curValue + value;
        curValue = Mathf.Clamp(curValue, 0, maxValue);
    }
}
