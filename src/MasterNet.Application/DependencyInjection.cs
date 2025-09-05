using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MasterNet.Application.Core;

namespace MasterNet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services
    )
    {
        services.AddMediatR(configuration =>
        {
            configuration
            .RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        //services.AddFluentValidationAutoValidation();
        //services.AddValidatorsFromAssemblyContaining<CreateCourseCommand>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddAutoMapper(config => {
            config.AddProfile<MappingProfile>();
        });

        return services;
    }

}