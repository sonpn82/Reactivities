using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Details
    {
        public class Query : IRequest<Result<Profile>>
        {
            public string? Username {get; set;}
        }

        public class Handler : IRequestHandler<Query, Result<Profile>>
        {
      private readonly DataContext _context; // database to get users data
      private readonly IMapper _mapper;  // automapper to map prop between objects
      private readonly IUserAccessor _userAccessor;  // to get current user and passdown to mappers for following feature

      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
      {
        _userAccessor = userAccessor;
        _context = context;
        _mapper = mapper;  
      }

      public async Task<Result<Profile>> Handle(Query request, CancellationToken cancellationToken)
      {
        var user = await _context.Users
            .ProjectTo<Profile>(_mapper.ConfigurationProvider, 
              new {currentUsername = _userAccessor.GetUsername()})  // map a Users object to profile object
            .SingleOrDefaultAsync(x => x.Username == request.Username);

        return Result<Profile>.Success(user!);
        // do not need to return null here, already handle it in HandleResult method
      }
    }
    }
}