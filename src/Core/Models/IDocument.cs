using System;
using System.Collections.Generic;
using System.Text;
using PlatoCore.Abstractions;

namespace PlatoCore.Models
{
    public interface IDocument : ISerializable
    {

        int Id { get; set; }

    }

}
