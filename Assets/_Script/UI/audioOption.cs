using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class audioOption : Singleton<audioOption>
{
    public GameObject systemGroup;
    public GameObject audioGroup;

    public Slider slider_masterVolume;
    public Slider slider_bgmVolume;
    public Slider slider_effectVolume;

    public float masterVolume;
    public float bgmVolume;
    public float effectVolume;

    public Text masterVol_text;
    public Text bgmVol_text;
    public Text effectVol_text;
    
    void Start()
    {
        masterVolume = GameManager.instance.optionSettingData.masterVolume_setting;
        bgmVolume = GameManager.instance.optionSettingData.bgmVolume_setting;
        effectVolume = GameManager.instance.optionSettingData.effectVolume_setting;

        slider_masterVolume.value = masterVolume;
        slider_bgmVolume.value = bgmVolume;
        slider_effectVolume.value = effectVolume;

        masterVol_text.text = (masterVolume*10).ToString();
        bgmVol_text.text = (bgmVolume*10).ToString();
        effectVol_text.text = (effectVolume*10).ToString();

        audioGroup.SetActive(false);
    }

    void Update()
    {
        
    }

    public void onClickSelf()
    {
        UIManager.instance.clickSoundGen();

        //���� ������ ���� ���� ������ �� 
        audioGroup.SetActive(!audioGroup.activeSelf);

        //������ɼ� â �̿��� �ٸ� �ɼ��� ��� �� 
        systemGroup.SetActive(false);
    }

    public void OnDisable()
    {
        //â�� ������ �ý��� �ɼ�â�� ���� 
        audioGroup.SetActive(false);
    }

    public void sliderCheck_master()
    {
        slider_masterVolume.value = Mathf.Round(slider_masterVolume.value * 10) / 10f; //�����̴����� �Ҽ��� ���� ù��° �ڸ����� �ݿø� 
        masterVolume = slider_masterVolume.value; //�����̴��� 0.1 �����θ� ������ 

        GameManager.instance.optionSettingData.masterVolume_setting = masterVolume;
        masterVol_text.text = (masterVolume * 10).ToString();

        //�ٲ� �����ͺ��� ���� ���� 
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.optionSettingData);
        string filePath = Application.persistentDataPath + GameManager.instance.optionFileName;
        File.WriteAllText(filePath, ToJsonData);
    }

    public void sliderCheck_bgm()
    {
        slider_bgmVolume.value = Mathf.Round(slider_bgmVolume.value * 10) / 10f; //�����̴����� �Ҽ��� ���� ù��° �ڸ����� �ݿø� 
        bgmVolume = slider_bgmVolume.value;

        GameManager.instance.optionSettingData.bgmVolume_setting = bgmVolume;
        bgmVol_text.text = (bgmVolume * 10).ToString();

        //�ٲ� bgm���� ���� ���� 
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.optionSettingData);
        string filePath = Application.persistentDataPath + GameManager.instance.optionFileName;
        File.WriteAllText(filePath, ToJsonData);
    }

    public void sliderCheck_effect()
    {
        slider_effectVolume.value = Mathf.Round(slider_effectVolume.value * 10) / 10f;
        effectVolume = slider_effectVolume.value;

        GameManager.instance.optionSettingData.effectVolume_setting = effectVolume;
        effectVol_text.text = (effectVolume * 10).ToString();

        //�ٲ� ����Ʈ���� ���� ���� 
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.optionSettingData);
        string filePath = Application.persistentDataPath + GameManager.instance.optionFileName;
        File.WriteAllText(filePath, ToJsonData);
    }
}
