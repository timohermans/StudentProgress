namespace StudentProgress.Core.Entities;

public class Setting : AuditableEntity<int>
{
    public string Key { get; private set; }
    public string Value { get; private set; }

    #nullable disable
    private Setting()
    {
    } 
    #nullable enable

    public Setting(string key, string value)
    {
        Key = key;
        Value = value;
    }
}