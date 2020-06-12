using System.Collections.Generic;

namespace DataStat.WebCore.CommonSuport.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
