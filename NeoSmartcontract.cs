using System;
using System.ComponentModel;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace NeoSmartcontract
{
    [DisplayName("NeoSmartcontract")]
    [ManifestExtra("Author", "NEO")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "This is a NeoSmartcontract")]
    public class NeoSmartcontract : SmartContract
    {
        public static bool Main()
        {
            return true;
        }
    }
}
