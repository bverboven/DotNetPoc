using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SelfHostingApiWithAuth.Jwt.Models;
using SelfHostingApiWithAuth.Jwt.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace SelfHostingApiWithAuth.Jwt.Extensions
{
    public static class JwtExtensions
    {
        public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services, Action<JwtTokenOptions> configureOptions)
        {
            var options = new JwtTokenOptions();
            configureOptions(options);

            if (string.IsNullOrEmpty(options.Secret))
            {
                throw new NullReferenceException($"Missing {nameof(options.Secret)} in {typeof(JwtTokenOptions).FullName}");
            }

            return services
                .AddTransient(_ => new JwtTokenHelper(options))
                .AddAuthentication(options.AuthenticationScheme)
                .AddJwtBearer(options.AuthenticationScheme, x =>
                {
                    var secretKeyBytes = Encoding.ASCII.GetBytes(options.Secret);
                    var symmetricKey = new SymmetricSecurityKey(secretKeyBytes);

                    var audiences = options.Audiences ??
                                    (!string.IsNullOrWhiteSpace(options.Audience) ? new[] { options.Audience } : null);


                    x.RequireHttpsMetadata = true;
                    x.SaveToken = false;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = options.Authority,
                        ValidAudiences = audiences,
                        IssuerSigningKey = symmetricKey,
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = options.Authority != null,
                        ValidateAudience = audiences?.Any() ?? false,
                        ValidateLifetime = options.LifeSpan > 0,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
        public static SwaggerGenOptions AddJwtAuthentication(this SwaggerGenOptions o, string authenticationScheme = JwtBearerDefaults.AuthenticationScheme)
        {
            // https://stackoverflow.com/questions/43447688/setting-up-swagger-asp-net-core-using-the-authorization-headers-bearer/#answer-64899768
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = authenticationScheme,
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Enter your JWT Bearer token",

                Reference = new OpenApiReference
                {
                    Id = authenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            o.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });

            return o;
        }
    }
}
