using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


/*  questionID�EResponseID�̊Ǘ�
 * �y�[�W�J�ڂ̃N���b�N���ɌĂяo���āCID�擾�ˊi�[���Ă���
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


    public static bool questionflag = false;//Question�擾���Ă��邩�ǂ���
    public static bool postflag = false;//Post���擾���Ă��邩�ǂ���
    public static bool sidflag = false;//Post���擾���Ă��邩�ǂ���
    public static int maxbodynum = 0;

    public static int BGRANum;

    public static string SilhouettePath = "C:/Users/tagah/Unity/Chillhouette/Assets/Images/";//�J����
    //public static string SilhouettePath = "C:/Users/yoshi/taga/Images/";//�{�Ԋ�
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
        Debug.Log("������������������������������������Section Start (SID:" + SID + " ,RCNum:" + RandomColorNum + ")������������������������������������");
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
