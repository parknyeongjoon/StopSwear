using Newtonsoft.Json.Linq;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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

                //uiManager.SetInfoPanel(responseToken["role"].ToString());
                uiManager.role = responseToken["role"].ToString();
                //uiManager.CloseLoginTab();
                if (responseToken["role"].ToString() == "STUDENT")
                {
                    recorder.Set_JWT_TOKEN(responseToken["token"].ToString());
                    recorder.StartRecording();
                    SceneManager.LoadSceneAsync("StudentStatScene");
                    return;
                }
                else
                {
                    SceneManager.LoadSceneAsync("TeacherStatScene");
                }
            }
            else
            {
                toast.showToast("로그인 실패");
            }
        });

        EmailIF.text = "";
        PWDIF.text = "";
    }

    public void StartSignIn()
    {
        StartCoroutine(SignIn());
    }

    public IEnumerator SignUp()
    {
        bool emailExist = false;
        string pattern = @"^[a-zA-Z0-9]+(?:\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(?:\.[a-zA-Z0-9]+)*$";

        if(!Regex.IsMatch(SUEmailIF.text, pattern)){
            toast.showToast("이메일 형식을 확인해주세요.");
            Debug.Log("이메일 형식");
            SUEmailIF.text = "";
            yield break;
        }

        yield return httpController.GetMethod("auth/exists/email?email=" + SUEmailIF.text, (response) =>
        {
            if (response == "true")
            {
                toast.showToast("이메일이 이미 존재합니다.");
                SUEmailIF.text = "";
                emailExist = true;
            }
        });

        if (emailExist)
        {
            yield break;
        }

        JObject userDataJson = new JObject();
        userDataJson["email"] = SUEmailIF.text;
        userDataJson["password"] = GetSha256Hash(SUPWDIF.text);
        userDataJson["name"] = SUuserNameIF.text;

        if (isTeacherToggle.isOn) // teacher
        {
            bool schoolIdExist = true;
            bool classIdExist = false;
            yield return httpController.GetMethod("auth/exists/school?schoolId=" + SUCodeIF.text , (response) =>
            {
                if (response == "false")
                {
                    toast.showToast("학교 코드가 올바르지 않습니다.");
                    SUCodeIF.text = "";
                    schoolIdExist = false;
                }
            });
            yield return httpController.GetMethod("auth/exists/class?classId=" + SUclassNameIF.text, (response) =>
            {
                if (response == "true")
                {
                    toast.showToast("학급이름이 이미 존재합니다.");
                    SUclassNameIF.text = "";
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
                    SUCodeIF.text = "";
                    toast.showToast("학급 코드 인식 실패");
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
                OpenCheckPanel();
            });
        }
    }

    Coroutine voiceCheckCoroutine = null;
    public void RecordVoiceCheckBtn()
    {
        if(voiceCheckCoroutine == null)
        {
            StartCoroutine(RecordVoiceCheck());
        }
        else
        {
            Debug.Log("이미 녹음 중");
        }
    }

    IEnumerator RecordVoiceCheck()
    {
        recorder.RecordVoiceCheck();
        yield return new WaitForSeconds(10);
        recorder.SendVoiceCheck();
        OpenSignInPanel();
        voiceCheckCoroutine = null;
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
        EmailIF.text = "";
        PWDIF.text = "";
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
        SUclassNameIF.text = "";
        SUCodeIF.text = "";
        SUEmailIF.text = "";
        SUPWDIF.text = "";
        SUuserNameIF.text = "";
        isTeacherToggle.isOn = false;
        SUclassNameIF.gameObject.SetActive(false);
        SignUpPanel.SetActive(true);
    }

    public void CloseAllPanel()
    {
        SignInPanel.SetActive(false);
        FindPWDPanel.SetActive(false);
        SignUpPanel.SetActive(false);
        VoiceCheckPanel.SetActive(false);
    }

    #region signup
    public void ToggleClassNameIF()
    {
        SUclassNameIF.gameObject.SetActive(isTeacherToggle.isOn);
    }

    public void OpenCheckPanel()
    {
        VoiceCheckPanel.SetActive(true);
    }

    public void CloseCheckPanel()
    {
        VoiceCheckPanel.SetActive(false);
    }
    #endregion
    #endregion
}
