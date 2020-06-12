using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStat.FrameWork.Repository;
using DataStat.WebCore.CommonSuport.AuditingLogs;
using DataStat.WebCore.CommonSuport.Authentication;
using DataStat.WebCore.CommonSuport.Authentication.JwtBearer;
using DataStat.WebCore.CommonSuport.Filter;
using DataStat.WebCore.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DataStat.WebSite
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";
        private readonly IConfigurationRoot _appConfiguration;
        private readonly string _webRootPath;


        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
            _webRootPath = env.WebRootPath;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<JwtSetting>(Configuration.GetSection("JwtSetting"));
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<ITokenService, TokenService>();
            //services.AddControllers();
            //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHttpClient();
            //services.AddHttpClient<HTTPClientHelper>();
            services.AddHttpContextAccessor();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton(typeof(IRepository<,>), typeof(RepositoryImpl<,>));
            //services.AddScoped<IRepository<TEntity>, RepositoryImpl<TEntity>>();

            var tokenAuthConfig = new TokenAuthConfiguration();
            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromHours(8);


            // MVC
            services.AddMvc(
                options => {
                    options.Filters.Add(new CorsAuthorizationFilterFactory(_defaultCorsPolicyName));
                    options.Filters.Add(typeof(LogFilter));
                    options.Filters.Add<ExceptionFilter>();
                    options.Filters.Add(typeof(WebApiResultFilter));
                    options.RespectBrowserAcceptHeader = true;
                }
            ).AddApplicationPart(System.Reflection.Assembly.Load(new System.Reflection.AssemblyName("DataStat.WebCore")))
            .AddApplicationPart(System.Reflection.Assembly.Load(new System.Reflection.AssemblyName("DataStat.FrameWork")));
            //services.Add(new ServiceDescriptor(typeof(WorldContext), new WorldContext(Configuration.GetConnectionString("DefaultConnection"))));

            //跨域访问控制
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        //.WithOrigins(
                        //     //App: CorsOrigins in appsettings.json can contain more than one address separated by comma.
                        //    _appConfiguration["App:CorsOrigins"]
                        //        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        //        .Select(o => o.RemovePostFix("/"))
                        //        .ToArray()
                        //)
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                )
            );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            //是否开启发行人认证和发行人
                            ValidateIssuer = true,
                            ValidIssuer = _appConfiguration["Authentication:JwtBearer:Issuer"],

                            //是否开启订阅人认证和订阅人
                            ValidateAudience = true,
                            ValidAudience = _appConfiguration["Authentication:JwtBearer:Audience"],

                            //是否开启密钥认证和key值
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"])),

                            //是否开启时间认证
                            ValidateLifetime = true,
                            //是否该令牌必须带有过期时间
                            RequireExpirationTime = true,

                            //认证时间的偏移量
                            ClockSkew = TimeSpan.Zero
                        };
                    });

            //Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("综治平台-数据统计服务", new OpenApiInfo { Title = "综治平台-数据统计服务 API", Version = "v1.0" });
                //options.DocInclusionPredicate((docName, description) =>
                //{
                //    var classInfo = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)description.ActionDescriptor);
                //    var attributes = classInfo.MethodInfo.CustomAttributes;
                //    if (attributes.Count(t => t.AttributeType == typeof(HiddenApiAttribute)) == 0)
                //    {
                //        if (new string[] { "CreateByBatch", "UpdateByBatch", "UpdateDeleteByBatch", "GetAll" }.Contains(classInfo.MethodInfo.Name))
                //        {
                //            return false;
                //        }
                //        return true;
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //});

                // Define the BearerAuth scheme that's in use
                //options.OperationFilter<SwaggerHeader>();
                //var security = new Dictionary<string, IEnumerable<string>> { { "bearerAuth", new string[] { } }, };
                //options.AddSecurityRequirement(security);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                //Json Token认证方式，此方式为全局添加
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme }
                        }, new List<string>() }
                });

                //在接口类、方法标记属性 [HiddenApi]，可以阻止【Swagger文档】生成
                //options.DocumentFilter<HiddenApiFilter>();
                //options.DocumentFilter<CustomDocumentFiliter>();
                // Assign scope requirements to operations based on AuthorizeAttribute
                //options.OperationFilter<SecurityRequirementsOperationFilter>();
                //获取项目指定路径下xml文件
                // 为 Swagger JSON and UI设置xml文档注释路径
                //var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录
                //var xmlPath = Path.Combine(basePath, "swagger.xml");
                options.IncludeXmlComments(Path.Combine(_webRootPath, "swagger.xml"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseCors(); // Enable CORS!
            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            //DefaultFilesOptions o = new DefaultFilesOptions();
            //o.DefaultFileNames.Add("index.html");    //将index.html改为需要默认起始页的文件名.
            //app.UseDefaultFiles(o);
            app.UseStaticFiles();
            

            app.UseAuthentication();//声明支持

            app.UseJwtTokenMiddleware(); //中间件

            //app.UseLogLogMiddleware();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });




            //Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            //Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                //string url;
                //if (_appConfiguration["App:ServerRootAddress"].IsNullOrEmpty())
                //{
                //    url = string.Empty;
                //}
                //else
                //{
                //    url = _appConfiguration["App:ServerRootAddress"] + "/swagger/";
                //}
                options.InjectJavascript($"swagger/ui/abp.js");
                options.InjectJavascript($"swagger/ui/on-complete.js");
                options.InjectJavascript($"swagger/ui/swagger.js"); // 加载中文包
                options.InjectStylesheet($"swagger/ui/swagger.css"); // 加载中文包
                options.SwaggerEndpoint($"swagger/综治平台-数据统计服务/swagger.json", "综治平台-数据统计服务 API");
                options.RoutePrefix = string.Empty;
            }); // URL: /swagger


            //启用中间件服务生成Swagger作为JSON终结点
            //app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "综治平台-数据统计服务 API");
            //    c.RoutePrefix = string.Empty;
            //});

            //app.Run(context =>
            //{
            //    context.Response.Redirect("/index.html"); //可以支持虚拟路径或者index.html这类起始页.
            //    return Task.FromResult(0);
            //    //await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
