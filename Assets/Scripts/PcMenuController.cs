using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PcMenuController : MonoBehaviour
{
    //public Toggle multi; 
    //public GameObject[] players = new GameObject[2];
    public Toggle obson;
    public DataCollector dc = null;
    public InputField savename = null;
    public Toggle indexed;
    public Toggle reseti;
    public GameObject mc = null;
    public GameObject[] cs = new GameObject[5];
    private int cactive = 0;
    private Rect izq = new Rect(0, 0.501f, 0.499f, 0.499f);
    private Rect der = new Rect(0.501f, 0.501f, 0.499f, 0.499f);
    private Rect frontal = new Rect(0.25f, 0, 0.5f, 0.499f);
    private Rect inicialrect = new Rect(0, 0, 1, 1);
    private bool vision = false;
    public GameObject helptext = null;
    private bool helpon = false;
    public GameObject recindi = null;
    public InputField loadname = null;
    public InputField loadpath = null;
    public Toggle showobs;
    public GameObject columna = null;
    public GameObject trackprefab = null;
    private GameObject trackingj1 = null;
    private GameObject trackingj2 = null;
    private StreamReader visfile = null;
    private int vispos;
    private bool autovis = false;
    public Toggle visautotoggle = null;
    //public GameObject markprefab = null;
    public GameObject markj1 = null;
    public GameObject markj2 = null;
    public InputField rectime = null;
    public InputField speedthreshold = null;
    //public NetManagerV nm = null;
    private bool como = false;
    public GameObject hostMenu;
    public GameObject clientMenu;
    private float vistemp = 0;

    private GameObject newmenu;

    private GameObject zeropos;

    public GameObject columnprefab;

    // Start is called before the first frame update
    void Start()
    {
        loadpath.text = Application.dataPath + "/Data/";
        loadname.text = "Dummy01";
        rectime.text = "0.5";
        speedthreshold.text = "0";
        zeropos = GameObject.Find("ZERO");
        /*status.text = "Desconectado";
        nm.SetOffline(true);*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            int ola;
            ola = como ? -10 : 10;
            como ^= true;
            GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>().depth = ola;
        }
        if (vision)
        {
            if (cactive != 0 && cactive != 5)
            {
                helptext.SetActive(helpon);
            }
            else
            {
                helptext.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SetCameraOn(0);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SetCameraOn(3);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SetCameraOn(1);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SetCameraOn(2);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SetCameraOn(5);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (cactive != 5)
                {
                    SetCameraOn((cactive+1) % 3 + 1);
                }
                else SetCameraOn(3);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (cactive != 5)
                {
                    SetCameraOn((cactive) % 3 + 1);
                }
                else SetCameraOn(1);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                helpon ^= true;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dc.GrabaSwitch();
                recindi.GetComponent<Toggle>().isOn ^= true;
            }
        }
        if (vispos > 0)
        {
            if (Input.GetKey(KeyCode.N) || autovis)
            {
                if (Time.time - vistemp >= 0.1)
                {
                    vistemp = Time.time;
                    AddNextPoint();
                }
            }
        }
    }

    public void ActiveCamSystem(bool x)
    {
        vision = x;
    }
    
    public void Visualizar()
    {
        if (showobs.isOn)
        {
            //columna = PhotonNetwork.InstantiateSceneObject("colum", Vector3.zero, Quaternion.identity);
            columna.GetComponent<CilinderAdjust>().player = columna;
            columna.SetActive(true);
        }
        autovis = visautotoggle.isOn;
        trackingj1 = Instantiate(trackprefab);
        Material colored = trackingj1.GetComponent<LineRenderer>().material;
        colored.color = Color.blue;
        Material newone = new Material(colored);
        newone.color = Color.black;
        //markj1 = markprefab;
        markj1.GetComponent<Renderer>().material = newone;

        trackingj2 = Instantiate(trackprefab);
        Material anotherone = trackingj2.GetComponent<LineRenderer>().material;
        anotherone.color = Color.red;
        Material anotherother = new Material(anotherone);
        anotherother.color = Color.white;
        //markj2 = markprefab;
        markj2.GetComponent<Renderer>().material = anotherother;
        /*tracking.transform.position = new Vector3(0, 0.2f, 0);
        LineRenderer lr = tracking.AddComponent<LineRenderer>();
        lr.loop = false;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;*/

        string filepath = loadpath.text;
        filepath += filepath[filepath.Length-1] == '/' ? loadname.text : "/" + loadname.text;
        filepath += ".csv";
        visfile = new StreamReader(filepath);
        visfile.ReadLine();
        AddNextPoint();
    }

    public void SetColumn(bool x)
    {
        if (x)
        {
            columna = GameObject.Instantiate(columnprefab);
            //columna = PhotonNetwork.InstantiateSceneObject("Colum", Vector3.zero, Quaternion.identity) as GameObject;
            //Debug.Log(columna.scene);
            Debug.Log("COLVIS CREADA");
        }
        else
        {
            PhotonNetwork.Destroy(columna.GetComponent<PhotonView>());
            Debug.Log("COLVIS DESTRUIDA");
        }
    }

    public void ResetVisualizar()
    {
        Destroy(trackingj1);
        Destroy(trackingj2);
        ActiveCamSystem(false);
        visfile.Close();
        helpon = false;
        vispos = 0;
        cactive = 0;
        if (showobs.isOn)
        {
            PhotonNetwork.Destroy(columna);
        }
    }

    private void AddNextPoint()
    {
        if (!visfile.EndOfStream)
        {
            string[] line = visfile.ReadLine().Split(',');
            string[] auxj1 = line[3].Split(';');
            string[] auxj2 = line[4].Split(';');
            /*foreach(string ola in aux)
            {
                Debug.Log("/ "+ola.Replace(',','.')+"/");
            }
            Debug.Log(aux[0].Substring(1).Replace(',', '.'));
            Debug.Log(float.Parse(aux[0].Substring(1)/*.Replace(',','.'), System.Globalization.CultureInfo.InvariantCulture));
            Debug.Log(float.Parse(aux[0].Substring(1)/*.Replace(',','.')));
            Debug.Log(float.Parse(aux[0].Substring(1).Replace(',','.'), System.Globalization.CultureInfo.InvariantCulture));
            Debug.Log(float.Parse(aux[0].Substring(1).Replace(',','.')));
            Debug.Log(aux[1].Substring(1, aux[1].Length - 1).Replace(',', '.'));*/

            float x = float.Parse(auxj1[0].Substring(1).Replace(',','.'), System.Globalization.CultureInfo.InvariantCulture);
            float z = float.Parse(auxj1[1].Substring(1, auxj1[1].Length - 2).Replace(',','.'), System.Globalization.CultureInfo.InvariantCulture);
            Vector3 nextpos = new Vector3(x + zeropos.transform.position.x, 0.2f, z + zeropos.transform.position.z);
            trackingj1.GetComponent<LineRenderer>().positionCount = vispos + 1;
            trackingj1.GetComponent<LineRenderer>().SetPosition(vispos, nextpos);
            GameObject nextmark = Instantiate(markj1, nextpos, Quaternion.identity);
            nextmark.transform.parent = trackingj1.transform;

            x = float.Parse(auxj2[0].Substring(1).Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            z = float.Parse(auxj2[1].Substring(1, auxj2[1].Length - 2).Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            nextpos = new Vector3(x + zeropos.transform.position.x, 0.2f, z + zeropos.transform.position.z);
            trackingj2.GetComponent<LineRenderer>().positionCount = vispos + 1;
            trackingj2.GetComponent<LineRenderer>().SetPosition(vispos, nextpos);
            nextmark = Instantiate(markj2, nextpos, Quaternion.identity);
            nextmark.transform.parent = trackingj2.transform;

            vispos += 1;
        }
    }

    public void ExperimentStart()
    {
        dc.filename = savename.text;
        Debug.Log(savename.text);
        Debug.Log(dc.filename);
        dc.indexed = indexed.isOn;
        dc.resetb = reseti.isOn;
        //PhotonNetwork.Instantiate("StartMenu", new Vector3(3.9f, 1.2f, 3.36f), Quaternion.identity).SetActive(true);
        //mc.conobs = obson.isOn;
        Debug.Log("ES " + obson.isOn.ToString());
        dc.columon = obson.isOn;
        Debug.Log("Y AHORA " + dc.columon.ToString());
        dc.threshold = float.Parse(speedthreshold.text, System.Globalization.CultureInfo.InvariantCulture);
        dc.rectime = float.Parse(rectime.text, System.Globalization.CultureInfo.InvariantCulture);
        newmenu = (GameObject) GameObject.Instantiate(mc);//.SetActive(true);
        newmenu.transform.parent = GameObject.Find("The Center").transform;
        newmenu.transform.localPosition = new Vector3(0, 1.2f, 1f);
        //Invoke("MenuOn", 2);
        //bool helo = newmenu.activeSelf == true;

        /*if (multi)
        {
            players[1].transform.DetachChildren();
        }
        players[0].transform.DetachChildren();
        Destroy(players[0]);
        for (int i = 0; i < players[1].transform.childCount; ++i)
        {
            Destroy(players[1].transform.GetChild(i).gameObject);
        }
        Destroy(players[1]);*/
        vision = true;
    }

    private void MenuOn()
    {
        newmenu.SetActive(true);
    }

    private void SetCameraOn(int x)
    {
        if (x != cactive)
        {
            if (x == 5)
            {
                cs[cactive].SetActive(false);
                cs[1].GetComponent<Camera>().rect = izq;
                cs[2].GetComponent<Camera>().rect = der;
                cs[3].GetComponent<Camera>().rect = frontal;
                cs[1].SetActive(true);
                cs[2].SetActive(true);
                cs[3].SetActive(true);
                cs[4].SetActive(true);
                cactive = x;
                recindi.GetComponent<Canvas>().worldCamera = cs[2].GetComponent<Camera>();
            }
            else{
                if (cactive != 5)
                {
                    cs[x].SetActive(true);
                    cs[cactive].SetActive(false);
                    helptext.GetComponent<Canvas>().worldCamera = cs[x].GetComponent<Camera>();
                    recindi.GetComponent<Canvas>().worldCamera = cs[x].GetComponent<Camera>();
                    cactive = x;
                }
                else
                {
                    cs[1].GetComponent<Camera>().rect = inicialrect;
                    cs[2].GetComponent<Camera>().rect = inicialrect;
                    cs[3].GetComponent<Camera>().rect = inicialrect;
                    cs[1].SetActive(false);
                    cs[2].SetActive(false);
                    cs[3].SetActive(false);
                    cs[4].SetActive(false);
                    cs[x].SetActive(true);
                    helptext.GetComponent<Canvas>().worldCamera = cs[x].GetComponent<Camera>();
                    recindi.GetComponent<Canvas>().worldCamera = cs[x].GetComponent<Camera>();
                    cactive = x;
                }
            }
        }
    }

    public void ActiveCurrentMenu(){
        hostMenu.SetActive(PhotonNetwork.IsMasterClient);
        clientMenu.SetActive(!PhotonNetwork.IsMasterClient);
    }

    public void Reset()
    {
        Debug.Log("Poff, cruzo los dedos");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Cierra()
    {
        Debug.Log("se cerro");
        Application.Quit();
    }
}
