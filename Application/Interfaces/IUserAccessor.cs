using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    // to get the username of current user anywhere in our app
    public interface IUserAccessor
    {
        string GetUsername();
    }
}