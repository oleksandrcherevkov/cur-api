using CleanTemplate.Auth.Helpers;

namespace CleanTemplate.Auth.Models;

public abstract class UserModel<TRole>
    where TRole : Enum
{
    public UserModel()
    {
        _id = Guid.NewGuid().ToString();
    }

    public UserModel(string email, string password, TRole role)
        : this()
    {
        this.Email = email;
        this.Password = password;
        this.Role = role;
    }

    private string _id;
    public string Id
    {
        get { return _id; }
    }
    private string _email;
    public string Email
    {
        get { return _email; }
        set
        {
            _email = value;
            EmailNormalized = value.Normalize();
        }
    }
    public string EmailNormalized { get; private set; }

    private string _password;
    public string Password
    {
        get { return _password; }
        set
        {
            _password = HeasherHelper.HashPassword(value);
        }
    }
    public bool VerifyPassword(string password)
    {
        return HeasherHelper.VerifyHashedPassword(Password, password);
    }
    public TRole Role { get; set; }
}