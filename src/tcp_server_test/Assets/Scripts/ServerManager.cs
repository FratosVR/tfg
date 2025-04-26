using Defective.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ServerManager : Singleton<ServerManager>
{
    //NO UTILIZAR AppManager.Instance 

    //All Locations Winners Info
    private string winnerMessage = "Felicidades";
    private int[] idWinner;
    private string[] usernameWinner;
    private string[] contactWinner;
    //DateTime and GoalDateList
    private DateTime currentAppDate = new DateTime();
    private List<DateTime> goalAppDateList;
    private bool[] goalDateReached;
    //Award Control
    private bool[] awardTaken;

    private System.Random random = new System.Random();

    #region userInfo
    //SETERS
    public void setUsernameWinner(string u, int location) { usernameWinner[location] = u; }
    public void setIdWinner(int u, int location) { idWinner[location] = u; }
    public void setContactWinner(string u, int location) { contactWinner[location] = u; }

    //GETTERS
    public string getWinnerMessage() { return winnerMessage; }
    public string getUsernameWinner(int location) { return usernameWinner[location]; }
    public int getIdWinner(int location) { return idWinner[location]; }
    public string getContactWinner(int location) { return contactWinner[location]; }

    public int generateRandomId()
    {
        //TODO hacer que no se repitan los numeros generados
        return random.Next(100);
    }
    #endregion

    #region DateInfo
    public DateTime getCurrentAppTime() { return currentAppDate; }
    public List<DateTime> getGoalAppDateList()
    {
        refreshGoalDates();
        return goalAppDateList;
    }

    public void setCurrentAppTime(DateTime serverTime) { currentAppDate = serverTime; }

    public bool[] getGoalDateReached() { return goalDateReached; }

    public void refreshGoalDates()
    {
        //Setting up GoalDateList to check if GoalDates are smaller than spected
        int dimension = goalAppDateList.Count;
        for (int i = 0; i < dimension; ++i)
        {
            while (goalAppDateList[i] < currentAppDate)
            {
                goalAppDateList[i] = goalAppDateList[i].AddDays(1);
            }
        }
        //Sort Goal DateTime List
        goalAppDateList.Sort();
    }
    public void addGoalDate(int hours = 0, int minutes = 0, int seconds = 0)
    {
        Debug.Log("GoalDate Added");
        DateTime aux = DateTime.Now;
        aux = aux.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
        goalAppDateList.Add(aux);
        refreshGoalDates();
    }
    public void addGoalMinutes(int minutes)
    {
        addGoalDate(0, minutes, 0);
    }
    public void addGoalSeconds(int seconds)
    {
        addGoalDate(0, 0, seconds);

    }
    #endregion

    #region Award
    //Award Generation
    [SerializeField]
    [Range(0, 1)]
    private float awardProbability = 0.5f;
    public int ballCountObjetive = 15;
    int ballCountToEnd = 0;

    //Getters
    public bool[] getAwardTaken() { return awardTaken; }

    //TODO do SERVER petition
    public bool generateAward(int location)
    {
        if (!awardTaken[location] && random.NextDouble() <= awardProbability)
        {
            awardTaken[location] = true;
        }
        return awardTaken[location];
    }

    public bool generateAwardCount(int location)
    {
        ballCountToEnd++;
        if (ballCountToEnd == ballCountObjetive - 15)
        {
            // Tercer audio
            GameManager.Instance.OnAlmostEndingGame();
            //estadoJuego(AppManager.gameStateServer.ganador);
        }

        if (ballCountToEnd == ballCountObjetive - 1)
        {
            // Tercer audio
            //GameManager.Instance.OnAlmostEndingGame();
            estadoJuego(AppManager.gameStateServer.ganador);
        }

        if (!awardTaken[location] && ballCountToEnd == ballCountObjetive)
        {
            awardTaken[location] = true;
        }

        return awardTaken[location];
    }

    #endregion



    private void Awake()
    {
        GlobalInstance = true;

        //All Locations Winners Info
        idWinner = new int[10];
        usernameWinner = new string[10];
        contactWinner = new string[10];
        //DateTime and GoalDateList
        currentAppDate = new DateTime();
        goalAppDateList = new List<DateTime>();
        goalDateReached = new bool[10];
        //Award Control
        awardTaken = new bool[10];

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "GeospatialTemplate")
        {
            ballCountToEnd = 0;
            AppManager.Instance.setIsWinner(0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set GoalDateTime
        goalAppDateList.Add(new DateTime(DateTime.Now.Year +1, 9, 28, 13, 0, 0));    //Pase de las 7
        goalAppDateList.Add(new DateTime(DateTime.Now.Year + 1, 9, 28, 14, 0, 0));   //Pase de las 8
        refreshGoalDates();
    }

    // Update is called once per frame
    void Update()
    {
        currentAppDate = DateTime.Now;  //TODO SERVER petition
                                        //Refresh goalDateReached per location

        for (int i = 0; i < goalDateReached.Count(); ++i)
        {

            //Debug.Log("CurrentAppTime: "+ getCurrentAppTime() + "  GoalAppDate" + goalAppDateList[0]);
            if (!goalDateReached[i] && goalAppDateList.Count != 0 && goalAppDateList[0] <= getCurrentAppTime())
                goalDateReached[i] = true;
        }
    }

    public void registerUser(string nombre, string apellidos, string email, string telefono, string dni, int terminos, int comunicaciones, string alias)
    {
        StartCoroutine(registerUserNet(nombre, apellidos, email, telefono, dni, terminos, comunicaciones, alias));
    }

    IEnumerator registerUserNet(string nombre, string apellidos, string email, string telefono, string dni, int terminos, int comunicaciones, string alias)
    {

        string message = "{    \"nombre\":\"" + nombre + "\",    \"apellidos\":\"" + apellidos + "\",    \"email\":\"" + email + "\",    \"terminos\":" + terminos + ",    \"comunicaciones\":\"" + comunicaciones + "\",    \"telefono\":\"" + telefono + "\",     \"dni\":\"" + dni + "\",     \"alias\":\"" + alias + "\" }";
        using (UnityWebRequest www = UnityWebRequest.Post("https://www.centrodealtoentretenimientoorange.com/wow/registro", message, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);
                string errorMessage = "";
                if (jsonObject != null && jsonObject.HasField("messages"))
                    errorMessage = quitarcomillas(jsonObject["messages"]["error"].ToString());
                else
                    errorMessage = www.error;

                Debug.Log(errorMessage);
                registerManager.Instance.showError(errorMessage);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);
                AppManager.Instance.setUserServerID(quitarcomillas(jsonObject["id"].ToString()));

                registerManager.Instance.toggleSMS(true);
                Debug.Log(AppManager.Instance.getUserID());
                Debug.Log("Register complete!");
            }
        }
    }


    public void loginUser(string codigo)
    {
        StartCoroutine(loginUserNet(AppManager.Instance.getUserServerID(), codigo));

    }
    IEnumerator loginUserNet(string id, string codigo)
    {

        //string message = "{\r\n    \"id\": \"" + id + "\",\r\n    \"code\": \"" + codigo + "\"\r\n}";

        string message = "{\r\n    \"id\": \"" + id + "\",\r\n    \"code\": \"" + codigo + "\",\r\n    \"ubicacion\":\"" + AppManager.Instance.getCurrentLocation() + "\",\r\n    \"idusuario\": \"" + AppManager.Instance.getUserID() + "\",\r\n    \"he_ganado\":" + AppManager.Instance.getIsWinner() + ",\r\n    \"alias\":\"" + AppManager.Instance.getUserAlias() + "\"\r\n}";
        Debug.Log("Mensaje login: " + message);
        using (UnityWebRequest www = UnityWebRequest.Post("https://www.centrodealtoentretenimientoorange.com/wow/login", message, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);
                string errorMessage = "";
                if (jsonObject != null && jsonObject.HasField("messages"))
                    errorMessage = quitarcomillas(jsonObject["messages"]["error"].ToString());
                else
                    errorMessage = www.error;
                registerManager.Instance.showError(errorMessage);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);
                AppManager.Instance.setUserJWT(quitarcomillas(jsonObject["token"].ToString()));


                registerManager.Instance.toggleSMS(false);
                registerManager.Instance.toggleRecogerPremio(true);

                //participa();
                Debug.Log(AppManager.Instance.getUserJWT());
                Debug.Log("Login complete!");
            }
        }
    }

    public void preloginUser(string telefono)
    {
        StartCoroutine(preLoginUserNet(telefono));
    }

    IEnumerator preLoginUserNet(string telefono)
    {

        string message = "{\r\n    \"telefono\": \"" + telefono + "\"\r\n}";
        using (UnityWebRequest www = UnityWebRequest.Post("https://www.centrodealtoentretenimientoorange.com/wow/prelogin", message, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);
                string errorMessage = quitarcomillas(jsonObject["messages"]["error"].ToString());
                registerManager.Instance.showError(errorMessage);
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Acierto: " + www.downloadHandler.text);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);
                //AppManager.Instance.setUserID(quitarcomillas(jsonObject["id"].ToString()));
                AppManager.Instance.setUserServerID(quitarcomillas(jsonObject["id"].ToString()));

                registerManager.Instance.toggleSMS(true);
                Debug.Log(AppManager.Instance.getUserID());
                Debug.Log("Login complete!");
            }
        }
    }

    public void estadoJuego(AppManager.gameStateServer state)
    {
        StartCoroutine(estadojuego(state));
    }

    IEnumerator estadojuego(AppManager.gameStateServer state)
    {
        Debug.Log("https://pre.minijuegos.cmasa.es/wow/ping/" + AppManager.Instance.getCurrentLocation().ToString() + "/" + AppManager.Instance.getUserPhone() + "/" + AppManager.Instance.getUserAlias() + "/" + state.ToString());
        //string message = "{\r\n    \"telefono\": \"" + "\"\r\n}";
        using (UnityWebRequest www = UnityWebRequest.Get("https://www.centrodealtoentretenimientoorange.com/wow/ping/" + AppManager.Instance.getCurrentLocation().ToString() + "/" + AppManager.Instance.getUserPhone() + "/" + AppManager.Instance.getUserAlias() + "/" + state.ToString()))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);
                //string errorMessage = quitarcomillas(jsonObject["messages"]["error"].ToString());
                //registerManager.Instance.showError(errorMessage);
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Acierto: " + www.downloadHandler.text);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);

                switch (state)
                {
                    case AppManager.gameStateServer.comienzo:
                        //Debug.Log(int.Parse(quitarcomillas(jsonObject["tiempo"].ToString())));
                        GameManager.Instance.setServerState(jsonObject["message"].ToString());

                        Debug.Log("Modo de juego : " + jsonObject["modo_juego"].ToString());
                        AppManager.Instance.setCurrentType((AppManager.gameType)(int.Parse(jsonObject["modo_juego"].ToString())-1));
                        //AppManager.Instance.setCurrentType(AppManager.gameType.entrenamiento);


                        if ((AppManager.Instance.getCurrentType() == AppManager.gameType.evento))
                        {
                            Debug.Log("Seteamos tiempo según server");
                            GameManager.Instance.addGoalSecondGame(int.Parse(quitarcomillas(jsonObject["tiempo"].ToString())));
                            GameManager.Instance.refreshCountdownTimer();

                        }
                        Debug.Log("Premio posible " + jsonObject["premio"].ToString());
                        //AppManager.Instance.setPremioPosible(5);
                        //AppManager.Instance.setIsWinner(5);
                        AppManager.Instance.setPremioPosible(int.Parse(quitarcomillas(jsonObject["premio"].ToString())));
                              this.ballCountObjetive = int.Parse(quitarcomillas(jsonObject["balones"].ToString()));

                        Debug.Log("Listado premios posibles " + jsonObject["listado_premio"].ToString());

                        var array = jsonObject["listado_premio"];

                        Debug.Log("Primer Premio " + array.count);
                                                                                  
                        foreach(JSONObject premio in array)
                        {
                            AppManager.Instance.addPremiosPosibles(quitarcomillas(premio.ToString()));
                        }
                        //AppManager.Instance.setPremiosPosibles((List<string>).ToString()));

                        break;

                    case AppManager.gameStateServer.final:
                        Debug.Log(int.Parse(quitarcomillas(jsonObject["tiempo"].ToString())));
                        GameManager.Instance.setServerState(jsonObject["message"].ToString());
                        //GameManager.Instance.addGoalSecondGame(int.Parse(quitarcomillas(jsonObject["tiempo"].ToString())));
                        //GameManager.Instance.refreshCountdownTimer();                             
                        //GameManager.Instance.setServerState(jsonObject["message"].ToString());
                        Debug.Log(jsonObject["ubicacion"].ToString());
                        break;

                    case AppManager.gameStateServer.ganador:

                        Debug.Log(int.Parse(quitarcomillas(jsonObject["tiempo"].ToString())));
                        GameManager.Instance.setServerState(jsonObject["message"].ToString());
                        //Debug.Log("Has ganado: " + int.Parse(jsonObject["he_ganado"].ToString())); 
                        Debug.Log("Has ganado existe: " + jsonObject.HasField("he_ganado").ToString());
                        if (jsonObject.HasField("he_ganado"))
                        {
                            Debug.Log("Has ganado: " + (quitarcomillas(jsonObject["he_ganado"].ToString())));
                            AppManager.Instance.setIsWinner(int.Parse(quitarcomillas(jsonObject["he_ganado"].ToString())));
                            //AppManager.Instance.setIsWinner(1);                                                                             //TODO BORRAR
                        }
                        //GameManager.Instance.addGoalSecondGame(int.Parse(quitarcomillas(jsonObject["tiempo"].ToString())));
                        //GameManager.Instance.refreshCountdownTimer();
                        Debug.Log(jsonObject["ubicacion"].ToString());
                        break;
                }



                Debug.Log("Login complete!");
            }
        }

    }



    public void gameInit()
    {
        StartCoroutine(gameInitNet());
    }

    IEnumerator gameInitNet()
    {
        using (UnityWebRequest www = UnityWebRequest.Post("https://www.centrodealtoentretenimientoorange.com/wow/init", null, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);
                string errorMessage = quitarcomillas(jsonObject["messages"]["error"].ToString());
                registerManager.Instance.showError(errorMessage);
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Acierto: " + www.downloadHandler.text);
                JSONObject jsonObject = new JSONObject(www.downloadHandler.text);
                //AppManager.Instance.setUserID(quitarcomillas(jsonObject["id"].ToString()));
                Debug.Log(jsonObject["url_video"].ToString());

                Debug.Log("Login complete!");
            }
        }
    }

    private string quitarcomillas(string str)
    {
        string strAux = str.Remove(0, 1);
        strAux = strAux.Remove(strAux.Length - 1, 1);
        return strAux;
    }
}


