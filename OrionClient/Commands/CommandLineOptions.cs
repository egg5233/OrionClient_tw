using CommandLine;
using OrionEventLib;

namespace OrionClient.Commands
{
    internal class CommandLineOptions
    {
        [Option("gpu", HelpText = "Enables the gpu using selected GPUs or all supported if none are selected")]
        public bool EnableGPU { get; set; }

        [Option("opencl", HelpText = "Uses OpenCL devices rather than Cuda", Hidden = true)]
        public bool OpenCL { get; set; }

        [Option("disable-cpu", HelpText = "Disables the cpu")]
        public bool DisableCPU { get; set; }

        [Option('k', "keypair", HelpText = "Keypair file path")]
        public string? KeyFile { get; set; }

        [Option("ice", HelpText = "Ignore SSL error")]
        public bool ignoreCertError { get; set; }

        [Option("key", HelpText = "Public key for pools that don't require a keypair")]
        public string? PublicKey { get; set; }

        [Option("worker", HelpText = "worker")]
        public string? WorkerName { get; set; }

        #region Pool Settings

        [Option("pool", HelpText = "Selects pool (Values: ec1ipse, excalivator , twpore , twbitz , twbitz2 , twbitz3)")]
        public string? Pool { get; set; }

        #endregion

        #region CPU Settings

        [Option('t', "cpu-threads", HelpText = "Total threads to use for mining (0 = all threads)")]
        public int? CPUThreads { get; set; }

        [Option('a', "cpu-auto", HelpText = "Auto selects best CPU hasher for device")]
        public bool AutoSelectCPU { get; set; }

        [Option("cpu-hasher", HelpText = "Stock/Hybrid/Full AVX512/Native AVX2/Hybrid AVX2/Native Stock")]
        public string? hasher { get; set; }

        [Option('r', "ratio", HelpText = "Nonce ratio , default=0.33")]
        public double? ratio { get; set; }

        [Option("timeout", HelpText = "Timeout , default = 180 second")]
        public int? timeout { get; set; }

        #endregion

        #region GPU Settings

        [Option("gpu-batch-size", HelpText = "Higher values use more ram and take longer to run. Lower values can cause lower hashrates")]
        public int? BatchSize { get; set; }
        //[Option("gpu-block-size", HelpText = "GPU block size")]
        //public int? BlockSize { get; set; }
        [Option("gpu-gen-threads", HelpText = "CPU threads to use to generation program for GPU (0 = all threads)")]
        public int? ProgramGenerationThreads { get; set; }

        #endregion

        #region Event Server Settings

        [Option("event-server", HelpText = "Event server url/IP to send event updates")]
        public string? WebsocketUrl { get; set; }

        [Option("event-port", HelpText = "Event server Port to send event updates")]
        public int? Port { get; set; }

        [Option("event-id", HelpText = "Arbitrary id for that's sent in all event messages")]
        public string? Id { get; set; }

        [Option("event-reconnect", HelpText = "How often, in milliseconds, to try connecting to server")]
        public int? ReconnectTimeMs { get; set; }

        [Option("event-serialization", HelpText = "Type of serialization to use for event messages. Allow values: Binary, Json")]
        public SerializationType? Serialization { get; set; }

        #endregion
    }
}
