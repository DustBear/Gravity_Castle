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

        //켜져 있으면 끄고 꺼져 있으면 켬 
        audioGroup.SetActive(!audioGroup.activeSelf);

        //오디오옵션 창 이외의 다른 옵션은 모두 끔 
        systemGroup.SetActive(false);
    }

    public void OnDisable()
    {
        //창을 닫으면 시스템 옵션창도 꺼짐 
        audioGroup.SetActive(false);
    }

    public void sliderCheck_master()
    {
        slider_masterVolume.value = Mathf.Round(slider_masterVolume.value * 10) / 10f; //슬라이더값을 소숫점 이하 첫번째 자리에서 반올림 
        masterVolume = slider_masterVolume.value; //슬라이더는 0.1 단위로만 움직임 

        GameManager.instance.optionSettingData.masterVolume_setting = masterVolume;
        masterVol_text.text = (masterVolume * 10).ToString();

        //바뀐 마스터볼륨 설정 저장 
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.optionSettingData);
        string filePath = Application.persistentDataPath + GameManager.instance.optionFileName;
        File.WriteAllText(filePath, ToJsonData);
    }

    public void sliderCheck_bgm()
    {
        slider_bgmVolume.value = Mathf.Round(slider_bgmVolume.value * 10) / 10f; //슬라이더값을 소숫점 이하 첫번째 자리에서 반올림 
        bgmVolume = slider_bgmVolume.value;

        GameManager.instance.optionSettingData.bgmVolume_setting = bgmVolume;
        bgmVol_text.text = (bgmVolume * 10).ToString();

        //바뀐 bgm볼륨 설정 저장 
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

        //바뀐 이펙트볼륨 설정 저장 
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.optionSettingData);
        string filePath = Application.persistentDataPath + GameManager.instance.optionFileName;
        File.WriteAllText(filePath, ToJsonData);
    }
}
