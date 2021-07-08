using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;
using Newtonsoft.Json;
using System;

public class Login : MonoBehaviour
{

    private string AuthKey = "AIzaSyBgIVV4AMZcjJ3PCR1qt4nAz8PmvBOcwfc";
    private string databaseURL = "https://test-ebe23.firebaseio.com/Users/";
    public static string localid;
    public static string idToken;
    public static string username = null;
    public bool? success = null;
    public bool usernameGet = false; 
    public bool displayFail = false;
    public bool? signUpSuccess = null;
    public static User currentUser;
    public static string dob = null;


    public TMP_InputField signInEmail;
    public TMP_InputField signInPassword;
    public TMP_InputField signUpEmail;
    public TMP_InputField signUpUsername;
    public TMP_InputField signUpPassword;
    public TMPro.TMP_Dropdown Month;
    public TMPro.TMP_Dropdown Year;
    EventSystem system;


    public TextMeshProUGUI failLogin;
    public string[] signUpErrorTxt = { "Password Needs to be 6 characters or longer!", 
        "Email needs to be in a proper format!"};

    private void Start()
    {
        //system = EventSystem.current;
    }
    private void Update()
    {

        if (success == true && usernameGet)
        {
            loadScene();
        }
        else if (success == false && !displayFail)
        {
            StartCoroutine("displayInvalidUser");
        }
        else if (signUpSuccess == false && !displayFail)
        {
            StartCoroutine("displayInvalidUser");
        }

    }

    // Apparently you need to have password length > 6 and cannot sign up with same email otherwise error
    public void SignUpUser(string email, string name, string password)
    {
        string userData = "{\"email\":\""+ email +"\", \"password\":\""+password+"\"}";
        RestClient.Post(url: "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + AuthKey, userData).Then(onResolved: response =>
         {
             SignResponse r = JsonConvert.DeserializeObject<SignResponse>(response.Text);
             localid = r.localid;
             idToken = r.idToken;
             RestClient.DefaultRequestHeaders["Authorization"] = idToken;
             username = name;
             PostToDatabase();
         }).Catch(error =>
        {
            signUpSuccess = false;
        });
    }


    public void SignInUser(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\"}";
        RestClient.Post(url: "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + AuthKey, userData).Then(onResolved: response =>
        {
            
            SignResponse r = JsonConvert.DeserializeObject<SignResponse>(response.Text);
            localid = r.localid;
            idToken = r.idToken;
            success = true;
            RestClient.DefaultRequestHeaders["Authorization"] = idToken;
            getUsername(localid);
        }).Catch(error =>
        {
            success = false;
        });


    }

    public void signUpButton()
    {
        if (signUpPassword.text.Length < 6)
        {
            StartCoroutine("displaySignupError", 0);
            return;
        }
        if (!signUpEmail.text.Contains("@") && !signUpEmail.text.Contains(".com"))
        {
            StartCoroutine("displaySignupError",1);
            return;
        }
        if (signUpEmail.text != "" && signUpUsername.text != "" && signUpPassword.text != "")
        {
            dob = Month.options[Month.value].text + Year.options[Year.value].text;
            SignUpUser(signUpEmail.text, signUpUsername.text, signUpPassword.text);
        }
    }



    public void signInButton()
    {
        SignInUser(signInEmail.text, signInPassword.text);
        
    }

    private void PostToDatabase()
    {
        Difficulty tempDiff = new Difficulty(0, 0, 0, 0);
        User user = new User(username, dob, 0, tempDiff);
        RestClient.Put(url: "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Users/" + localid + ".json", user).Then(onResolved:response => {
            currentUser = user;
            loadScene();
        }).Catch(error => {
            signUpSuccess = false;
            print("error");
        });
    }

    IEnumerator displayInvalidUser()
    {
        displayFail = true;
        failLogin.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        failLogin.gameObject.SetActive(false);
        displayFail = false;
        success = null;
    }

    IEnumerator displaySignupError(int val)
    {
        failLogin.text = signUpErrorTxt[val];
        failLogin.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        failLogin.gameObject.SetActive(false);
        signUpSuccess = null;
    }

    public void loadScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void getUsername(string userid)
    { 
        RestClient.Get(url: "https://test-ebe23-default-rtdb.asia-southeast1.firebasedatabase.app/Users/" + userid + ".json").Then(onResolved: response =>
        {
            User user = JsonConvert.DeserializeObject<User>(response.Text);
            username = user.username;
            dob = user.dob;
            Difficulty tempDiff = user.diff;
            int totalPoints = user.Total_Points;
            currentUser = new User(username, dob, totalPoints, tempDiff);
            usernameGet = true;
        }).Catch(error => {
            usernameGet = false;
            print("Username Error");
        });
    }
}
