using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;   //Windowsの音声認識で使用
public class GetVoice : MonoBehaviour {
    //認識装置が起動し、最初の5秒間音声が聞こえない場合は、タイムアウトになる。
    //認識装置が結果を与えたが20秒間無音を聞くと、認識装置はタイムアウトする。

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

    //音声認識中に何もない場合は，30秒たったら自動で終わる
    //（これWindowsSpeechのInitialSilenceTimeoutSecondsつかえばいいのでは？）
    private DateTime StartGetVoiceTime;
    private float StopGetVoiceSeconds = 30;
    private int GetVoiceNum = 0;

    //確認画面で一定時間放置されると自動で送信する
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

        //初期化
        SpeechbubbleObject.SetActive(false);
        GetVoiceAnimationObject.SetActive(false);
        this.UIM.GetVoiceButtonSetActive(false);
        this.UIM.RePostButtonSetActive(false);
        this.UIM.GetVoiceOKButtonSetActive(false);


        dictationRecognizer = new DictationRecognizer();
    }

    // 録音ボタン呼び出しで実行
    public void getVoice() {
        //音声入力時間の取得
        if (GetVoiceNum == 0) {
            this.StartGetVoiceTime = DateTime.Now;
        }
        GetVoiceNum++;


        //UI

        //音声入力ボタン非表示
        //音声入力中UI表示
        this.UIM.CheckVoiceUI();
        this.UIM.GetVoiceButtonSetActive(false);
        this.UIM.GetVoiceOKButtonSetActive(false);
        this.UIM.ReGetVoiceButtonSetActive(false);
        CheckVoiceObject.SetActive(false);
        SpeechbubbleObject.SetActive(true);
        
        //認識中アニメーション再生
        GetVoiceAnimationObject.SetActive(true);
        anime.enabled = true;
        anime.Play("RawImage");

        //いろいろ初期化
        GetVoiceText.text = "";

        //初期化
        dictationRecognizer = new DictationRecognizer();

        dbc.SetOperationLog("StartTake_Voice");


        //認識開始
        Debug.Log("音声認識開始");
        dictationRecognizer.Start();
        dictationRecognizer.AutoSilenceTimeoutSeconds = 120;//音声入力がないために音声認識を終了する際の、終了する前の経過時間 (秒)
        dictationRecognizer.InitialSilenceTimeoutSeconds = 20;//現在のセッションで音声入力がまったくないために音声認識を終了する際の、終了する前の経過時間 (秒)
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;//DictationRecognizer_DictationResult処理を行う
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;//DictationRecognizer_DictationHypothesis処理を行う
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;//DictationRecognizer_DictationComplete処理を行う
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;//DictationRecognizer_DictationError処理を行う
    }

    //DictationResult：音声が特定の認識精度で認識されたときに発生するイベント
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence) {
        Debug.Log("認識した音声：" + text);
        GetVoiceText.text = "";

        VoiceMessage = text;
        dictationRecognizer.Stop();
        GetVoiceText.text = VoiceMessage;
    }

    //DictationHypothesis：音声入力中に発生するイベント
    private void DictationRecognizer_DictationHypothesis(string text) {
        Debug.Log("音声認識中：" + text);
        GetVoiceText.text = text;
    }

    //DictationComplete：音声認識セッションを終了したときにトリガされるイベント
    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause) {
        Debug.Log("音声認識完了");
        StopVoice();

        string themessage = GetVoiceText.text;

        TimeSpan t = DateTime.Now - StartGetVoiceTime;
        if (t.TotalSeconds > StopGetVoiceSeconds) {
            //タイムアウト
            Debug.Log("TimeOutです...");
            GetVoiceNum = 0;
            this.UIM.AfterPostUI();
            return;
        }

        if (themessage == "") {
            //再度音声認識
            Debug.Log("もっかい音声認識開始");
            getVoice();
        }
        else {
            //音声認識の確認
            dbc.SetOperationLog("EndTake_Voice");

            //PQ.CheckVoiceMessage(themessage);
            this.CheckVoiceCoroutine = CheckVoiceMessage(themessage);
            this.CheckVoiceObject.SetActive(true);
            StartCoroutine(this.CheckVoiceCoroutine);
            GetVoiceNum = 0;

            Debug.Log("音声認識テキスト：" + themessage);
            //再認識ボタン表示
        }
    }

    //DictationError：音声認識セッションにエラーが発生したときにトリガされるイベント
    private void DictationRecognizer_DictationError(string error, int hresult) {
        Debug.Log("音声認識エラー");
    }

    //音声入力の停止
    public void StopVoice() {
        Debug.Log("音声認識の強制終了");
        Debug.Log("スレッドID:" + Thread.CurrentThread.ManagedThreadId);
        //dictationRecognizer.Stop();
        dictationRecognizer.Dispose();

        //アニメーション停止
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
                Debug.Log("ボタン押下を感知！スレッド止めます！");
                PushButton = false;
                _slider.value = 1;
                yield break;
            } else if (startSID != IDManeger.SID ||  !postmaneger.sectionflag) {
                Debug.Log("セクション終了を感知!　文字投稿しときます！");
                postmaneger.PostVoiceMessage(themessage);
                _slider.value = 1;
                yield break;

            }

            yield return new WaitForSeconds(0.5f);
            _slider.value -= (float) 0.5 / waittime;
            Debug.Log(_slider.value);
        }

        Debug.Log("Retake押されず一定時間経過したので投稿処理を記述します");
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
        //ほんとうはここで投稿できたことをもっと明確に表現する必要がある．
    }


}


