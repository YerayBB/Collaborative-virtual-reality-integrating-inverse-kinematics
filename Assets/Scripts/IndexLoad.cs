using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class IndexLoad : MonoBehaviour
{
    #region MonoBehaviour CallBacks
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/Data";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path += "/Index.txt";
        if (File.Exists(path)){
            StreamReader st = new StreamReader(path);
            gameObject.GetComponent<Text>().text = st.ReadLine();
            st.Close();
        }
        else
        {
            gameObject.GetComponent<Text>().text = "00";
        }
    }
    #endregion
}
