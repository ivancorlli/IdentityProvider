
using System.Text.RegularExpressions;

namespace IdentityProvider.ValueObject;

public class MedicalInfo
{

    public static int MaxLength = 300;
    public static Regex Reg = new Regex("^[a-zA-Z0-9., ]+$");

    public Uri Aptitude { get; private set; } = default!;
    public string Disabilities { get; private set; } = default!;

    private MedicalInfo() { }
    public MedicalInfo(Uri aptitude)
    {
        Aptitude = aptitude;
    }

    public MedicalInfo(string disabilities)
    {
        Disabilities = disabilities;
    }

    public MedicalInfo(Uri aptitude, string disabilities)
    {
        Aptitude = aptitude;
        Disabilities = disabilities;
    }
}