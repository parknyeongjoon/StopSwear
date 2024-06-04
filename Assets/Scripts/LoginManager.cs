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

    void AutoSignIn() // ���߿� �ϱ�
    {
        EmailIF.text = PlayerPrefs.GetString("");
    }

    public void StartSignIn()
    {
        StartCoroutine(SignIn());
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
                    //recorder.StartRecording();
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
                toast.showToast("�α��� ����");
            }
        });

        EmailIF.text = "";
        PWDIF.text = "";
    }

    public void StartSignUp()
    {
        StartCoroutine(SignUp());
    }

    public IEnumerator SignUp()
    {
        // email validation
        if (!isValidEmail(SUEmailIF.text))
        {
            yield break;
        }
        bool emailExist = false;
        yield return httpController.GetMethod("auth/exists/email?email=" + SUEmailIF.text, (response) =>
        {
            if (response == "true")
            {
                toast.showToast("�̸����� �̹� �����մϴ�.");
                SUEmailIF.text = "";
                emailExist = true;
            }
        });
        if (emailExist) { yield break; }

        // password validation
        if (!IsValidPassword(SUPWDIF.text)) { yield break; }

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
                    toast.showToast("�б� �ڵ尡 �ùٸ��� �ʽ��ϴ�.");
                    SUCodeIF.text = "";
                    schoolIdExist = false;
                }
            });
            yield return httpController.GetMethod("auth/exists/class?classId=" + SUclassNameIF.text, (response) =>
            {
                if (response == "true")
                {
                    toast.showToast("�б��̸��� �̹� �����մϴ�.");
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
                    toast.showToast("�б� �ڵ� �ν� ����");
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

    bool isValidEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9]+(?:\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(?:\.[a-zA-Z0-9]+)*$";

        if (!Regex.IsMatch(SUEmailIF.text, pattern))
        {
            toast.showToast("�̸��� ������ Ȯ�����ּ���.");
            Debug.Log("�̸��� ����");
            SUEmailIF.text = "";
            return false;
        }

        return true;
    }

    bool IsValidPassword(string password)
    {
        if (password.Length < 8)
        {
            toast.showToast("Password must be at least 8 characters long.");
            Debug.Log("���� ����");
            return false;
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            toast.showToast("Password must contain at least one uppercase letter.");
            Debug.Log("�빮�� X");
            return false;
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            toast.showToast("Password must contain at least one lowercase letter.");
            Debug.Log("�ҹ��� x");
            return false;
        }

        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            toast.showToast("Password must contain at least one number.");
            Debug.Log("���� x");
            return false;
        }

        if (!Regex.IsMatch(password, @"[\W_]"))
        {
            toast.showToast("Password must contain at least one special character.");
            Debug.Log("Ư�� ���� x");
            return false;
        }

        if(SUPWDIF.text != SUPWDCheckIF.text)
        {
            toast.showToast("Password check is not correct");
            Debug.Log("��� ��ȣ ��ġ x");
            return false;
        }
        return true;
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
            Debug.Log("�̹� ���� ��");
        }
    }

    IEnumerator RecordVoiceCheck()
    {
        recorder.RecordVoiceCheck();

        float time = 0.0f;
        RecordingProgressImg.gameObject.SetActive(true);

        while (true)
        {

            time += Time.deltaTime;
            RecordingProgressImg.fillAmount = time / 10;
            if(time > 10.0f) { break; }
            yield return null;
        }
        RecordingProgressImg.fillAmount = 0;

        recorder.SendVoiceCheck();
        OpenSignInPanel();
        voiceCheckCoroutine = null;
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
    [SerializeField] TMP_InputField FEmailIF, FuserNameIF;
    [SerializeField] TMP_Text FResultTxt;
    [SerializeField] GameObject SignUpPanel;
    [SerializeField] TMP_InputField SUEmailIF, SUPWDIF, SUPWDCheckIF, SUuserNameIF, SUCodeIF, SUclassNameIF;
    [SerializeField] Toggle isTeacherToggle;
    [SerializeField] GameObject VoiceCheckPanel;
    [SerializeField] Image RecordingProgressImg;

    public void OpenSignInPanel()
    {
        CloseAllPanel();
        EmailIF.text = "";
        PWDIF.text = "";
        SignInPanel.SetActive(true);
    }

    public void OpenFindPWDPanel()
    {
        FEmailIF.text = "";
        FuserNameIF.text = "";
        FResultTxt.text = "";
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
