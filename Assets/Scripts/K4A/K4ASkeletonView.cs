//* This script is visualizing Joints Data on "2.5D or 3D Unity Scale."
//* Not visualizing on "2D Scale", you do not have to inherit "K4ASourceView" class.
//* (2020/03/07.Sat)
//* Y.Akematsu @yoslab

using UnityEngine;

#pragma warning disable 0649

namespace K4AdotNet.BodyTracking{
public class K4ASkeletonView : MonoBehaviour{
    private K4AManager k4aManager;              // Manager
    [SerializeField]
    private GameObject K4AManagerObject;        // Manager Object
    private int numOfBody;                      // How many bodies are there.
    [SerializeField]
    private GameObject SkeletonRootPrefab;      // Prefab
    private GameObject[] SkeletonRootObjects;   // Skeleton Root Object's Array
    private GameObject CenterSkeletonRootObject;   // Skeleton Root Object's Array

    [SerializeField] protected GameObject PostManeger;  // Manager Object
    protected PostManeger postmaneger;

    [SerializeField] private float Raito = 0.05f;
    [SerializeField] private int ajastY = 100;

    //@ Destroy All Joint Objects
    private void DestroyAllSkeletons(){
        Debug.Log("DestroyAllSkeletons");
        if(this.SkeletonRootObjects == null)return;
        for(int i=0;i<this.SkeletonRootObjects.Length;i++){
            Destroy(this.SkeletonRootObjects[i]);
        }
        this.SkeletonRootObjects = null;
    }

    // Start is called before the first frame update
    private void Start(){
        this.k4aManager = this.K4AManagerObject.GetComponent<K4AManager>();
        this.postmaneger = this.PostManeger.GetComponent<PostManeger>();
        if (this.k4aManager.KinectDevice == null || !this.k4aManager.KinectDevice.IsConnected)this.k4aManager.InitKinect();
    }

    // Update is called once per frame
    private void Update(){

            numOfBody = this.k4aManager.NumOfTrackedBody;

            if (postmaneger.JudgeSession()) {
                SkeletonRootPrefab.SetActive(true);
            }
            else{
                SkeletonRootPrefab.SetActive(false);
                return;
            }

            if (IDManeger.maxbodynum < numOfBody) {
                IDManeger.maxbodynum = numOfBody;
            }


            //中心人物判断
            Vector3[] bodies = new Vector3[numOfBody];

            int TheCenterBodyID = 0;
            float themindistance = 0;
            //Body取得
            for (uint i = 0; i < numOfBody; i++) {
                //へそ座標取得
                float theSpineNavelDistance = GetPositionNavel(this.k4aManager.BodyData[i]).sqrMagnitude;
                //Debug.Log(i + "番目:" + theSpineNavelDistance + "へそ" + GetPositionNavel(this.k4aManager.BodyData[i], (JointType)0));
                if (i == 0) {
                    themindistance = theSpineNavelDistance;
                }
                else if (themindistance > theSpineNavelDistance) {
                    themindistance = theSpineNavelDistance;
                    TheCenterBodyID = (int)i;
                }
            }

            //Debug.Log("中心人物：" + TheCenterBodyID + "番目 距離:" + themindistance + "右手座標:" + GetPosition(this.k4aManager.BodyData[TheCenterBodyID], (JointType)16));

            //SkeletonRootPrefab.
            Transform t = SkeletonRootPrefab.transform;
            t.position = GetPosition(this.k4aManager.BodyData[TheCenterBodyID], (JointType)16);
            //Debug.Log("実際に配置された右手座標::" + t.position);

        }


        private Vector3 GetPosition(K4ABody body,JointType jt) {
            //int ajastY = 120;//BodyIndexMapの配置場所に合わせる
            Float2? point = body.convertedPos[(int)(jt)];
            if (point == null) return this.transform.position;//これはようわからん

            int x = (int)body.convertedPos[(int)(jt)].Value.X;
            //int y = (int)this.JointsRoot.Body.convertedPos[(int)(jt)].Value.Y;
            int y = (int)body.convertedPos[(int)(jt)].Value.Y + this.ajastY;
            
            float px = (float)(this.k4aManager.ImageSize.x / 2 - x) * this.Raito * (this.k4aManager.IsMirror ? 1 : -1);
            float py = (float)(this.k4aManager.ImageSize.y / 2 - y) * this.Raito;
            
            return new Vector3(px, py, -0.01f);//ボタンより前に出すために-0.01f
        }

        private Vector3 GetPositionNavel(K4ABody body) {
            JointType jt = (JointType)0;
            //int ajastY = 120;//BodyIndexMapの配置場所に合わせる
            Float2? point = body.convertedPos[(int)(jt)];
            if (point == null) return this.transform.position;//これはようわからん

            float x = body.skeleton[(JointType)jt].PositionMm.X;
            float y = body.skeleton[(JointType)jt].PositionMm.Y;
            float z = body.skeleton[(JointType)jt].PositionMm.Z;

            return new Vector3(x, 0, 0);//ボタンより前に出すために-0.01f
        }


        // OnDestroy is called before destroying attached object
        private void OnDestroy(){
            DestroyAllSkeletons();
        }
    }
}
