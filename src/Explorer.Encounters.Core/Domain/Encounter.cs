using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Explorer.Encounters.Core.Domain
{
    public class Encounter : AggregateRoot
    {
        public int CreatorId { get; init; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public GeoLocation Location { get; private set; }
        public int Xp { get; private set; }
        public EncounterStatus Status { get; private set; }
        public EncounterType Type { get; private set; }
        public int? RequiredParticipants { get; private set; }

        public HiddenLocationEncounter? HiddenLocationDetails { get; private set; }
        private Encounter() { } // EF

        public Encounter(int creatorId, string name, string description, GeoLocation location, int xp, EncounterType type)
        {
            CreatorId = creatorId;
            SetBasics(name, description, location, xp, type);
            Status = EncounterStatus.Draft; //default
        }

        public Encounter(string name, string description, GeoLocation location, int xp, EncounterType type, int? requiredParticipants)
        {
            SetBasics(name, description, location, xp, type, requiredParticipants);
            Status = EncounterStatus.Draft;
        }

        public void Update(string name, string description, GeoLocation location, int xp, EncounterType type, int? requiredParticipants)
        { SetBasics(name, description, location, xp, type, requiredParticipants); }
        public void Update(int creatorId,string name, string description, GeoLocation location, int xp, EncounterType type)
        {
            
            if (creatorId != CreatorId)
                throw new InvalidOperationException("Only the creator can update the encounter.");

            var previousType = Type;
            SetBasics(name, description, location, xp, type);
            // Ako više NIJE location challenge → brišemo hidden config
            
            if (previousType == EncounterType.Location && type != EncounterType.Location)
            {
                HiddenLocationDetails = null;
            }
        }

        public void ChangeStatus(EncounterStatus newStatus)
        {
            if (newStatus == Status) return;

            
            // iz draft predje u active
            //iy active u archived
            var allowed =
                (Status == EncounterStatus.Draft && newStatus == EncounterStatus.Active) ||
                (Status == EncounterStatus.Active && newStatus == EncounterStatus.Draft) ||
                (Status == EncounterStatus.Active && newStatus == EncounterStatus.Archived);

            if (!allowed)
                throw new InvalidOperationException($"Status change {Status} -> {newStatus} is not allowed.");

            Status = newStatus;
        }

        private void SetBasics(string name, string description, GeoLocation location, int xp, EncounterType type, int? requiredParticipants)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            if (xp < 0)
                throw new ArgumentOutOfRangeException(nameof(xp), "Xp must be >= 0.");

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Xp = xp;
            Type = type;
            if(Type == EncounterType.Social)
            {
                RequiredParticipants = requiredParticipants;
            } else
            {
                RequiredParticipants = null;
            }
        }
        public void SetHiddenLocationDetails(HiddenLocationEncounter details)
        {
            if (Type != EncounterType.Location)
                throw new InvalidOperationException("Hidden location is allowed only for Location encounters.");

            HiddenLocationDetails = details ?? throw new ArgumentNullException(nameof(details));
        }

    }

}
