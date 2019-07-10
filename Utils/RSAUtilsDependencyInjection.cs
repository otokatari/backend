using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XC.RSAUtil;

namespace OtokatariBackend.Utils
{

    public class RSAKeyFiles
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

    }

    public static class RSAUtilsDependencyInjection
    {
        public static IServiceCollection AddRsaUtils(this IServiceCollection services)
        {
            var keyFiles = services.BuildServiceProvider().GetRequiredService<IOptions<RSAKeyFiles>>().Value;
            var logger = services.BuildServiceProvider().GetService<ILogger<IServiceCollection>>();
            StreamReader pubKey = new StreamReader(keyFiles.PublicKey, Encoding.UTF8);
            StreamReader privKey = new StreamReader(keyFiles.PrivateKey, Encoding.UTF8);
            var pubKeyStr = pubKey.ReadToEnd();
            logger.LogInformation($"Public key loaded. {keyFiles.PublicKey}");
            var privKeyStr = privKey.ReadToEnd();
            logger.LogInformation($"Private key loaded. {keyFiles.PrivateKey}");
            pubKey.Close();
            privKey.Close();
            return services.AddSingleton(_ => new RsaPkcs8Util(Encoding.UTF8, pubKeyStr, privKeyStr));
        }
    }
}
