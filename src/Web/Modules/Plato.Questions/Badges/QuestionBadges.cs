﻿using System.Collections.Generic;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Models.Badges;

namespace Plato.Questions.Badges
{
    public class QuestionBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("QuestionBadgesFirst",
                "First Question",
                "Posted a question",
                "fal fa-question-circle",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("QuestionBadgesBronze",
                "Quick Learner",
                "Posted several questions",
                "fal fa-question-circle",
                BadgeLevel.Bronze,
                10,
                5);

        public static readonly Badge Silver =
            new Badge("QuestionBadgesSilver",
                "Inquisitive",
                "Contributed many questions",
                "fal fa-question-circle",
                BadgeLevel.Silver,
                25,
                10);
        
        public static readonly Badge Gold =
            new Badge("QuestionBadgesGold",
                "Know It All",
                "Contributed dozens of questions",
                "fal fa-question-circle",
                BadgeLevel.Gold,
                50,
                20);

        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                First,
                Bronze,
                Silver,
                Gold
            };

        }
        
    }

}