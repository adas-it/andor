namespace Family.Budget.ComponentTest.Utils;
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
using System.Security.Cryptography;
using Xunit;

public static class MockJwtTokens
{
    public static string GenerateJwtToken(Dictionary<string, object> claims)
    {
        using (var rsa = RSA.Create(2048))
        {
            var privateKeyBytes = Convert.FromBase64String("MIIEogIBAAKCAQEAmJ/SnE1PIreoOf0Or8AeK4l3LuQDsdd9+s/qNnGAmZ+UOUh9RHFohZa6HtqogbBA6BGkFPZD/A4h5P1qxv54hDOyflcC3DQwZ0VDQ8OJXKBFAWVzIBkohVqEaMWG4G7xIJcz+guCqQmJ5i2YhZbwMyt4AwHYu9xqXHG14WIFvbmRbvLEUtWVbhrtMLio9vWvZ7okuQVZGL3Z7icUWkQiMoaKABkiW6qvzKECBcKkgLTfACmbdH7ZbjWv8i/+KUy7SNLtt7ja2fO3wykarOk18FA5heGCBBJ9n1BnOZ6wog20femVioXEYwou1i4na7gaWzulJgGjtEWBcTQSf8oJVwIDAQABAoIBAAa7jiPc+S+z4FTmh95S4EHVZ4+G+W6/7cvBnjhWm3CrbHN2cigxxIWYF4/C34oRB1v1Y0KVdnxI90/NqgcrNI/IZAhUuEiUeMolcQktOAMBAupD2mdBFNuFGbAxfniqDDL+2IkgNtxUEmQ4ALFr7h6jvUCMtU3cNEjtrzNJCq3Kz2E7og3uYtl+PR5ddfdBNMAjDAZ/t1vYsnHj6zZOJqYVY/hd/kwcooezQnxzgUibQheNwNgs4tYi8KKdCWN9MqreG6/Aa6Fx13yy8lhUf8rOI4e3g6F32k5Hq1ifP67uPwCdosSD56zycBU7iuqMNVwdaS8od/DYLr6xB7r36okCgYEAyZY7PEOCLVSIc6PIbPxC1RQx1Hhmr01LXuzLZWOWXFiCwxt0LiEdoBul4iwJdQbaClTKf0vARYz0ozH4pY7L8aSmuP+ko991BaKmCNgZ3lK6yDHBI3I4FlNWKsMvUGxS+YaUJsOAkjSA4QGQI1CU7ua+ygq0AbnFA2+vNTz4NG0CgYEAwdJAZ8cHicKE31SRogPN3D55sDFpdPf7tU7ldZAa3xFa/pzr6a6O89tTqJkrtHgf1qJaYa2BegVnDuplNxW2s39xq00DAIgiV+89Sa6oi9Vgda4ttbSFrjbQ6ocHp+uHuyfNj+HKpao7SclQ4A7RW2u901o0eb+xf0G899OK8lMCgYBq33vuGjhUwgFJlaZ3qVHhV8CHTYHbJJZ18AJxcVfRA5fnypFQt3vWW3IZs5Eb+xLz8ToePNHlzbIRJzgUxdz38nv1josB2kFXSIDQJYmpZaDi7AXiPfbgVzRSnYJjF9rxsViwKGvsl2tQXxzErD8ZXTn/Mad3Vr7NcJOqHPXWDQKBgCtjdNWGgZJ+QB56oQtizJ/EQJuHxDLSHL/uxIE33DwZd4RN/qXWVNwUOjsg5t7EkWQL/i2lmLmHIhX9tODK9JZkLj8Jw2VIFGAZB2BKQCLlhm1xq3vkyJRCYyxNRBJ8MGteMq9F/YZugE05SuiNiKJtkRYSOHMUzZhOUA4uhglPAoGAPK2/IwY7MEAL8NNBhalR4gLbymZqSmJo9x4PRHMUl/t4KWYT2tnKIsuBo1laTf1Qku4Niu1mJTxhZ/JIdSlj96FNlqstkySnBbbEMrBvgor4G4urINQ/2bTSxKbTtqztHoujNFtc1KZEEp32C1atSyeIwOaGl4SZb4dVr1N6ZV4=");

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
