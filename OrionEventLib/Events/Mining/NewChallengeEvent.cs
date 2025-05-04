using System.Text;

namespace OrionEventLib.Events.Mining
{
    public class NewChallengeEvent : MiningEvent
    {
        public override SubEventTypes SubEventType => SubEventTypes.NewChallenge;
        public required byte[] Challenge { get; set; }
        public required ulong StartNonce { get; set; }
        public required ulong EndNonce { get; set; }
        public required int ChallengeId { get; set; }

        public required ulong Cutoff { get; set; }

        public override ArraySegment<byte> Serialize(EventSerializer eventSerializer)
        {
            base.Serialize(eventSerializer);

            eventSerializer.WriteBytes(Challenge);
            eventSerializer.WriteU64(StartNonce);
            eventSerializer.WriteU64(EndNonce);
            eventSerializer.WriteS32(ChallengeId);
            eventSerializer.WriteU64(Cutoff);

            return eventSerializer.GetData();
        }

        public override void Deserialize(EventDeserializer eventDeserializer)
        {
            base.Deserialize(eventDeserializer);

            Challenge = eventDeserializer.ReadBytes();
            StartNonce = eventDeserializer.ReadU64();
            EndNonce = eventDeserializer.ReadU64();
            ChallengeId = eventDeserializer.ReadS32();
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Event: {EventType}. Subtype: {SubEventType}. Time: {DateTimeOffset.FromUnixTimeSeconds(Timestamp).ToLocalTime().DateTime.ToLongTimeString()}");
            builder.AppendLine($"Start nonce: {StartNonce}. End nonce: {EndNonce}. Challenge Id: {ChallengeId}");

            return builder.ToString();
        }
    }
}
