using ContactBook.Data;
using ContactBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Services
{
    public class Utilities
    {
        private readonly ContactBookContext _contactBookContext;

        public Utilities(ContactBookContext contactBookContext)
        {
            _contactBookContext = contactBookContext;
        }

        public List<AppUser> GetAllUsers(PaginParameter paginParameter)
        {
            var contacts = _contactBookContext.appUsers.OrderBy(on => on.FirstName)
                .Skip((paginParameter.PageNumber - 1) * paginParameter.PageSize)
                .Take(paginParameter.PageSize)
                .ToList();
            return contacts;
        }
    }
}
