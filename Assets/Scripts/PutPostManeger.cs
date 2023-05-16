using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutPostManeger : MonoBehaviour {
    [SerializeField] public Post[] posts;
    [SerializeField] GameObject[] PostPrefabs;

    // Start is called before the first frame update
    void Start() {

        for (int i = 0; i<5;i++) {
            //Debug.Log("PutPost " + posts[i].file_name + " " + posts[i].message);
            PostPrefabs[i].GetComponent<PlaySilhouette>().directory = posts[i].file_name;
            PostPrefabs[i].GetComponent<PlaySilhouette>().message = posts[i].message;
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
