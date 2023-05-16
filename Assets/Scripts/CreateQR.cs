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
 * QR�R�[�h�̕\���E�Ď�
 * �O�DquestionID,ResponseID�̎擾
 * �P�D��̃e�[�u���̍쐬
 * �Q�Duid�̎擾
 * �R�DQR�R�[�h�\��
 * �S�DDB�Ď�
 * �T�D(�񓚁E�ԓ��̏ꍇ)�V���G�b�g�擾�V�[����
*/


public class CreateQR : MonoBehaviour {
    //public SpriteRenderer QRcodeSprite;//�ŏI�I�ɕ\������SpriteRenderer�I�u�W�F�N�g
    [SerializeField] RawImage QRRawImage_Question;
    [SerializeField] GameObject QRObject_Coment;
    [SerializeField] RawImage QRRawImage_Coment;

    private Texture2D EncodedQRTextire_Question;//�G���R�[�h���ďo����QR�R�[�h��Txture2D������
    private Texture2D EncodedQRTextire_Coment;//�G���R�[�h���ďo����QR�R�[�h��Txture2D������
    private int QrTxtureW = 256;//�쐬����e�N�X�`���T�C�Y
    private int QrTxtureH = 256;//�쐬����e�N�X�`���T�C�Y
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
        Debug.Log("�R�����g���e�pQR�z�u QID:" + QID + " UID:" +uid);
        QRObject_Coment.SetActive(true);
        QRRawImage_Coment.enabled = true;

        QrTxtureH = (int)QRRawImage_Coment.GetComponent<RectTransform>().sizeDelta.y;
        QrTxtureW = (int)QRRawImage_Coment.GetComponent<RectTransform>().sizeDelta.x;

        string ComentLink = ImageLink_Coment + "?uid="+uid+"&QID="+QID+"&SID=" + IDManeger.SID;//���ƂŏC��

        QRRawImage_Coment.texture = Createqr(ComentLink);
    }


    public Texture2D Createqr(string ImageLink) {
        Texture2D EncodedQRTextire = new Texture2D(QrTxtureW, QrTxtureH);
        var color32 = Encode(ImageLink, EncodedQRTextire.width, EncodedQRTextire.height);
        EncodedQRTextire.SetPixels32(color32);
        EncodedQRTextire.Apply();
        return EncodedQRTextire;
    }

    //32 �r�b�g�`���ł� RGBA �̐F�̕\��
    //https://docs.unity3d.com/ja/2018.4/ScriptReference/Color32.html
    //�G���R�[�h�����i�����̓T���v���ʂ�j
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
