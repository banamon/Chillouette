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

    private bool checkDBCoroutineFlag = true;//QRDB監視子ルーチンを止めるためのFlag

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
        Debug.Log("保存開始：" + operation);
        StartCoroutine(SetOperationLogCoroutine(operation));
    }



    public void ChangeQuestion() {

        //QR監視を止める
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
         * SectionのDB作成
         * +開始時間
         * +実験用写真のpath 
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
            //SIDの取得
            Debug.Log("SID:" + post.text);
            IDManeger.StartSession(int.Parse(post.text));
            
            //実験ログ写真用ディレクトリ作成
            //Directory.CreateDirectory(ExperimentImagePath + post.text);
            //Directory.CreateDirectory(IDManeger.ExperimentImagePath + post.text);

            //Unity開始直後にセッション入ると，Questionが取れて無くてエラー発生する
            //Questionがしゅとくされるまで　待つ必要がありそう
            Debug.Log("QID" + IDManeger.GetQID());
            CreateDB(IDManeger.GetQID());
        }
        else {
            Debug.LogWarning(post.error);
            Debug.LogWarning(post.text);
        }
    }

    public IEnumerator EndSectionCoroutine() {
        //QR監視を止める
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
            Debug.Log("セッション終了処理" + post.text);
        }
        else {
            Debug.LogWarning(post.error);
            Debug.LogWarning(post.text);
        }
    }

    //DBの作成⇒あとで選択画面に移動orQRと一緒の処理するからクラス分け
    public IEnumerator CreateDBCoroutine(int QID) {
        Debug.Log("CreateDB QID:" + QID + " SID:" + IDManeger.SID);
        WWWForm form = new WWWForm();
        string url = "https://www2.yoslab.net/~taga/Chillhouette/CreateDB.php";
        form.AddField("QID", QID);
        form.AddField("SID", IDManeger.SID);
        WWW post = new WWW(url, form);
        yield return post;

        if (post.error == null) {
            //uidの格納
            string uid = post.text;
            IDManeger.Setuid(uid);
            Debug.Log("DB作成完了 UID:" + uid);
        }
        else {
            Debug.LogWarning(post.error);
        }
    }

    private IEnumerator PostGifCoroutine(int questionID, string uid, string giffilename) {
        Debug.Log("シルエット投稿 QID:" + questionID + "uid:" + uid  + "filename" + giffilename);

        //DBへ保存
        WWWForm form = new WWWForm();
        string url = "https://www2.yoslab.net/~taga/Chillhouette/upload_GIF.php";

        form.AddField("uid", uid);
        form.AddField("QID", questionID);
        form.AddField("filename", giffilename);
        //シルエットGIFを付与
        //form.AddBinaryData("file", bytes, giffilename + ".gif", "image/gif");

        WWW post = new WWW(url, form);

        yield return post;

        Debug.Log(post.text);
        if (post.error == null) {
            Debug.Log("シルエット登録完了! GetPostします！");
            GetPost(IDManeger.GetQID());
        }
        else {
            Debug.Log(post.error);
            Debug.Log(post.text);
        }
    }

    //png画像をバイト配列に変換
    byte[] readPngFile(string path) {
        //pathからPNG画像を開く
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
            Debug.Log("Question取得完了："+ReturnQuestions.text);
            IDManeger.Question = JsonHelper.FromJson<Question>(ReturnQuestions.text);
            IDManeger.questionflag = true;
        }
        else {
            Debug.Log(ReturnQuestions.error);
        }

        Debug.Log("Question取得完了："+ IDManeger.postflag);
        //Postがロードされていないとき...？
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
            Debug.Log("Postの取得成功 QID=" + theQID + "のPosts：" + ReturnPosts.text);
            string json_post = ReturnPosts.text;
            IDManeger.Posts = JsonHelper.FromJson<Post>(json_post);
            IDManeger.postflag = true;
            PM.PutPost();
        }
        else {
            Debug.LogError("Postの取得失敗 ERR：" + ReturnPosts.error + " " + ReturnPosts.text);
            GetPost(theQID);//これをしていいものか...
        }
    }

    private IEnumerator PostMessageCoroutine(string themessage) {
        Debug.Log("Call PostMessageCoroutine");
        string uid = IDManeger.uid;
        int QID = IDManeger.GetQID();

        //QR監視を止める
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
            Debug.LogError("Postの送信に失敗しました" + ReturnPosts.error + " " + ReturnPosts.text);
            PostMessage(themessage);//再挑戦（これをしたら無限ループなる．．？n階だけとかにすればいいかも
        }
    }

    //DBの監視をする
    private IEnumerator CheckDBCoroutine(string uid, int QID) {
        Debug.Log("DB監視開始 QID:" + QID +" uid:"+ uid);
        checkDBCoroutineFlag = true;
        int max = 120;//60秒だけ見ておく
        for (int i = 0; i < max; i++) {
            yield return new WaitForSeconds(1);

            if (!checkDBCoroutineFlag) {
                Debug.Log("QR監視終了します！");
                yield break;
            }

            WWWForm form = new WWWForm();
            string url = "https://www2.yoslab.net/~taga/Chillhouette/CheckDB.php?uid=" + uid + "&QID=" + QID;
            WWW post = new WWW(url, form);
            // 1秒待つ
            yield return post;

            if (post.error != null) {
                Debug.LogError("QR監視ERR: "+post.error);
                Debug.Log(post.text);
            }
            //Debug.Log("CheckDB post.text" + post.text);
            string thequestion = post.text;
            if (thequestion != "") {
                Debug.Log("DB監視でQRテキスト入力感知！「" + thequestion + "」");

                //音声認識がある場合は停止する
                PM.StopGetVoice();

                //もしQR読み込み時と一緒のQIDで，セッションが一緒なら，EndPostして，GetPostしよう
                PM.EndPost();
                GetPost(IDManeger.GetQID());
                yield break;
            }
        }
    }

    //DBの監視をする
    private IEnumerator SetOperationLogCoroutine(string operation) {
        WWWForm form = new WWWForm();
        form.AddField("mode", "OPERATION");
        form.AddField("SID", IDManeger.SID);
        form.AddField("operation", operation);
        string url = "https://www2.yoslab.net/~taga/Chillhouette/SectionManeger.php";
        WWW post = new WWW(url, form);
        yield return post;

        if (post.error == null) {
            Debug.Log("操作ログ保存完了："+ operation + " " + post.text);
        }
        else {
            Debug.LogWarning(post.error);
            Debug.LogWarning(post.text);
        }
    }
}

// 配列の要素に使用するクラス
[Serializable]
public class Question {
    public int QID;
    public string question;
    //public Post[] posts;
}

// 配列の要素に使用するクラス
[Serializable]
public class Post {
    public int id;
    public string file_name;
    public string message;
    public string PostMethod;
    public string insert_time;
}


/// <summary>
/// <see cref="JsonUtility"/> に不足している機能を提供します。
/// </summary>
public static class JsonHelper {
    /// <summary>
    /// 指定した string を Root オブジェクトを持たない JSON 配列と仮定してデシリアライズします。
    /// </summary>
    public static T[] FromJson<T>(string json) {
        // ルート要素があれば変換できるので
        // 入力されたJSONに対して(★)の行を追加する
        //
        // e.g.
        // ★ {
        // ★     "array":
        //        [
        //            ...
        //        ]
        // ★ }
        //
        string dummy_json = $"{{\"{DummyNode<T>.ROOT_NAME}\": {json}}}";

        // ダミーのルートにデシリアライズしてから中身の配列を返す
        var obj = JsonUtility.FromJson<DummyNode<T>>(dummy_json);
        return obj.array;
    }

    /// <summary>
    /// 指定した配列やリストなどのコレクションを Root オブジェクトを持たない JSON 配列に変換します。
    /// </summary>
    /// <remarks>
    /// 'prettyPrint' には非対応。整形したかったら別途変換して。
    /// </remarks>
    public static string ToJson<T>(IEnumerable<T> collection) {
        string json = JsonUtility.ToJson(new DummyNode<T>(collection)); // ダミールートごとシリアル化する
        int start = DummyNode<T>.ROOT_NAME.Length + 4;
        int len = json.Length - start - 1;
        return json.Substring(start, len); // 追加ルートの文字を取り除いて返す
    }

    // 内部で使用するダミーのルート要素
    [Serializable]
    private struct DummyNode<T> {
        // 補足:
        // 処理中に一時使用する非公開クラスのため多少設計が変でも気にしない

        // JSONに付与するダミールートの名称
        public const string ROOT_NAME = nameof(array);
        // 疑似的な子要素
        public T[] array;
        // コレクション要素を指定してオブジェクトを作成する
        public DummyNode(IEnumerable<T> collection) => this.array = collection.ToArray();
    }
}