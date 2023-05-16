using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostComentManeger : MonoBehaviour
{
    [SerializeField]
    GameObject CreateQRObject;
    CreateQR createqr;

    [SerializeField]
    GameObject PostComentUI;

    // Start is called before the first frame update
    void Start()
    {
        PostComentUI.SetActive(false);
        createqr = CreateQRObject.GetComponent<CreateQR>();
    }

    public void StartComentUI() {
        PostComentUI.SetActive(true);
        //QR•\Ž¦
        createqr.ShowQR_Coment(IDManeger.uid, IDManeger.GetQID(),IDManeger.GetQuestion()) ;
    }
}
