using Solnet.Wallet;
using Spectre.Console;

namespace OrionClientLib.Pools
{
    public class CustomOreHQPool : OreHQPool
    {
        public override string Name { get; } = "Custom Ore-HQ Pool";
        public override string DisplayName => $"{Name} ({WebsocketUrl?.Host ?? "Unknown"})";
        public override string Description => $"Custom pool using Ore-HQ pool implementation";
        public override bool DisplaySetting => true;
        public override Coin Coins { get; } = Coin.Ore;
        public override string ArgName => "custom";

        public override Dictionary<string, string> Features { get; } = new Dictionary<string, string>();

        public override bool HideOnPoolList { get; } = false;
        public override string HostName { get; protected set; }

        public override Uri WebsocketUrl => _poolSettings?.CustomDomain == null ? null : new Uri($"wss://{_poolSettings.CustomDomain}/v2/ws?timestamp={_timestamp}");

        public override string Website => String.Empty;
        public override bool StakingEnabled => false;

        private bool _ignoreCertError;
        public override bool IgnoreCertError
        {
            get => _ignoreCertError;
            set => _ignoreCertError = value;
        }

        public override void SetWalletInfo(Wallet wallet, string publicKey , string workerName , bool ignoreCertError , double ratio)
        {
            _poolSettings ??= new HQPoolSettings(Name);
            _poolSettings.CPUNonceRatio = ratio;
            _ignoreCertError = ignoreCertError;
            if (String.IsNullOrEmpty(HostName))
            {
                _poolSettings.LoadAsync().Wait();

                HostName = _poolSettings.CustomDomain;
            }

            base.SetWalletInfo(wallet, publicKey , workerName , ignoreCertError , ratio);
        }

        public override async Task<(bool, string)> SetupAsync(CancellationToken token, bool initialSetup = false)
        {
            try
            {
                if (initialSetup || !Uri.TryCreate(_poolSettings.CustomDomain, UriKind.RelativeOrAbsolute, out var _))
                {
                    TextPrompt<string> textPrompt = new TextPrompt<string>("Enter url for custom pool: ");
                    textPrompt.AllowEmpty();

                    if (!String.IsNullOrEmpty(_poolSettings.CustomDomain))
                    {
                        textPrompt.DefaultValue(_poolSettings.CustomDomain);
                    }

                    textPrompt.Validate((str) =>
                    {
                        if (String.IsNullOrEmpty(str))
                        {
                            return true;
                        }

                        if (Uri.TryCreate(str, UriKind.Absolute, out Uri _))
                        {
                            return true;
                        }

                        str = $"http://{str}";


                        if (Uri.TryCreate(str, UriKind.Absolute, out Uri _))
                        {
                            return true;
                        }

                        return false;
                    });

                    string response = await textPrompt.ShowAsync(AnsiConsole.Console, token);

                    if (String.IsNullOrEmpty(response))
                    {
                        return (false, String.Empty);
                    }

                    if (!Uri.TryCreate(response, UriKind.Absolute, out var customDomain))
                    {
                        response = $"http://{response}";

                        if (!Uri.TryCreate(response, UriKind.Absolute, out customDomain))
                        {
                            return (false, $"Invalid url");
                        }
                    }

                    _poolSettings.CustomDomain = customDomain.Host;
                    await _poolSettings.SaveAsync();

                    //Initialize client
                    if (_ignoreCertError) {
                        var handler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                            {

                                return true;
                            }
                        };
                        _client = new HttpClient(handler)
                        {
                            BaseAddress = new Uri($"https://{WebsocketUrl.Host}"),
                            Timeout = TimeSpan.FromSeconds(5)
                        };
                    } else {
                        _client = new HttpClient()
                        {
                            BaseAddress = new Uri($"https://{WebsocketUrl.Host}"),
                            Timeout = TimeSpan.FromSeconds(5)
                        };
                    }
                }
            }
            finally
            {
                AnsiConsole.Clear();
            }

            return await base.SetupAsync(token, initialSetup);
        }
    }
}
