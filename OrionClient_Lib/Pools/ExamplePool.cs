using NLog;
using OrionClientLib.Hashers.Models;
using OrionClientLib.Pools.Models;
using Solnet.Rpc;
using Solnet.Wallet;
using Spectre.Console;
using System.Security.Cryptography;

namespace OrionClientLib.Pools
{
    //UI lib: https://spectreconsole.net/
    public class ExamplePool : BasePool
    {
        private static readonly Logger _logger = LogManager.GetLogger("Main");

        public override string Name => "Example Pool";

        //Name displayed
        public override string DisplayName => $"[b]{Name}[/]";
        public override bool DisplaySetting => false;

        public override string Description => "Example pool sample";
        public override string ArgName => "example";

        //Features displayed on pool list
        //Currently not displayed anywhere
        public override Dictionary<string, string> Features => new Dictionary<string, string>
        {
            { "Feature 1 example", "Example feature description"},
            { "Feature 2 example", "[red]Another description[/]"}
        };

        public override bool HideOnPoolList => false;

        public override Coin Coins => Coin.Ore;

        //Whether or not a full keypair (public:private) key is required to run the pool
        public override bool RequiresKeypair => false;

        public override event EventHandler<NewChallengeInfo> OnChallengeUpdate;
        public override event EventHandler<(string[] columns, byte[] data)> OnMinerUpdate;
        public override event EventHandler PauseMining;
        public override event EventHandler ResumeMining;

        private bool _ignoreCertError;
        public override bool IgnoreCertError
        {
            get => _ignoreCertError;
            set => _ignoreCertError = value;
        }
        
        private IRpcClient _rpcClient = ClientFactory.GetClient(Cluster.MainNet);
        //private IRpcClient _rpcClient = ClientFactory.GetClient(RPC_URL);

        //Streaming RPC client
        //private IStreamingRpcClient _rpcStreamingClient = ClientFactory.GetStreamingClient(Cluster.MainNet);

        private System.Timers.Timer _sw;
        private int _challengeId = 0;
        private DifficultyInfo _bestDifficulty;
        private ExamplePoolSettings _settings;

        public override string[] TableHeaders()
        {
            //Headers for a table of information the pool can provide
            //Return null to disable table
            return ["Time", "Difficulty", "Ore Reward"];
        }

        public override void SetWalletInfo(Wallet wallet, string publicKey , string workerName , bool ignoreCertError , double ratio)
        {
            //This can be called multiple times

            _settings ??= new ExamplePoolSettings(Name);

            //wallet is when a private key is needed. This can be null
            //publicKey is when only the public key is needed
        }

        public override async Task<bool> ConnectAsync(CancellationToken token)
        {
            //Handle any connection logic to the pool


            //Load current settings, if they exist
            await _settings.LoadAsync();

            //As an example, set a new random challenge every 30s
            _sw = new System.Timers.Timer(30000);
            _sw.Elapsed += (sender, data) =>
            {
                //Array item length must be the same as the table header length
                OnMinerUpdate?.Invoke(this,
                    ([
                        DateTime.Now.ToShortTimeString(),
                        _bestDifficulty?.BestDifficulty.ToString() ?? "0",
                        (RandomNumberGenerator.GetInt32(0, 1000000000) / 1000000000.0).ToString()
                    ], null));

                _bestDifficulty = null;
                SetRandomChallenge();
            };
            _sw.Start();

            SetRandomChallenge();

            return true;
        }

        public override void DifficultyFound(DifficultyInfo info)
        {
            //Gets called whenever a hasher finds a higher difficulty for current challenge

            //Wrong id, ignore solution
            if (info.ChallengeId != _challengeId)
            {
                return;
            }

            if (_bestDifficulty == null || _bestDifficulty.BestDifficulty < info.BestDifficulty)
            {
                _bestDifficulty = info;
            }

            return;
        }

        public override async Task<bool> DisconnectAsync()
        {
            //Handle any disconnection logic to the pool

            _sw?.Dispose();
            return true;
        }

        public override async Task<double> GetFeeAsync(CancellationToken token)
        {
            //Returns current pool fee
            return 0;
        }

        public override async Task<(bool, string)> SetupAsync(CancellationToken token, bool initialSetup = false)
        {
            //Called when someone selects the pool through the "Run Setup" menu or selects "Start Mining"

            //initialSetup is true when pool is selected through "Run Setup". Allowing user to change already set options is best

            //Handle any setup required
            //Can be things like verifying registered status, ata creationg, etc

            //Load current saved settings
            await _settings.LoadAsync();

            //An example of doing a setup for pool settings. Only allow changing when the value is empty or through initial setup
            if (String.IsNullOrEmpty(_settings.ExampleSetting) || initialSetup)
            {
                TextPrompt<string> example = new TextPrompt<string>("Just an example. Type anything: ");
                example.DefaultValue(_settings.ExampleSetting);
                _settings.ExampleSetting = await example.ShowAsync(AnsiConsole.Console, token);
                await _settings.SaveAsync();

                //Text prompt doesn't clear the console
                AnsiConsole.Clear();
            }

            return (true, String.Empty);
        }

        public override async Task<(bool, string)> OptionsAsync(CancellationToken token)
        {
            //This is called when the user clicks the pool in the initial menu
            //You can use this to setup custom calls (staking, balance, claim, etc)

            SelectionPrompt<string> selectionPrompt = new SelectionPrompt<string>();
            selectionPrompt.Title("Example title");

            for (int i = 0; i < 5; i++)
            {
                selectionPrompt.AddChoice($"Choice {i}");
            }

            //Use cancellation token given as it will allow the user to go back to main menu with Ctrl+C
            string choice = await selectionPrompt.ShowAsync(AnsiConsole.Console, token);

            return default;
        }

        private void SetRandomChallenge()
        {
            byte[] challenge = new byte[32];
            RandomNumberGenerator.Fill(challenge);

            OnChallengeUpdate?.Invoke(this, new NewChallengeInfo
            {
                Challenge = challenge,
                ChallengeId = Interlocked.Increment(ref _challengeId),
                StartNonce = 0,
                EndNonce = ulong.MaxValue
            });
        }

        private class ExamplePoolSettings : PoolSettings
        {
            public ExamplePoolSettings(string poolName) : base(poolName)
            {
            }

            public string ExampleSetting { get; set; }
        }
    }
}
