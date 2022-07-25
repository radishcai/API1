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
            //ע��Swagger�ĵ�
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Title = "CC1.Api",
                    Version = "V1",
                    Description = $"Core.WebApi HTTP API V1",
                });
                c.OrderActionsBy(o => o.RelativePath);

                // ��ȡxmlע���ļ���Ŀ¼
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "CC1.API.xml");
                c.IncludeXmlComments(xmlPath, true);//Ĭ�ϵĵڶ���������false�������controller��ע�ͣ��ǵ��޸�
                // ��ȡxmlע���ļ���Ŀ¼
                var xmlPathModel = Path.Combine(AppContext.BaseDirectory, "CC1.Model.xml");
                c.IncludeXmlComments(xmlPathModel, true);//Ĭ�ϵĵڶ���������false�������controller��ע�ͣ��ǵ��޸�


                #region Swaggerʹ�ü�Ȩ���
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Description = "ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�",
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
            #region IOC����ע��
            services.AddCustomIOC();
            #endregion

            #region JWT��Ȩ
            services.AddCustomJWT();
            #endregion

            #region AutoMapper
            services.AddAutoMapper(typeof(CustomAutoMapperProfile));
            #endregion
        }

        //�м��
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            //�Զ����쳣�����м��
            app.UseMiddleware<ErrorHandlingMiddleware>();

            //��ӵ��ܵ��� ��Ȩ
            app.UseAuthentication();
            //��Ȩ
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
                          ValidateIssuer = true,//�Ƿ���֤Issuer
                          ValidateAudience = true,//�Ƿ���֤Audience
                          ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                          ValidateIssuerSigningKey = true,//�Ƿ���֤SecurityKey
                          ValidAudience = "http://localhost:5000",//API��ַ
                          ValidIssuer = "http://localhost:6060",//JWT��ַ,
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SDMC-CJAS1-SAD-DFSFA-SADHJVF-VF")),//��ȡSecurityKey
                          ClockSkew = TimeSpan.FromMinutes(60) //�������ʱ�䣬����Чʱ��=����ʱ��+�������ʱ�䣬�����õĻ�Ĭ��5����
                      };

                      //�Զ����Ȩʧ�ܷ��ؽ��
                      options.Events = new JwtBearerEvents
                      {
                          //Ȩ����֤ʧ�ܺ�ִ��
                          OnChallenge = context =>
                          {
                              //��ֹĬ�ϵķ��ؽ��(������)
                              context.HandleResponse();
                              var result = JsonConvert.SerializeObject(new { Code = "401", Msg = "��֤ʧ��" });
                              context.Response.ContentType = "application/json";
                              //��֤ʧ�ܷ���401
                              context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                              context.Response.WriteAsync(result);
                              return Task.FromResult(0);
                          },
                          //�Զ������
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
