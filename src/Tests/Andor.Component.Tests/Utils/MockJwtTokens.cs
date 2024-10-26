using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Security.Cryptography;

namespace Andor.Component.Tests.Utils;

public static class MockJwtTokens
{
    public static string GenerateJwtToken(Dictionary<string, object> claims)
    {
        using (var rsa = RSA.Create(2048))
        {
            var privateKeyBytes = Convert.FromBase64String("MIIEowIBAAKCAQEAxsJMegxGhTjBohG6Wk1ZUZWbQu5R+LoBn/cuJuhU6hn4lpPI1T3yuKVOw8yiflGF1AxstwdTufN7QkZnl18eVuYdR9heh1ruwstOO4ZjXsm+Z6WispRHbN3DO6jQqI9b8rMHpLcAM3EQ7xYKg4AmKwU8rYg59hLBIdTRnK1BqWj0nfabQjIm42uXyXs3nBiGGCPLi9T9/0UonJURS8hgGUZIEL8a46AGuG+Y9YiELiQd9IVWvYX/zaKQf2rjWPlWEW0pPZuV02vNJrlA1e3NeWTCFC2v0yJUfkCzensGUaA6KGdhZewXM+w7qAFp92pEAiKTjxN8dejrKtegyHuZAQIDAQABAoIBAA5BngQsELvaBhOGo3/6J9OxAC21E+acCuVLMMGELSEMg1bjm0ZmVtxVKRjkuuwGE2wSBxM6wI/oLbljfOcv4Kh5gXSniaI6TMFmnUIqQcLpBBIBoLGf7p6ZqnkDQxIwFDeMjVoMcUR/UehgQ5lKjhJMYh2NVaD3l+hCEFXlXH4W44K90j3E6TZVq+x0LV2vFY0V3VV4ZWYsIL/1Ph2N9oEYqBR7HwLg9pMrTLjglwKqtoLdZOroROlKaebc/xl979Wjf9wwDFEn6nznU9WVFaV0oWxiBO4ChuqtqPEgFVQ5r2wtF77uqLs9G+OPmhrmRE1j74E7RKq3Oj6fMYQP01ECgYEA6MZib31uhsbmhBOVgkjr4i7Jar75EQqyt38cWrzvMrbhHJfLTKNfhq3Th72ingdKgBrr/wNEjD5TyEKeOlkR0caMdEzFchTPPp7fzxsHGsjFhwY66rphUHHSA8tx3MStg5GYSgdUdLJo4/S+Lzfg5uovca1dvVRSgnDkRQkGoMkCgYEA2pcRMnLV19ZT98CezGHlDeFaIzA8JhtVAJVusEdrq3tbJz6Pmc581/x0/6HG/9sxnQBrimK3M9bLlDow4Y5sMHY1kXbB03XXUwC0cJPmJ5I7cMAu9S7I/4RqdYhlPfS/Nr1WkDMuA/dwoPrPI+cDU/2CpvUtjJgAap0JOt4jynkCgYA2EbbVvOYKinMLJN1qqPOZ01JX6EpEah6/PthgP1i0iARcNuozQBO1XfUvp535ZSop/Zbp4mFuzh95+fd1CDF6b8s6TIbki+7j1dWY7udEknRHe+v9kBOAsx+cYApvxIOldxFPyMoWXUa+BKCUqT8lhCpHFoUasFaGFCEB5CnU+QKBgBPTT8P7llmVHtiBp+Vxm4y+u6YlYc8y+2qqIvfqAmHzNjlA+1U/3SLZyuLEkX/zKXTL7PUILwWMnbo77OMIP9fFYOZolvvO3FCy56quP1mncaywQZILRD+oit9OF6Ce6hjU410Ax1OkxZpUJSVkdnDPtWHfKtptjtwckhG8xBJBAoGBANjYNkhmhsbxY1A7XJubVbYX/n6f2b5PwscY3xmlcpGMWmLkIf0LWU+F5eTh1lUmRiA9j3a/WVUwnFzIDGdRHcjBSqO6QC870KD8XYGt59Vu3fhUdSZr1nCo2heYG2gs2Pp5KNoyKAeHpPVbtE4n3RRUkpgP3NmVfsfbPTYdEDDi");

            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            var key = new RsaSecurityKey(rsa);
            var handler = new JsonWebTokenHandler();
            var now = DateTime.UtcNow;

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://identity.adasit.com.br/realms/FamilyBudget",
                Audience = "family_budget",
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddMinutes(5),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256),
                Claims = claims

            };

            return handler.CreateToken(descriptor);
        }
    }
}
public class MockJwtTokensTests
{
    [Fact]
    public void GenerateJwtTokeTestn()
    {
        using (var rsa = RSA.Create(2048))
        {
            RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));

            AsymmetricCipherKeyPair keys = rsaKeyPairGenerator.GenerateKeyPair();

            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keys.Private);
            RsaPrivateKeyStructure rsaPrivateKey = RsaPrivateKeyStructure.GetInstance(privateKeyInfo.ParsePrivateKey());
            byte[] pkcs2Der = rsaPrivateKey.ToAsn1Object().GetDerEncoded();

            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keys.Public);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();

            RsaPublicKeyStructure rsaPublicKey = RsaPublicKeyStructure.GetInstance(publicKeyInfo.ParsePublicKey());
            byte[] pkcs1Der = rsaPublicKey.ToAsn1Object().GetDerEncoded();

            var privateKey = Convert.ToBase64String(pkcs2Der);

            var publicKey = Convert.ToBase64String(serializedPublicBytes);

            string path = Path.Combine("..",
                                       "..",
                                       "..",
                                       "..",
                                       "Andor.Component.Tests",
                                       "pacts",
                                       "MyTest.txt");
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("privateKey");
                    sw.WriteLine(privateKey);
                    sw.WriteLine("publicKey");
                    sw.WriteLine(publicKey);
                }
            }

            var privateKeyBytes = Convert.FromBase64String(privateKey);

            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            var key = new RsaSecurityKey(rsa);
            var handler = new JsonWebTokenHandler();
            var now = DateTime.UtcNow;

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://identity.adasit.com.br/realms/FamilyBudget",
                Audience = "family_budget",
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddMinutes(5),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256),

            };

            string jwt = handler.CreateToken(descriptor);

            TokenValidationResult result = handler.ValidateToken(jwt,
                new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuers = new[] { "https://identity.adasit.com.br/realms/FamilyBudget" },
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = BuildRSAKey(publicKey),
                    ValidateLifetime = true

                });
        }
    }


    private static RsaSecurityKey BuildRSAKey(string publicKey)
    {
        var publicKeyJwt = publicKey;

        RSA rsa = RSA.Create();

        rsa.ImportSubjectPublicKeyInfo(
            source: Convert.FromBase64String(publicKeyJwt),
            bytesRead: out _
        );

        var IssuerSigningKey = new RsaSecurityKey(rsa);
        return IssuerSigningKey;
    }
}
