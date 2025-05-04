namespace OrionClientLib.Pools
{
    public class Ec1ipseOrePool : OreHQPool
    {
        public override string Name { get; } = "Ec1ipse Pool [[Unofficial]]";
        public override string DisplayName => Name;
        public override bool DisplaySetting => true;
        public override string ArgName => "ec1ipse";

        public override string Description => $"[green]{Coins}[/] pool using Ore-HQ pool implementation. 5% commission. Operators (discord): Ec1ipse | Kriptikz";
        public override Coin Coins { get; } = Coin.Ore;

        public override Dictionary<string, string> Features { get; } = new Dictionary<string, string>();

        public override bool HideOnPoolList { get; } = false;
        public override string HostName { get; protected set; } = "ec1ipse.me";

        public override Dictionary<Coin, double> MiniumumRewardPayout => new Dictionary<Coin, double> { { Coin.Ore, 0.05 } };

        public override string Website => "https://stats.ec1ipse.me/miner-info/miner?pubkey={0}";
        public override bool StakingEnabled => true;

        private bool _ignoreCertError;
        public override bool IgnoreCertError
        {
            get => _ignoreCertError;
            set => _ignoreCertError = value;
        }
    }
}
