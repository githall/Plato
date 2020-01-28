using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace Plato.Questions.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        // FollowQuestions
        // AutoFollowQuestions
        // AutoFollowQuestionAnswers

        public static readonly Permission FollowQuestions =
            new Permission("FollowQuestions", "Can follow questions");

        public static readonly Permission AutoFollowQuestions =
            new Permission("AutoFollowQuestions", "Automatically follow new questions");

        public static readonly Permission AutoFollowQuestionAnswers =
            new Permission("AutoFollowQuestionAnswers", "Automatically follow questions when posting replies");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowQuestions,
                AutoFollowQuestions,
                AutoFollowQuestionAnswers
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        FollowQuestions,
                        AutoFollowQuestions,
                        AutoFollowQuestionAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowQuestions,
                        AutoFollowQuestions,
                        AutoFollowQuestionAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowQuestions,
                        AutoFollowQuestions,
                        AutoFollowQuestionAnswers
                    }
                }
            };

        }

    }

}
