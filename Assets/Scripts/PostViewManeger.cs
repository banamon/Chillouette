using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostViewManeger : MonoBehaviour {

    [SerializeField] GameObject PostsObjects;
    [SerializeField] GameObject[] ParkPrefabs;
    [SerializeField] public int ajastY = -1000;
    [SerializeField] public int ajastZ = 100;
    [SerializeField] public int EndZ = 10;

    GameObject[] PostPrefabsList = new GameObject[3];

    private int R = 1000;
    private Post[] posts;
    private int postindex = 0;
    private float RotateSpeedAngle = 0.3f;//1sでまわる角度（らしい）
    private float PostIntervalAngle = Mathf.PI / 48;



    public bool PutPostFlag = false;

    void Awake() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }

    // Start is called before the first frame update
    void Start() {
    }


    public void PutPostStart() {
        Debug.Log("PutPostStart");
        //のちのち初期化処理
        ResetPVM();

        //Postの取得
        this.posts = new Post[IDManeger.Posts.Length];
        this.posts = IDManeger.Posts;

        //設置開始
        FirstPutPreafb();
    }

    private void Update() {
        if (PutPostFlag) {
            Transform t = this.PostsObjects.transform;
            // 中心点centerの周りを、軸axisで、period周期で円運動
            t.RotateAround(
                new Vector3(0,ajastY,ajastZ),
                Vector3.left,
                RotateSpeedAngle * Time.deltaTime
                //360 / 100 * Time.deltaTime
            );

            if (PostPrefabsList[0].transform.position.z < EndZ) {
                Destroy(PostPrefabsList[0]);
                PostPrefabsList[0] = PostPrefabsList[1];
                PostPrefabsList[1] = PostPrefabsList[2];
                PutPreafb();
            }
        }
    }

    public void FirstPutPreafb() {
        for (int i = 0; i < 3; i++) {
            float angle = i * PostIntervalAngle;//あとで調整
            float y = R * Mathf.Cos(angle) + ajastY;
            float z = R * Mathf.Sin(angle) + ajastZ;
            Vector3 thePos = new Vector3(0, y, z);

            Quaternion theQua = Quaternion.Euler(angle * Mathf.Rad2Deg, 0, 0);
            int RandomInt = Random.Range(0, 5);
            //RandomInt = 1;

            GameObject obj = Instantiate(ParkPrefabs[RandomInt], thePos, theQua);
            obj.transform.SetParent(PostsObjects.transform);
            Debug.Log("ParkPrefab配置：" + RandomInt);

            PostPrefabsList[i] = obj;
            if (this.posts.Length < 3) {
                if (i == 0) {
                    obj.GetComponent<PutPostManeger>().posts = GetPost5(this.posts);
                }
                else {
                    obj.GetComponent<PutPostManeger>().posts = GetPostEmpty5();
                }
            }
            else {
                obj.GetComponent<PutPostManeger>().posts = GetPost5(this.posts);
            }
        }
        PutPostFlag = true;
    }

    public void PutPreafb() {
        //Debug.Log("【PutPrefab】index" + postindex);
        float angle = 2 * PostIntervalAngle;//あとで調整 消える位置が後ろになると，前にしないと間隔が以上にあいちゃう
        float y = R * Mathf.Cos(angle) + ajastY;
        float z = R * Mathf.Sin(angle) + ajastZ;
        Vector3 thePos = new Vector3(0, y, z);

        Quaternion theQua = Quaternion.Euler(angle * Mathf.Rad2Deg, 0, 0);

        int RandomInt = Random.Range(0, 5);
        Debug.Log("ParkPrefab配置：" + RandomInt);

        GameObject obj = Instantiate(ParkPrefabs[RandomInt], thePos, theQua);
        obj.transform.SetParent(PostsObjects.transform);

        PostPrefabsList[2] = obj;
        obj.GetComponent<PutPostManeger>().posts = GetPost5(this.posts);

    }

    public Post[] GetPost5(Post[] posts) {
        Post[] post5 = new Post[5];
        int postNum = posts.Length;
        int addindex = 5;

        for (int i = 0; i < 5; i++) {
            if (postindex + i >= posts.Length) {
                post5[i] = new Post();
                post5[i].message = "";
                post5[i].file_name = "";
                continue;
            }
            if (postNum < 4 && i > 0) {//3人の時は一人ずつ配置
                post5[i] = new Post();
                post5[i].message = "";
                post5[i].file_name = "";
                addindex = 1;
                continue;
            }
            else if (3 < postNum && postNum < 6 && i > 2) {//4，5人の場合は2人ずつ配置
                post5[i] = new Post();
                post5[i].message = "";
                post5[i].file_name = "";
                addindex = 2;
                continue;
            }
            else if (5 < postNum && postNum < 10 && i > 3) {//4，5人の場合は3人ずつ配置
                post5[i] = new Post();
                post5[i].message = "";
                post5[i].file_name = "";
                addindex = 3;
                continue;
            } else {//それ以外
                post5[i] = posts[postindex + i];
            }
        }


        if (postindex + addindex >= posts.Length) {
            postindex = 0;
        }
        else {
            postindex += addindex;
        }

        return post5;
    }

    public Post[] GetPostEmpty5() {
        Post[] post5 = new Post[5];
        for (int i = 0; i < 5; i++) {
            post5[i] = new Post();
            post5[i].message = "";
            post5[i].file_name = "";
        }
        return post5;
    }

    public void ResetPVM() {
        postindex = 0;
        PutPostFlag = false;
        DeleteAllPost();
    }



    public void DeleteAllPost() {
        // すべての子オブジェクトを取得
        foreach (Transform n in PostsObjects.transform) {
            GameObject.Destroy(n.gameObject);
        }
    }
}