using StudentProgress.Web.Lib.Configuration;

namespace StudentProgress.CoreTests;

public class CoreTestConfiguration : ICoreConfiguration
{
    public string MediaLocation => "Media";
}