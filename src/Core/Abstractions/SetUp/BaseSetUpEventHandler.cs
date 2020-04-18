﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace PlatoCore.Abstractions.SetUp
{

    public abstract class BaseSetUpEventHandler : ISetUpEventHandler
    {

        public string ModuleId => Path.GetFileNameWithoutExtension(this.GetType().Assembly.ManifestModule.Name);

        public abstract Task SetUp(ISetUpContext context, Action<string, string> reportError);

    }

}
