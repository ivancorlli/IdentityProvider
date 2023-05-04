namespace IdentityProvider.Helper;

public class HideString
{
    public static string HideEmail(string Email)
    {
        string[] local = Email.Split("@");
        string domain = local[1];
        string email = local[0].Substring(0, 3);
        local = local[0].Split(email);
        string text = string.Empty;
        foreach (char st in local[1]) text += "*";
        return $"{email}{text}{domain}";
    }

    public static string HidePhone(string Phone)
    {
        string number = Phone.Substring(Phone.Length - 3);
        string[] phone = Phone.Split(number);
        string hidden = string.Empty;
        foreach (char st in phone[0]) hidden += "*";
        return $"{hidden}{number}";
    }
}