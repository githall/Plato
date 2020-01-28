

namespace PlatoCore.Net.Abstractions
{

    public interface IClientIpAddress
    {
        
        string GetIpV4Address(bool tryUseXForwardHeader = true);

        string GetIpV6Address();
        
    }
}
