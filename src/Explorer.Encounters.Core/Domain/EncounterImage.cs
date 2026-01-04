using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class EncounterImage : ValueObject
    {
        public string Url { get; private set; }
        public EncounterImage() { }

        [JsonConstructor]
        public EncounterImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL cannot be empty.", nameof(url));
            Url = url;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
        }
    }
}
