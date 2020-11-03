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
using SHB.Core.Entities;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

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

        public void ConfigureServices(IServiceCollection services)
        {
            #region Identity

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(WebConstants.ConnectionStringName))
                , ServiceLifetime.Transient);

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Lockout.AllowedForNewUsers = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            #endregion

            #region Services
            services.AddTransient<DbContext>((_) =>
            {
                var connStr = Configuration.GetConnectionString(WebConstants.ConnectionStringName);
                return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                                         .UseSqlServer(connStr)
                                         .Options);
            });

            services.AddScoped<IUnitOfWork, EfCoreUnitOfWork>();
            services.AddScoped(typeof(IDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));
            services.RegisterGenericRepos(typeof(ApplicationDbContext));

            services.AddScoped<IErrorCodeService, ErrorCodeService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IWalletNumberService, WalletNumberService>();
            services.AddScoped<ICouponService, CouponService>();
            //services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IReferralService, ReferralService>();
            //services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<ITerminalService, TerminalService>();
            services.AddScoped<IEmployeeService, EmployeeService>();


            //services.AddScoped<IVehicleModelService, VehicleModelService>();
            //services.AddScoped<IVehicleMakeService, VehicleMakeService>();
            //services.AddScoped<IBookingService, BookingService>();
            //services.AddScoped<IManifestService, ManifestService>();
            //services.AddScoped<ISeatManagementService, SeatManagementService>();
            //services.AddScoped<ITripService, TripService>();
            //services.AddScoped<IDiscountService, DiscountService>();
            //services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ICouponService, CouponService>();
            //services.AddScoped<IMtuReports, MtuReportService>();
            //services.AddScoped<ITripAvailabilityService, TripAvailabilityService>();
            //services.AddScoped<IPickupPointService, PickupPointService>();
            //services.AddScoped<IAccountTransactionService, AccountTransactionService>();
            //services.AddScoped<IFareService, FareService>();
            //services.AddScoped<IFareCalendarService, FareCalendarService>();
            //services.AddScoped<IVehicleService, VehicleService>();
            //services.AddScoped<IVehicleTripRegistrationService, VehicleTripRegistrationService>();
            //services.AddScoped<IAccountSummaryService, AccountSummaryService>();
            //services.AddScoped<IHireRequestService, HireRequestService>();
            //services.AddScoped<IBookingReportService, BookingReportService>();
            //services.AddScoped<IFeedbackService, FeedbackService>();
            //services.AddScoped<ISubRouteService, SubRouteService>();
            //services.AddScoped<IJourneyManagementService, JourneyManagementService>();
            //services.AddScoped<IManifestService, ManifestService>();
            //services.AddScoped<IFranchizeService, FranchizeService>();
            //services.AddScoped<IPassportTypeService, PassportTypeService>();
            //services.AddScoped<IHireBusService, HireBusService>();
            //services.AddScoped<IHirePassengerService, HirePassengerService>();
            //services.AddScoped<IAgentLoctionService, AgentLocationService>();
            //services.AddScoped<IAgentCommissionService, AgentCommissionService>();
            //services.AddScoped<IAgentsService, AgentsService>();
            //services.AddScoped<IWorkshopService, WorkshopService>();
            //services.AddScoped<IGeneralTransactionService, GeneralTransactionService>();
            services.AddScoped<ICompanyInfo, CompanyInfoService>();
            //services.AddScoped<IInventorySetupService, InventorySetupService>();

            #endregion

            services.Configure<JwtConfig>(options =>
                        Configuration.GetSection(WebConstants.Sections.AuthJwtBearer).Bind(options));

            //services.Configure<BookingConfig>(options =>
            //           Configuration.GetSection(WebConstants.Sections.Booking).Bind(options));

            services.Configure<AppConfig>(options =>
                     Configuration.GetSection(WebConstants.Sections.App).Bind(options));

            services.Configure<SmtpConfig>(options =>
                     Configuration.GetSection(WebConstants.Sections.Smtp).Bind(options));

            services.Configure<PaymentConfig.Paystack>(options =>
                     Configuration.GetSection(WebConstants.Sections.Paystack).Bind(options));

            services.Configure<DataProtectionTokenProviderOptions>(o =>
                     o.TokenLifespan = TimeSpan.FromHours(3));

            #region Auth
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
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
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            //#endregion

            //#region Auth
            services.AddAuthorization(options =>
            {
                SetupPolicies(options);
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
            services.AddTransient<IAppMenu, AppMenuService>();
            services.AddTransient<IEmployeeService, EmployeeService>();

            //if (Environment.IsDevelopment())
            //{ }
            //services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SHB Web API", Version = "v1" });
            //    options.DocInclusionPredicate((docName, description) => true);

            //    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            //    {
            //        In = ParameterLocation.Header,
            //        Description = "Token Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.ApiKey
            //    });
            //    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
            //       {
            //         new OpenApiSecurityScheme
            //         {
            //           Reference = new OpenApiReference
            //           {
            //             Type = ReferenceType.SecurityScheme,
            //             Id = "Bearer"
            //           }
            //          },
            //          new string[] { }
            //        }
            //      });

            //});

            //services.AddSwaggerGen(options => {
            //    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Sharedbook", Version = "v1" });
            //});



            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                });

        


            //services.AddHangfire(configuration => configuration
            //    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UseSqlServerStorage(Configuration.GetConnectionString("Database"), new SqlServerStorageOptions
            //    {
            //        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //        QueuePollInterval = TimeSpan.Zero,
            //        UseRecommendedIsolationLevel = true,
            //        DisableGlobalLocks = true
            //    }));

            //services.AddHangfireServer();

            //services.AddScoped<IRecurringJob, RecurringJob>();
        }

        private static void SetupPolicies(Microsoft.AspNetCore.Authorization.AuthorizationOptions options)
        {
            options.AddPolicy("Manage Customer", policy =>
                 policy.RequireClaim("Permission", PermissionClaimsProvider.ManageCustomer.Value));

            options.AddPolicy("Manage Employee", policy =>
                 policy.RequireClaim("Permission", PermissionClaimsProvider.ManageEmployee.Value));

            options.AddPolicy("Manage Report", policy =>
                policy.RequireClaim("Permission", PermissionClaimsProvider.ManageReport.Value));

            options.AddPolicy("Manage State", policy =>
                policy.RequireClaim("Permission", PermissionClaimsProvider.ManageState.Value));

            options.AddPolicy("Manage Region", policy =>
                policy.RequireClaim("Permission", PermissionClaimsProvider.ManageRegion.Value));

            options.AddPolicy("Manage HireBooking", policy =>
                policy.RequireClaim("Permission", PermissionClaimsProvider.ManageHireBooking.Value));

            options.AddPolicy("Manage Vehicle", policy =>
                policy.RequireClaim("Permission", PermissionClaimsProvider.ManageVehicle.Value));

            options.AddPolicy("Manage Terminal", policy =>
              policy.RequireClaim("Permission", PermissionClaimsProvider.ManageTerminal.Value));

            options.AddPolicy("Manage Route", policy =>
              policy.RequireClaim("Permission", PermissionClaimsProvider.ManageRoute.Value));

            options.AddPolicy("Manage Trip", policy =>
              policy.RequireClaim("Permission", PermissionClaimsProvider.ManageTrip.Value));
        }

        public void Configure(IApplicationBuilder app,
            // IRecurringJobManager recurringJobManager,
            IServiceProvider serviceProvider/*, ILoggerFactory loggerFactory*/)
        {

            if (Environment.IsDevelopment())
            {

                UserSeed.SeedDatabase(app);
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseCors(x =>
            {
                x.WithOrigins(Configuration["App:CorsOrigins"]
                  .Split(",", StringSplitOptions.RemoveEmptyEntries)
                  .Select(o => o.RemovePostFix("/"))
                  .ToArray())
             .AllowAnyMethod()
             .AllowAnyHeader();
            });



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //if (Environment.IsDevelopment())
            //{ }
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("http://localhost:5001/" + "swagger/v1/swagger.json",
                "sharedbook API");
            });
            //app.useswaggerui(options =>
            //{
            //    options.swaggerendpoint(configuration["app:serverrootaddress"].ensureendswith('/') + "swagger/v1/swagger.json", "lme.web api v1");
            //});


            //app.UseHangfireDashboard();
            //app.UseHangfireServer();
            //recurringJobManager.AddOrUpdate(
            //    "Run every minute",
            //    //() => serviceProvider.GetService<IRecurringJob>().BackgroundJob(),
            //    () => serviceProvider.GetService<IBookingService>().VerifyPaystack(),
            //    "*/5 * * * *");
            //app.UseFileServer();



        }
    }
}
