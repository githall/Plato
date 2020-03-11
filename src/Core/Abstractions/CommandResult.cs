﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PlatoCore.Abstractions
{

    public class CommandResultBase : ICommandResultBase
    {

        private readonly List<CommandError> _errors = new List<CommandError>();

        public bool Succeeded { get; protected set; }

        public IEnumerable<CommandError> Errors => (IEnumerable<CommandError>)this._errors;

        public CommandResultBase Success()
        {
            return new CommandResultBase()
            {
                Succeeded = true
            };
        }

        public CommandResultBase Failed(string message)
        {
            var result = new CommandResultBase()
            {
                Succeeded = false
            };

            result._errors.Add(new CommandError(message));

            return result;
        }

        public CommandResultBase Failed(params CommandError[] errors)
        {
            var result = new CommandResultBase()
            {
                Succeeded = false
            };
            if (errors != null)
                result._errors.AddRange((IEnumerable<CommandError>)errors);
            return result;
        }

    }

    public class CommandResult<TResponse> : ICommandResult<TResponse> where TResponse : class
    {

        private readonly List<CommandError> _errors = new List<CommandError>();

        public bool Succeeded { get; protected set; }

        public TResponse Response { get; protected set; }

        public IEnumerable<CommandError> Errors => (IEnumerable<CommandError>)this._errors;

        public CommandResult()
        {
        }

        public CommandResult(ICommandResultBase result)
        {
            this.Succeeded = result.Succeeded;
            _errors.AddRange(result.Errors);
        }

        public virtual CommandResult<TResponse> Success()
        {
            return new CommandResult<TResponse>()
            {
                Succeeded = true
            };
        }

        public virtual ICommandResult<TResponse> Success(object response)
        {

            // No response object just return success
            if (response == null)
            {
                return Success();
            }

            // Cast our generic response object to expected type
            return new CommandResult<TResponse>()
            {
                Response = (TResponse)Convert.ChangeType(response, typeof(TResponse)),
                Succeeded = true
            };
        }

        public virtual ICommandResult<TResponse> Failed(string message)
        {
            var result = new CommandResult<TResponse>()
            {
                Succeeded = false
            };

            result._errors.Add(new CommandError(message));

            return result;
        }

        public virtual ICommandResult<TResponse> Failed(IEnumerable<string> messages)
        {

            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            var result = new CommandResult<TResponse>()
            {
                Succeeded = false
            };

            foreach (var message in messages)
            {
                result._errors.Add(new CommandError(message));
            }            

            return result;
        }

        public virtual ICommandResult<TResponse> Failed(params CommandError[] errors)
        {
            var result = new CommandResult<TResponse>()
            {
                Succeeded = false
            };
            if (errors != null)
                result._errors.AddRange((IEnumerable<CommandError>)errors);
            return result;
        }

        public override string ToString()
        {
            if (!this.Succeeded)
            {
                return
                    $"{(object)"Failed"} : {(object)string.Join(",", (IEnumerable<string>)this.Errors.Select<CommandError, string>((Func<CommandError, string>)(x => x.Code)).ToList<string>())}";
            }

            return "Succeeded";
        }

    }

}
