using Neo.SmartContract;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using System;
using System.ComponentModel;
using Neo.SmartContract.Framework.Services;
using System.Numerics;
using System.Security.Principal;
using Neo.SmartContract.Framework.Native;

namespace NeoNft
{
    public class MyTokenState : Nep11TokenState
    {
        private string image;

        public string Image
        {
            get { return Storage.Get(Storage.CurrentContext, "Link") + image + ".jpg"; }
            set { image = value; }
        }


        public static void SetLink(string link)
        {
            Storage.Put(Storage.CurrentContext, "Link", link);
        }


        public MyTokenState(ByteString tokenId)
        {
            //TODO: Replace it with your own URL.
            Image = tokenId;
        }
    }

    [ManifestExtra("Author", "Youtpout")]
    [ManifestExtra("Description", "Cryptomonster for Cryptomon game")]
    [SupportedStandards("NEP-11")]
    public class NeoNft : Nep11Token<MyTokenState>
    {
        [InitialValue("NYsxPYkvyFEAs9rakjWKUuhK7AdUiPC2Fo", ContractParameterType.Hash160)]
        private static readonly UInt160 Owner = default;

        private const byte Prefix_Minter = 0x99;
        public static readonly StorageMap MinterMap = new StorageMap(Storage.CurrentContext, Prefix_Minter);

        private static bool IsOwner() => Runtime.CheckWitness(Owner);

        public static string Name() => "Cryptomonster NFT";
        public override string Symbol() => "CMONSTER";

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

        public static void Mint(UInt160 caller, UInt160 account, ByteString tokenId)
        {
            UInt160 minter = GetMinter(caller);
            ExecutionEngine.Assert(Runtime.CheckWitness(minter), "Not a minter!");
            Nep11Token<MyTokenState>.Mint(tokenId, new MyTokenState(tokenId));
        }

        public static void Burn(UInt160 caller, ByteString tokenId)
        {
            UInt160 minter = GetMinter(caller);
            ExecutionEngine.Assert(Runtime.CheckWitness(minter), "Not a minter!");
            Nep11Token<MyTokenState>.Burn(tokenId);
        }

        public static bool Main()
        {
            return true;
        }
    }
}
