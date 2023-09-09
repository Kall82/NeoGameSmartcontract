using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;
using System.Threading;

namespace Neo.SmartContract.Examples
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
        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());
        public override byte Decimals() => Factor();

        public static string Name() => "Cryptomon Coin";
        public override string Symbol() => "CMONCOIN";

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

        public static UInt160 GetMinter(UInt160 account)
        {
            var value = MinterMap.Get(account);
            return value != null ? account : null;
        }

        public static void SetMinter(UInt160 account, bool canMint)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            if (canMint)
            {
                MinterMap.Put(account, "");
            }
            else
            {
                MinterMap.Delete(account);
            }
        }

        public static void Mint(UInt160 caller, UInt160 account, BigInteger amount)
        {
            UInt160 minter = GetMinter(caller);
            ExecutionEngine.Assert(Runtime.CheckWitness(minter), "Not a minter!");
            Nep17Token.Mint(account, amount);
        }

        public static void Burn(UInt160 caller, UInt160 account, BigInteger amount)
        {
            UInt160 minter = GetMinter(caller);
            ExecutionEngine.Assert(Runtime.CheckWitness(minter), "Not a minter!");
            Nep17Token.Burn(account, amount);
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
