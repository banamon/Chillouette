using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


/*  questionID・ResponseIDの管理
 * ページ遷移のクリック時に呼び出して，ID取得⇒格納しておく
 *
*/
public class IDManeger : MonoBehaviour {
    [SerializeField]
    DBC dbc;

    public static string filename { get; set; }
    public static string message { get; set; }
    //public static int QID { get; set; }//QutstionID
    public static int QuestionIndex = 0;//QutstionID
    public static string uid { get; set; }//uid
    public static int SID { get; set; }//SesionID
    public static int RandomColorNum { get; set; }//SesionID
    public static Question[] Question { get; set; }//SesionID
    public static Post[] Posts { get; set; }//SesionID


    public static bool questionflag = false;//Question取得しているかどうか
    public static bool postflag = false;//Postを取得しているかどうか
    public static bool sidflag = false;//Postを取得しているかどうか
    public static int maxbodynum = 0;

    public static int BGRANum;

    public static string SilhouettePath = "C:/Users/tagah/Unity/Chillhouette/Assets/Images/";//開発環境
    //public static string SilhouettePath = "C:/Users/yoshi/taga/Images/";//本番環境
    //public static string ExperimentImagePath = "D:/photo_path/";

    void Start() {
        postflag = false;
        questionflag = false;
        sidflag = false;
        SID = 0;
    }

    public static void StartSession(int theSID) {
        System.Random random = new System.Random();
        RandomColorNum = random.Next(0, BGRANum - 1);
        SID = theSID;
        sidflag = true;
        Debug.Log("＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝Section Start (SID:" + SID + " ,RCNum:" + RandomColorNum + ")＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝");
    }

    public static void EndSession() {
        sidflag = false;
        SID = 0;
    }

    public static void Setuid(string theuid) {
        uid = theuid;
    }

    public static void SetPostMessage(string thepostmessage) {
        message = thepostmessage;
    }


    public static int GetQID() {
        Debug.Log("GetQID" + Question[QuestionIndex].QID);
        return Question[QuestionIndex].QID;
    }

    public static string Getuid() {
        return uid;
    }

    public static string GetPostMessage() {
        return message;
    }
    public static string GetQuestion() {
        return Question[QuestionIndex].question;
    }
}
