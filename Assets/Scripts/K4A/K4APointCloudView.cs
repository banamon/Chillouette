//* This script is visualizing Point Cloud data.
//* This script is not inheritenced K4ASourceView class. (Because visualized data is 3D-Scale data.)
//* (2020/03/07/Sat.)
//* Y.Akematsu @yoslab

using UnityEngine;

#pragma warning disable 0649

namespace K4AdotNet.Sensor{
public class K4APointCloudView : MonoBehaviour{
    private K4AManager k4aManager;          // Manager
    [SerializeField]
    private GameObject K4AManagerObject;    // Manager Object
    private Mesh mesh;

    private int numOfPoint; // Number of 3D Point
    private Vector3[] vertices; // Position array
    private Color32[] colors;   // Color array
    private int[] indices;      // Vertices Painting List


    //@ Initialize Point Cloud Setting
    public void InitializePCL(){
        this.k4aManager = this.K4AManagerObject.GetComponent<K4AManager>();
        if(this.k4aManager.KinectDevice == null || !this.k4aManager.KinectDevice.IsConnected)this.k4aManager.InitKinect();//! You must initialize Kinect before initialize pcl!

        this.numOfPoint = this.k4aManager.ImageSize.x * this.k4aManager.ImageSize.y;
        
        this.mesh = new Mesh();
        this.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        this.vertices   = new Vector3[this.numOfPoint];
        this.colors     = new Color32[this.numOfPoint];
        this.indices    = new int[this.numOfPoint];

        for(int i=0;i<this.numOfPoint;i++){
            int x = i%this.k4aManager.ImageSize.x;
            int y = i/this.k4aManager.ImageSize.x;
            float px = (float)(this.k4aManager.ImageSize.x/2-x)/20;
            float py = (float)(this.k4aManager.ImageSize.y/2-y)/20;

            this.vertices[i].x = px;
            this.vertices[i].y = py;
            this.vertices[i].z = 0;

            this.colors[i].r = 0;
            this.colors[i].g = 255;
            this.colors[i].b = 255;
            this.colors[i].a = 255;

            this.indices[i] = i;
        }

        this.mesh.vertices = this.vertices;
        this.mesh.colors32 = this.colors;
        this.mesh.SetIndices(this.indices,MeshTopology.Points,0);
        
        this.gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }
    //@ Get Point Cloud Data (set mesh)
    public void GetPCLData(short[] xyzData,byte[] colorData){
        for(int i=0;i<this.vertices.Length;i++){
            this.vertices[i].x = xyzData[3*i+0] * 0.001f * ( this.k4aManager.IsMirror ? -1 : 1 );
            this.vertices[i].y = xyzData[3*i+1] * -0.001f;
            this.vertices[i].z = xyzData[3*i+2] * 0.001f;

            this.colors[i].b = colorData[4*i+0];
            this.colors[i].g = colorData[4*i+1];
            this.colors[i].r = colorData[4*i+2];
            this.colors[i].a = colorData[4*i+3];
        }

        this.mesh.vertices = this.vertices;
        this.mesh.colors32 = this.colors;
        this.mesh.RecalculateBounds();
    }
    // Start is called before the first frame update
    private void Start(){
        InitializePCL();
    }

    // Update is called once per frame
    private void Update(){
        GetPCLData(this.k4aManager.XYZImageData,this.k4aManager.ColorData);
    }
}
}
