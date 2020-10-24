using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SHB.Business.Messaging;
using SHB.Business.Messaging.Email;
using SHB.Business.Messaging.Sms;
using SHB.Business.Services;
using SHB.Core.Caching;
using SHB.Core.Configuration;
using SHB.Core.Domain.Entities;
using SHB.Core.Extensions;
using SHB.Core.Utilities;
using SHB.Data.efCore;
using SHB.Data.efCore.Context;
using SHB.Data.UnitOfWork;
using SHB.WebApi.Infrastructure.Services;
using SHB.WebApi.Utils;
using SHB.WebApi.ViewModels;
using SHB.WebAPI.Utils;
using SHB.WebAPI.Utils.Extentions;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace SHB.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Identity

            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString(WebConstants.ConnectionStringName)).EnableSensitiveDataLogging()
             , ServiceLifetime.Transient);


            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Lockout.AllowedForNewUsers = false;
                //options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            #endregion

            #region Services

            services.AddTransient<DbContext>((_) => {
                var connStr = Configuration.GetConnectionString(WebConstants.ConnectionStringName);
                return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                                         .UseSqlServer(connStr,
                     b => b.MigrationsAssembly(typeof(ApplicationDbContext).FullName))
                                         .Options);
            });

            services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();
            services.AddScoped(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
            services.RegisterGenericRepos(typeof(ApplicationDbContext));

            services.AddScoped<IErrorCodeService, ErrorCodeService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IWalletNumberService, WalletNumberService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IReferralService, ReferralService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<ITerminalService, TerminalService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ICompanyInfo, CompanyInfoService>();
            services.AddScoped<IAppMenu, AppMenuService>();



            #endregion
            services.Configure<JwtConfig>(options =>
                 Configuration.GetSection(WebConstants.Sections.AuthJwtBearer).Bind(options));

            services.Configure<AppConfig>(options =>
                     Configuration.GetSection(WebConstants.Sections.App).Bind(options));

            services.Configure<SmtpConfig>(options =>
                     Configuration.GetSection(WebConstants.Sections.Smtp).Bind(options));

            services.Configure<PaymentConfig.Paystack>(options =>
                     Configuration.GetSection(WebConstants.Sections.Paystack).Bind(options));

            services.Configure<DataProtectionTokenProviderOptions>(o =>
                     o.TokenLifespan = TimeSpan.FromHours(3));

            #region Auth
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                var jwtConfig = new JwtConfig();

                Configuration.Bind(WebConstants.Sections.AuthJwtBearer, jwtConfig);

                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(3),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.SecurityKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateLifetime = true,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context => {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddCors();
            services.AddDistributedMemoryCache();
            #endregion

            services.AddHttpContextAccessor();
            services.AddTransient<IServiceHelper, ServiceHelper>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IMailService, SmtpEmailService>();
            services.AddTransient<ISMSService, SMSService>();
            services.AddTransient<IWebClient, WebClient>();
            services.AddSingleton<IGuidGenerator>((s) => SequentialGuidGenerator.Instance);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            if (Environment.IsDevelopment())
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SHB Web API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);

                    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Token Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                       {
                         new OpenApiSecurityScheme
                         {
                           Reference = new OpenApiReference
                           {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                           }
                          },
                          new string[] { }
                        }
                      });

                                    });
                                }


            //services.AddMvc()
            //services.AddRazorPages()
            services.AddControllers()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app /*, IHostingEnvironment env */)
        {
            if (Environment.IsDevelopment())
            {
                UserSeed.SeedDatabase(app);
                app.UseDeveloperExceptionPage();
            }
            else
            {

                //app.UseHsts();
                app.UseExceptionHandler("/Error");
            }

            app.UseCors(x => {
                x.WithOrigins(Configuration["App:CorsOrigins"]
                  .Split(",", StringSplitOptions.RemoveEmptyEntries)
                  .Select(o => o.RemovePostFix("/"))
                  .ToArray())
             .AllowAnyMethod()
             .AllowAnyHeader();
            });


            //app.UseMvc();
            //app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint(Configuration["App:ServerRootAddress"].EnsureEndsWith('/') + "swagger/v1/swagger.json", "LME.Web API V1");
            });
        }
    }
}
