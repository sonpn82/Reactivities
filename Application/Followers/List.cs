using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Application.Profiles;
using Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using Application.Interfaces;

namespace Application.Followers
{
    // return a list of following or follower profile from selected user
    public class List
    {
        // query contains 2 parameters: type of follow and username
        public class Query : IRequest<Result<List<Profiles.Profile>>>
        {
            public string? Predicate { get; set; }  // can be either followers or following
            public string? Username { get; set; }
        }

    public class Handler : IRequestHandler<Query, Result<List<Profiles.Profile>>>
    {
        private readonly DataContext _context;  // to get UserFollowings table data
        private readonly IMapper _mapper;  // to map from UserFollowing.Obsever or Target (AppUser type) to Profile
      private readonly IUserAccessor _userAccessor;  // to get current user

      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
      {
        _mapper = mapper;
        _userAccessor = userAccessor;
        _context = context;
      }

      public async Task<Result<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
      {
        var profiles = new List<Profiles.Profile>();

        // check if follow type is followers or following
        switch (request.Predicate)
        {
            case "followers":
                // get the userfollowing by the Target username is equal to username
                profiles = await _context.UserFollowings.Where(x => x.Target!.UserName == request.Username)
                    .Select(u => u.Observer) // get all the observer of above userfollowing
                    .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider, 
                        new {currentUsername = _userAccessor.GetUsername()})  // passdown the currentUsername to the mapper method
                    .ToListAsync();
                break;

            case "following":
                profiles = await _context.UserFollowings.Where(x => x.Observer!.UserName == request.Username)
                    .Select(u => u.Target)
                    .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider,
                      new {currentUsername = _userAccessor.GetUsername()})  // passdown the currentUsername to the mapper method
                    .ToListAsync();
                break;
        }

        return Result<List<Profiles.Profile>>.Success(profiles);
      }
    }
  }
}