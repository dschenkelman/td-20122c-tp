﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CourseManagement.MessageProcessing.Actions;
using CourseManagement.Messages;
using CourseManagement.Persistence.Repositories;

namespace CourseManagement.MessageProcessing.Rules
{
    class AddNewTicketToDatabaseRule :BaseRule
    {
        private const string SubjectPattern = @"\[CONSULTA-(PUBLICA|PRIVADA)\][a-zA-Z\ ]*";
        
        private readonly ICourseManagementRepositories courseManagementRepositories;

        private readonly Regex subjectRegex;

        public AddNewTicketToDatabaseRule(
            IActionFactory actionFactory, 
            ICourseManagementRepositories courseManagementRepositories) : base(actionFactory)
        {
            this.courseManagementRepositories = courseManagementRepositories;
            this.subjectRegex = new Regex(SubjectPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public override bool IsMatch(IMessage message, bool previouslyMatched)
        {
            Match match = this.subjectRegex.Match(message.Subject);
            if (!match.Success)
            {
                return false;
            }

            return this.courseManagementRepositories.Tickets.Get(t => t.MessageSubject == message.Subject).ToList().Count == 0;
        }
    }
}