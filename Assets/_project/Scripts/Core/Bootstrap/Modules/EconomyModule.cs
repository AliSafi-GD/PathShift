using _project.Scripts.Core.Economy;
using VContainer;

namespace _project.Scripts.Core.Bootstrap.Modules
{
    public static class EconomyModule
    {
        public static void Install(IContainerBuilder builder, WalletConfig walletConfig)
        {
            builder.Register<IWallet>(_ => new Wallet(walletConfig), Lifetime.Singleton);
        }
    }
}
