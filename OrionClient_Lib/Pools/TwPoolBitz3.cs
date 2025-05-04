using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using Spectre.Console;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionClientLib.Pools
{
    public class TwPoolBitz3 : OreHQPool
    {
        public override string Name { get; } = "Twbitz";
        public override string DisplayName => Name;
        public override string Description => $"Ore/Coal/Bitz";
        public override bool DisplaySetting => true;
        public override Coin Coins { get; } = Coin.Ore;
        public override string ArgName => "twbitz3";

        public override Dictionary<string, string> Features { get; } = new Dictionary<string, string>();

        public override bool HideOnPoolList { get; } = false;
        public override string HostName { get; protected set; } = "93.179.125.204";

        string? WorkerName;

        public override Uri WebsocketUrl => new Uri($"wss://{HostName}/v2/ws?timestamp={_timestamp}");

        public override string Website => String.Empty;
        public override bool RequiresKeypair => false;

        public override bool StakingEnabled => false;

        private bool _ignoreCertError;
        public override bool IgnoreCertError
        {
            get => _ignoreCertError;
            set => _ignoreCertError = value;
        }
        
        public override void SetWalletInfo(Wallet wallet, string publicKey , string workerName  , bool ignoreCertError , double ratio)
        {
            WorkerName = workerName;
            _poolSettings ??= new HQPoolSettings(Name);

            if (String.IsNullOrEmpty(HostName))
            {
                _poolSettings.LoadAsync().Wait();

                HostName = _poolSettings.CustomDomain;
            }

            base.SetWalletInfo(wallet, publicKey , workerName  , ignoreCertError , ratio);
        }

        public override async Task<bool> ConnectAsync(CancellationToken token)
        {
            if(!await UpdateTimestampAsync(token))
            {
                return false;
            }

            byte[] tBytes = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(tBytes, _timestamp);

            Base58Encoder _encoder = new Base58Encoder();
            byte[] sigBytes = RequiresKeypair ? _wallet.Sign(tBytes) : new byte[64];
            string sig = _encoder.EncodeData(sigBytes);

            _authorization = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_publicKey}.{WorkerName}:{sig}"))}";
            bool result = await base.ConnectAsync(token);
            await RefreshStakeBalancesAsync(false, token);

            return result && await SendReadyUp(false);
        }
    }
}
