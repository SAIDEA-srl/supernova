using OrangeButton.Models;

namespace UniversalMapper.Models;

public class UUIDMap
{

    public Guid UUID { get; set; }

    public AlternativeIdentifier AlternativeIdentifier { get; set; } = new AlternativeIdentifier();
}