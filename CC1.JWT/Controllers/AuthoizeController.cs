
using CC1.Common._MD5;
using CC1.Common.ApiResult;
using CC1.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CC1.JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthoizeController : ControllerBase
    {
        //JWT文档https://blog.csdn.net/weixin_44442366/article/details/124017306?spm=1001.2101.3001.6661.1&utm_medium=distribute.pc_relevant_t0.none-task-blog-2%7Edefault%7ECTRLIST%7Edefault-1-124017306-blog-121863725.pc_relevant_multi_platform_whitelistv2_ad_hc&depth_1-utm_source=distribute.pc_relevant_t0.none-task-blog-2%7Edefault%7ECTRLIST%7Edefault-1-124017306-blog-121863725.pc_relevant_multi_platform_whitelistv2_ad_hc&utm_relevant_index=1
        private IUserService _userService;

        public AuthoizeController(IUserService userService)
        {
            _userService = userService;
        }
        
        /// <summary>
        /// 登录接口，获取token
        /// </summary>
        /// <param name="userCode">用户编号</param>
        /// <param name="userPwd">用户密码</param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ApiResult> Login(string userCode, string userPwd)
        {
            if (string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(userPwd))
            {
                return ApiResultHelper.Error("用户编号或密码为空");
            }
            string strPwd = MD5Helper.MD5Encrypt32(userPwd);

            var user = await _userService.FindAsync(a => a.UserCode == userCode && a.UserPwd == strPwd && a.IsValid==1);
            if (user == null)
            {
                return ApiResultHelper.Error("用户编号或密码错误");
            }

            //修改登录时间和登录次数
            user.LoginNum = user.LoginNum + 1;
            user.LastTime = DateTime.Now;
            _userService.EditAsync(user);

            //登陆成功
            var claims = new Claim[]
                {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Id", user.Id.ToString()),
                new Claim("UserName", user.UserName)
                    //不能放敏感信息 
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SDMC-CJAS1-SAD-DFSFA-SADHJVF-VF"));
            //issuer代表颁发Token的Web应用程序，audience是Token的受理者
            var token = new JwtSecurityToken(
                issuer: "http://localhost:6060",//JWT
                audience: "http://localhost:5000",//API
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return ApiResultHelper.Success(jwtToken);
        }
    }
}
