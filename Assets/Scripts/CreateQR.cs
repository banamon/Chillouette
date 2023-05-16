using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

//QR
using ZXing;
using ZXing.QrCode;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Script;
using System.Linq;


/*
 * QRコードの表示・監視
 * ０．questionID,ResponseIDの取得
 * １．空のテーブルの作成
 * ２．uidの取得
 * ３．QRコード表示
 * ４．DB監視
 * ５．(回答・返答の場合)シルエット取得シーンへ
*/


public class CreateQR : MonoBehaviour {
    //public SpriteRenderer QRcodeSprite;//最終的に表示するSpriteRendererオブジェクト
    [SerializeField] RawImage QRRawImage_Question;
    [SerializeField] GameObject QRObject_Coment;
    [SerializeField] RawImage QRRawImage_Coment;

    private Texture2D EncodedQRTextire_Question;//エンコードして出来たQRコードのTxture2Dが入る
    private Texture2D EncodedQRTextire_Coment;//エンコードして出来たQRコードのTxture2Dが入る
    private int QrTxtureW = 256;//作成するテクスチャサイズ
    private int QrTxtureH = 256;//作成するテクスチャサイズ
    string ImageLink_Question = "https://www2.yoslab.net/~taga/Chillhouette/input_QR_Question.php";
    string ImageLink_Coment = "https://www2.yoslab.net/~taga/Chillhouette/input_QR_Comment.php";
    
    void Start() {
        ShowQR_Question();
        QRRawImage_Coment.enabled = false;
        QRObject_Coment.SetActive(false);
    }

    public void ShowQR_Question() {
        Debug.Log("ShowQR_Question");
        QrTxtureH = (int)QRRawImage_Question.GetComponent<RectTransform>().sizeDelta.y;
        QrTxtureW = (int)QRRawImage_Question.GetComponent<RectTransform>().sizeDelta.x;

        QRRawImage_Question.texture = Createqr(this.ImageLink_Question);
    }

    public void ShowQR_Coment(string uid, int QID, string question) {
        Debug.Log("コメント投稿用QR配置 QID:" + QID + " UID:" +uid);
        QRObject_Coment.SetActive(true);
        QRRawImage_Coment.enabled = true;

        QrTxtureH = (int)QRRawImage_Coment.GetComponent<RectTransform>().sizeDelta.y;
        QrTxtureW = (int)QRRawImage_Coment.GetComponent<RectTransform>().sizeDelta.x;

        string ComentLink = ImageLink_Coment + "?uid="+uid+"&QID="+QID+"&SID=" + IDManeger.SID;//あとで修正

        QRRawImage_Coment.texture = Createqr(ComentLink);
    }


    public Texture2D Createqr(string ImageLink) {
        Texture2D EncodedQRTextire = new Texture2D(QrTxtureW, QrTxtureH);
        var color32 = Encode(ImageLink, EncodedQRTextire.width, EncodedQRTextire.height);
        EncodedQRTextire.SetPixels32(color32);
        EncodedQRTextire.Apply();
        return EncodedQRTextire;
    }

    //32 ビット形式での RGBA の色の表現
    //https://docs.unity3d.com/ja/2018.4/ScriptReference/Color32.html
    //エンコード処理（ここはサンプル通り）
    private static Color32[] Encode(string textForEncoding, int width, int height) {
        var writer = new BarcodeWriter {
            Format = BarcodeFormat.QR_CODE,

            Options = new QrCodeEncodingOptions {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
}
