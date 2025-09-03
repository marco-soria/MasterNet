using FluentValidation;
using FluentValidation.AspNetCore;
using MasterNet.Application.Courses.CreateCourse;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using MasterNet.Application.Core;

namespace MasterNet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services
    )
    {
        services.AddMediatR(configuration => {
            configuration
            .RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateCourseCommand>();

        services.AddAutoMapper(config => {
            config.AddProfile<MappingProfile>();
        });

        return services;
    }

}