using OrangeButton.Models;

namespace UniversalMapper.Models;

public class Collection
{
    public Guid CollectionId { get; set; }

    public List<AlternativeIdentifier> Identifiers { get; set; }
}
