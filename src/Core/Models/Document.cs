using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PlatoCore.Abstractions;

namespace PlatoCore.Models
{
    public class BaseDocument : Serializable, IDocument
    {

        public int Id { get; set; }

        //public string Serialize()
        //{
        //    return JsonConvert.SerializeObject(this);
        //}

    }
}
