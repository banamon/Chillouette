using Microsoft.Azure.Kinect.Sensor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlaySilhouette : MonoBehaviour {
    [SerializeField] public string directory;
    [SerializeField] public string message;

    //string Silhouettepath = "C:/Users/tagah/Unity/Chillhouette/Assets/Images/";

    [SerializeField] RawImage rawimage;
    [SerializeField] RawImage speechbubble;
    [SerializeField] Text messageText;
    [SerializeField] RawImage ShadowRawImage;//シルエットの下にある影


    private int ViewComentPosZ = 100;

    float DelayTime = 0.1f;

    // Start is called before the first frame update
    void Start() {
        //Debug.Log("PlaySilhouette " + directory  + " " + message );
        //TexH = rawimage.transform.
        //rawimage = this.gameObject.GetComponent<RawImage>();

        //if (message == "") {
        //    speechbubble.enabled = false;
        //    messageText.enabled = false;
        //}
        //else {
        //    speechbubble.enabled = true;
        //    messageText.enabled = true;
        //    messageText.text = message;
        //}

        speechbubble.enabled = false;
        messageText.enabled = false;

        if (directory == "") {
            rawimage.enabled = false;
            ShadowRawImage.enabled = false;
        }
        else {
            StartCoroutine(PlayPNG(directory));
        }

    }
    IEnumerator PlayPNG(string directory) {
        string[] files;
        try {
           files = System.IO.Directory.GetFiles(IDManeger.SilhouettePath + directory, "*.png", System.IO.SearchOption.AllDirectories);
        }
        catch(DirectoryNotFoundException e) {
            speechbubble.enabled = false;
            rawimage.enabled = false;
            messageText.enabled = false;
            ShadowRawImage.enabled = false;
            yield break;
        }        
        
        int imageNum = files.Length;

        //Debug.Log("PlayPNG：" + imageNum + "枚");
        while (true) {
            //Debug.Log("無限ループ" + directory);
            for (int i = 0; i < imageNum; i++) {
                if (message != "" && this.transform.position.z < ViewComentPosZ) {
                    speechbubble.enabled = true;
                    messageText.enabled = true;
                    messageText.text = message;
                }

                Destroy(rawimage.texture);

                //byte[] bytes = File.ReadAllBytes(Silhouettepath + directory + "/" + i + ".png");
                byte[] bytes = File.ReadAllBytes(IDManeger.SilhouettePath + directory + "/" + i + ".png");
                Texture2D texture = new Texture2D(512, 512);
                texture.filterMode = FilterMode.Trilinear;
                texture.LoadImage(bytes);

                rawimage.texture = texture;
                //rawimage.SetNativeSize();
                yield return new WaitForSeconds(DelayTime);
            }
        }
    }

}
