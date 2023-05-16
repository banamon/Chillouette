//* This script is visualizing Sensor or Tracking Data on "2D Unity Scale."
//* If you use "Color", "Depth", "IR", "BodyIndex", or use these in combination, use this script as base class.
//* Maybe you will write visualizing process much easier. 
//* I wrote example that the case of you use this script as base class.
//* Please refer "K4AColorView.cs" as sub class.
//* (2020/03/07/Sat.)
//* Y.Akematsu @yoslab

using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

public class K4ASourceView : MonoBehaviour{
    protected K4AManager k4aManager;        // Manager
    [SerializeField]
    protected GameObject K4AManagerObject;  // Manager Object
    protected Texture2D myTex;
    protected Renderer myRenderer;
    [SerializeField]
    public float Raito;             // Scale of Screen (GameObject)

    [SerializeField]
    public RawImage UserViewRawImage;

    [SerializeField] public GameObject SaveGifObject;
    SaveGif savegif;

    [SerializeField] public GameObject PostManegerObject;
    public PostManeger postManeger;

    //シルエット撮影用
    private DateTime PreviousSaveTime = DateTime.MinValue;
    TimeSpan timeElapsed;
    private float delyaTime = 100f;

    ////実験用写真保存
    //private float tmpTime = 10;
    //private float interval = 10f;



    // Start is called before the first frame update
    protected void Start(){

        this.savegif = SaveGifObject.GetComponent<SaveGif>();
        this.postManeger = PostManegerObject.GetComponent<PostManeger>();

        timeElapsed = new TimeSpan (0,0,(int)delyaTime);


        this.k4aManager = this.K4AManagerObject.GetComponent<K4AManager>();
        if(this.k4aManager.KinectDevice == null || !this.k4aManager.KinectDevice.IsConnected)this.k4aManager.InitKinect();

        int w = this.k4aManager.ImageSize.x;
        int h = this.k4aManager.ImageSize.y;
        
        this.myTex = new Texture2D(w,h,TextureFormat.BGRA32,false);
        this.transform.localScale = new Vector3(w*Raito,h*Raito,1);
    }

    //@ Paint
    protected void UpdateTexture(byte[] data){
        //if (data == null || !postManeger.JudgeSession(k4aManager.NumOfTrackedBody)) return;
        if (data == null) return;


        this.myTex.LoadRawTextureData(data);
        this.myTex.Apply();
        
        UserViewRawImage.texture = this.myTex;

        //シルエット撮影
        if (savegif.gifflag) {
            //1枚目の場合は時間取得
            if (PreviousSaveTime == DateTime.MinValue) {
                PreviousSaveTime = DateTime.Now;
            }

            //人がいなくなった場合は撮影の終了処理
            if (this.k4aManager.NumOfTrackedBody < 1) {
                savegif.gifflag = false;
                savegif.EndGetSilhouette();
                return;
            }

            //マイフレームではなく，delyaTime経過すると撮影する
            if (timeElapsed.TotalMilliseconds > delyaTime) {
                timeElapsed = TimeSpan.Zero;
                PreviousSaveTime = DateTime.Now;
                savegif.SaveTheGif(this.myTex);
            }
            else {
                timeElapsed = DateTime.Now - PreviousSaveTime;
            }


        }
        else {
            PreviousSaveTime = DateTime.MinValue;
        }
    }
}
