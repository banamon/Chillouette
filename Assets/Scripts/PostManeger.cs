using K4AdotNet;
using K4AdotNet.BodyTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostManeger : MonoBehaviour
{
    /*
     * 投稿に関する処理を管理するClass
     * * 人物発見⇒GIF撮影＆保存
     * * GIF保存完了⇒ボタン・QR表示
     * 
     */
    [SerializeField] protected GameObject K4AManagerObject;  // Manager Object
    protected K4AManager k4aManager;        // Manager
    [SerializeField] protected GameObject SaveGifObject;  // Manager Object
    protected SaveGif savegif;
    [SerializeField] protected GameObject DBCObject;  // Manager Object
    protected DBC dbc;
    [SerializeField] protected GameObject PostViewManegerObject;  // Manager Object
    protected PostViewManeger PVM;
    [SerializeField] protected GameObject GetVoiceObject;  // Manager Object
    protected GetVoice getvoice;


    [SerializeField] public GameObject CheckGifUI;
    [SerializeField] Slider _slider;
    [SerializeField] public GameObject UIManegerObject;
    UIManeger UIM;

    [SerializeField] RawImage CheckGifImage;
    [SerializeField] Text QuestionText;


    public bool sectionflag = false;
    public bool section_gifflag = false;//SectionでGIFを撮影後かどうか
    public bool section_gifgettingflag = false;//GIFを撮影中かどうか
    public bool playCheckPNGflag = false;
    //UniGifImage unigifimage;

    //string GifPath = "C:/Users/tagah/Unity/Chillhouette/Assets/Images/";

    [SerializeField] GameObject CreateQRObject;
    CreateQR createqr;

    //シルエット撮影関係
    private int waittime = 20;//Silhouette自動確認までの時間
    public bool PushbuttonFlag = false;//Silhouette確認画面でOKボタンが押されたかどうか
    private IEnumerator CheckSilhouetteCoroutine;
    private IEnumerator PlaySilhouetteCoroutine;

    //session関係
    DateTime NoBodyStartTime;
    TimeSpan NoHumanTimeSpan = new TimeSpan(0, 0, 0);
    int SessionEndTimeSeconds = 3;
    float SessionStartDistance = 3500000;//近づいたらセッションスタート

    //scenereset
    float ResetSceneTimeElapsed;
    float ResetSceneTimeSpan = 60;


    //自動画面遷移
    public float timeOut = 60;//s
    private float timeElapsed;

    float delytime = 0.1f;


    private void Awake() {
        createqr = CreateQRObject.GetComponent<CreateQR>();

        this.UIM = UIManegerObject.GetComponent<UIManeger>();
        this.PVM = PostViewManegerObject.GetComponent<PostViewManeger>();
        this.getvoice = GetVoiceObject.GetComponent<GetVoice>();

        this.CheckGifUI.SetActive(false);
        this.k4aManager = this.K4AManagerObject.GetComponent<K4AManager>();
        this.savegif = this.SaveGifObject.GetComponent<SaveGif>();
        this.dbc = this.DBCObject.GetComponent<DBC>();
        dbc.GetQuestion();
    }


    // Start is called before the first frame update
    void Start()
    {
        //CheckUI再生
        //unigifimage = CheckGifImage.GetComponent<UniGifImage>();


        //var path = Path.Combine(Application.streamingAssetsPath, "log", "example.txt");
        //var logWriter = new LogWriter(path, this.GetCancellationTokenOnDestroy());
    }

    // Update is called once per frame
    void Update()
    {
        int NumofBody = k4aManager.NumOfTrackedBody;

        if (JudgeSession() && !sectionflag) {//セクションの始まり
            if (IDManeger.questionflag && IDManeger.postflag) {
                sectionflag = true;
                timeElapsed = 0.0f;
                dbc.StartSection();
            }
            else {
                Debug.Log("DB接続がまだのため始められません");
            }
        } else if ((NumofBody == 0 && sectionflag)|| (!JudgeSession() && sectionflag)) {//セクションの終わり判定開始
            if (NoHumanTimeSpan.TotalSeconds == 0) {
                Debug.Log("NoBodyStart");
                dbc.SetOperationLog("Stop");
                NoBodyStartTime = DateTime.Now;
                NoHumanTimeSpan = new TimeSpan(0, 0, 1);//とりあえず1秒にしておく
                return;
            } else if (NoHumanTimeSpan.TotalSeconds < SessionEndTimeSeconds) {
                Debug.Log("NObody:" + NoHumanTimeSpan.TotalSeconds);
                NoHumanTimeSpan = DateTime.Now - NoBodyStartTime;
                return;
            } else {
                //ここもsessionEnd処理をメッソド化したい
                //他にも音声入力中や，GIF撮影中などの場合，初期化することが山ほどある
                //各クラスで処理中断クラス書いてこっちで呼び出すorセッションFlagを各クラスで読み取り各々で中断処理
                NoHumanTimeSpan = TimeSpan.Zero;
                sectionflag = false;
                section_gifflag = false;
                section_gifgettingflag = false;
                playCheckPNGflag = false;

                this.UIM.DefaultUI();
                dbc.EndSection();
                IDManeger.maxbodynum = 0;
                IDManeger.EndSession();
                StopGetVoice();
                Debug.Log("＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝Section End＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝");
                return;
            }
        } else if (sectionflag && IDManeger.sidflag) {//セクションの中
            if (NoHumanTimeSpan != TimeSpan.Zero) {
                dbc.SetOperationLog("Restart");
            }

            NoHumanTimeSpan = TimeSpan.Zero;

            if (!section_gifflag && !section_gifgettingflag) {
                //Start GIF
                section_gifgettingflag = true;
                savegif.StartGetGif();
            }
            else if (section_gifflag) {
                //End GIF
                //今の段階でここは，GIF保存されたらずっとここに来る気がする

                return;
            }
        }
        else {//セッションじゃない
            ResetSceneTimeElapsed += Time.deltaTime;
            if (ResetSceneTimeElapsed >= ResetSceneTimeSpan) {
                Debug.Log("一定時間経過したためsceneロードします");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                timeElapsed = 0.0f;
            }
        }
    }


    public void EndSectionGif() {
        if (sectionflag) {
            section_gifgettingflag = false;
            section_gifflag = true;
        }
    }

    //private IEnumerator ViewGifCoroutine(UniGifImage unigifimage, string filename) {
    //    Debug.Log("ViewGifCoroutine " + unigifimage + " " +filename);
    //    Debug.Log(GifPath + filename + "/" + filename + ".gif");
    //    yield return StartCoroutine(unigifimage.SetGifFromUrlCoroutine(GifPath + filename + "/" + filename + ".gif"));
    //}

    IEnumerator CheckPlayPNGCoroutine(string directory) {
        string[] files = System.IO.Directory.GetFiles(IDManeger.SilhouettePath + directory, "*", System.IO.SearchOption.AllDirectories);
        int imageNum = files.Length;
        int startSID = IDManeger.SID;

        //Debug.Log("PlayPNG：" + directory + " " + imageNum + "枚");
        while (true) {
            for (int i = 0; i < imageNum; i++) {
                if (!playCheckPNGflag) {
                    continue;
                }

                Destroy(CheckGifImage.texture);
                byte[] bytes = File.ReadAllBytes(IDManeger.SilhouettePath + directory + "/" + i + ".png");
                Texture2D texture = new Texture2D(312, 312);
                texture.filterMode = FilterMode.Trilinear;
                texture.LoadImage(bytes);

                CheckGifImage.texture = texture;
                //rawimage.SetNativeSize();
                yield return new WaitForSeconds(delytime);
            }
            
            if (startSID != IDManeger.SID || !sectionflag || !playCheckPNGflag) {
                yield break;
            }
        }
    }

    public void CheckGif() {
        //CheckGifUI表示
        this.CheckGifUI.SetActive(true);

        //StartCoroutine(ViewGifCoroutine(unigifimage, IDManeger.filename));
        playCheckPNGflag = true;

        //シルエット再生
        this.PlaySilhouetteCoroutine = CheckPlayPNGCoroutine(IDManeger.filename);
        StartCoroutine(this.PlaySilhouetteCoroutine);

        //一定時間後にシルエット自動投稿させる
        this.CheckSilhouetteCoroutine = CheckSilhouetteMessage();
        StartCoroutine(this.CheckSilhouetteCoroutine);

    }

    private IEnumerator CheckSilhouetteMessage() {
        this.UIM.CheckGifUI();
        int startSID = IDManeger.SID;

        for (int i = 0; i<waittime*2;i++) {
            if (PushbuttonFlag) {
                Debug.Log(i + "秒で選択されたのでコルーチン止める");
                PushbuttonFlag = false;
                _slider.value = 1;
                yield break;
            }else if (startSID != IDManeger.SID||!sectionflag) {
                Debug.Log("セクション終了を感知");
                string[] files = System.IO.Directory.GetFiles(IDManeger.SilhouettePath + IDManeger.filename, "*", System.IO.SearchOption.AllDirectories);
                int imageNum = files.Length;
                if (imageNum > 10) {
                    Debug.Log("セッションは終わったけど10枚以上だから投稿します");
                    PostGif();
                    PushbuttonFlag = false;
                }
                _slider.value = 1;
                yield break;

            }
            yield return new WaitForSeconds(0.5f);
            _slider.value -= (float)0.5f / waittime;
        }

        Debug.Log("TimeOut＆勝手にポスト");
        dbc.SetOperationLog("TimeOut_Silhouette");
        _slider.value = 1;

        PostGif();
        if (sectionflag) {
            //セクション中の場合はUI表示
            StartComentUI();
        }
        PushbuttonFlag = false;

    }

    public void PostGif() {
        Debug.Log("PostGIf");
        //UniGIFの再生を止める
        playCheckPNGflag = false;
        //CheckUI非表示
        this.CheckGifUI.SetActive(false);
        dbc.PostGif(IDManeger.GetQID(), IDManeger.uid, IDManeger.filename);
    }

    public void ReTakeGif() {
        Debug.Log("RetakeGif");
        //UniGifの再生を止める
        playCheckPNGflag = false;

        //CheckUI非表示
        this.CheckGifUI.SetActive(false);
        //再撮影
        //savegif.StartGetGif();
        section_gifflag = false;//SectionでGIFを撮影後かどうか
        section_gifgettingflag = false;//GIFを撮影中かどうか
    }


    public void EndPost() {
        //QR読み込み⇒別の質問⇒QR入力の場合おかしくなりそう
        this.UIM.AfterPostUI();
        this.UIM.RePostButtonSetActive(true);
    }

    //シルエット撮影後に投稿用UI表示
    public void StartComentUI() {
        //CommentUI表示
        this.UIM.PostComentUI();
        //QRコード表示
        createqr.ShowQR_Coment(IDManeger.uid, IDManeger.GetQID(), IDManeger.GetQuestion());
        //DB監視開始
        dbc.CheckDB(IDManeger.uid, IDManeger.GetQID()) ;
    }

    //音声入力の
    public void PostVoiceMessage(string themessage) {
        dbc.PostMessage(themessage);
    }

    public void RePost() {
        dbc.CreateDB(IDManeger.GetQID());
        //UIを最初の状態へ
        this.UIM.DefaultUI();
        //シルエット撮影
        savegif.StartGetGif();
    }

    public void StopGetVoice() {
        getvoice.StopVoice();
    }


    //投稿表示
    public void PutPost() {
        PVM.PutPostStart();
    }

    public void SetQuestionText(string questiontext) {
        this.QuestionText.text = questiontext;
    }

    //お題の変更
    public void ChangeQuestion() {
        this.dbc.ChangeQuestion();
        this.UIM.AfterPostUI();
    }

    public void EndSesshon() {
        //いろいろな処理を初期化する（最悪ロードしなおす）
        //自動OKを止める


        //UI削除
        this.UIM.DefaultUI();
        //ID系削除（どうせ次更新されるけ別にいいかも）

    }

    public  bool JudgeSession() {
        int NumOfBody = k4aManager.NumOfTrackedBody;
        bool flag = false;
        if (NumOfBody == 0) {
            return flag;
        }
        for (int i = 0; i < NumOfBody; i++) {
            float distance = GetPositionNavel(k4aManager.BodyData[i]).sqrMagnitude;
            //Debug.Log(i + "：距離 : "+distance);
            if (distance < this.SessionStartDistance) {
                flag = true;
            }
        }
        return flag;
    }

    //セッション管理
    private Vector3 GetPositionNavel(K4ABody body) {
        JointType jt = (JointType)0;
        //int ajastY = 120;//BodyIndexMapの配置場所に合わせる
        Float2? point = body.convertedPos[(int)(jt)];
        if (point == null) return this.transform.position;//これはようわからん

        float x = body.skeleton[(JointType)jt].PositionMm.X;
        float y = body.skeleton[(JointType)jt].PositionMm.Y;
        float z = body.skeleton[(JointType)jt].PositionMm.Z;

        return new Vector3(x, y, z);//ボタンより前に出すために-0.01f
    }

}