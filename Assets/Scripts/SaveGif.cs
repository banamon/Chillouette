using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveGif : MonoBehaviour {
    public bool gifflag = false;
    private string directoryname;
    //private string GifPath = "C:/Users/tagah/Unity/Chillhouette/Assets/Images/";
    private int GifCount = 0;
    private int MAXGIFCOUNT = 50;
    private ushort DelayTime = 3;

    [SerializeField] protected GameObject K4AManagerObject;  // Manager Object
    protected K4AManager k4aManager;        // Manager
    [SerializeField]
    protected GameObject PostManeger;  // Manager Object
    protected PostManeger postmaneger;
    [SerializeField] public GameObject UIManegerObject;
    UIManeger UIM;
    [SerializeField] protected GameObject DBCObject;  // Manager Object
    protected DBC dbc;

    //[SerializeField] UnityEngine.UI.Image RecordingGageImage;
    [SerializeField] Slider _slider;
    protected Texture2D myTex;

    //[SerializeField] UnityEngine.UI.RawImage rawImage;
    private void Start() {
        this.UIM = this.UIManegerObject.GetComponent<UIManeger>();
        this.postmaneger = this.PostManeger.GetComponent<PostManeger>();
        this.k4aManager = this.K4AManagerObject.GetComponent<K4AManager>();
        this.UIM.RecordingUISetActive(false);
        this.dbc = this.DBCObject.GetComponent<DBC>();

    }


    public void StartGetGif() {
        this.GifCount = 0;
        this.gifflag = true;
        this.UIM.RecordingUISetActive(true);
        this.directoryname = System.DateTime.Now.ToString("MMddHHmmssfff");
        //Directory.CreateDirectory(GifPath + directoryname);
        Directory.CreateDirectory(IDManeger.SilhouettePath + directoryname);
        dbc.SetOperationLog("StartTake_Silhouette");
    }

    public void SaveTheGif(Texture2D texture) {
        //png画像の作成→保存
        var png = texture.EncodeToPNG();
        //File.WriteAllBytes(GifPath + directoryname + "/" + this.GifCount + ".png", png);
        File.WriteAllBytes(IDManeger.SilhouettePath + directoryname + "/" + this.GifCount + ".png", png);
        GifCount++;
        _slider.value = (float)GifCount / MAXGIFCOUNT;
        if (GifCount > MAXGIFCOUNT || !gifflag) {
            gifflag = false;
            Debug.Log("PNG保存完了");
            EndGetSilhouette();
        }
    }

    public void EndGetSilhouette() {
        Debug.Log("GIFCount " + GifCount);
        dbc.SetOperationLog("EndTake_Silhouette");
        _slider.value = 0;//ゲージリセット
        IDManeger.filename = directoryname;
        this.UIM.RecordingUISetActive(false);
        GifCount = 0;
        postmaneger.EndSectionGif();
        postmaneger.CheckGif();
    }
}
