namespace Supernova.Models.GatewayHooks;

public enum HookStatus
{

    [System.Runtime.Serialization.EnumMember(Value = @"Created")]
    Created,

    [System.Runtime.Serialization.EnumMember(Value = @"Execution")]
    Execution,

    [System.Runtime.Serialization.EnumMember(Value = @"End")]
    End,

}