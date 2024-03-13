﻿namespace Family.Budget.ComponentTest.Utils;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Xunit;

public static class MockJwtTokens
{
    public static string GenerateJwtToken(Dictionary<string, object> claims)
    {
        using (var rsa = RSA.Create(2048))
        {
            var privateKeyBytes = Convert.FromBase64String("MIIEpAIBAAKCAQEArUR9uO3SYpoCPZIw90H241kz4aSVdUGAMv/vLnT9Rnk7lFuHXrz6NWWrO5RQtHfKCriVKf/TIJtJcEeuh3oUQBanGwLAwcWSvvzB0gunfPAVCqGHA7J7nNhnGHl8lWp2tSkSMMD1UDDGSRpc7uF6nNUMtp9b748X3VFBS9+PHGLu6sFIZTWSmT3pIbAWSMsNwmflTpO7RKnzYOpCXANG1mO57DLs4n/Y2MevuI0wQLhgKT8x/FhdDPNJtbzHYI21LvYQNNXIIvuaVOB+VCHUWRhHFuRiLsCMej/0+gMZZdTTNloVPN+109auuQBvqz4jnxK2eGc/qJdo045BOYZb4wIDAQABAoIBAE3ckeyo/pptn+UgTV3HaFVZEB3tknY7RYtlhIupaemh8BqmmOoDXSLiubSP3UaD8CiqMunJOwyTi8hnhe/+dZwpMdXLAmjxSpUFy2mC8DSORL5ewOfx7GeavZV7fnc8fPTC7Jg5FeP/zxMpxCpa6/kPKmPkr/EyqeQ8L+4TG+ICeeRfXH/l8rUWCLmxVUyjZoclnTaqX8Prz9j2hocxLczq+kxIeAMVp6jUvLXbjgKrutkg51UIyew/9gxJDMgjQkjCLymkpSh5LQyZmFJiROgFM5tAxIRSyHi/fKEXU4y7pmI0lgO9jpv4xEzBUEOTACfXKI2aGq3n0GfEJaA04PECgYEA9TeaOVnglhkoavbKZaANjyD+MUWmg0p5mEsDXvU2QCQqNMerLv7Lu6lV2YSagr545ZhxBOM9ZmjUii6Zeke+wdz94Q3Tcc9VTft+a9OURoe3agACMrQ/6t80YUHhXJaCiL42UkSOJeC5dbQoHjq/iqYZeGEdc/3dYomcqJDwcbECgYEAtOL0fsw8z7mFsY9waiHi9mMsSYkfpiuTMBKJ+3yatrcV89/2CN3aHryw23y3IW5B4dEndkkZpreTNnUNXE6ncGxaBQDyNJ/sgOFUOcizSSuX1HMLBmsgiFTvkPwahGPsWRUt+lAI1l4mpb7DwFpfwAIBvHB1ndtoy1gNIYPt19MCgYEA7poY1b8veb2nB1V1oSb4qpFm5VipakzYPiiXchlVnVnTO6IhXkwMv2BIqauOkLN3fag0KJZYU0KjtIUdT89pvMckvYyvdqlQjt2sqIyqVWqPPHQfr2iWXRqspAnvmCDXiOjUgH42doLQ0sfm1WtCn015sRmGCCw38z6FkbveSjECgYBNi/7FXUm0xBRMyvGmjmB+wIV8AQ1Qb93p/gipiAHL0zHWG22e331jMAGiFxVrf5k+9iu0yiH1Fz5FWmkf6Zhe2hS9kGpFpUn+9prlKuWxuL6elbNkTQjlaYRK48l6eFwa1LmhVi5zuGlZJS9Gh3gFGNclshC6XBXRFy6J78VHgQKBgQDMCWYq72A7++h8YQniNp7YVi6FaKob1FVRracMG+dxKGfKrxJ/1CY7ps460ZvbrqLu4Au9ql8TpRj2uI7UFAabdUqmcd29RJBTJ6RlE/s9wISDlFd1nnzd/l59c26EL1x+53gddmw86H3J3z7quNVDmnMcQOYah+T35HoN+J0tnA==");

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
                                       "Family.Budget.ComponentTest",
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
