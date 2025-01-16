namespace Supernova.Models;

public enum HookStatus
{

    [System.Runtime.Serialization.EnumMember(Value = @"Created")]
    Created = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"Execution")]
    Execution = 1,

    [System.Runtime.Serialization.EnumMember(Value = @"End")]
    End = 2,

}