//* This script is General Purpose for Azure Kinect Sensor Source Manager.
//* (2020/03/07/Sat.)
//* Y.Akematsu @yoslab

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using K4AdotNet.Sensor;
using K4AdotNet.BodyTracking;

#pragma warning disable 0649

// Display Scale 
public enum DisplayScale : byte {
    SIMPLE = 0, // Get 2D data on each original scale
    COLOR,      // Get 2D data on color scale
    DEPTH,      // Get 2D data on depth scale
    NONE        // Not get 2D data
}

public class K4AManager : MonoBehaviour{

    public DisplayScale Scale;  // DisplayMode
    public bool IsMirror = true;// TRUE:opossite direction(sensor and terminal screen),FALSE:same direction
    public bool ViewSkeleton;   // Display Body Joint
    public bool ViewPCL;        // Display PCL (Only Depth Scale)
    public bool ViewIMU;        // Display IMU (accelerator, gyro, and temperature)
    //# Image Size
    public Vector2Int ImageSize {get;private set;}
    //# Sync
    private int DeviceID; // Device ID (Use for Sync)
    //# Basical Params
    private Device device;          // Kinect Sensor
    private Tracker tracker;        // Kinect Body Tracker
    public BodyFrame bodyFrame;    // Kinect BodyFrame for Body-Tracking
    //# Settings for Kinect Sensor
    private DeviceConfiguration DeviceConfig; // Config for Kinect Sensor
    //## Params of Device Config
    public ColorResolution CResln;      // Color Resolution
    public ImageFormat ImgFormat;       // Format
    public DepthMode DMode;             // Depth Mode (Depth Resolution)
    public bool SyncImageOnly = true;   // Sync
    public FrameRate Fps;               // FPS
    //# Support
    private Transformation trans; //# Convert Space
    private Calibration calib;
    //# Settings for Kinect Body Tracker
    private TrackerConfiguration TrackerConfig;// Config for Kinect Body Tracker
    //# Prams of Tracker Config (Not using now...)
    //public SensorOrientation Orientation;   //
    //public Int32 GpuID;                     //
    //public TrackerProcessingMode PMode;     //
    //# Prop
    public Device KinectDevice{// Sensor
        get{
            return this.device;
        }
    }
    //## Data
    public byte[] ColorData{ get; private set;}     // Color
    public ushort[] DepthData{ get; private set;}   // Depth
    public byte[] IRData{ get; private set;}        // Infrared
    public K4ABody[] BodyData{get;private set;}     // Body (Tracking Id ,Skeleton etc...)
    public byte[] BodyIndexData{get;private set;}   // Body Index
    public int NumOfTrackedBody{get;private set;}   // NumOfBody
    public short[] XYZImageData{get;private set;}   // 3D-Point (PCL)
    public ImuSample ImuSampleData{get;private set;}// Accelerator, Gyro, and Temperature


    //@ Copy Memory
    [DllImport("kernel32.dll",SetLastError = false)] //! Use for Extern
    private static extern void CopyMemory(IntPtr dest,IntPtr src,UIntPtr lengh);
    public static void Copy<T>(IntPtr src,T[] dest,int startIndex,int length) where T:struct{
        var gch = GCHandle.Alloc(dest,GCHandleType.Pinned);
        try{
            var targetPtr = Marshal.UnsafeAddrOfPinnedArrayElement(dest,startIndex);
            var bytes2Copy = Marshal.SizeOf(typeof(T))*length;
            CopyMemory (targetPtr,src,(UIntPtr)bytes2Copy);
        }finally{
            gch.Free();
        }
    }
    //@ Set Device ID (Parameter is an index of device's array)
    public void SetDeviceID(int id){
        this.DeviceID = id;
    }
    //@ Initialize Sensor Data(Image) Size
    private void InitializeSensorData(DeviceConfiguration dconfig){
        // Color Scale
        int cw = 0,ch =0;
        switch (dconfig.ColorResolution) {
            case ColorResolution.R720p:
                cw = 1280; ch = 720;
                break;
            case ColorResolution.R1080p:
                cw = 1920; ch = 1080;
                break;
            case ColorResolution.R1440p:
                cw = 2560; ch = 1440;
                break;
            case ColorResolution.R2160p:
                cw = 3840; ch = 2160;
                break;
            case ColorResolution.R1536p:
                cw = 2048; ch = 1536;
                break;
            case ColorResolution.R3072p:
                cw = 4096; ch = 3072;
                break;
            default:
                break;
        }
        // Depth Scale
        int dw = 0, dh = 0;
        switch(dconfig.DepthMode){
            case DepthMode.NarrowViewUnbinned:
                dw = 640; dh = 576;
                break;
            case DepthMode.NarrowView2x2Binned:
                dw = 320; dh = 288;
                break;
            case DepthMode.WideViewUnbinned:
                dw = 1024; dh = 1024;
                break;
            case DepthMode.WideView2x2Binned:
                dw = 512; dh = 512;
                break;
            case DepthMode.PassiveIR:
                dw = 1024; dh = 1024;
                break;
            default:
                break;
        }
        // Data(pixels)
        switch(this.Scale){
            case DisplayScale.SIMPLE:
                this.ColorData      = new byte[cw * ch * 4];
                this.DepthData      = new ushort[dw * dh];
                this.IRData         = new byte[dw * dh];
                this.BodyIndexData  = new byte[dw * dh];
                this.ImageSize = new Vector2Int(cw,ch);
                break;
            case DisplayScale.COLOR:
                this.ColorData      = new byte[cw * ch * 4];
                this.DepthData      = new ushort[cw*ch];
                this.IRData         = new byte[cw*ch];
                this.BodyIndexData  = new byte[cw*ch];
                this.ImageSize = new Vector2Int(cw,ch);
                break;
            case DisplayScale.DEPTH:
                this.ColorData      = new byte[dw * dh * 4];
                this.DepthData      = new ushort[dw * dh];
                this.IRData         = new byte[dw * dh];
                this.BodyIndexData  = new byte[dw * dh];
                this.ImageSize = new Vector2Int(dw,dh);
                break;
            default:
                break;
        }

        if(this.ViewSkeleton){
            this.BodyData = new K4ABody[8];
            for(int i=0;i<this.BodyData.Length;i++){
                this.BodyData[i].bodyId = 0;
                this.BodyData[i].convertedPos = new K4AdotNet.Float2?[32];
            }
        }
        // PCL
        if(this.Scale == DisplayScale.DEPTH && this.ViewPCL)this.XYZImageData = new short[dw*dh*3];
        // IMU
        if(this.ViewIMU)this.device.StartImu();
    }
    //@ Initialize Kinect and get data from sensor
    public void InitKinect(){
        // Set Device to List (Running Sensor)
        SetDeviceID(K4ADevicesManager.Instance.K4AMngList.Count);
        K4ADevicesManager.Instance.SetK4ADevice(this);//! Not External Sync
        // Sensor
        this.DeviceConfig = new DeviceConfiguration{
            ColorResolution = this.CResln,
            ColorFormat = this.ImgFormat,
            DepthMode = this.DMode,
            SynchronizedImagesOnly = this.SyncImageOnly,
            CameraFps = this.Fps
        };
        this.device= Device.Open(this.DeviceID);
        //CheckHardwareVersion(this.device.Version);
        this.device.StartCameras(this.DeviceConfig);
        // Body Tracker
        this.device.GetCalibration(this.DeviceConfig.DepthMode, this.DeviceConfig.ColorResolution,out this.calib);
        this.TrackerConfig = TrackerConfiguration.Default;
        this.tracker = new Tracker(this.calib,this.TrackerConfig);
        // Image
        InitializeSensorData(this.DeviceConfig);
        // Get Data from Sensor
        Task task = KinectLoop();
    }
    //@ Get Body Data (Return the number of tracked bodies)
    private int GetBodyData(BodyFrame bframe){

        int len = bframe.BodyCount;
        //Debug.Log("GetBodyData:" + len　+"人-----------------------------------------");
        int[] NowBodyID = new int[len];
        int[] ActiveBodyDataIndex = new int[len];
        for(int i=0;i<len;i++){
            BodyId bodyId = bframe.GetBodyId(i);
            NowBodyID[i] = bodyId;
            //Debug.Log("bodyID" + bodyId+ "-----");  
            for(int j=0;j<this.BodyData.Length;j++){
                if(this.BodyData[j].isTracked&&bodyId.Value == this.BodyData[j].bodyId.Value){// 更新
                    bframe.GetBodySkeleton(i,out this.BodyData[j].skeleton);
                    //Debug.Log("BodyData[" + j + "].BodyID" + bodyId + "のスケルトン更新");
                    ActiveBodyDataIndex[i] = j;
                    break;
                }else if(bodyId>0){
                    if (this.BodyData[j].bodyId == 0) {
                        bframe.GetBodySkeleton(i,out this.BodyData[j].skeleton);
                        this.BodyData[j].bodyId     = bodyId;
                        this.BodyData[j].isTracked  = true;
                        //Debug.Log("BodyData["+j+"]:" + bodyId);
                        ActiveBodyDataIndex[i] = j;
                        break;
                    }
                }
            }
            //DebugBodyData();
        }

        //Debug
        //string log ="";
        //for (int i= 0;i<ActiveBodyDataIndex.Length ; i++) {
        //    log = log +ActiveBodyDataIndex[i] + " ";
        //}
        //Debug.Log("Active index " + log);

        for (int i = 0; i<this.BodyData.Length;i++) {
            //もともと０の場合はスキップ
            if (this.BodyData[i].bodyId == 0) {
                continue;
            }

            bool flag = false;
            for (int j = 0; j < ActiveBodyDataIndex.Length;j++) {
                if (i == ActiveBodyDataIndex[j]) {
                    flag = true;
                }
            }
            if (!flag) {
                //Debug.Log("修正!! BodyData[" + i + "] bodyID:" + BodyData[i].bodyId + "->0");
                this.BodyData[i].bodyId = 0;
                this.BodyData[i].isTracked = false;
                this.BodyData[i].convertedPos = new K4AdotNet.Float2?[32];
            }
        }
        DebugBodyData();


        return len;
    }

    public void ResetBodyData() {//田賀作　セッション終わりなどに呼び出す
        Debug.Log("ResetBodyData");
        for (int i = 0; i < this.BodyData.Length; i++) {
            this.BodyData[i].bodyId = 0;
            this.BodyData[i].isTracked = false;
            this.BodyData[i].convertedPos = new K4AdotNet.Float2?[32];
        }
    }

    public void DebugBodyData() {
        string log = "";
        for (int i = 0; i < this.BodyData.Length; i++) {
            log = log + " [" + i + "] id:"+BodyData[i].bodyId+" " + BodyData[i].isTracked ;
            //Debug.Log("BodyData["+i+"] bodyid=" + BodyData[i].bodyId + " isTracked=" + BodyData[i].isTracked);
        }
        //Debug.Log(log + "-------------------------------");
    }

    public int CountActiveBodyData() {
        int count = 0;
        for (int i = 0; i < this.BodyData.Length; i++) {
            if (this.BodyData[i].bodyId != 0) {
                count++;
            }    
        }
        return count;
    }

    //private int GetBodyData(BodyFrame bframe) {
    //    int len = bframe.BodyCount;
    //    for (int i = 0; i < len; i++) {
    //        BodyId bodyId = bframe.GetBodyId(i);
    //        for (int j = 0; j < this.BodyData.Length; j++) {
    //            if (this.BodyData[j].isTracked && bodyId.Value == this.BodyData[j].bodyId.Value) {// 
    //                bframe.GetBodySkeleton(i, out this.BodyData[j].skeleton);
    //                break;
    //            }
    //            else if (bodyId > 0) {
    //                bframe.GetBodySkeleton(i, out this.BodyData[j].skeleton);
    //                this.BodyData[j].bodyId = bodyId;
    //                this.BodyData[j].isTracked = true;
    //                break;
    //            }
    //        }
    //    }
    //    return len;
    //}
    //@ Get IMU Sample
    private void GetIMUSample(){
        this.ImuSampleData = this.device.GetImuSample();
    }    
    //@ Mapper: Color->Depth
    private Image MapColorImage2DepthScale(Image colorImage,Image depthImage){
        Image cvtimg = new Image(colorImage.Format,depthImage.WidthPixels,depthImage.HeightPixels);
        try{
            this.trans.ColorImageToDepthCamera(depthImage,colorImage,cvtimg);
        }catch(Exception ex){
            print(ex);
        }
        return cvtimg;
    }
    //@ Mapper: Depth->Color
    private Image MapDepthImage2ColorScale(Image depthImage,Image colorImage){
        Image cvtimg = new Image(depthImage.Format,colorImage.WidthPixels,colorImage.HeightPixels);
        try{
            this.trans.DepthImageToColorCamera(depthImage,cvtimg);
        }catch(Exception ex){
            print(ex);
        }
        return cvtimg;
    }
    //@ Mapper: Depth(Exclude Depth)->Color
    private Image MapDepthImage2ColorScaleCustom(Image depthImage,Image customImage,Image cvtDepthImage,Image colorImage){
        Image cvtimg = new Image(customImage.Format,colorImage.WidthPixels,colorImage.HeightPixels);
        try{
            this.trans.DepthImageToColorCameraCustom(depthImage,customImage,cvtDepthImage,cvtimg,TransformationInterpolation.Linear,colorImage.WidthPixels*colorImage.HeightPixels);
        }catch(Exception ex){
            print(ex);
        }
        return cvtimg;
    }
    //@ Mapper: Camera->Color
    private void MapCamera2ColorScale(){
        for(int i=0;i<this.BodyData.Length;i++){
            for(int j=0;j<32;j++){
                this.BodyData[i].convertedPos[j] 
                    = this.calib.Convert3DTo2D(this.BodyData[i].skeleton[(JointType)j].PositionMm,CalibrationGeometry.Color,CalibrationGeometry.Color);
            }
        }
    }
    //@ Mapper: Camera->Depth
    private void MapCamera2DepthScale(){
        for(int i=0;i<this.BodyData.Length;i++){
            for(int j=0;j<32;j++){
                this.BodyData[i].convertedPos[j] 
                    = this.calib.Convert3DTo2D(this.BodyData[i].skeleton[(JointType)j].PositionMm,CalibrationGeometry.Depth,CalibrationGeometry.Depth);
            }
        }
    }
    //@ Mapper: Depth->PCL
    private Image MapDepthImage2PCL(Image depthImage){
        int xyzImageStride = depthImage.WidthPixels * sizeof(short) * 3;
        Image cvtImage 
            = Image.CreateFromArray(this.XYZImageData, ImageFormat.Custom, 
                                    depthImage.WidthPixels, depthImage.HeightPixels, xyzImageStride);
        this.trans.DepthImageToPointCloud(depthImage,CalibrationGeometry.Depth,cvtImage);
        return cvtImage;
    }
    //@ Main: Get Data from Azure Kinect(Async Loop Process)
    private async Task KinectLoop(){
        this.trans = this.calib.CreateTransformation( );
        while(true){
            using(Capture capture = await Task.Run(()=>this.device.GetCapture()).ConfigureAwait(true)){
                // Get Body Data (BodyFrame)
                this.tracker.EnqueueCapture(capture);
                this.bodyFrame = this.tracker.PopResult();
                if(this.ViewSkeleton)this.NumOfTrackedBody = GetBodyData(this.bodyFrame);
                // Get Sensor Data (Color, Depth, IR, BodyIndex, Body, and IMU)
                switch(this.Scale){
                    case DisplayScale.SIMPLE: // Original Scale
                        Marshal.Copy(capture.ColorImage.Buffer,this.ColorData,0,this.ColorData.Length); // Color
                        Copy(capture.DepthImage.Buffer,this.DepthData,0,this.DepthData.Length);         // Depth
                        Marshal.Copy(capture.IRImage.Buffer,this.IRData,0,this.IRData.Length);          // IR
                        Marshal.Copy(this.bodyFrame.BodyIndexMap.Buffer,this.BodyIndexData,0,this.BodyIndexData.Length); // Body Index
                        if(this.ViewSkeleton)MapCamera2ColorScale();// Skeleton
                        if(this.ViewIMU)GetIMUSample(); // IMU
                        break;
                    case DisplayScale.COLOR: // Color Scale
                        // Color
                        Marshal.Copy(capture.ColorImage.Buffer,this.ColorData,0,this.ColorData.Length);     
                        // Depth
                        Image depthImg = MapDepthImage2ColorScale(capture.DepthImage,capture.ColorImage);   
                        Copy(depthImg.Buffer,this.DepthData,0,this.DepthData.Length);                       
                        // Body Index
                        Image biImg = MapDepthImage2ColorScaleCustom(capture.DepthImage,this.bodyFrame.BodyIndexMap,depthImg,capture.ColorImage);
                        Marshal.Copy(biImg.Buffer,this.BodyIndexData,0,this.BodyIndexData.Length);
                        // Skeleton
                        if(this.ViewSkeleton)MapCamera2ColorScale();
                        // IMU
                        if(this.ViewIMU)GetIMUSample();
                        break;
                    case DisplayScale.DEPTH: // Depth Scale
                        // Color
                        Image colorImg = MapColorImage2DepthScale(capture.ColorImage,capture.DepthImage);
                        Marshal.Copy(colorImg.Buffer,this.ColorData,0,this.ColorData.Length);
                        // Depth
                        Copy(capture.DepthImage.Buffer,this.DepthData,0,this.DepthData.Length);
                        // IR
                        Marshal.Copy(capture.IRImage.Buffer,this.IRData,0,this.IRData.Length);
                        // Body Index
                        Marshal.Copy(this.bodyFrame.BodyIndexMap.Buffer,this.BodyIndexData,0,this.BodyIndexData.Length);
                        // Skeleton
                        if(this.ViewSkeleton)MapCamera2DepthScale();
                        // PCL
                        if(this.ViewPCL){
                            Image pclImage = MapDepthImage2PCL(capture.DepthImage);
                            Copy(pclImage.Buffer,this.XYZImageData,0,this.XYZImageData.Length);
                        }
                        // IMU
                        if(this.ViewIMU)GetIMUSample();
                        break;
                    case DisplayScale.NONE: // Not 2D-View
                        if(this.ViewIMU)GetIMUSample();// IMU
                        break;
                    default:
                        break;
                }
            }
        }
    }
    //@ Close Device
    public void CloseDevice(){
        K4ADevicesManager.Instance.ReleaseK4ADevice(this.DeviceID);
        if(this.tracker != null){
            this.tracker.Shutdown();
            this.tracker.Dispose();
            this.tracker = null;
        }
        if(this.device!=null){
            if(this.ViewIMU) this.device.StopImu();
            this.device.StopCameras();
            this.device.Dispose();
            this.device = null;
        }
    }
    // OnDestroy is called before destroying attached object
    //! Do not forget write this process!
    private void OnDestroy(){
        CloseDevice();
    }
    //@ Hardware Version Information
    public void CheckHardwareVersion(HardwareVersion version){
        print($"ColorCameraFirmwareVersion:{version.ColorCameraFirmwareVersion}");
        print($"DepthCamereFirmwareVersion:{version.DepthCamereFirmwareVersion}");
        print($"AudioDeviceFirmwareVersion:{version.AudioDeviceFirmwareVersion}");
        print($"DepthSensorFirmwareVersion:{version.DepthSensorFirmwareVersion}");
        print($"FirmwareBuild:{version.FirmwareBuild}");
        print($"FirmwareSignature:{version.FirmwareSignature}");
    }
}