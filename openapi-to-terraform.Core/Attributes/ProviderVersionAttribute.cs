using System;

namespace openapi_to_terraform.Core.Extensions.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ProviderVersionAttribute : Attribute
    {
        public string ProviderVersion
        {
            get
            {
                return _providerVersion;
            }
        }

        private string _providerVersion { get; set; }
        
        public ProviderVersionAttribute(string providerVersion)
        {
            _providerVersion = providerVersion;
        }
    }
}