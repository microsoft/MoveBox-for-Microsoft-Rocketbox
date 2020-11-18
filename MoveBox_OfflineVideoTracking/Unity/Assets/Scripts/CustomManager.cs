using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomManager: MonoBehaviour
{
    public GameObject[] avatars;
    private List<LoadPose> loadPoseScripts;
    private List<GameObject> Bip01Objs;

    // Start is called before the first frame update
    void Start()
    {
        loadPoseScripts = new List<LoadPose>();
        Bip01Objs = new List<GameObject>();
        for (int i=0; i < avatars.Length; i++)
        {
            loadPoseScripts.Add(avatars[i].GetComponent<LoadPose>());
            GameObject bip01Obj = avatars[i].transform.Find("Bip01").gameObject;
            Bip01Objs.Add(bip01Obj);            
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
          
            for(int i=0; i < loadPoseScripts.Count; i++)
            {
                loadPoseScripts[i].startRender = true;
                //Ajust the Bip01 position
                Bip01Objs[i].transform.rotation = Quaternion.Euler(90f, 0.0f, 90f);             
            }
        }
    }
}
