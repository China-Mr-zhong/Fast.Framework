using System;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace Fast.Framework.Web.Test
{

    /// <summary>
    /// Token 工具类
    /// </summary>
    public static class Token
    {

        /// <summary>
        /// Jwt 安全令牌处理程序
        /// </summary>
        private static readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

        /// <summary>
        /// 颁发者
        /// </summary>
        private static readonly string issuer;

        /// <summary>
        /// 接收者
        /// </summary>
        private static readonly string audience;

        /// <summary>
        /// Token验证参数
        /// </summary>
        public static readonly TokenValidationParameters tokenValidationParameters;

        /// <summary>
        /// 对称安全密钥
        /// </summary>
        private static readonly SymmetricSecurityKey symmetricSecurityKey;

        /// <summary>
        /// 构造方法
        /// </summary>
        static Token()
        {
            jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            issuer = "fast.framework";
            audience = "user";
            symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("6A4EC34D-9955-49B1-83D7-80B6CF1E13CD"));

            tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,//验证颁发者
                ValidateAudience = true,//验证接收者
                ValidateLifetime = true,//验证过期时间
                ValidateIssuerSigningKey = true, //是否验证签名
                ValidIssuer = issuer,//颁发者
                ValidAudience = audience,//接收者
                IssuerSigningKey = symmetricSecurityKey,//解密密钥
                ClockSkew = TimeSpan.Zero //缓冲时间
            };
        }

        /// <summary>
        /// 创建JwtToken
        /// </summary>
        /// <param name="expirationTime">过期时间</param>
        /// <param name="claims">自定义Claims</param>
        /// <returns></returns>
        public static string CreateJwtToken(DateTime expirationTime, IEnumerable<Claim> claims = null)
        {
            var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: expirationTime,
                signingCredentials: creds);
            return jwtSecurityTokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 验证JwtToken
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="func">委托</param>
        /// <returns></returns>
        public static bool VerifyJwtToken(string token, Func<ClaimsPrincipal, bool> func = null)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }
            var parameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = symmetricSecurityKey,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                var claims = jwtSecurityTokenHandler.ValidateToken(token, parameters, out SecurityToken securityToken);
                if (func == null)
                {
                    return true;
                }
                return func.Invoke(claims);
            }
            catch
            {
                return false;
            }
        }
    }
}
