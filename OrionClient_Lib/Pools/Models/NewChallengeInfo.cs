namespace OrionClientLib.Pools.Models
{
    public class NewChallengeInfo
    {
        public ulong StartNonce { get; set; }
        public ulong EndNonce { get; set; }
        public byte[] Challenge { get; set; }
        public int ChallengeId { get; set; }

        public ulong Cutoff { get; set; }

        public ulong TotalCPUNonces { get; set; }

        public ulong CPUStartNonce => StartNonce;
        public ulong CPUEndNonce => StartNonce + TotalCPUNonces;

        public ulong GPUStartNonce => CPUEndNonce + 1;
        public ulong GPUEndNonce => EndNonce;
    }
}
