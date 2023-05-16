using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;   //Windows�̉����F���Ŏg�p
public class GetVoice : MonoBehaviour {
    //�F�����u���N�����A�ŏ���5�b�ԉ������������Ȃ��ꍇ�́A�^�C���A�E�g�ɂȂ�B
    //�F�����u�����ʂ�^������20�b�Ԗ����𕷂��ƁA�F�����u�̓^�C���A�E�g����B

    public static GetVoice instance;
    DictationRecognizer dictationRecognizer;

    [SerializeField] Text GetVoiceText;
    //Text GetVoiceText;

    //[SerializeField] PostScript postscript;
    [SerializeField] GameObject SpeechbubbleObject;
    [SerializeField] GameObject GetVoiceAnimationObject;
    Animator anime;

    [SerializeField] public GameObject UIManegerObject;
    UIManeger UIM;

    [SerializeField] private GameObject PostManegerObject;
    private PostManeger postmaneger;

    [SerializeField] GameObject CheckVoiceObject;
    [SerializeField] Slider _slider;

    [SerializeField] protected GameObject DBCObject;  // Manager Object
    protected DBC dbc;

    //�����F�����ɉ����Ȃ��ꍇ�́C30�b�������玩���ŏI���
    //�i����WindowsSpeech��InitialSilenceTimeoutSeconds�����΂����̂ł́H�j
    private DateTime StartGetVoiceTime;
    private float StopGetVoiceSeconds = 30;
    private int GetVoiceNum = 0;

    //�m�F��ʂň�莞�ԕ��u�����Ǝ����ő��M����
    public string VoiceMessage;
    private int waittime = 20;

    private IEnumerator CheckVoiceCoroutine;


    public bool PushButton = false;
    // Start is called before the first frame update
    void Start() {

        postmaneger = PostManegerObject.GetComponent<PostManeger>();
        this.UIM = UIManegerObject.GetComponent<UIManeger>();
        this.dbc = this.DBCObject.GetComponent<DBC>();
        anime = GetVoiceAnimationObject.transform.GetChild(0).GetComponent<Animator>();

        //������
        SpeechbubbleObject.SetActive(false);
        GetVoiceAnimationObject.SetActive(false);
        this.UIM.GetVoiceButtonSetActive(false);
        this.UIM.RePostButtonSetActive(false);
        this.UIM.GetVoiceOKButtonSetActive(false);


        dictationRecognizer = new DictationRecognizer();
    }

    // �^���{�^���Ăяo���Ŏ��s
    public void getVoice() {
        //�������͎��Ԃ̎擾
        if (GetVoiceNum == 0) {
            this.StartGetVoiceTime = DateTime.Now;
        }
        GetVoiceNum++;


        //UI

        //�������̓{�^����\��
        //�������͒�UI�\��
        this.UIM.CheckVoiceUI();
        this.UIM.GetVoiceButtonSetActive(false);
        this.UIM.GetVoiceOKButtonSetActive(false);
        this.UIM.ReGetVoiceButtonSetActive(false);
        CheckVoiceObject.SetActive(false);
        SpeechbubbleObject.SetActive(true);
        
        //�F�����A�j���[�V�����Đ�
        GetVoiceAnimationObject.SetActive(true);
        anime.enabled = true;
        anime.Play("RawImage");

        //���낢�돉����
        GetVoiceText.text = "";

        //������
        dictationRecognizer = new DictationRecognizer();

        dbc.SetOperationLog("StartTake_Voice");


        //�F���J�n
        Debug.Log("�����F���J�n");
        dictationRecognizer.Start();
        dictationRecognizer.AutoSilenceTimeoutSeconds = 120;//�������͂��Ȃ����߂ɉ����F�����I������ۂ́A�I������O�̌o�ߎ��� (�b)
        dictationRecognizer.InitialSilenceTimeoutSeconds = 20;//���݂̃Z�b�V�����ŉ������͂��܂������Ȃ����߂ɉ����F�����I������ۂ́A�I������O�̌o�ߎ��� (�b)
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;//DictationRecognizer_DictationResult�������s��
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;//DictationRecognizer_DictationHypothesis�������s��
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;//DictationRecognizer_DictationComplete�������s��
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;//DictationRecognizer_DictationError�������s��
    }

    //DictationResult�F����������̔F�����x�ŔF�����ꂽ�Ƃ��ɔ�������C�x���g
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence) {
        Debug.Log("�F�����������F" + text);
        GetVoiceText.text = "";

        VoiceMessage = text;
        dictationRecognizer.Stop();
        GetVoiceText.text = VoiceMessage;
    }

    //DictationHypothesis�F�������͒��ɔ�������C�x���g
    private void DictationRecognizer_DictationHypothesis(string text) {
        Debug.Log("�����F�����F" + text);
        GetVoiceText.text = text;
    }

    //DictationComplete�F�����F���Z�b�V�������I�������Ƃ��Ƀg���K�����C�x���g
    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause) {
        Debug.Log("�����F������");
        StopVoice();

        string themessage = GetVoiceText.text;

        TimeSpan t = DateTime.Now - StartGetVoiceTime;
        if (t.TotalSeconds > StopGetVoiceSeconds) {
            //�^�C���A�E�g
            Debug.Log("TimeOut�ł�...");
            GetVoiceNum = 0;
            this.UIM.AfterPostUI();
            return;
        }

        if (themessage == "") {
            //�ēx�����F��
            Debug.Log("�������������F���J�n");
            getVoice();
        }
        else {
            //�����F���̊m�F
            dbc.SetOperationLog("EndTake_Voice");

            //PQ.CheckVoiceMessage(themessage);
            this.CheckVoiceCoroutine = CheckVoiceMessage(themessage);
            this.CheckVoiceObject.SetActive(true);
            StartCoroutine(this.CheckVoiceCoroutine);
            GetVoiceNum = 0;

            Debug.Log("�����F���e�L�X�g�F" + themessage);
            //�ĔF���{�^���\��
        }
    }

    //DictationError�F�����F���Z�b�V�����ɃG���[�����������Ƃ��Ƀg���K�����C�x���g
    private void DictationRecognizer_DictationError(string error, int hresult) {
        Debug.Log("�����F���G���[");
    }

    //�������͂̒�~
    public void StopVoice() {
        Debug.Log("�����F���̋����I��");
        Debug.Log("�X���b�hID:" + Thread.CurrentThread.ManagedThreadId);
        //dictationRecognizer.Stop();
        dictationRecognizer.Dispose();

        //�A�j���[�V������~
        anime.enabled = true;
        GetVoiceAnimationObject.SetActive(false);
    }


    private IEnumerator CheckVoiceMessage(string themessage) {
        this.UIM.CheckVoiceUI();
        this.UIM.ReGetVoiceButtonSetActive(true);
        this.UIM.GetVoiceOKButtonSetActive(true);
        int startSID = IDManeger.SID;

        Debug.Log("CheckVoiceMessage " + themessage);
        for (int i = 0; i < waittime*2; i++) {
            if (PushButton) {
                Debug.Log("�{�^�����������m�I�X���b�h�~�߂܂��I");
                PushButton = false;
                _slider.value = 1;
                yield break;
            } else if (startSID != IDManeger.SID ||  !postmaneger.sectionflag) {
                Debug.Log("�Z�N�V�����I�������m!�@�������e���Ƃ��܂��I");
                postmaneger.PostVoiceMessage(themessage);
                _slider.value = 1;
                yield break;

            }

            yield return new WaitForSeconds(0.5f);
            _slider.value -= (float) 0.5 / waittime;
            Debug.Log(_slider.value);
        }

        Debug.Log("Retake�����ꂸ��莞�Ԍo�߂����̂œ��e�������L�q���܂�");
        dbc.SetOperationLog("TimeOut_Voice");

        _slider.value = 1;

        SpeechbubbleObject.SetActive(false);
        PostMessage(themessage);
    }

    public void StopCheckVoiceCoroutine() {
        if (this.CheckVoiceCoroutine != null) {
            StopCoroutine(this.CheckVoiceCoroutine);
        }
    }

    public void RegetVoice() {
        this.UIM.ReGetVoiceButtonSetActive(false);
        getVoice();
    }

    public void OKVoice() {
        PostMessage(GetVoiceText.text);
    }

    public void PostMessage(string themessage) {
        this.UIM.AfterPostUI();
        postmaneger.PostVoiceMessage(themessage);
        //�ق�Ƃ��͂����œ��e�ł������Ƃ������Ɩ��m�ɕ\������K�v������D
    }


}


