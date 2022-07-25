using CC1.API.AutoMapper;
using CC1.API.Middleware;
using CC1.IService;
using CC1.Repository;
using CC1.Service;
using IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC1.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //注入Swagger文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Title = "CC1.Api",
                    Version = "V1",
                    Description = $"Core.WebApi HTTP API V1",
                });
                c.OrderActionsBy(o => o.RelativePath);

                // 获取xml注释文件的目录
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "CC1.API.xml");
                c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改
                // 获取xml注释文件的目录
                var xmlPathModel = Path.Combine(AppContext.BaseDirectory, "CC1.Model.xml");
                c.IncludeXmlComments(xmlPathModel, true);//默认的第二个参数是false，这个是controller的注释，记得修改


                #region Swagger使用鉴权组件
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                         new OpenApiSecurityScheme
                         {
                            Reference=new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                }
            },
            new string[] {}
          }
        });
                #endregion

            });

            #region SqlSugarIOC
            services.AddSqlSugar(new IocConfig()
            {
                ConnectionString = this.Configuration["SqlConn"],
                DbType = IocDbType.SqlServer,
                IsAutoCloseConnection = true
            });
            #endregion
            #region IOC依赖注入
            services.AddCustomIOC();
            #endregion

            #region JWT鉴权
            services.AddCustomJWT();
            #endregion

            #region AutoMapper
            services.AddAutoMapper(typeof(CustomAutoMapperProfile));
            #endregion
        }

        //中间件
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            //自定义异常处理中间件
            app.UseMiddleware<ErrorHandlingMiddleware>();

            //添加到管道中 鉴权
            app.UseAuthentication();
            //授权
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "Core.WebApi HTTP API V1");
                c.RoutePrefix = "";
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        
    }


    public static class IOCExtend
    {
        public static IServiceCollection AddCustomIOC(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }

        public static IServiceCollection AddCustomJWT(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,//是否验证Issuer
                          ValidateAudience = true,//是否验证Audience
                          ValidateLifetime = true,//是否验证失效时间
                          ValidateIssuerSigningKey = true,//是否验证SecurityKey
                          ValidAudience = "http://localhost:5000",//API网址
                          ValidIssuer = "http://localhost:6060",//JWT网址,
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SDMC-CJAS1-SAD-DFSFA-SADHJVF-VF")),//获取SecurityKey
                          ClockSkew = TimeSpan.FromMinutes(60) //缓冲过期时间，总有效时间=过期时间+缓冲过期时间，不配置的话默认5分钟
                      };

                      //自定义鉴权失败返回结果
                      options.Events = new JwtBearerEvents
                      {
                          //权限验证失败后执行
                          OnChallenge = context =>
                          {
                              //终止默认的返回结果(必须有)
                              context.HandleResponse();
                              var result = JsonConvert.SerializeObject(new { Code = "401", Msg = "验证失败" });
                              context.Response.ContentType = "application/json";
                              //验证失败返回401
                              context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                              context.Response.WriteAsync(result);
                              return Task.FromResult(0);
                          },
                          //自定义参数
                          OnMessageReceived = context =>
                          {
                              context.Token = context.Request.Headers["access_token"];
                              return Task.CompletedTask;
                          }
                      };

                      
                  });
            return services;
        }
    }
}
