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
    private float RotateSpeedAngle = 0.3f;//1s�ł܂��p�x�i�炵���j
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
        //�̂��̂�����������
        ResetPVM();

        //Post�̎擾
        this.posts = new Post[IDManeger.Posts.Length];
        this.posts = IDManeger.Posts;

        //�ݒu�J�n
        FirstPutPreafb();
    }

    private void Update() {
        if (PutPostFlag) {
            Transform t = this.PostsObjects.transform;
            // ���S�_center�̎�����A��axis�ŁAperiod�����ŉ~�^��
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
            float angle = i * PostIntervalAngle;//���ƂŒ���
            float y = R * Mathf.Cos(angle) + ajastY;
            float z = R * Mathf.Sin(angle) + ajastZ;
            Vector3 thePos = new Vector3(0, y, z);

            Quaternion theQua = Quaternion.Euler(angle * Mathf.Rad2Deg, 0, 0);
            int RandomInt = Random.Range(0, 5);
            //RandomInt = 1;

            GameObject obj = Instantiate(ParkPrefabs[RandomInt], thePos, theQua);
            obj.transform.SetParent(PostsObjects.transform);
            Debug.Log("ParkPrefab�z�u�F" + RandomInt);

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
        //Debug.Log("�yPutPrefab�zindex" + postindex);
        float angle = 2 * PostIntervalAngle;//���ƂŒ��� ������ʒu�����ɂȂ�ƁC�O�ɂ��Ȃ��ƊԊu���ȏ�ɂ������Ⴄ
        float y = R * Mathf.Cos(angle) + ajastY;
        float z = R * Mathf.Sin(angle) + ajastZ;
        Vector3 thePos = new Vector3(0, y, z);

        Quaternion theQua = Quaternion.Euler(angle * Mathf.Rad2Deg, 0, 0);

        int RandomInt = Random.Range(0, 5);
        Debug.Log("ParkPrefab�z�u�F" + RandomInt);

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
            if (postNum < 4 && i > 0) {//3�l�̎��͈�l���z�u
                post5[i] = new Post();
                post5[i].message = "";
                post5[i].file_name = "";
                addindex = 1;
                continue;
            }
            else if (3 < postNum && postNum < 6 && i > 2) {//4�C5�l�̏ꍇ��2�l���z�u
                post5[i] = new Post();
                post5[i].message = "";
                post5[i].file_name = "";
                addindex = 2;
                continue;
            }
            else if (5 < postNum && postNum < 10 && i > 3) {//4�C5�l�̏ꍇ��3�l���z�u
                post5[i] = new Post();
                post5[i].message = "";
                post5[i].file_name = "";
                addindex = 3;
                continue;
            } else {//����ȊO
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
        // ���ׂĂ̎q�I�u�W�F�N�g���擾
        foreach (Transform n in PostsObjects.transform) {
            GameObject.Destroy(n.gameObject);
        }
    }
}