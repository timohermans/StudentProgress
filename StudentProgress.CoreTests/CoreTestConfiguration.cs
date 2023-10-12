using StudentProgress.Core;
using StudentProgress.Web.Lib.Infrastructure;

namespace StudentProgress.CoreTests;

public class CoreTestConfiguration : ICoreConfiguration
{
    public string MediaLocation => "Media";
}