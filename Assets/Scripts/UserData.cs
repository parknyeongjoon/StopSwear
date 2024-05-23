public enum UserRole
{
    student,
    teacher,
    admin
}

public class UserInfo
{
    public string email;
    public string password;
    public string name;
    public string role;
    public string schoolName = "tempSchoolName";
    public string className = "tempClassName";
    public string schoolId;
    public string classId;

    public UserInfo()
    {

    }
}