using RootMotion.FinalIK;
using UnityEngine;
using System.IO;
using System;
using Photon.Pun;

public class DataCollector : MonoBehaviourPun
{
    #region Private variables
    //Related to the path file
    private string filepath;
    private string finalname;
    private StreamWriter stremwr = null;
    private int actualframe;

    //positions of the players
    private Vector2 lastplace;
    private Vector2 lastplace2;
    private Transform tracking = null;
    private Transform tracking2 = null;

    //Related to the recording system 
    private bool graba = false;
    private bool activo = false;
    private float starttime;
    private float reloadtime;
    #endregion

    #region Public variables
    //Related to the path
    public string filename { get; set; }
    public bool indexed { get; set; }
    public bool resetb { get; set; }

    //Players
    public GameObject player = null;
    public GameObject obstacle = null;

    //related to the recording system
    public bool on = true;
    [Tooltip("Minimo de velocidad para que escriba datos en fichero") ]
    public float threshold = 0.0f;
    public float rectime;
    public bool columon;
    public bool themaster = false;
    #endregion

    #region Public Metods

    public void Activate()
    {
        if (on && graba)
        {
            Debug.Log("SE EMPIEZA A ACTIVAR");
            activo = true;
            starttime = Time.time;
            //actualframe = 0;
            tracking = player.GetComponent<VRIK>().references.head;
            if (obstacle.GetComponent<VRIK>() == null)
            {
                tracking2 = obstacle.transform;
            }
            else
            {
                tracking2 = obstacle.GetComponent<VRIK>().references.head;
            }
            lastplace = new Vector2(tracking.position.x - transform.position.x, tracking.position.z - transform.position.z);
            lastplace2 = new Vector2(tracking2.position.x - transform.position.x, tracking2.position.z - transform.position.z);
            //obsPos = new Vector2(obstacle.transform.position.x, obstacle.transform.position.z);
            filepath = Application.dataPath + "/Data";
            finalname = "/" + filename;
            if (indexed)
            {
                if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath);
                string text = "";
                if (resetb || !File.Exists(filepath + "/Index.txt"))
                {
                    text = "00";
                    StreamWriter outStream = File.CreateText(filepath + "/Index.txt");
                    outStream.WriteLine(text);
                    outStream.Close();
                }
                else
                {
                    StreamReader st = new StreamReader(filepath + "/Index.txt");
                    text = st.ReadLine();
                    st.Close();
                }
                finalname = finalname + text;
                int auxi = Convert.ToInt32(text) + 1;
                text = auxi.ToString();
                if (auxi < 10) text = "0" + text;
                StreamWriter sw = File.CreateText(filepath + "/Index.txt");
                sw.WriteLine(text);
                sw.Close();
            }
            finalname = finalname + ".csv";
            stremwr = File.CreateText(filepath + finalname);
            string[] rowTemp = new string[8];
            rowTemp[0] = "Time stamp";
            rowTemp[1] = "Con piezaJ1";
            rowTemp[2] = "Con piezaJ2";
            rowTemp[3] = "PosicionJ1";
            rowTemp[4] = "PosicionJ2";
            rowTemp[5] = "VelocidadJ1";
            rowTemp[6] = "VelocidadJ2";
            rowTemp[7] = "Distancia";
            stremwr.WriteLine(string.Join(",", rowTemp));
            InvokeRepeating("PrintData", 0, rectime);
            if (rectime >= 0.5) reloadtime = 0.1f;
            else reloadtime = rectime * 0.1f;
            reloadtime = rectime - reloadtime;
            Invoke("UpdatePos", reloadtime);
            Debug.Log("Se ACTIVÖ");
        }
        else
        {
            Invoke("Activate", 0.1f);
        }
    }

    public void GrabaSwitch()
    {
        graba ^= true;
    }

    public void TheMaster(bool x)
    {
        themaster = x;
    }
    #endregion

    #region Monobehaviour CallBacks
    private void OnApplicationFocus(bool focus)
    {
        on = focus;
    }

    private void OnApplicationQuit()
    {
        if (stremwr != null)
        {
            if (stremwr.BaseStream != null) stremwr.Close();
        }
    }
    #endregion

    #region Private metods
    private void PrintData()
    {
        Debug.Log("PATATA");
        Debug.Log(activo.ToString() + on.ToString() + graba.ToString());
        if (activo && on && graba)
        {
            Debug.Log("FRITA");
            float taux = Time.time - starttime;
            string[] data = new string[8];
            Vector2 trac1xz = new Vector2(tracking.position.x - transform.position.x, tracking.position.z - transform.position.z);
            Vector2 trac2xz = new Vector2(tracking2.position.x - transform.position.x, tracking2.position.z - transform.position.z);
            float tres = speed(trac1xz, lastplace);
            float cuatro = speed(trac2xz, lastplace2);
            if (tres >= threshold || cuatro >= threshold)
            {
                data[0] = ToTimeString(taux).Replace(',', '.');
                data[1] = player.GetComponent<PloyerControler>().holding.ToString();
                data[2] = obstacle.GetComponent<PloyerControler>() == null ? false.ToString() : obstacle.GetComponent<PloyerControler>().holding.ToString();
                data[3] = "(" + trac1xz.x.ToString().Replace(',', '.') + "; " + trac1xz.y.ToString().Replace(',', '.') + ")";
                Debug.Log(data[3]);
                data[4] = "(" + trac2xz.x.ToString().Replace(',', '.') + "; " + trac2xz.y.ToString().Replace(',', '.') + ")";
                Debug.Log(data[4]+" hy");
                data[5] = tres.ToString().Replace(',', '.');
                data[6] = cuatro.ToString().Replace(',', '.');
                data[7] = distance(trac1xz, trac2xz).ToString().Replace(',', '.');
                stremwr.WriteLine(string.Join(",", data));
            }
        }
        Invoke("UpdatePos", reloadtime);
    }

    private void UpdatePos()
    {
        lastplace = new Vector2(tracking.position.x - transform.position.x, tracking.position.z - transform.position.z);
        lastplace2 = new Vector2(tracking2.position.x - transform.position.x, tracking2.position.z - transform.position.z);
    }

    private string ToTimeString(float x)
    {
        float h = x / 3600;
        float m = x % 3600 / 60;
        float s = x % 60;
        return ((int)Math.Floor(h)).ToString()+":"+((int)Math.Floor(m)).ToString()+":"+s.ToString("0.00");
    }

    private float speed(Vector2 x, Vector2 y)
    {
        return (float)(distance(x, y) / (rectime-reloadtime));
    }
    private double distance(Vector2 x, Vector2 y)
    {
        double a = (x.x - y.x);
        double b = (x.y - y.y);
        return Math.Sqrt(a * a + b * b);
    }
    #endregion

    #region RPC metods
    [PunRPC]
    public void AddPlayerDC(int GOPlayer, bool p2)
    {
        Debug.Log("RPC PLAYERDC CALLED WITH ID " + GOPlayer.ToString());
        if (PhotonNetwork.CurrentRoom.MaxPlayers == (byte)1)
        {
            player = PhotonView.Find(GOPlayer).gameObject;
            if (columon)
            {
                obstacle = PhotonNetwork.InstantiateSceneObject("Colum", Vector3.zero, Quaternion.identity);
                obstacle.GetComponent<CilinderAdjust>().player = player;
                obstacle.SetActive(true);
            }
            else
            {
                obstacle = PhotonNetwork.Instantiate("Nothing", Vector3.zero, Quaternion.identity);
            }
        }
        if (p2)
        {
            obstacle = PhotonView.Find(GOPlayer).gameObject;
        }
        else
        {
            player = PhotonView.Find(GOPlayer).gameObject;
        }
        if (PhotonNetwork.IsMasterClient && themaster)
        {
            Debug.Log("EsMaster");
            if (obstacle != null && player != null)
            {
                Debug.Log("MATACOLUMNAS " + columon.ToString());
                if (obstacle.GetComponent<VRIK>() == null) obstacle.SetActive(columon);
                Activate();
            }
        }
        /*if (player == null)
        {
            Debug.Log(PhotonNetwork.CurrentRoom.MaxPlayers);
            player = PhotonView.Find(GOPlayer).gameObject;
            if (PhotonNetwork.CurrentRoom.MaxPlayers == (byte)1)
            {
                Debug.Log("SetUpObstacle");
                
                if (columon)
                {
                    obstacle = PhotonNetwork.InstantiateSceneObject("Colum", Vector3.zero, Quaternion.identity);
                    obstacle.GetComponent<CilinderAdjust>().player = player;
                    obstacle.SetActive(true);
                }
                else
                {
                    obstacle = PhotonNetwork.Instantiate("Nothing", Vector3.zero, Quaternion.identity);
                }
            }
        }
        else
        {
            obstacle = PhotonView.Find(GOPlayer).gameObject;
        }
        if (PhotonNetwork.IsMasterClient && themaster)
        {
            Debug.Log("EsMaster");
            if (obstacle != null)
            {
                Debug.Log("MATACOLUMNAS " + columon.ToString());
                if (obstacle.GetComponent<VRIK>() == null) obstacle.SetActive(columon);
                Activate();
            }
        }
    }previo a fijar 1 a master*/
    }
    #endregion
}
