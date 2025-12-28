using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Explorer.Encounters.Core.Domain
{
    public class HiddenLocationEncounter : ValueObject
    {
        public EncounterImage Image { get; private set; }
        public double ActivationRadiusMeters { get;  set; }
        public GeoLocation PhotoLocation { get; private set; }
        public int SecondsToViewPhoto { get; private set; } = 30;
        public int DistanceMeters { get; private set; } = 5;
        public HiddenLocationEncounter() { }
        [JsonConstructor]
        public HiddenLocationEncounter(EncounterImage image, double activationRadiusMeters, GeoLocation photoLocation)
        {
        
            if (activationRadiusMeters <= 0)
                throw new ArgumentOutOfRangeException(nameof(activationRadiusMeters), "ActivationRadiusMeters must be > 0.");
            Image = image ?? throw new ArgumentNullException(nameof(image));
            ActivationRadiusMeters = activationRadiusMeters;
            PhotoLocation = photoLocation ?? throw new ArgumentNullException(nameof(photoLocation));
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Image;
            yield return ActivationRadiusMeters;
            yield return PhotoLocation;
            yield return SecondsToViewPhoto;
            yield return DistanceMeters;
        }

    }
}
