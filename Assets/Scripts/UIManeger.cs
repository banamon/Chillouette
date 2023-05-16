using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManeger : MonoBehaviour
{
    //UI
    [SerializeField] GameObject CheckGifUIObject;
    [SerializeField] GameObject PostComentUIObject;
    [SerializeField] GameObject DefaultUIObject;
    [SerializeField] GameObject CheckVoiceUIObject;
    [SerializeField] GameObject AfterPostUIObject;

    //Button
    [SerializeField] GameObject GetVoiceButtonObject;
    [SerializeField] GameObject GetVoiceOKButtonObject;
    [SerializeField] GameObject ReGetVoiceButtonObject;
    [SerializeField] GameObject RePostButtonObject;
    [SerializeField] GameObject ChangeQuestionButtonObject;

    //Other
    [SerializeField] GameObject RecordingUI;

    // Start is called before the first frame update
    void Start()
    {
        DefaultUI();
        DefaultUIObject.SetActive(true);
        CheckVoiceUIObject.SetActive(false);
    }

    //１．人がいないとき
    public void DefaultUI() {
        CheckGifUIObject.SetActive(false);
        PostComentUIObject.SetActive(false);
        CheckVoiceUIObject.SetActive(false);
        AfterPostUIObject.SetActive(false);
        GetVoiceButtonSetActive(false);
        GetVoiceOKButtonSetActive(false);
        ReGetVoiceButtonSetActive(false);
        RePostButtonSetActive(false);
        ChangeQuestionButtonSetActive(false);
    }

    //２．Recoding中のデザイン
    public void RecordingUISetActive(bool flag) {
        RecordingUI.SetActive(flag);
    }

    //3．Silhouette撮影確認画面
    public void CheckGifUI() {
        CheckGifUIObject.SetActive(true);
        PostComentUIObject.SetActive(false);
        CheckVoiceUIObject.SetActive(false);
    }


    //4．シルエット取得後の音声入Comment
    public void PostComentUI() {
        Debug.Log("PostComentUI");
        CheckGifUIObject.SetActive(false);
        GetVoiceButtonSetActive(true);
        ChangeQuestionButtonSetActive(true);

        PostComentUIObject.SetActive(true);
        CheckVoiceUIObject.SetActive(false);
        RePostButtonSetActive(false);
        GetVoiceOKButtonSetActive(false);
        ReGetVoiceButtonSetActive(false);
    }

    //6．音声メッセージ確認画面
    public void CheckVoiceUI() {
        DefaultUI();
        CheckVoiceUIObject.SetActive(true);

    }

    //7．投稿後の画面
    public void AfterPostUI() {
        DefaultUI();
        AfterPostUIObject.SetActive(true);
        RePostButtonSetActive(true);
        ChangeQuestionButtonSetActive(true);
    }

    //「音声入力」ボタン
    public void GetVoiceButtonSetActive(bool flag) {
        GetVoiceButtonObject.SetActive(flag);
    }
    //「音声確認OK」ボタン
    public void GetVoiceOKButtonSetActive(bool flag) {
        GetVoiceOKButtonObject.SetActive(flag);
    }
    //「再録音」ボタン
    public void ReGetVoiceButtonSetActive(bool flag) {
        ReGetVoiceButtonObject.SetActive(flag);
    }
    //「再投稿」ボタン
    public void RePostButtonSetActive(bool flag) {
        RePostButtonObject.SetActive(flag);
    }

    public void ChangeQuestionButtonSetActive(bool flag) {
        ChangeQuestionButtonObject.SetActive(flag);
    }
}
