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

    //�P�D�l�����Ȃ��Ƃ�
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

    //�Q�DRecoding���̃f�U�C��
    public void RecordingUISetActive(bool flag) {
        RecordingUI.SetActive(flag);
    }

    //3�DSilhouette�B�e�m�F���
    public void CheckGifUI() {
        CheckGifUIObject.SetActive(true);
        PostComentUIObject.SetActive(false);
        CheckVoiceUIObject.SetActive(false);
    }


    //4�D�V���G�b�g�擾��̉�����Comment
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

    //6�D�������b�Z�[�W�m�F���
    public void CheckVoiceUI() {
        DefaultUI();
        CheckVoiceUIObject.SetActive(true);

    }

    //7�D���e��̉��
    public void AfterPostUI() {
        DefaultUI();
        AfterPostUIObject.SetActive(true);
        RePostButtonSetActive(true);
        ChangeQuestionButtonSetActive(true);
    }

    //�u�������́v�{�^��
    public void GetVoiceButtonSetActive(bool flag) {
        GetVoiceButtonObject.SetActive(flag);
    }
    //�u�����m�FOK�v�{�^��
    public void GetVoiceOKButtonSetActive(bool flag) {
        GetVoiceOKButtonObject.SetActive(flag);
    }
    //�u�Ę^���v�{�^��
    public void ReGetVoiceButtonSetActive(bool flag) {
        ReGetVoiceButtonObject.SetActive(flag);
    }
    //�u�ē��e�v�{�^��
    public void RePostButtonSetActive(bool flag) {
        RePostButtonObject.SetActive(flag);
    }

    public void ChangeQuestionButtonSetActive(bool flag) {
        ChangeQuestionButtonObject.SetActive(flag);
    }
}
