using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

public class DBC : MonoBehaviour
{
    [SerializeField] GameObject PostManegerObject;
    PostManeger PM;

    //private string ExperimentImagePath = "D:/photo_path/";
    private string GetDBURL = "https://www2.yoslab.net/~taga/Chillhouette/GetDB.php";
    private string PostMessageURL = "https://www2.yoslab.net/~taga/Chillhouette/upload_message.php";

    private bool checkDBCoroutineFlag = true;//QRDB�Ď��q���[�`�����~�߂邽�߂�Flag

    private void Start() {

        PM = PostManegerObject.GetComponent<PostManeger>();
    }

    public void StartSection() {
        StartCoroutine(StartSectionCoroutine());
    }
    public void EndSection() {
        StartCoroutine(EndSectionCoroutine());
    }

    public void CreateDB(int QID) {
        StartCoroutine(CreateDBCoroutine(QID));
    }

    public void PostGif(int QID, string uid, string giffilename) {
        Debug.Log("PostGif : uid" + uid + " giffilename" + giffilename);
        StartCoroutine(PostGifCoroutine(QID,uid, giffilename));
    }

    public void GetQuestion() {
        IDManeger.questionflag = false;
        StartCoroutine(GetQuestionCoroutine());
    }
    public void GetPost(int theQID) {
        IDManeger.postflag = false;
        StartCoroutine(GetPostCoroutine(theQID));
    }

    public void PostMessage(string message) {
        StartCoroutine(PostMessageCoroutine(message));
    }

    public void CheckDB(string uid,int theQID) {
        StartCoroutine(CheckDBCoroutine(uid,theQID));
    }

    public void SetOperationLog(string operation) {
        Debug.Log("�ۑ��J�n�F" + operation);
        StartCoroutine(SetOperationLogCoroutine(operation));
    }



    public void ChangeQuestion() {

        //QR�Ď����~�߂�
        checkDBCoroutineFlag = false;

        IDManeger.postflag = false;
        int QuestionLength = IDManeger.Question.Length;
        int DebugBeforeQIndex = IDManeger.QuestionIndex;
        if (IDManeger.QuestionIndex + 1 >= QuestionLength) {
            IDManeger.QuestionIndex = 0;
        }
        else {
            IDManeger.QuestionIndex += 1;
        }
        Debug.Log("ChangeQuestion " + DebugBeforeQIndex + " -> " + IDManeger.QuestionIndex);

        this.GetQuestion();
        //this.PM.SetQuestionText(IDManeger.GetQuestion());
    }

    public IEnumerator StartSectionCoroutine() {
        /*
         * Section��DB�쐬
         * +�J�n����
         * +�����p�ʐ^��path 
         */

        string startime = DateTime.Now.ToString();
        Debug.Log("SectionStart " + startime);
        WWWForm form = new WWWForm();
        form.AddField("mode", "START");
        form.AddField("starttime", startime);
        string url = "https://www2.yoslab.net/~taga/Chillhouette/SectionManeger.php";
        WWW post = new WWW(url, form);
        yield return post;

        if (post.error == null) {
            //SID�̎擾
            Debug.Log("SID:" + post.text);
            IDManeger.StartSession(int.Parse(post.text));
            
            //�������O�ʐ^�p�f�B���N�g���쐬
            //Directory.CreateDirectory(ExperimentImagePath + post.text);
            //Directory.CreateDirectory(IDManeger.ExperimentImagePath + post.text);

            //Unity�J�n����ɃZ�b�V��������ƁCQuestion�����Ė����ăG���[��������
            //Question������Ƃ������܂Ł@�҂K�v�����肻��
            Debug.Log("QID" + IDManeger.GetQID());
            CreateDB(IDManeger.GetQID());
        }
        else {
            Debug.LogWarning(post.error);
            Debug.LogWarning(post.text);
        }
    }

    public IEnumerator EndSectionCoroutine() {
        //QR�Ď����~�߂�
        checkDBCoroutineFlag = false;


        string endtime = DateTime.Now.ToString();
        Debug.Log("SectionEnd " + endtime);
        WWWForm form = new WWWForm();
        form.AddField("mode", "END");
        form.AddField("SID", IDManeger.SID);
        form.AddField("maxbodynum", IDManeger.maxbodynum);
        form.AddField("endtime", endtime);
        string url = "https://www2.yoslab.net/~taga/Chillhouette/SectionManeger.php";
        WWW post = new WWW(url, form);
        yield return post;

        if (post.error == null) {
            Debug.Log("�Z�b�V�����I������" + post.text);
        }
        else {
            Debug.LogWarning(post.error);
            Debug.LogWarning(post.text);
        }
    }

    //DB�̍쐬�˂��ƂőI����ʂɈړ�orQR�ƈꏏ�̏������邩��N���X����
    public IEnumerator CreateDBCoroutine(int QID) {
        Debug.Log("CreateDB QID:" + QID + " SID:" + IDManeger.SID);
        WWWForm form = new WWWForm();
        string url = "https://www2.yoslab.net/~taga/Chillhouette/CreateDB.php";
        form.AddField("QID", QID);
        form.AddField("SID", IDManeger.SID);
        WWW post = new WWW(url, form);
        yield return post;

        if (post.error == null) {
            //uid�̊i�[
            string uid = post.text;
            IDManeger.Setuid(uid);
            Debug.Log("DB�쐬���� UID:" + uid);
        }
        else {
            Debug.LogWarning(post.error);
        }
    }

    private IEnumerator PostGifCoroutine(int questionID, string uid, string giffilename) {
        Debug.Log("�V���G�b�g���e QID:" + questionID + "uid:" + uid  + "filename" + giffilename);

        //DB�֕ۑ�
        WWWForm form = new WWWForm();
        string url = "https://www2.yoslab.net/~taga/Chillhouette/upload_GIF.php";

        form.AddField("uid", uid);
        form.AddField("QID", questionID);
        form.AddField("filename", giffilename);
        //�V���G�b�gGIF��t�^
        //form.AddBinaryData("file", bytes, giffilename + ".gif", "image/gif");

        WWW post = new WWW(url, form);

        yield return post;

        Debug.Log(post.text);
        if (post.error == null) {
            Debug.Log("�V���G�b�g�o�^����! GetPost���܂��I");
            GetPost(IDManeger.GetQID());
        }
        else {
            Debug.Log(post.error);
            Debug.Log(post.text);
        }
    }

    //png�摜���o�C�g�z��ɕϊ�
    byte[] readPngFile(string path) {
        //path����PNG�摜���J��
        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
            BinaryReader bin = new BinaryReader(fileStream);
            byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);
            bin.Close();
            return values;
        }
    }


    private IEnumerator GetQuestionCoroutine() {
        Debug.Log("Call GetQuestionCoroutine");
        WWWForm form = new WWWForm();
        form.AddField("mode", "getQuestion");
        WWW ReturnQuestions = new WWW(GetDBURL, form);
        yield return ReturnQuestions;
        if (ReturnQuestions.error == null) {
            Debug.Log("Question�擾�����F"+ReturnQuestions.text);
            IDManeger.Question = JsonHelper.FromJson<Question>(ReturnQuestions.text);
            IDManeger.questionflag = true;
        }
        else {
            Debug.Log(ReturnQuestions.error);
        }

        Debug.Log("Question�擾�����F"+ IDManeger.postflag);
        //Post�����[�h����Ă��Ȃ��Ƃ�...�H
        //if (!IDManeger.postflag) {
        //    GetPost(IDManeger.GetQID());
        //    PM.SetQuestionText(IDManeger.GetQuestion());
        //}
        GetPost(IDManeger.GetQID());
        PM.SetQuestionText(IDManeger.GetQuestion());
    }

    private IEnumerator GetPostCoroutine(int theQID) {
        Debug.Log("Call GetPostCoroutine from QID" + theQID);
        WWWForm form_post = new WWWForm();
        form_post.AddField("mode", "getPost");
        form_post.AddField("QID", theQID);
        WWW ReturnPosts = new WWW(GetDBURL, form_post);
        yield return ReturnPosts;

        if (ReturnPosts.error == null) {
            Debug.Log("Post�̎擾���� QID=" + theQID + "��Posts�F" + ReturnPosts.text);
            string json_post = ReturnPosts.text;
            IDManeger.Posts = JsonHelper.FromJson<Post>(json_post);
            IDManeger.postflag = true;
            PM.PutPost();
        }
        else {
            Debug.LogError("Post�̎擾���s ERR�F" + ReturnPosts.error + " " + ReturnPosts.text);
            GetPost(theQID);//��������Ă������̂�...
        }
    }

    private IEnumerator PostMessageCoroutine(string themessage) {
        Debug.Log("Call PostMessageCoroutine");
        string uid = IDManeger.uid;
        int QID = IDManeger.GetQID();

        //QR�Ď����~�߂�
        checkDBCoroutineFlag = false;

        Debug.Log("Post!! qid:" + QID + " uid:" + uid + " message:" + themessage);

        WWWForm form = new WWWForm();
        form.AddField("message", themessage);
        form.AddField("uid", uid);
        form.AddField("QID", QID);
        form.AddField("PostMethod", "voice");
        form.AddField("type", "Post");
        WWW ReturnPosts = new WWW(PostMessageURL, form);
        yield return ReturnPosts;

        if (ReturnPosts.error == null) {
            Debug.Log(ReturnPosts.text);
            Debug.Log("Post Finish");
            GetPost(IDManeger.GetQID());
        }
        else {
            Debug.LogError("Post�̑��M�Ɏ��s���܂���" + ReturnPosts.error + " " + ReturnPosts.text);
            PostMessage(themessage);//�Ē���i����������疳�����[�v�Ȃ�D�D�Hn�K�����Ƃ��ɂ���΂�������
        }
    }

    //DB�̊Ď�������
    private IEnumerator CheckDBCoroutine(string uid, int QID) {
        Debug.Log("DB�Ď��J�n QID:" + QID +" uid:"+ uid);
        checkDBCoroutineFlag = true;
        int max = 120;//60�b�������Ă���
        for (int i = 0; i < max; i++) {
            yield return new WaitForSeconds(1);

            if (!checkDBCoroutineFlag) {
                Debug.Log("QR�Ď��I�����܂��I");
                yield break;
            }

            WWWForm form = new WWWForm();
            string url = "https://www2.yoslab.net/~taga/Chillhouette/CheckDB.php?uid=" + uid + "&QID=" + QID;
            WWW post = new WWW(url, form);
            // 1�b�҂�
            yield return post;

            if (post.error != null) {
                Debug.LogError("QR�Ď�ERR: "+post.error);
                Debug.Log(post.text);
            }
            //Debug.Log("CheckDB post.text" + post.text);
            string thequestion = post.text;
            if (thequestion != "") {
                Debug.Log("DB�Ď���QR�e�L�X�g���͊��m�I�u" + thequestion + "�v");

                //�����F��������ꍇ�͒�~����
                PM.StopGetVoice();

                //����QR�ǂݍ��ݎ��ƈꏏ��QID�ŁC�Z�b�V�������ꏏ�Ȃ�CEndPost���āCGetPost���悤
                PM.EndPost();
                GetPost(IDManeger.GetQID());
                yield break;
            }
        }
    }

    //DB�̊Ď�������
    private IEnumerator SetOperationLogCoroutine(string operation) {
        WWWForm form = new WWWForm();
        form.AddField("mode", "OPERATION");
        form.AddField("SID", IDManeger.SID);
        form.AddField("operation", operation);
        string url = "https://www2.yoslab.net/~taga/Chillhouette/SectionManeger.php";
        WWW post = new WWW(url, form);
        yield return post;

        if (post.error == null) {
            Debug.Log("���샍�O�ۑ������F"+ operation + " " + post.text);
        }
        else {
            Debug.LogWarning(post.error);
            Debug.LogWarning(post.text);
        }
    }
}

// �z��̗v�f�Ɏg�p����N���X
[Serializable]
public class Question {
    public int QID;
    public string question;
    //public Post[] posts;
}

// �z��̗v�f�Ɏg�p����N���X
[Serializable]
public class Post {
    public int id;
    public string file_name;
    public string message;
    public string PostMethod;
    public string insert_time;
}


/// <summary>
/// <see cref="JsonUtility"/> �ɕs�����Ă���@�\��񋟂��܂��B
/// </summary>
public static class JsonHelper {
    /// <summary>
    /// �w�肵�� string �� Root �I�u�W�F�N�g�������Ȃ� JSON �z��Ɖ��肵�ăf�V���A���C�Y���܂��B
    /// </summary>
    public static T[] FromJson<T>(string json) {
        // ���[�g�v�f������Εϊ��ł���̂�
        // ���͂��ꂽJSON�ɑ΂���(��)�̍s��ǉ�����
        //
        // e.g.
        // �� {
        // ��     "array":
        //        [
        //            ...
        //        ]
        // �� }
        //
        string dummy_json = $"{{\"{DummyNode<T>.ROOT_NAME}\": {json}}}";

        // �_�~�[�̃��[�g�Ƀf�V���A���C�Y���Ă��璆�g�̔z���Ԃ�
        var obj = JsonUtility.FromJson<DummyNode<T>>(dummy_json);
        return obj.array;
    }

    /// <summary>
    /// �w�肵���z��⃊�X�g�Ȃǂ̃R���N�V������ Root �I�u�W�F�N�g�������Ȃ� JSON �z��ɕϊ����܂��B
    /// </summary>
    /// <remarks>
    /// 'prettyPrint' �ɂ͔�Ή��B���`������������ʓr�ϊ����āB
    /// </remarks>
    public static string ToJson<T>(IEnumerable<T> collection) {
        string json = JsonUtility.ToJson(new DummyNode<T>(collection)); // �_�~�[���[�g���ƃV���A��������
        int start = DummyNode<T>.ROOT_NAME.Length + 4;
        int len = json.Length - start - 1;
        return json.Substring(start, len); // �ǉ����[�g�̕�������菜���ĕԂ�
    }

    // �����Ŏg�p����_�~�[�̃��[�g�v�f
    [Serializable]
    private struct DummyNode<T> {
        // �⑫:
        // �������Ɉꎞ�g�p�������J�N���X�̂��ߑ����݌v���ςł��C�ɂ��Ȃ�

        // JSON�ɕt�^����_�~�[���[�g�̖���
        public const string ROOT_NAME = nameof(array);
        // �^���I�Ȏq�v�f
        public T[] array;
        // �R���N�V�����v�f���w�肵�ăI�u�W�F�N�g���쐬����
        public DummyNode(IEnumerable<T> collection) => this.array = collection.ToArray();
    }
}