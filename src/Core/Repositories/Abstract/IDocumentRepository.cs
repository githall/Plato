using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PlatoCore.Models.Abstract;

namespace PlatoCore.Repositories.Abstract
{
    public interface IDocumentRepository
    {

        Task<DocumentEntry> UpdateAsync(DocumentEntry document);

        Task<DocumentEntry> GetAsync(int id);

        Task<DocumentEntry> GetByType(string type);

        Task<bool> DeleteAsync(int id);
    }

}
