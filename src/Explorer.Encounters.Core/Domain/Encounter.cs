using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class Encounter : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public GeoLocation Location { get; private set; }
        public int Xp { get; private set; }
        public EncounterStatus Status { get; private set; }
        public EncounterType Type { get; private set; }
        public int? RequiredParticipants { get; private set; }

        private Encounter() { } // EF

        public Encounter(string name, string description, GeoLocation location, int xp, EncounterType type, int? requiredParticipants)
        {
            SetBasics(name, description, location, xp, type, requiredParticipants);
            Status = EncounterStatus.Draft; //default
        }

        public Encounter(string name, string description, GeoLocation location, int xp, EncounterType type, int? requiredParticipants, EncounterStatus status)
        {
            SetBasics(name, description, location, xp, type, requiredParticipants);
            Status = status; //default
        }

        public void Update(string name, string description, GeoLocation location, int xp, EncounterType type, int? requiredParticipants)
        {
            SetBasics(name, description, location, xp, type, requiredParticipants);
        }

        public void ChangeStatus(EncounterStatus newStatus)
        {
            if (newStatus == Status) return;

            
            // iz draft predje u active
            //iy active u archived
            var allowed =
                (Status == EncounterStatus.Draft && newStatus == EncounterStatus.Active) ||
                (Status == EncounterStatus.Draft && newStatus == EncounterStatus.Pending) ||
                (Status == EncounterStatus.Active && newStatus == EncounterStatus.Draft) ||
                (Status == EncounterStatus.Pending && newStatus == EncounterStatus.Active) ||
                (Status == EncounterStatus.Pending && newStatus == EncounterStatus.Declined) ||
                (Status == EncounterStatus.Active && newStatus == EncounterStatus.Archived);

            if (!allowed)
                throw new InvalidOperationException($"Status change {Status} -> {newStatus} is not allowed.");

            Status = newStatus;
        }

        public void SetPending(Encounter encounter)
        {
            encounter.Status = EncounterStatus.Pending;
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
    }

}
