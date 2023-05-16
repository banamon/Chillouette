using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClickManeger : MonoBehaviour {
    bool PointerInFlag = false;

    private Collider2D BtnCollider2D;
    private DateTime InStarttime = DateTime.MinValue;
    TimeSpan timeElapsed;
    private float maxtime = 3f;

    protected GameObject GetVoiceObject;  // Manager Object
    public GetVoice getvoice;

    protected GameObject PostManegerObject;  // Manager Object
    public PostManeger postmaneger;

    protected GameObject DBCObject;  // Manager Object
    protected DBC dbc;

    Image target_circle;

    //Color32 HoverButtonColor32 = new Color32(253,255,160,255);
    Color32 HoverButtonColor32 = new Color32(253, 255, 140, 255);
    Color32 DefaltButtonColor32 = new Color32(255,255,255,255);


    private void Start() {
        this.GetVoiceObject = GameObject.Find("GetVoiceObject");
        this.PostManegerObject = GameObject.Find("PostManegerObject");
        this.DBCObject = GameObject.Find("DBCObject");
        this.getvoice = GetVoiceObject.GetComponent<GetVoice>();
        this.postmaneger = PostManegerObject.GetComponent<PostManeger>();
        this.dbc = DBCObject.GetComponent<DBC>();
    }

    // Update is called once per frame
    void Update() {
        if (PointerInFlag) {
            timeElapsed = DateTime.Now - InStarttime;
            this.target_circle.fillAmount = (float)timeElapsed.TotalSeconds / maxtime;
            if (timeElapsed.TotalSeconds > maxtime) {
                Debug.Log("Click" + BtnCollider2D.name);
                ClickButton(BtnCollider2D.name);

                PointerInFlag = false;
                InStarttime = DateTime.MinValue;
            }
        }
        else if (this.target_circle != null) {
                this.target_circle.fillAmount = 0;
            
        }
    }

    // 重なり瞬間判定
    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Enter:" + other.name + this.HoverButtonColor32);
        PointerInFlag = true;
        InStarttime = DateTime.Now;
        BtnCollider2D = other;
        this.target_circle = other.gameObject.transform.GetChild(0).Find("CircleImage").GetComponent<Image>();
        other.gameObject.GetComponent<Renderer>().material.color = this.HoverButtonColor32;
    }

    // 重なり離脱の判定
    void OnTriggerExit2D(Collider2D other) {
        Debug.Log("Exit:" + other.name);
        PointerInFlag = false;
        InStarttime = DateTime.MinValue;
        BtnCollider2D = null;
        other.gameObject.transform.GetChild(0).Find("CircleImage").GetComponent<Image>().fillAmount = 0;
        other.gameObject.GetComponent<Renderer>().material.color = this.DefaltButtonColor32;
    }

    void ClickButton(string btnname) {
        switch (btnname) {
            //Silhouette撮影
            case "CheckGif_ReTake_Button":
                Debug.Log("再撮影ボタンClick");
                this.postmaneger.PushbuttonFlag = true;
                this.postmaneger.ReTakeGif();
                dbc.SetOperationLog("RetakeBtn_Silhouette");
                break;
            case "CheckGif_OK_Button":
                Debug.Log("撮影OKボタンClick");
                this.postmaneger.PushbuttonFlag = true;
                this.postmaneger.PostGif();
                dbc.SetOperationLog("OkBtn_Silhouette");
                //投稿用UI表示
                postmaneger.StartComentUI();
                break;


            //音声入力
            case "GetVoiceButton":
                Debug.Log("音声入力ボタンClick");
                this.getvoice.getVoice();//音声認識の開始
                break;
            case "ReGetVoiceButton":
                Debug.Log("再撮影ボタンClick");
                this.getvoice.PushButton = true;
                this.getvoice.RegetVoice();//音声認識の開始
                dbc.SetOperationLog("RetakeBtn_Voice");
                break;
            case "VoiceOKButton":
                Debug.Log("音声OKClick");
                this.getvoice.PushButton = true;
                this.getvoice.OKVoice();//音声認識の開始
                dbc.SetOperationLog("OkBtn_Voice");
                break;

            //その他
            case "RePostButton":
                Debug.Log("再投稿ボタンクリック");
                dbc.SetOperationLog("RePostBtn");
                //シルエット撮影開始
                postmaneger.RePost();
                break;
            case "ChangeQuestionButton":
                Debug.Log("お題変更ボタンクリック");
                dbc.SetOperationLog("NextQuestionBtn");
                //シルエット撮影開始
                postmaneger.ChangeQuestion();
                break;
            default: //numが「1」と「2」以外の時に実行する
                Debug.Log("ERR:" + btnname + "is not found in Button");
                break;//switch文を抜ける
        }
    }
}
