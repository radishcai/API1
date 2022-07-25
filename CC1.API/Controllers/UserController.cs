using AutoMapper;
using CC1.Common._MD5;
using CC1.Common.ApiResult;
using CC1.IService;
using CC1.Model;
using CC1.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 创建用户 api/User/Create
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<ApiResult> Create(User user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.UserCode) || string.IsNullOrEmpty(user.UserPwd))
            {
                return ApiResultHelper.Error("必填信息为空!");
            }
            var data = await _userService.FindAsync(c => c.UserCode == user.UserCode);
            if (data != null){
                return ApiResultHelper.Error("用户编号已存在!");
            }
            //密码MD5加密
            user.UserPwd = MD5Helper.MD5Encrypt32(user.UserPwd);
            user.IsValid = 1;
            user.Create = DateTime.Now;
            bool flag= await _userService.CreateAsync(user);
            if (!flag) {
                return ApiResultHelper.Error("保存失败，请联系系统管理员!");
            }
            return ApiResultHelper.Success(user);
        }


        /// <summary>
        /// 修改用户 api/User/Edit
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        public async Task<ApiResult> Edit([FromServices] IMapper iMapper,UserDTO userDto)
        {
            var user= iMapper.Map<User>(userDto);
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.UserCode) || string.IsNullOrEmpty(user.UserPwd))
            {
                return ApiResultHelper.Error("必填信息为空!");
            }
            var data = await _userService.FindAsync(c => c.UserCode == user.UserCode);
            if (data != null)
            {
                return ApiResultHelper.Error("用户编号已存在!");
            }
            //密码MD5加密
            user.UserPwd = MD5Helper.MD5Encrypt32(user.UserPwd);
            user.Create = DateTime.Now;
            bool flag = await _userService.CreateAsync(user);
            if (!flag)
            {
                return ApiResultHelper.Error("保存失败，请联系系统管理员!");
            }
            return ApiResultHelper.Success(user);
        }
    }
}
