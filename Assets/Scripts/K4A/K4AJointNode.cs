//* This script uses for joints with using "K4AJointRoot class (K4AJointRoot.cs)."
//* You can also visualize joint position on Unity Space.
//* 2020/02/24/Mon.
//* Y.Akematsu @yoslab

using UnityEngine;

namespace K4AdotNet.BodyTracking{
public class K4AJointNode : MonoBehaviour{
    private K4AJointsRoot JointsRoot;   // Root Class
    private JointType jointType;        // Use what kind of joint
    private float raito;                // Scale of Viewer Object
    private float zpos;

    public int ajastY;//BodyIndexMapの配置場所に合わせる


    //@ Initialize Joint before the first frame update
    public void InitNode(K4AJointsRoot root,float raito,JointType jointType){
        this.gameObject.name = "Joint:"+jointType.ToString();
        this.JointsRoot = root;
        this.transform.parent = root.transform;
        this.jointType  = jointType;
        this.raito      = raito;
    }
    //@ Get Joint Position on Unity
    private Vector3 GetPosition(JointType jt){
        Debug.Log("GetPosition" + ajastY);
        Float2? point = this.JointsRoot.Body.convertedPos[(int)(jt)];
        if(point == null)return this.transform.position;

        int x = (int)this.JointsRoot.Body.convertedPos[(int)(jt)].Value.X;
        //int y = (int)this.JointsRoot.Body.convertedPos[(int)(jt)].Value.Y;
        int y = (int)this.JointsRoot.Body.convertedPos[(int)(jt)].Value.Y+ ajastY;
        float px = (float)(this.JointsRoot.K4AMngProp.ImageSize.x/2-x)*this.raito*(this.JointsRoot.IsMirror?1:-1);
        float py = (float)(this.JointsRoot.K4AMngProp.ImageSize.y/2-y)*this.raito;
        return new Vector3(px,py,0);
    }
    //@ Get Joint Orientation(Rotation) on Unity
    private Vector3 GetOrientaion(JointType jt){
        K4AdotNet.Quaternion q = this.JointsRoot.Body.skeleton[(int)jt].Orientation;
        return (new UnityEngine.Quaternion(-q.X,-q.Y,q.Z,q.W)).eulerAngles;
    }
    // Update is called once per frame
    private void Update(){
        this.transform.position     = GetPosition(this.jointType);
        this.transform.eulerAngles = Vector3.zero;
        //this.transform.eulerAngles = GetOrientaion(this.jointType);
    }
}
}
