using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string? Id { get; set; }
        }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;  // database
      private readonly IUserAccessor _userAccessor;  // to get current user

      public Handler(DataContext context, IUserAccessor userAccessor)
      {
            _context = context;
            _userAccessor = userAccessor;
      }

      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        // get user with photo collection from current user name
        var user = await _context.Users.Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

        if (user == null) return null!;

        // find the photo with id from photo collection
        var photo = user.Photos!.FirstOrDefault(x => x.Id == request.Id);

        if(photo == null) return null!;

        // find the current main photo in the photo collection
        var currentMain = user.Photos!.FirstOrDefault(x => x.IsMain);

        // change current main photo to not main
        if (currentMain != null) currentMain.IsMain = false;

        // set the selected photo to main
        photo.IsMain = true;

        // save to database
        var success = await _context.SaveChangesAsync() > 0;

        if(success) return Result<Unit>.Success(Unit.Value);

        return Result<Unit>.Failure("Problem setting main photo");
      }
    }
  }
}