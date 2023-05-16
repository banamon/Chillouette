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
     * ���e�Ɋւ��鏈�����Ǘ�����Class
     * * �l��������GIF�B�e���ۑ�
     * * GIF�ۑ������˃{�^���EQR�\��
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
    public bool section_gifflag = false;//Section��GIF���B�e�ォ�ǂ���
    public bool section_gifgettingflag = false;//GIF���B�e�����ǂ���
    public bool playCheckPNGflag = false;
    //UniGifImage unigifimage;

    //string GifPath = "C:/Users/tagah/Unity/Chillhouette/Assets/Images/";

    [SerializeField] GameObject CreateQRObject;
    CreateQR createqr;

    //�V���G�b�g�B�e�֌W
    private int waittime = 20;//Silhouette�����m�F�܂ł̎���
    public bool PushbuttonFlag = false;//Silhouette�m�F��ʂ�OK�{�^���������ꂽ���ǂ���
    private IEnumerator CheckSilhouetteCoroutine;
    private IEnumerator PlaySilhouetteCoroutine;

    //session�֌W
    DateTime NoBodyStartTime;
    TimeSpan NoHumanTimeSpan = new TimeSpan(0, 0, 0);
    int SessionEndTimeSeconds = 3;
    float SessionStartDistance = 3500000;//�߂Â�����Z�b�V�����X�^�[�g

    //scenereset
    float ResetSceneTimeElapsed;
    float ResetSceneTimeSpan = 60;


    //������ʑJ��
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
        //CheckUI�Đ�
        //unigifimage = CheckGifImage.GetComponent<UniGifImage>();


        //var path = Path.Combine(Application.streamingAssetsPath, "log", "example.txt");
        //var logWriter = new LogWriter(path, this.GetCancellationTokenOnDestroy());
    }

    // Update is called once per frame
    void Update()
    {
        int NumofBody = k4aManager.NumOfTrackedBody;

        if (JudgeSession() && !sectionflag) {//�Z�N�V�����̎n�܂�
            if (IDManeger.questionflag && IDManeger.postflag) {
                sectionflag = true;
                timeElapsed = 0.0f;
                dbc.StartSection();
            }
            else {
                Debug.Log("DB�ڑ����܂��̂��ߎn�߂��܂���");
            }
        } else if ((NumofBody == 0 && sectionflag)|| (!JudgeSession() && sectionflag)) {//�Z�N�V�����̏I��蔻��J�n
            if (NoHumanTimeSpan.TotalSeconds == 0) {
                Debug.Log("NoBodyStart");
                dbc.SetOperationLog("Stop");
                NoBodyStartTime = DateTime.Now;
                NoHumanTimeSpan = new TimeSpan(0, 0, 1);//�Ƃ肠����1�b�ɂ��Ă���
                return;
            } else if (NoHumanTimeSpan.TotalSeconds < SessionEndTimeSeconds) {
                Debug.Log("NObody:" + NoHumanTimeSpan.TotalSeconds);
                NoHumanTimeSpan = DateTime.Now - NoBodyStartTime;
                return;
            } else {
                //������sessionEnd���������b�\�h��������
                //���ɂ��������͒���CGIF�B�e���Ȃǂ̏ꍇ�C���������邱�Ƃ��R�قǂ���
                //�e�N���X�ŏ������f�N���X�����Ă������ŌĂяo��or�Z�b�V����Flag���e�N���X�œǂݎ��e�X�Œ��f����
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
                Debug.Log("������������������������������������Section End������������������������������������");
                return;
            }
        } else if (sectionflag && IDManeger.sidflag) {//�Z�N�V�����̒�
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
                //���̒i�K�ł����́CGIF�ۑ����ꂽ�炸���Ƃ����ɗ���C������

                return;
            }
        }
        else {//�Z�b�V��������Ȃ�
            ResetSceneTimeElapsed += Time.deltaTime;
            if (ResetSceneTimeElapsed >= ResetSceneTimeSpan) {
                Debug.Log("��莞�Ԍo�߂�������scene���[�h���܂�");
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

        //Debug.Log("PlayPNG�F" + directory + " " + imageNum + "��");
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
        //CheckGifUI�\��
        this.CheckGifUI.SetActive(true);

        //StartCoroutine(ViewGifCoroutine(unigifimage, IDManeger.filename));
        playCheckPNGflag = true;

        //�V���G�b�g�Đ�
        this.PlaySilhouetteCoroutine = CheckPlayPNGCoroutine(IDManeger.filename);
        StartCoroutine(this.PlaySilhouetteCoroutine);

        //��莞�Ԍ�ɃV���G�b�g�������e������
        this.CheckSilhouetteCoroutine = CheckSilhouetteMessage();
        StartCoroutine(this.CheckSilhouetteCoroutine);

    }

    private IEnumerator CheckSilhouetteMessage() {
        this.UIM.CheckGifUI();
        int startSID = IDManeger.SID;

        for (int i = 0; i<waittime*2;i++) {
            if (PushbuttonFlag) {
                Debug.Log(i + "�b�őI�����ꂽ�̂ŃR���[�`���~�߂�");
                PushbuttonFlag = false;
                _slider.value = 1;
                yield break;
            }else if (startSID != IDManeger.SID||!sectionflag) {
                Debug.Log("�Z�N�V�����I�������m");
                string[] files = System.IO.Directory.GetFiles(IDManeger.SilhouettePath + IDManeger.filename, "*", System.IO.SearchOption.AllDirectories);
                int imageNum = files.Length;
                if (imageNum > 10) {
                    Debug.Log("�Z�b�V�����͏I���������10���ȏゾ���瓊�e���܂�");
                    PostGif();
                    PushbuttonFlag = false;
                }
                _slider.value = 1;
                yield break;

            }
            yield return new WaitForSeconds(0.5f);
            _slider.value -= (float)0.5f / waittime;
        }

        Debug.Log("TimeOut������Ƀ|�X�g");
        dbc.SetOperationLog("TimeOut_Silhouette");
        _slider.value = 1;

        PostGif();
        if (sectionflag) {
            //�Z�N�V�������̏ꍇ��UI�\��
            StartComentUI();
        }
        PushbuttonFlag = false;

    }

    public void PostGif() {
        Debug.Log("PostGIf");
        //UniGIF�̍Đ����~�߂�
        playCheckPNGflag = false;
        //CheckUI��\��
        this.CheckGifUI.SetActive(false);
        dbc.PostGif(IDManeger.GetQID(), IDManeger.uid, IDManeger.filename);
    }

    public void ReTakeGif() {
        Debug.Log("RetakeGif");
        //UniGif�̍Đ����~�߂�
        playCheckPNGflag = false;

        //CheckUI��\��
        this.CheckGifUI.SetActive(false);
        //�ĎB�e
        //savegif.StartGetGif();
        section_gifflag = false;//Section��GIF���B�e�ォ�ǂ���
        section_gifgettingflag = false;//GIF���B�e�����ǂ���
    }


    public void EndPost() {
        //QR�ǂݍ��݁˕ʂ̎����QR���͂̏ꍇ���������Ȃ肻��
        this.UIM.AfterPostUI();
        this.UIM.RePostButtonSetActive(true);
    }

    //�V���G�b�g�B�e��ɓ��e�pUI�\��
    public void StartComentUI() {
        //CommentUI�\��
        this.UIM.PostComentUI();
        //QR�R�[�h�\��
        createqr.ShowQR_Coment(IDManeger.uid, IDManeger.GetQID(), IDManeger.GetQuestion());
        //DB�Ď��J�n
        dbc.CheckDB(IDManeger.uid, IDManeger.GetQID()) ;
    }

    //�������͂�
    public void PostVoiceMessage(string themessage) {
        dbc.PostMessage(themessage);
    }

    public void RePost() {
        dbc.CreateDB(IDManeger.GetQID());
        //UI���ŏ��̏�Ԃ�
        this.UIM.DefaultUI();
        //�V���G�b�g�B�e
        savegif.StartGetGif();
    }

    public void StopGetVoice() {
        getvoice.StopVoice();
    }


    //���e�\��
    public void PutPost() {
        PVM.PutPostStart();
    }

    public void SetQuestionText(string questiontext) {
        this.QuestionText.text = questiontext;
    }

    //����̕ύX
    public void ChangeQuestion() {
        this.dbc.ChangeQuestion();
        this.UIM.AfterPostUI();
    }

    public void EndSesshon() {
        //���낢��ȏ���������������i�ň����[�h���Ȃ����j
        //����OK���~�߂�


        //UI�폜
        this.UIM.DefaultUI();
        //ID�n�폜�i�ǂ������X�V����邯�ʂɂ��������j

    }

    public  bool JudgeSession() {
        int NumOfBody = k4aManager.NumOfTrackedBody;
        bool flag = false;
        if (NumOfBody == 0) {
            return flag;
        }
        for (int i = 0; i < NumOfBody; i++) {
            float distance = GetPositionNavel(k4aManager.BodyData[i]).sqrMagnitude;
            //Debug.Log(i + "�F���� : "+distance);
            if (distance < this.SessionStartDistance) {
                flag = true;
            }
        }
        return flag;
    }

    //�Z�b�V�����Ǘ�
    private Vector3 GetPositionNavel(K4ABody body) {
        JointType jt = (JointType)0;
        //int ajastY = 120;//BodyIndexMap�̔z�u�ꏊ�ɍ��킹��
        Float2? point = body.convertedPos[(int)(jt)];
        if (point == null) return this.transform.position;//����͂悤�킩���

        float x = body.skeleton[(JointType)jt].PositionMm.X;
        float y = body.skeleton[(JointType)jt].PositionMm.Y;
        float z = body.skeleton[(JointType)jt].PositionMm.Z;

        return new Vector3(x, y, z);//�{�^�����O�ɏo�����߂�-0.01f
    }

}