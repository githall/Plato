using PlatoCore.Data.Abstractions;
using Plato.Files.Models;
using PlatoCore.Security.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace Plato.Files.ViewModels
{
    public class AttachmentsViewModel
    {

        public IPagedResults<File> Results { get; set; }

        public FileInfo Info { get; set; }

        public FileOptions Options { get; set; }

        public RouteValueDictionary DeleteRoute { get; set; }

        public IPermission PostPermission { get; set; }

        public IPermission DeleteOwnPermission { get; set; }

        public IPermission DeleteAnyPermission { get; set; }

    }

}
