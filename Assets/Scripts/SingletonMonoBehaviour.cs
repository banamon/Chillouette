using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T>  : MonoBehaviour where T:SingletonMonoBehaviour<T> {

	protected static readonly string[]findTags = {
		"GameController"
	};

	protected static T instance;

	public static T Instance{
		get{
			if(instance== null){
				Type type = typeof(T);
				foreach(var tag in findTags){// タグで管理系の"可能性のある"オブジェクトを探しだす
					GameObject [] objs = GameObject.FindGameObjectsWithTag(tag);

					for(int j = 0;j<objs.Length;j++){
						instance = (T)objs[j].GetComponent(type);
						if(instance!=null)return instance;
					}
				}
			}
			return instance;
		}
	}

	protected bool CheckInstance(){
		if(instance==null){
			instance = (T)this;
			DontDestroyOnLoad(this.gameObject);
			return true;
		}else if(Instance == this){
			return true;
		}
		Destroy(this.gameObject);
		return false;
	}
	
	virtual protected void Awake(){// UnityのStartより早くに始めるメソッド(親クラスなのでvirtual)
        CheckInstance();
    }
}