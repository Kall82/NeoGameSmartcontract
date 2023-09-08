using System;
using System.Numerics;
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace ContractTemplate
{
    [ManifestExtra("Author", "Youtpout")]
    [ManifestExtra("Description", "Token for Cryptomon game")]
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public partial class NEP17Demo : Nep17Token
    {
        [InitialValue("NYsxPYkvyFEAs9rakjWKUuhK7AdUiPC2Fo", ContractParameterType.Hash160)]
        private static readonly UInt160 owner = default;
        // Prefix_TotalSupply = 0x00; Prefix_Balance = 0x01;
        private const byte Prefix_Contract = 0x02;
        private const byte Prefix_Minter = 0x03;
        public static readonly StorageMap ContractMap = new StorageMap(Storage.CurrentContext, Prefix_Contract);
        public static readonly StorageMap MinterMap = new StorageMap(Storage.CurrentContext, Prefix_Minter);

        private static readonly byte[] ownerKey = "owner".ToByteArray();
        private static readonly byte[] minterKey = "minter".ToByteArray();

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());
        public override byte Decimals() => Factor();

        public static string Name() => "Cryptomon Coin";
        public override string Symbol() => "COIN";

        public static byte Factor() => 8;

        public static void _deploy(object data, bool update)
        {
            if (update) return;
            ContractMap.Put(ownerKey, owner);
            Nep17Token.Mint(owner, 100000000 * BigInteger.Pow(10, Factor()));
        }

        public static UInt160 GetOwner()
        {
            return (UInt160)ContractMap.Get(ownerKey);
        }

        public static UInt160 GetMinter() =>
             (UInt160)ContractMap.Get(minterKey);

        public static bool IsMinter() =>
            Runtime.CheckWitness(GetMinter());


        public static void SetMinter(UInt160 account, bool canMint)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            MinterMap.Put(account, "1"); // keep small value (less gas).
        }

        public static new void Mint(UInt160 account, BigInteger amount)
        {
            if (!IsMinter()) throw new InvalidOperationException("No Authorization!");
            Nep17Token.Mint(account, amount);
        }

        public static new void Burn(UInt160 account, BigInteger amount)
        {
            if (IsMinter()) throw new InvalidOperationException("No Authorization!");
            Nep17Token.Mint(account, amount);
        }

        public static bool Update(ByteString nefFile, string manifest)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            ContractManagement.Update(nefFile, manifest, null);
            return true;
        }

        public static bool Destroy()
        {
            throw new InvalidOperationException("Unauthorize!");
        }
    }
}