//* This script is manager of Devies(Kinect Sensor).
//* If you want to use a few or more devies on one PC, please use it.
//* (2020/03/07.Sat)
//* Y.Akematsu @yoslab

using System.Collections.Generic;

namespace K4AdotNet.Sensor{
public class K4ADevicesManager : SingletonMonoBehaviour<K4ADevicesManager>{
    public List<K4AManager> K4AMngList{get;private set;}
    
    //@ Set K4AManager to array
    public void SetK4ADevice(K4AManager manager){
        try{
            this.K4AMngList.Add(manager);
        }catch{

        }
    }
    //@ Release K4AManager from array
    public void ReleaseK4ADevice(int id){
        if(this.K4AMngList==null || this.K4AMngList.Count == 0)return;
        this.K4AMngList.RemoveAt(id);
        if(this.K4AMngList.Count>0){
            for(int i=0;i<this.K4AMngList.Count;i++){
                this.K4AMngList[i].SetDeviceID(i);
            }
        }
    }
    // Start is called before the first frame update
    private new void Awake()
    {
        base.Awake();
        this.K4AMngList = new List<K4AManager>();
    }
}
}
