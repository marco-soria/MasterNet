using MasterNet.Domain.Security;

namespace MasterNet.WebApi.Extensions;

public static class PoliciesConfiguration
{

    public static IServiceCollection AddPoliciesServices(
        this IServiceCollection services
    )
    {
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(
                PolicyMaster.COURSE_READ, policy =>
                   policy.RequireAssertion(
                    context => context.User.HasClaim(
                    c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.COURSE_READ
                    )
                   )
            );

            opt.AddPolicy(
                PolicyMaster.COURSE_WRITE, policy =>
                   policy.RequireAssertion(
                    context => context.User.HasClaim(
                    c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.COURSE_WRITE
                    )
                   )
            );

            opt.AddPolicy(
                PolicyMaster.COURSE_UPDATE, policy =>
                   policy.RequireAssertion(
                    context => context.User.HasClaim(
                    c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.COURSE_UPDATE
                    )
                   )
            );


            opt.AddPolicy(
              PolicyMaster.COURSE_DELETE, policy =>
                 policy.RequireAssertion(
                  context => context.User.HasClaim(
                  c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.COURSE_DELETE
                  )
                 )
          );


            opt.AddPolicy(
             PolicyMaster.INSTRUCTOR_CREATE, policy =>
                policy.RequireAssertion(
                 context => context.User.HasClaim(
                 c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.INSTRUCTOR_CREATE
                 )
                )
         );

            opt.AddPolicy(
                       PolicyMaster.INSTRUCTOR_UPDATE, policy =>
                          policy.RequireAssertion(
                           context => context.User.HasClaim(
                           c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.INSTRUCTOR_UPDATE
                           )
                          )
                   );

            opt.AddPolicy(
                       PolicyMaster.INSTRUCTOR_READ, policy =>
                          policy.RequireAssertion(
                           context => context.User.HasClaim(
                           c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.INSTRUCTOR_READ
                           )
                          )
                   );

            opt.AddPolicy(
                      PolicyMaster.COMMENT_READ, policy =>
                         policy.RequireAssertion(
                          context => context.User.HasClaim(
                          c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.COMMENT_READ
                          )
                         )
                  );

            opt.AddPolicy(
                       PolicyMaster.COMMENT_CREATE, policy =>
                          policy.RequireAssertion(
                           context => context.User.HasClaim(
                           c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.COMMENT_CREATE
                           )
                          )
                   );

            opt.AddPolicy(
                       PolicyMaster.COMMENT_DELETE, policy =>
                          policy.RequireAssertion(
                           context => context.User.HasClaim(
                           c => c.Type == CustomClaims.POLICIES && c.Value == PolicyMaster.COMMENT_DELETE
                           )
                          )
                   );


        }


        );




        return services;
    }

}