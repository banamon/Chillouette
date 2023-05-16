//* This script uses for getting sensor status, accelerometer, gyro, and temperature.
//* 2020/03/14/Sat.
//* Y.Akematsu @yoslab
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#pragma warning disable 0649

namespace K4AdotNet.Sensor{
public class K4AImuView : MonoBehaviour{
    private K4AManager k4aManager;              // Manager
    [SerializeField]
    private GameObject K4AManagerObject;        // Manager Object

    private const float GravitationalConstant = 9.81f; // Gravity Accelerometer
    private const float RotThreshold = 0.0f;   // Threshold

    private const float minTmpt = 10f;          // Min Temperature
    private const float maxTmpt = 25f;          // Max Temperature
    private const float warningWidth = 5f;      // Vacancy

    private UnityEngine.Quaternion myQuaternion;
    [SerializeField]
    private Material MyMat;

    [SerializeField]
    private int CurrentTempDisplay;


    //@
    private void PaintTemperature(float t,float min,float max,float vac){
        float r = 0f;
        float g = 0f;
        float b = 0f;
        float middle = (max+min)/2;

        if(t>max+vac){
            r = 1.0f;
        }else if(t>=max){
            r = (t-middle)/(max-middle+vac);
            g = 1.0f-(t-max)/vac;
        }else if(t>middle){
            r = (t-middle)/(max-middle+vac);
            g = 1.0f;
        }else if(t>min){
            g = 1.0f;
            b = (middle-t)/(middle-min+vac);
        }else if(t>min-vac){
            g = 1.0f-(min-t)/vac;
            b = (middle-t)/(middle-min+vac);
        }else{
            b = 1.0f;
        }

        if(this.MyMat.HasProperty("_Color")){
            this.MyMat.SetColor("_Color",new Color(r,g,b,1.0f));
        }
    }
    //@ Visualize Gyro and Accel
    private void GetImuTransform(){
        if(!this.k4aManager.ViewIMU)return;

        Float3? gyro = this.k4aManager.ImuSampleData.GyroSample;
        if(gyro == null)return;
        // Get Gyro in Degree
        Vector3 eulerAngleSpeed = new Vector3(gyro.Value.Y,gyro.Value.Z,gyro.Value.X)*(this.k4aManager.IsMirror?1:-1);
        int rx = (Mathf.Abs(eulerAngleSpeed.x*Mathf.Rad2Deg)>RotThreshold)?(int)(eulerAngleSpeed.x*Mathf.Rad2Deg):0;
        int ry = (Mathf.Abs(eulerAngleSpeed.y*Mathf.Rad2Deg)>RotThreshold)?(int)(eulerAngleSpeed.y*Mathf.Rad2Deg):0;
        int rz = (Mathf.Abs(eulerAngleSpeed.z*Mathf.Rad2Deg)>RotThreshold)?(int)(eulerAngleSpeed.z*Mathf.Rad2Deg):0;
        Vector3 eulerAngleVelocity = new Vector3(rx,ry,rz);
        this.transform.rotation =  UnityEngine.Quaternion.Euler(eulerAngleVelocity);
        // Update Rotation
        UnityEngine.Quaternion deltaQuat = UnityEngine.Quaternion.Euler(eulerAngleVelocity*Time.deltaTime);
        this.myQuaternion *= deltaQuat;
        // this.transform.localRotation = this.myQuaternion; //# You can see the sensor rotation.(Not angle-speed)
        
        // Get Accelerometer
        Float3? acc = this.k4aManager.ImuSampleData.AccelerometerSample;
        if(acc == null)return;
        Vector3 accelerometer = new Vector3(acc.Value.Y*(this.k4aManager.IsMirror?-1:1),-acc.Value.Z,acc.Value.X*(this.k4aManager.IsMirror?-1:1));
        // Update Position
        Vector3 deltaAcc 
            = (this.myQuaternion*Vector3.right).normalized*accelerometer.x
            + (this.myQuaternion*Vector3.up).normalized*accelerometer.y
            + (this.myQuaternion*Vector3.forward).normalized*accelerometer.z;
        this.transform.localPosition = deltaAcc;
        this.transform.position -= new Vector3(0,GravitationalConstant,0); //! Exclude Gravitational Accelerometer
    }
    // Start is called before the first frame update
    private void Start(){
        this.k4aManager = this.K4AManagerObject.GetComponent<K4AManager>();
        if(this.k4aManager.KinectDevice == null || !this.k4aManager.KinectDevice.IsConnected)this.k4aManager.InitKinect();
        this.myQuaternion = this.transform.rotation;
    }

    // Update is called once per frame
    private void Update(){
        GetImuTransform();
        PaintTemperature(this.k4aManager.ImuSampleData.Temperature,minTmpt,maxTmpt,warningWidth);
        this.CurrentTempDisplay = (int)this.k4aManager.ImuSampleData.Temperature;
    }
    // OnDestroy is called before destroying attached object
    private void OnDestroy(){
        if(this.MyMat.HasProperty("_Color")){
            this.MyMat.SetColor("_Color",Color.white);
        }
    }
}
}
