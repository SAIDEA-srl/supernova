using OrangeButton.Models;

namespace UniversalMapper.Models;

public class Collection 
{
    public Guid CollectionId { get; internal set; }

    public List<AlternativeIdentifier> Identifiers { get; internal set; }
}
