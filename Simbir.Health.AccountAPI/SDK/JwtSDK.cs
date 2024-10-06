using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Simbir.Health.AccountAPI.Model.Database.DTO.ValidTokens;
using Simbir.Health.AccountAPI.SDK.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Simbir.Health.AccountAPI.SDK
{
    public class JwtSDK : IJwtService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cache;

        public JwtSDK() { }

        public JwtSDK(IConfiguration conf, ICacheService cache)
        {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(string.Empty);
            _configuration = conf;
            _cache = cache;
        }

        private string? RSAPrivateKey(string? type)
        {
            if (type == "RT")
            {
                return @"-----BEGIN RSA PRIVATE KEY-----
MIICXAIBAAKBgQCeXVuaJGOQleufBCm+80wkZwEI34QhypgPc1rFpAJqq6NLDNVu
HbKmqE8qejzg5VAHnkTJk4c+3urwITUKtc+z1/VOCPbR+GeIdX65k9EH8YolRnwe
972vqNYd4NiTKRSFSldsdSw2yUj0grwALkSir4JaiBD4EakJ4NrkWdb2+wIDAQAB
AoGAFbHQZLNreFkxaB1X4rLN0YbS23ZTUZXBcwxoeP7Y3egZfKSLcIRc/vu7rKQG
RwDjD8gcwEiXlINRSAgkjg0OIOt/1AsbGsBdZhh2NCWWCLumeKuyGB+6xWAm9Qb8
oIxzBmkHdgx1ykLsTQH6cakoX51slTJ7tdbFM00fQBswLyECQQDiRGbhZZYEkEk6
CeTJimw/iQWGXtLVV6T6ck0eRrpG8m3FHQTxEqPOHEQFK6iMJEocjjSZ6tm3Es6q
xu2/4LQJAkEAsyy5csFBjnCybvHDSdfLCa33tjoLeCTFFJM6uF8a8swuyVLWuTKl
YYY5zoCreeuhO92pSwWpCG+8MzX5Yh974wJARIPb92Kwg59BXT7Dtbehwbd3IdIy
24FXprLX4VQfcf5U+Pwpk+pGCdKLUll/Bzix7GWvTfBMjuA2DoaAVbrwKQJBAIQU
aRRt39yXuQFN2O77U2HsS1maik/jkyBqs/OrsBrhZ2/jUAQvkHhG0SAn+8AhcbbG
3QA/yO4+J9b8Z7zsho8CQHVULz1iy2uHeKZawAZqiME10Uy0OEmswV3Yot18hLhe
+kAUrjGyT6o5p2SRh5yYpQ5pANC9yK3CQ1wGr97z0yw=
-----END RSA PRIVATE KEY-----";
            }
            else if (type == "JWT")
            {
                return @"-----BEGIN RSA PRIVATE KEY-----
MIICWgIBAAKBgHk1+4gUWP1RFPaIX3S1AHFShLatEIkzvY1TvKF2HBhtZoM6XHUw
eQDF3AW32Ic9W+uvvkhqM87d8RPf5yT70BTyK+8FVISvDDmVJ4+W5jU0zrxPOs/E
uCSEaiL4pkJ5Z57PUwJs47Kkr/2AVqVtakAxozIPBVVGSvbFKDiaJqprAgMBAAEC
gYAB3bvmp3GLTOFGvmjB0juUgxG6AkYb5qkHcP+ZeMkL8zs9z/s2bq2ePm2ZxO/X
fTFaGpWSZdgwWihpBux3HE1iz4UxLL1YQwfHYfvWrdsn4KxvZJMvWSn/QiUAyP6E
hmc8iyx6Berg+3f2sqbZK8QzR9K6+l8iD268vkJlJeiz6QJBAOrm2w/2iDrhJ+0a
O1hrN8i8Vm/Jpmq8q/rbE0bIExmIMB3fsSgcQ3E8Gvt2/iucceLaZTvhFESpEt6a
Duxv+8cCQQCEGQI3M5139rLTopmWDfWZqVepge0h0sTevcNTqYiz6dkmaJyFwV7f
/l8xAjQq4A/CuuVozmNeSlA9CZ6QkvQ9AkBKDe+f77PxBAynRi7RaDPU9/M0GNl7
KvH5Ctnf5bGHyhSJRn0+TPLCHxXOkyv8Kb0JrJpfr0zJfJfzT5RG2+L/AkAPzGXl
cDjfBhQxF3tC5PyAmi4vyagwqWJ+OTfJI7eDvO4Jl+1QWeYc5/E+jULr9cwsv+l7
9WMQ/dZG1oWuSMUJAkB0Fh+olmOkqLiM/GzUXI3vgfyPeNKBMWICV8GAnu8G38M5
6215WNGNKJaV+51XMZngDX6CKYuwyhRfTbW0pjVu
-----END RSA PRIVATE KEY-----";
            }
            else
            {
                return "empty";
            }
        }

        public string? RSAPublicKey(string? type)
        {
            if (type == "RT")
            {
                return @"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCeXVuaJGOQleufBCm+80wkZwEI
34QhypgPc1rFpAJqq6NLDNVuHbKmqE8qejzg5VAHnkTJk4c+3urwITUKtc+z1/VO
CPbR+GeIdX65k9EH8YolRnwe972vqNYd4NiTKRSFSldsdSw2yUj0grwALkSir4Ja
iBD4EakJ4NrkWdb2+wIDAQAB
-----END PUBLIC KEY-----";
            }
            else if (type == "JWT")
            {
                return @"-----BEGIN PUBLIC KEY-----
MIGeMA0GCSqGSIb3DQEBAQUAA4GMADCBiAKBgHk1+4gUWP1RFPaIX3S1AHFShLat
EIkzvY1TvKF2HBhtZoM6XHUweQDF3AW32Ic9W+uvvkhqM87d8RPf5yT70BTyK+8F
VISvDDmVJ4+W5jU0zrxPOs/EuCSEaiL4pkJ5Z57PUwJs47Kkr/2AVqVtakAxozIP
BVVGSvbFKDiaJqprAgMBAAE=
-----END PUBLIC KEY-----";
            }
            else
            {
                return "empty";
            }
        }

        private async Task<JwtSecurityToken> RSAJwtValidation(string? token)
        {
            var rsa = RSA.Create();

            rsa.ImportFromPem(RSAPublicKey("JWT"));

            RsaSecurityKey issuerSigningKey = new RsaSecurityKey(rsa);

            TokenValidationParameters tk_valid = new TokenValidationParameters
            {
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = issuerSigningKey,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
                RequireExpirationTime = false,
                ValidateLifetime = false
            };

            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, tk_valid, out var rawValidatedToken);

                return (JwtSecurityToken)rawValidatedToken;
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
        }

        private async Task<JwtSecurityToken> RSARefreshTokenValidation(string? token)
        {
            var rsa = RSA.Create();

            rsa.ImportFromPem(RSAPublicKey("RT"));

            RsaSecurityKey issuerSigningKey = new RsaSecurityKey(rsa);

            TokenValidationParameters tk_valid = new TokenValidationParameters
            {
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = issuerSigningKey,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
                RequireExpirationTime = false,
                ValidateLifetime = false
            };

            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, tk_valid, out var rawValidatedToken);

                return (JwtSecurityToken)rawValidatedToken;
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
        }

        public async Task<Token_ValidProperties> AccessTokenValidation(string? bearerKey)
        {
            string bearer_key_without_prefix = bearerKey.Substring("Bearer ".Length);

            var validation = await RSAJwtValidation(bearer_key_without_prefix);

            var expectedAlg = SecurityAlgorithms.RsaSha512;

            if (validation == null)
            {
                return new Token_ValidProperties() { token_error = new Token_ValidError { errorLog = "unauthorized" } };
            }
            else
            {
                if (validation.Header?.Alg == null || validation.Header?.Alg != expectedAlg)
                {
                    return new Token_ValidProperties() { token_error = new Token_ValidError { errorLog = "unexpected_alg" } };
                }

                string userName = "";

                foreach (var claim in validation.Claims)
                {
                    if (claim.Type == "Username")
                        userName = claim.Value;
                }

                if (_cache.CheckExistKeysStorage(userName, "accessTokens"))
                {
                    if (_cache.GetKeyFromStorage(userName, "accessTokens") != bearer_key_without_prefix)
                        return new Token_ValidProperties() { token_error = new Token_ValidError { errorLog = "unauthorized" } };
                }
                else
                    return new Token_ValidProperties() { token_error = new Token_ValidError { errorLog = "unauthorized" } };
                
                Token_ValidSuccess valid_success = new Token_ValidSuccess
                {
                    userName = userName,
                    bearerWithoutPrefix = bearer_key_without_prefix
                };

                return new Token_ValidProperties() { 
                    token_success = valid_success
                };
            }
        }

        public async Task<Token_ValidProperties> RefreshTokenValidation(string? bearerKey)
        {
            var validation = await RSARefreshTokenValidation(bearerKey);

            var expectedAlg = SecurityAlgorithms.RsaSha512;

            if (validation == null)
            {
                return new Token_ValidProperties() { token_error = new Token_ValidError { errorLog = "unauthorized" } };
            }
            else
            {
                if (validation.Header?.Alg == null || validation.Header?.Alg != expectedAlg)
                {
                    return new Token_ValidProperties() { token_error = new Token_ValidError { errorLog = "unexpected_alg" } };
                }

                string userName = "";

                foreach (var claim in validation.Claims)
                {
                    if (claim.Type == "Username")
                        userName = claim.Value;
                }

                if (_cache.CheckExistKeysStorage(userName, "refreshTokens"))
                {
                    if (_cache.GetKeyFromStorage(userName, "refreshTokens") != bearerKey)
                        return new Token_ValidProperties() { token_error = new Token_ValidError { errorLog = "unauthorized" } };
                }
                else
                    return new Token_ValidProperties() { token_error = new Token_ValidError { errorLog = "unauthorized" } };

                Token_ValidSuccess valid_success = new Token_ValidSuccess
                {
                    userName = userName,
                    bearerWithoutPrefix = bearerKey
                };

                return new Token_ValidProperties()
                {
                    token_success = valid_success
                };
            }
        }

        public string JwtTokenCreation(string userName)
        {

            var rsaprivateKey = RSAPrivateKey("JWT");

            using var rsa = RSA.Create();
            rsa.ImportFromPem(rsaprivateKey);

            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha512)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Username", userName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = signingCredentials,
                
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

        public string RefreshTokenCreation(string userName)
        {
            var rsaprivateKey = RSAPrivateKey("RT");

            using var rsa = RSA.Create();
            rsa.ImportFromPem(rsaprivateKey);

            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha512)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Username", userName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = signingCredentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
