using DealerNetAPI.BusinessLogic;
using DealerNetAPI.BusinessLogic.ARCreditLimit;
using DealerNetAPI.BusinessLogic.Interface;
using DealerNetAPI.BusinessLogic.SchemeDFI;
using DealerNetAPI.BusinessLogic.VMS;
using DealerNetAPI.Extensions;
using DealerNetAPI.ResourceAccess;
using DealerNetAPI.ResourceAccess.ARCreditLimit;
using DealerNetAPI.ResourceAccess.Interface;
using DealerNetAPI.ResourceAccess.SchemeDFI;
using DealerNetAPI.ResourceAccess.VMS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealerNetAPI
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
            //Enable CORS ,Added by Vikrant --Step 1
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });

            //JSON Serializer ,Added by Vikrant --Step 3
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            services.AddControllers();

            AddJWTTokenServicesExtensions.AddJWTTokenServices(services, Configuration);

            #region All Interface Register

            services.AddScoped<ICommonDB, CommonDB>();
            services.AddScoped<IStateBusinessLogic, StateBusinessLogic>();
            services.AddScoped<IStateAccess, StateAccess>();
            services.AddScoped<IDistrictBusinessLogic, DistrictBusinessLogic>();
            services.AddScoped<IDistrictAccess, DistrictAccess>();
            services.AddScoped<IFirmTypeBusinessLogic, FirmTypeBusinessLogic>();
            services.AddScoped<IFirmTypeAccess, FirmTypeAccess>();
            services.AddScoped<IBankBusinessLogic, BankBusinessLogic>();
            services.AddScoped<IBankAccess, BankAccess>();
            services.AddScoped<IDocumentBusinessLogic, DocumentBusinessLogic>();
            services.AddScoped<IDocumentAccess, DocumentAccess>();
            services.AddScoped<ILookupBusinessLogic, LookupBusinessLogic>();
            services.AddScoped<ILookupAccess, LookupAccess>();
            services.AddScoped<ITermsConditionsBusinessLogic, TermsConditionsBusinessLogic>();
            services.AddScoped<ITermsConditionsAccess, TermsConditionsAccess>();
            services.AddScoped<IUserBusinessLogic, UserBusinessLogic>();
            services.AddScoped<IUserAccess, UserAccess>();
            services.AddScoped<IMenuBusinessLogic, MenuBusinessLogic>();
            services.AddScoped<IMenuAccess, MenuAccess>();

            services.AddScoped<IUtilitiesDFIBusinessLogic, UtilitiesDFIBusinessLogic>();
            services.AddScoped<IUtilitiesDFIAccess, UtilitiesDFIAccess>();
            services.AddScoped<ISchemeDFIBusinessLogic, SchemeDFIBusinessLogic>();
            services.AddScoped<ISchemeDFIAccess, SchemeDFIAccess>();

            #region Vednor Management System

            services.AddScoped<IUtilitiesVMSBusinessLogic, UtilitiesVMSBusinessLogic>();
            services.AddScoped<IUtilitiesVMSAccess, UtilitiesVMSAccess>();

            services.AddScoped<IVendorVMSAccess, VendorVMSAccess>();
            services.AddScoped<IVendorVMSBusinessLogic, VendorVMSBusinessLogic>();

            services.AddScoped<IVendorConfirmationAccess, VendorConfirmationAccess>();
            services.AddScoped<IVendorConfirmationBusinessLogic, VendorConfirmationBusinessLogic>();

            services.AddScoped<IVendorCommunicationAccess, VendorCommunicationAccess>();
            services.AddScoped<IVendorCommunicationBusinessLogic, VendorCommunicationBusinessLogic>();

            #endregion

            #region AR Credit Limit

            services.AddScoped<IUtilitiesARCreditLimitAccess, UtilitiesARCreditLimitAccess>();
            services.AddScoped<IUtilitiesARCreditLimitBusinessLogic, UtilitiesARCreditLimitBusinessLogic>();
            services.AddScoped<IARCreditLimitAccess, ARCreditLimitAccess>();
            services.AddScoped<IARCreditLimitBusinessLogic, ARCreditLimitBusinessLogic>();


            #endregion

            #endregion


            // Swagger Enabled
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "DealerNet Plus New API",
                        Description = "API for showing Swagger",
                        Version = "v1"
                    });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseMiddleware<Helper.EncryptionMiddleware>(); // For Payload enrypt/decrypt auto.
            app.UseCors(options => options
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()); // Added by Vikrant --Step 2

            // This middleware serves generated Swagger document as a JSON endpoint
            app.UseSwagger();
            // This middleware serves the Swagger documentation UI
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            app.UseAuthentication(); // This need to be added before UseAuthorization()
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
