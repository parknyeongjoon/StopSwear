using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    UIManager uiManager;
    HttpController httpController;
    AudioRecorderController recorder;
    ToastController toast;

    private void Start()
    {
        uiManager = UIManager.Instance();
        httpController = HttpController.Instance();
        recorder = AudioRecorderController.Instance();
        toast = ToastController.Instance();
    }

    #region utility

    void AutoSignIn() // 나중에 하기
    {
        EmailIF.text = PlayerPrefs.GetString("");
    }

    public IEnumerator SignIn()
    {
        string email = EmailIF.text;
        string pwd = PWDIF.text;
        pwd = GetSha256Hash(pwd);

        JObject userDataJson = new JObject();
        userDataJson["email"] = EmailIF.text;
        userDataJson["password"] = GetSha256Hash(PWDIF.text);

        string userDataString = userDataJson.ToString();

        yield return httpController.PostMethod("auth/login", userDataString, (response) =>
        {
            if(response != null)
            {

                JToken responseToken = JToken.Parse(response);
                httpController.SetJwtToken(responseToken["token"].ToString());

                // after login set options
                if (keepLoginToggle.isOn)
                {
                    PlayerPrefs.SetInt("KeepLogin", 1);
                    PlayerPrefs.SetString("Email", EmailIF.text);
                    PlayerPrefs.SetString("Password", PWDIF.text);
                }

                uiManager.SetInfoPanel(responseToken["role"].ToString());
                uiManager.CloseLoginTab();
                if (responseToken["role"].ToString() == "STUDENT")
                {
                    recorder.Set_JWT_TOKEN(responseToken["token"].ToString());
                    recorder.StartRecording();
                }
            }
        });
    }

    public void StartSignIn()
    {
        StartCoroutine(SignIn());
    }

    public IEnumerator SignUp()
    {
        bool emailExist = false;
        JObject userDataJson = new JObject();
        userDataJson["email"] = SUEmailIF.text;
        userDataJson["password"] = GetSha256Hash(SUPWDIF.text);
        userDataJson["name"] = SUuserNameIF.text;

        yield return httpController.GetMethod("auth/exists/email?email=" + SUEmailIF.text, (response) =>
        {
            if (response == "true")
            {
                toast.showToast("이메일이 이미 존재합니다. 다른 이메일을 사용해주세요.");
                emailExist = true;
            }
        });

        if (emailExist)
        {
            yield break;
        }


        if (isTeacherToggle.isOn) // teacher
        {
            bool schoolIdExist = true;
            bool classIdExist = false;
            yield return httpController.GetMethod("auth/exists/school?schoolId=" + SUCodeIF.text , (response) =>
            {
                if (response == "false")
                {
                    toast.showToast("학교 코드가 올바르지 않습니다.");
                    schoolIdExist = false;
                }
            });
            yield return httpController.GetMethod("auth/exists/class?classId=" + SUclassNameIF.text, (response) =>
            {
                if (response == "true")
                {
                    toast.showToast("학급이름이 이미 존재합니다.");
                    classIdExist = true;
                }
            });
            if (!schoolIdExist || classIdExist)
            {
                yield break;
            }

            userDataJson["create"] = false;
            userDataJson["schoolId"] = SUCodeIF.text;
            userDataJson["className"] = SUclassNameIF.text;
            string userDataString = userDataJson.ToString();


            yield return httpController.PostMethod("auth/signup/teacher", userDataString, (response) =>
            {
                toast.showToast("가입 성공");
                CloseCheckPanel();
                OpenSignInPanel();
            });
        }
        else // student
        {
            bool classIdExist = true;
            yield return httpController.GetMethod("auth/exists/class?classId=" + SUCodeIF.text, (response) =>
            {
                if (response == "false")
                {
                    classIdExist = false;
                }
            });
            if (!classIdExist)
            {
                yield break;
            }

            userDataJson["classId"] = SUCodeIF.text;
            string userDataString = userDataJson.ToString();
            yield return httpController.PostMethod("auth/signup/student", userDataString, (response) =>
            {
                CloseCheckPanel();
                OpenSignInPanel();
            });
        }
    }

    public void StartSignUp()
    {
        StartCoroutine(SignUp());
    }

    string GetSha256Hash(string input)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }

    void SetUserInfo()
    {
        // 서버에서 받아와서 넣기
        UserInfo userInfo = new UserInfo();
    }

    public void LogOut()
    {
        PlayerPrefs.SetInt("KeepLogin", 0);
        PlayerPrefs.DeleteKey("Email");

        uiManager.OpenLoginTab();
        uiManager.SetInfoPanel(null);
        OpenSignInPanel();
    }
    #endregion

    #region UI
    [SerializeField] GameObject SignInPanel;
    [SerializeField] TMP_InputField EmailIF, PWDIF, userNameIF;
    [SerializeField] Toggle keepLoginToggle;
    [SerializeField] GameObject FindPWDPanel;
    [SerializeField] GameObject SignUpPanel;
    [SerializeField] TMP_InputField SUEmailIF, SUPWDIF, SUuserNameIF, SUCodeIF, SUclassNameIF;
    [SerializeField] Toggle isTeacherToggle;
    [SerializeField] GameObject VoiceCheckPanel;

    public void OpenSignInPanel()
    {
        CloseAllPanel();
        SignInPanel.SetActive(true);
    }

    public void OpenFindPWDPanel()
    {
        CloseAllPanel();
        FindPWDPanel.SetActive(true);
    }

    public void OpenSignUpPanel()
    {
        CloseAllPanel();
        SignUpPanel.SetActive(true);
    }

    public void CloseAllPanel()
    {
        SignInPanel.SetActive(false);
        FindPWDPanel.SetActive(false);
        SignUpPanel.SetActive(false);
    }

    #region signup
    public void ToggleClassNameIF()
    {
        SUclassNameIF.gameObject.SetActive(isTeacherToggle.isOn);
    }

    public void OpenCheckPanel()
    {
        if (!isTeacherToggle.isOn)
        {
            VoiceCheckPanel.SetActive(true);
        }
        else
        {
            StartSignUp();
        }
    }

    public void CloseCheckPanel()
    {
        VoiceCheckPanel.SetActive(false);
    }
    #endregion
    #endregion
}
