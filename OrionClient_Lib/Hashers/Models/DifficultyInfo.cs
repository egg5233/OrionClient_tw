namespace OrionClientLib.Hashers.Models
{
    public class DifficultyInfo
    {
        public bool IsCPU { get; private set; }

        public ulong BestNonce { get; private set; }
        public int BestDifficulty { get; private set; }
        public byte[] BestSolution { get; private set; }
        public int ChallengeId { get; private set; }

        private object _locker = new object();

        public DifficultyInfo()
        {

        }

        public DifficultyInfo(int challengeId)
        {
            ChallengeId = challengeId;
        }

        public void UpdateDifficulty(int difficulty, byte[] solution, ulong nonce, bool isCPU)
        {
            lock (_locker)
            {
                if (difficulty <= BestDifficulty)
                {
                    return;
                }

                BestDifficulty = difficulty;
                BestNonce = nonce;
                BestSolution = solution;
                IsCPU = isCPU;
            }
        }

        public DifficultyInfo GetUpdateCopy()
        {
            lock (_locker)
            {
                return new DifficultyInfo
                {
                    BestDifficulty = BestDifficulty,
                    BestNonce = BestNonce,
                    BestSolution = BestSolution,
                    ChallengeId = ChallengeId,
                    IsCPU = IsCPU,
                };
            }
        }
    }
}
