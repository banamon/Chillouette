//* This script uses for joints of the body.
//* You can also visualize joint position on Unity Space.
//* 2020/02/24/Mon.
//* Y.Akematsu @yoslab

using UnityEngine;

#pragma warning disable 0649

namespace K4AdotNet.BodyTracking{
public class K4AJointsRoot : MonoBehaviour{
        private K4AManager k4aManager;
        public K4AManager K4AMngProp{
            get{
                return this.k4aManager;
            }
            private set{}
        }
        public bool IsMirror{
            get; private set;
        }
        public int DataId{
            get; private set;
        } // Body Data array's index in Manager class
        private const int JointCount = 2;  // Number of Joints per body
        //private const int JointCount = 32;  // Number of Joints per body
        public K4ABody Body{get;private set;}
        [SerializeField]
        private GameObject JointPrefab;     // Prefab
        [SerializeField]
        private GameObject JointPrefab_Base;     // Prefab
        private GameObject[] JointObjects;  // Actually, this param need not be used, for process. it uses for debug.

        [SerializeField]
        public int ajastY;//100?

        //@ Initialize Joints Root berofe the first frame update
        public bool InitRoot(int dataId,K4AManager manager,float r){
            Debug.Log("InitRoot!!!!!!!");
            this.k4aManager = manager;
            this.IsMirror = manager.IsMirror;
            this.DataId = dataId;
            this.Body = this.k4aManager.BodyData[this.DataId];
            this.JointObjects = new GameObject[JointCount];

            if (this.Body.bodyId == 0) {
                Debug.LogError("DataID" + dataId + "は用意されていません");
                    return false;
            }

            manager.DebugBodyData();

            Debug.Log(" BodyData[" + DataId + "].BodyID= " + this.Body.bodyId + " isTracked" + Body.isTracked);


            //へそ配置
            this.JointObjects[0] = Instantiate(this.JointPrefab_Base) as GameObject;
            K4AJointNode node_base= this.JointObjects[0].GetComponent<K4AJointNode>();
            node_base.InitNode(this, r, (JointType)0);//16	HANDTIP_RIGHT	HAND_RIGHT

            //ポインター配置
            this.JointObjects[1] = Instantiate(this.JointPrefab) as GameObject;
            K4AJointNode node = this.JointObjects[1].GetComponent<K4AJointNode>();
            node.ajastY = this.ajastY;
            node.InitNode(this, r, (JointType)16);//16	HANDTIP_RIGHT	HAND_RIGHT

            this.gameObject.name = "Body:"+this.Body.bodyId.Value.ToString();
            this.transform.position = Vector3.zero;

            Debug.Log(this.Body.bodyId.Value.ToString() + " " + this.Body.isTracked);
                return true;
        }


        private void OnDestroy(){
        this.k4aManager.BodyData[this.DataId].bodyId.Value = 0;
        this.k4aManager.BodyData[this.DataId].isTracked = false;
    }
}
}
