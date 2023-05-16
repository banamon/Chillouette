//* This script is an example sub class of visualizing data on 2D UnityScale.
//* In this script, you can visualize color data.
//* (2020/03/07/Sat.)
//* Y.Akematsu @yoslab

using Microsoft.Azure.Kinect.Sensor;
using System;
using System.IO;
using UnityEngine;

public class K4AColorView : K4ASourceView{

    //https://www.sejuku.net/blog/32288
    BGRA[] RandamBGRA;
    //実験用写真保存
    private float tmpTime = 3;
    private float interval = 10f;
    //private string ExperimentImagePath = "D:/photo_path/";

    void Awake() {
        
        RandamBGRASet();
    }

    //private void Start() {
    //    //RandamBGRASet();
    //}

    //@ Set Color Data 
    private byte[] SetColorData(){
        switch (this.k4aManager.Scale){
            case DisplayScale.SIMPLE:
            case DisplayScale.COLOR:
            case DisplayScale.DEPTH:
            return this.k4aManager.ColorData;
        }
        return null;
    }
    //@ Set Body Index Color Data
    private byte[] SetHumanColorData(){
        switch (this.k4aManager.Scale) {
            case DisplayScale.COLOR:
            case DisplayScale.DEPTH:
                byte[] bodyIndexData = this.k4aManager.BodyIndexData;
                byte[] colorData = this.k4aManager.ColorData;

                if (bodyIndexData == null) return this.k4aManager.ColorData;
                //Array.Reverse(bodyIndexData);

                int RandamBGRANum = IDManeger.RandomColorNum;

                System.Threading.Tasks.Parallel.For(0, bodyIndexData.Length, i => {
                    //int index = BeforecolorArr.Length - 1 - i;
                    //うえみたいにして入れていくと逆にできる
                    if (bodyIndexData[i] == 255) {
                        colorData[i * 4 + 0] = 0;
                        colorData[i * 4 + 1] = 0;
                        colorData[i * 4 + 2] = 0;
                        colorData[i * 4 + 3] = 0;
                    }
                    else {
                        //人物領域
                        //colorData[i * 4 + 0] = 255;
                        //colorData[i * 4 + 1] = 120;
                        //colorData[i * 4 + 2] = 241;
                        //colorData[i * 4 + 3] = 255;
                        colorData[i * 4 + 0] = RandamBGRA[RandamBGRANum].B;
                        colorData[i * 4 + 1] = RandamBGRA[RandamBGRANum].G;
                        colorData[i * 4 + 2] = RandamBGRA[RandamBGRANum].R;
                        colorData[i * 4 + 3] = RandamBGRA[RandamBGRANum].A;
                    }
                });

                return colorData;
        }
        return null;
    }
    
    private byte[] NoHumanColorData(){
        byte[] colorData = this.k4aManager.ColorData;
        System.Threading.Tasks.Parallel.For(0, colorData.Length, i => {
                colorData[i] = 0;
        });
        return colorData;
    }

    // Update is called once per frame
    private void Update(){
        if(this.k4aManager.KinectDevice == null || !this.k4aManager.KinectDevice.IsConnected)return;
        //* Write Original process (as Sub Class Process).
        if(this.k4aManager.Scale != DisplayScale.NONE){
            //byte[] data = SetColorData();         // Example.1
            //byte[] colorhumandata = SetHumanColorData();    // Example.2

            ////実験用カラー写真撮影
            //if (IDManeger.SID != 0) {
            //    tmpTime += Time.deltaTime;
            //    if (tmpTime >= interval) {
            //        byte[] colordata = SetColorData();
            //        SavePhoto(colordata);
            //        tmpTime = 0;
            //    }
            //}

            //シルエットデータ取得
            if (this.postManeger.JudgeSession() && IDManeger.SID != 0) {
                //if (this.k4aManager.NumOfTrackedBody > 0 && IDManeger.SID != 0) {
                //Debug.Log("シルエット表示");
                byte[] colorhumandata = SetHumanColorData();    // Example.2
                UpdateTexture(colorhumandata);
            }
            else {
                //Debug.Log("シルエット非表示");
                byte[] nohumandata = NoHumanColorData();    // Example.2
                UpdateTexture(nohumandata);
            }



        }
        else if(this.k4aManager.ViewIMU)print(this.k4aManager.ImuSampleData.Temperature);
        
    }

    void RandamBGRASet() {
        // https://www.sejuku.net/blog/32288
        int RandamBGRANum = 14;
        IDManeger.BGRANum = RandamBGRANum;
        RandamBGRA = new BGRA[RandamBGRANum];
        //                            B      G      R      A
        RandamBGRA[0]  = new BGRA(    0, 　215, 　255, 　255);// #ffd700 あざやかな黄系の色
        RandamBGRA[1]  = new BGRA(　 98, 　249, 　255, 　255);// #f5b1aa 珊瑚色 さんごいろ
        RandamBGRA[2]  = new BGRA(　  0, 　153, 　255, 　255);// #B30C18 重厚な赤
        RandamBGRA[3]  = new BGRA(　 80, 　190, 　255, 　255);// #40e0d0 turquoise
        RandamBGRA[4]  = new BGRA(　 50, 　120, 　190, 　255);// #1e90ff dodgerblue
        RandamBGRA[5]  = new BGRA(　 60, 　100, 　255, 　255);
        RandamBGRA[6]  = new BGRA(　125, 　125, 　255, 　255);
        RandamBGRA[7]  = new BGRA(　170, 　120, 　255, 　255);
        RandamBGRA[8]  = new BGRA(　250, 　110, 　255, 　255);
        RandamBGRA[9]  = new BGRA(　 98, 　128, 　250, 　255);
        RandamBGRA[10] = new BGRA(　255, 　160, 　220, 　255);
        RandamBGRA[11] = new BGRA(　255, 　100, 　130, 　255);
        RandamBGRA[12] = new BGRA(　255, 　160, 　130, 　255);
        RandamBGRA[13] = new BGRA(　250, 　255, 　150, 　255);
        //RandamBGRA[14] = new BGRA(　255, 　204, 　  0, 　255);
        //RandamBGRA[15] = new BGRA(　255, 　153, 　  0, 　255);
        //RandamBGRA[16] = new BGRA(　255, 　182, 　  0, 　255);
    }

    //実験用カラー写真保存
    public void SavePhoto(byte[] data) {
        int w = this.k4aManager.ImageSize.x;
        int h = this.k4aManager.ImageSize.y;
        Texture2D Tex = new Texture2D(w, h, TextureFormat.BGRA32, false);
        Tex.LoadRawTextureData(data);
        Tex.Apply();
        var png = Tex.EncodeToPNG();
        //File.WriteAllBytes(IDManeger.ExperimentImagePath + IDManeger.SID + "/" + DateTime.Now.ToString("MMddHHmmss") + ".png", png);
    }
    
}