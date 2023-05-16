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

    // �d�Ȃ�u�Ԕ���
    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Enter:" + other.name + this.HoverButtonColor32);
        PointerInFlag = true;
        InStarttime = DateTime.Now;
        BtnCollider2D = other;
        this.target_circle = other.gameObject.transform.GetChild(0).Find("CircleImage").GetComponent<Image>();
        other.gameObject.GetComponent<Renderer>().material.color = this.HoverButtonColor32;
    }

    // �d�Ȃ藣�E�̔���
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
            //Silhouette�B�e
            case "CheckGif_ReTake_Button":
                Debug.Log("�ĎB�e�{�^��Click");
                this.postmaneger.PushbuttonFlag = true;
                this.postmaneger.ReTakeGif();
                dbc.SetOperationLog("RetakeBtn_Silhouette");
                break;
            case "CheckGif_OK_Button":
                Debug.Log("�B�eOK�{�^��Click");
                this.postmaneger.PushbuttonFlag = true;
                this.postmaneger.PostGif();
                dbc.SetOperationLog("OkBtn_Silhouette");
                //���e�pUI�\��
                postmaneger.StartComentUI();
                break;


            //��������
            case "GetVoiceButton":
                Debug.Log("�������̓{�^��Click");
                this.getvoice.getVoice();//�����F���̊J�n
                break;
            case "ReGetVoiceButton":
                Debug.Log("�ĎB�e�{�^��Click");
                this.getvoice.PushButton = true;
                this.getvoice.RegetVoice();//�����F���̊J�n
                dbc.SetOperationLog("RetakeBtn_Voice");
                break;
            case "VoiceOKButton":
                Debug.Log("����OKClick");
                this.getvoice.PushButton = true;
                this.getvoice.OKVoice();//�����F���̊J�n
                dbc.SetOperationLog("OkBtn_Voice");
                break;

            //���̑�
            case "RePostButton":
                Debug.Log("�ē��e�{�^���N���b�N");
                dbc.SetOperationLog("RePostBtn");
                //�V���G�b�g�B�e�J�n
                postmaneger.RePost();
                break;
            case "ChangeQuestionButton":
                Debug.Log("����ύX�{�^���N���b�N");
                dbc.SetOperationLog("NextQuestionBtn");
                //�V���G�b�g�B�e�J�n
                postmaneger.ChangeQuestion();
                break;
            default: //num���u1�v�Ɓu2�v�ȊO�̎��Ɏ��s����
                Debug.Log("ERR:" + btnname + "is not found in Button");
                break;//switch���𔲂���
        }
    }
}
